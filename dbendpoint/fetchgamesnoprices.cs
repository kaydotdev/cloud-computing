using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;
using GameStore.Games.Game;
using CSession = Cassandra.ISession;

namespace GameStore.Games.FetchGames
{
    public class FetchGamesNoPrices
    {
        private readonly CSession _session;
        private readonly ILogger<FetchGamesNoPrices> _logger;

        public FetchGamesNoPrices(ILogger<FetchGamesNoPrices> logger, CSession session)
        {
            _logger = logger;
            _session = session;
            session.UserDefinedTypes.Define(
                UdtMap.For<PriceHistory>("price_record")
                    .Map(p => p.DatePrice, "date_price")
                    .Map(p => p.InitialPrice, "initial_price")
                    .Map(p => p.FinalPrice, "final_price")
                    .Map(p => p.DiscountOnPrice, "discount_on_price"));
        }
        
        [FunctionName("fetchgamesnoprices")]
        public async Task<IActionResult> RunAsync(
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
            
            var games = new Table<Game.Game>(_session);
            var resultGames = games.Skip((page - 1) * itemsToFetch).Take(itemsToFetch).AsEnumerable();

            return new OkObjectResult(resultGames);
        }
        
        private static string[] StringifyQueryCollection(IQueryCollection query) =>
            new List<string>(query.Select(q => $"{q.Key}: {q.Value.ToString()}")).ToArray();
    }
}