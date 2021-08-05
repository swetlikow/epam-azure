using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DeliveryOrderProcessor.Models;

namespace DeliveryOrderProcessor
{
    public static class DeliverOrder
    {
        private static readonly string _endpointUri = "https://eshopcosmos.documents.azure.com:443/";
        private static readonly string _primaryKey = "3lx90PeV1rlCiDr0UztOqDfC50iJX3Sgs1OKWAKAL2RsUtxTBbldiDJSaOcOTFeEh0TYeoeBmlXIkCMqEpLtOw==";

        [FunctionName("DeliverOrder")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var order = JsonConvert.DeserializeObject<Order>(requestBody);

            var deliveryOrder = new DeliveryOrder(order.ShipToAddress, order.OrderItems.ToList());

            var addItemToDb = await AddItemToDb(deliveryOrder);

            return new OkObjectResult(addItemToDb);
        }

        private static async Task<ItemResponse<DeliveryOrder>> AddItemToDb(DeliveryOrder order)
        {
            try
            {
                var cosmosClient = new CosmosClient(_endpointUri, _primaryKey);

                var container = cosmosClient.GetContainer("orders", "items");

                var response = await container.CreateItemAsync(order);

                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
