using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Cassandra;
using Cassandra.Mapping;
using CSession = Cassandra.ISession;

namespace GameStore.Games.FetchGames
{
    public class FetchGames
    {
        private readonly CSession _session;
        private readonly ILogger<FetchGames> _logger;

        public FetchGames(ILogger<FetchGames> logger, CSession session)
        {
            _logger = logger;
            _session = session;
            _logger.LogInformation("Triggered function entry point");

            session.UserDefinedTypes.Define(
                UdtMap.For<Game.PriceHistory>("price_record")
                    .Map(p => p.DatePrice, "date_price")
                    .Map(p => p.InitialPrice, "initial_price")
                    .Map(p => p.FinalPrice, "final_price")
                    .Map(p => p.DiscountOnPrice, "discount_on_price"));
        }
        
        [FunctionName("fetchgames")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation($"Triggered game listing function with query params {{ {string.Join(", ", StringifyQueryCollection(req.Query))} }}");

            var itemsLimit = Convert.ToInt32(Environment.GetEnvironmentVariable("ENV_ITEMS_LIMIT") ?? "50");

            string pageId = req.Query["pageId"];
            string itemsCount =  req.Query["itemsCount"];

            if (string.IsNullOrEmpty(pageId) || string.IsNullOrEmpty(itemsCount))
                return new BadRequestErrorMessageResult("One or more required arguments missing");
            
            var itemsToFetch = Convert.ToInt32(itemsCount);
            var page = Convert.ToInt32(itemsCount);
            
            if (itemsToFetch > itemsLimit)
                return new BadRequestErrorMessageResult($"Request unit count exceeded limit in {itemsLimit} RU");
            
            if (!(0 < itemsToFetch && 0 < page))
                return new BadRequestErrorMessageResult($"One or more required arguments out of range");
            
            IMapper mapper = new Mapper(_session);
            var games = await mapper.FetchAsync<Game.Game>(
                @"SELECT name, description, origin, genres,
                   developers, release_date, price_history
                   FROM gamestore.games
                   LIMIT ?", itemsToFetch);

            return new OkObjectResult(games);
        }

        private static string[] StringifyQueryCollection(IQueryCollection query) =>
            new List<string>(query.Select(q => $"{q.Key}: {q.Value.ToString()}")).ToArray();
    }
}
