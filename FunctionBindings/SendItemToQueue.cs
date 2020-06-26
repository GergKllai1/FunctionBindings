using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FunctionBindings
{
    public static class SendItemToQueue
    {

        [FunctionName("SendItemToQueue")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogDebug("SendItemToQueue function starting ...");
                // Get the requestBody and deserialize it
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic deserializedBody = JsonConvert.DeserializeObject(requestBody);

                HttpHelper.CallAzureService("http://localhost:7071/api/ToStorageAccQueue", (string)deserializedBody.Text);

                return new OkObjectResult(requestBody);
            }
            catch (Exception err)
            {
                return new BadRequestObjectResult(err.Message);
            }
        }

        // Use output binding to create the table if not created and instert the data
        [FunctionName("ToStorageAccQueue")]
        [return: Queue("myqueue", Connection = "StorageAccountConnection")]
        public static string ToStorageAccQueue([HttpTrigger] dynamic input, ILogger log)
        {
            log.LogDebug($"ToStorageAccQueue function processed {input.Text.Value}");
            return input.Text.Value;
        }
    }
}
