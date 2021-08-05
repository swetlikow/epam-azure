using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace DeliveryOrderProcessor.Models
{
    public class DeliveryOrder
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public Address ShippingAddress { get; set; }
        public List<OrderItem> ListOfItems { get; set; }
        public decimal? FinalPrice { get; set; }

        public DeliveryOrder(){}

        public DeliveryOrder(Address address, List<OrderItem> items)
        {
            Id = Guid.NewGuid().ToString();
            ShippingAddress = address;
            ListOfItems = items;
            FinalPrice = items.Sum(x => x.UnitPrice);
        }
    }
}