using System;
using System.Net.Http;
using System.Text;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionBindings
{
    public static class QueueTrigger
    {
        [FunctionName("QueueTrigger")]
        public static void Run([QueueTrigger("myqueue", Connection = "StorageAccountConnection")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            // Get Text value, create request body
            var json = JsonConvert.SerializeObject(new { Text = myQueueItem });
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var client = new HttpClient();

            client.PostAsync("http://localhost:7071/api/ServiceBusQueueOutput", data);
        }


        [FunctionName("ServiceBusQueueOutput")]
        [return: ServiceBus("testqueue", Connection = "ServiceBusConnection")]
        public static string ServiceBusOutput([HttpTrigger] dynamic input, ILogger log)
        {
            log.LogInformation($"ServiceBusQueueOutput C# function processed: {input.Text}");
            return input.Text;
        }
    }
}
