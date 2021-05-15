using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Cassandra;
using Cassandra.Mapping;
using GameStore.Games.DTOs;
using CSession = Cassandra.ISession;

namespace GameStore.Games.FetchGames
{
    public class ListGames
    {
        private readonly CSession _session;
        private readonly ILogger<ListGames> _logger;

        public ListGames(ILogger<ListGames> logger, CSession session)
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
        
        [FunctionName("ListGames")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            HttpRequest req, ILogger log)
        {
            _logger.LogInformation($"Triggered game listing function with query params {{ {string.Join(", ", StringifyQueryCollection(req.Query))} }}");

            var itemsLimit = Convert.ToInt32(Environment.GetEnvironmentVariable("ENV_ITEMS_LIMIT") ?? "50");
            string pagingState =  req.Query["pagingState"];
            
            var statement = Cql.New(@"SELECT name, description, origin, genres,
                                        developers, release_date, price_history
                                        FROM gamestore.games").WithOptions(opt =>
                opt.SetPageSize(itemsLimit)
                    .SetPagingState(!string.IsNullOrEmpty(pagingState) ?
                        Convert.FromBase64String(pagingState) : null));

            var mapper = new Mapper(_session);
            var page = await mapper.FetchPageAsync<Game>(statement);
            
            return new OkObjectResult(new { Page = page.AsEnumerable(), PagingState = page.PagingState });
        }
        
        private static string[] StringifyQueryCollection(IQueryCollection query) =>
            new List<string>(query.Select(q => $"{q.Key}: {q.Value.ToString()}")).ToArray();
    }
}