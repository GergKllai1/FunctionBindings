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
    public static class SendItemToTableStorage
    {

        [FunctionName("SendItemToTableStorage")]
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

                var response = await client.PostAsync("http://localhost:7071/api/TableOutput", data);

                return new OkObjectResult(requestBody);
            }
            catch (Exception err)
            {
                return new BadRequestObjectResult(err.Message);
            }
        }

        public class Item
        {
            public Item(string text)
            {
                Text = text;
                PartitionKey = "Input";
                RowKey = Guid.NewGuid().ToString();

            }
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public string Text { get; set; }
        }

        // Use output binding to create the table if not created and instert the data
        [FunctionName("TableOutput")]
        [return: Table("MyTable", Connection = "TableStorageConnectionString")]
        public static Item TableOutput([HttpTrigger] dynamic input)
        {
            return new Item(input.Text.Value);
        }
    }
}
