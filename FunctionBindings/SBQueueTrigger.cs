using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
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
            log.LogDebug("C# ServiceBus queue trigger function starting ...");

            HttpHelper.CallAzureService("http://localhost:7071/api/ToSBTopic", mySbMsg);

            log.LogWarning($"C# ServiceBus queue trigger function processed message: {mySbMsg}");

        }

        // Using SB output binding to send message to specific topic + sub
        [FunctionName("ToSBTopic")]
        [return: ServiceBus("testtopic", Connection = "ServiceBusConnection")]
        public static Message ServiceBusOutput([HttpTrigger] dynamic input, ILogger log)
        {
            var msg = new Message();
            msg.To = "testsub";
            msg.Body = Encoding.ASCII.GetBytes(input.Text.Value);
            log.LogWarning($"ToSBTopic function processed: {msg.To} - {input.Text.Value}");
            return msg;
        }
    }

}
