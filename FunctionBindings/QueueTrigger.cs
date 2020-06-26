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
            log.LogWarning($"C# Queue trigger function processed: {myQueueItem}");

            HttpHelper.CallAzureService("http://localhost:7071/api/ToSBQueue", myQueueItem);
        }


        [FunctionName("ToSBQueue")]
        [return: ServiceBus("testqueue", Connection = "ServiceBusConnection")]
        public static string ServiceBusOutput([HttpTrigger] dynamic input, ILogger log)
        {
            log.LogWarning($"ToSBQueue C# function processed: {input.Text}");
            return input.Text;
        }
    }
}
