using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
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
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            try
            {

                // Get the requestBody and deserialize it
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic deserializedBody = JsonConvert.DeserializeObject(requestBody);

                // Get Text value, create request body
                var json = JsonConvert.SerializeObject(new { Text = deserializedBody.Text });
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var client = new HttpClient();

                var response = await client.PostAsync("http://localhost:7071/api/QueueOutput", data);

                return new OkObjectResult(requestBody);
            }
            catch (Exception err)
            {
                return new BadRequestObjectResult(err.Message);
            }
        }

        // Use output binding to create the table if not created and instert the data
        [FunctionName("QueueOutput")]
        [return: Queue("myqueue", Connection = "StorageAccountConnection")]
        public static string QueueOutput([HttpTrigger] dynamic input)
        {
            return input.Text.Value;
        }
    }
}
