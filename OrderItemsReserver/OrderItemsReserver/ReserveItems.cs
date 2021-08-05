using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Azure.WebJobs;
using System.Net.Http;
using Newtonsoft.Json;

namespace OrderItemsReserver
{
    public class ReserveItems
    {
        private readonly HttpClient _httpClient;

        public ReserveItems(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [FunctionName("ReserveItems")]
        public async Task Run([ServiceBusTrigger("orders", Connection = "AzureBusConnection")] string myQueueItem)
        {
            try
            {
                var id = await UploadFileAsync(myQueueItem);
            }
            catch (Exception e)
            {
                await HandleException(e.Message);
            }
        }

        private async Task<string> UploadFileAsync(string content)
        {
            var orderId = "Order-" + Guid.NewGuid();

            var container = new BlobContainerClient
            ("DefaultEndpointsProtocol=https;AccountName=eshopappstorage;AccountKey=IjL1DyGyLwgixHhHf/jfG03W6JAtBuAHY0q83LtJedQ6eqyiWxHfUQk1YukCt+FezXuso2N5eG0YkNn0jEY+TQ==;EndpointSuffix=core.windows.net",
                "orders");
            var blob = container.GetBlobClient(orderId);
            await blob.UploadAsync(GetContentStream(content), true);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = container.Name,
                Resource = "c",
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(24)
            };
            sasBuilder.SetPermissions(BlobContainerSasPermissions.All);

            container.GenerateSasUri(sasBuilder);

            return orderId;
        }

        private async Task HandleException(string eMessage)
        {
            var uri = "https://orderexception.azurewebsites.net:443/api/main/triggers/manual/invoke?api-version=2020-05-01-preview&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=OdDtmUfwUt9ryXccN7uA4Yxjcejlyy9RfHhz8jGvGxQ";
            var result = await _httpClient.PostAsync(uri, ToJson(eMessage));
        }

        private HttpContent ToJson(string eMessage)
        {
            var exceptionMessage = new ExceptionMessage
            {
                Message = eMessage
            };

            return new StringContent(JsonConvert.SerializeObject(exceptionMessage),
                Encoding.UTF8, "application/json");
        }

        private MemoryStream GetContentStream(string content)
        {
            var byteArray = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(byteArray);
            return stream;
        }
    }
}
