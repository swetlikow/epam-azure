using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.ApplicationCore.Services
{
    public class DeliveryOrderService : IDeliveryOrderService
    {
        private readonly HttpClient _httpClient;

        public DeliveryOrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SendOrder(Order order)
        {
            var uri = "https://deliveryorderprocessorfuction.azurewebsites.net/api/DeliverOrder";
            var result = await _httpClient.PostAsync(uri, ToJson(order));
        }

        private StringContent ToJson(object obj)
        {
            return new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");
        }
    }
}
