using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace FunctionBindings
{
    public static class SBTopicTrigger
    {
        [FunctionName("SBTopicTrigger")]
        public static void Run([ServiceBusTrigger("testtopic", "testsub", Connection = "ServiceBusConnection")] Message mySbMsg, ILogger log)
        {
            try
            {
                log.LogWarning($"C# ServiceBus topic trigger function starting ...");

                // immitate random errors
                if (new Random().Next(0, 2) == 1) throw new Exception("Something went wrong ...");

                log.LogWarning($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
            }
            catch (Exception err)
            {
                // By default the SB trigger abbadons message if function fails.
                // That could result in immediate retry. This script immitates waiting 
                // before running the trigger again.

                log.LogWarning($"{mySbMsg} is erroring out");
                var timeout = 5;

                for (int i = 0; i < timeout; i++)
                {
                    Thread.Sleep(1000);
                    Console.WriteLine($"Waiting ... {i} seconds");
                }
                throw err;
            }
        }
    }
}
