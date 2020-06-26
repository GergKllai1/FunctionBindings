using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace FunctionBindings
{
    public class HttpHelper
    {
        public static void CallAzureService(string uri, string text)
        {
            var json = JsonConvert.SerializeObject(new { Text = text });
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var client = new HttpClient();

            client.PostAsync(uri, data);
        }
    }
}
