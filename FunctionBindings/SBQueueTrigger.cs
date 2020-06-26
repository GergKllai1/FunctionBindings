using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace FunctionBindings
{
    public static class SBQueueTrigger
    {
        [FunctionName("SBQueueTrigger")]
        public static void Run(
            [ServiceBusTrigger("testqueue", Connection = "ServiceBusConnection")] string mySbMsg,
            ILogger log)
        {
            var json = JsonConvert.SerializeObject(new { Text = mySbMsg });
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var client = new HttpClient();

            client.PostAsync("http://localhost:7071/api/ServiceBusToTopic", data);
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {mySbMsg}");

        }

        [FunctionName("ServiceBusToTopic")]
        [return: ServiceBus("testtopic", Connection = "ServiceBusConnection")]
        public static Message ServiceBusOutput([HttpTrigger] dynamic input, ILogger log)
        {
            var msg = new Message();
            msg.To = "testsub";
            msg.Body = Encoding.ASCII.GetBytes(input.Text.Value);
            log.LogInformation($"C# function processed: {msg}");
            return msg;
        }
    }

}
