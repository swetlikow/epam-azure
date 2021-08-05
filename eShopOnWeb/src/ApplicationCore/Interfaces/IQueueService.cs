﻿using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces
{
    public interface IQueueService
    {
        Task SendOrder(Order order);
    }
}