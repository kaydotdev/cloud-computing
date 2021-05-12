using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GameStore.Games.FetchGames
{
    public static class FetchGames
    {
        [FunctionName("fetchgames")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"Triggered game listing function with query params {{ {string.Join(", ", StringifyQueryCollection(req.Query))} }}");

            var itemsLimit = Convert.ToInt32(Environment.GetEnvironmentVariable("ENV_ITEMS_LIMIT") ?? "50");

            string pageId = req.Query["pageId"];
            string itemsCount =  req.Query["itemsCount"];

            if (string.IsNullOrEmpty(pageId) || string.IsNullOrEmpty(itemsCount))
                return new BadRequestErrorMessageResult("One or more required arguments missing");
            
            if (Convert.ToInt32(itemsCount) > itemsLimit)
                return new BadRequestErrorMessageResult($"Request unit count exceeded limit in {itemsLimit} RU");

            return new OkObjectResult("Passed preliminary checks");
        }

        private static string[] StringifyQueryCollection(IQueryCollection query) =>
            new List<string>(query.Select(q => $"{q.Key}: {q.Value.ToString()}")).ToArray();
    }
}
