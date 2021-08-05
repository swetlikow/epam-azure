using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.ApplicationCore.Services
{
    public class QueueService : IQueueService
    {
        static string connectionString = "Endpoint=sb://eshopbus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Rr56D9+sj60QlwIxSe1AHhPCDnoyZCxF8QV3L4/wVTs=";
        static string queueName = "orders";

        public async Task SendOrder(Order order)
        {
            var client = new ServiceBusClient(connectionString);
            var sender = client.CreateSender(queueName);

            using var messageBatch = await sender.CreateMessageBatchAsync();

            if (!messageBatch.TryAddMessage(new ServiceBusMessage(order.ToJson())))
            {
                throw new Exception($"The message {order.Id} is too large to fit in the batch.");
            }

            try
            {
                await sender.SendMessagesAsync(messageBatch);
            }

            finally
            {
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }
        }
    }
}