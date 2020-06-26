using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FunctionBindings
{
    public static class SendItemToTableStorage
    {

        [FunctionName("SendItemToTableStorage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("SendItemToTableStorage function processed a request.");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic deserializedBody = JsonConvert.DeserializeObject(requestBody);

                log.LogInformation($"Item");

                var poco = new MyPoco();
                poco.Text = deserializedBody.Text;

                var json = JsonConvert.SerializeObject(poco);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var client = new HttpClient();

                var response = await client.PostAsync("http://localhost:7071/api/TableOutput", data);

                return new OkObjectResult(requestBody);
            }
            catch (Exception err)
            {
                return new BadRequestObjectResult(err.Message);
            }
        }

        public class MyPoco
        {
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public string Text { get; set; }
        }

        [FunctionName("TableOutput")]
        [return: Table("MyTable", Connection = "TableStorageConnectionString")]
        public static MyPoco TableOutput([HttpTrigger] dynamic input, ILogger log)
        {
            log.LogInformation($"C# http trigger function processed: {input.Text}");;
            return new MyPoco { PartitionKey = "Http", RowKey = Guid.NewGuid().ToString(), Text = input.Text };
        }
    }
}
