using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DeliveryOrderProcessor.Models
{
    public class Order
    {
        public int? Id { get; set; }
        public string BuyerId { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public Address ShipToAddress { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
