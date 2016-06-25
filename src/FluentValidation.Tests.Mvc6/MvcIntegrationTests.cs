namespace FluentValidation.Tests.Mvc6 {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net.Http;
    using System.Reflection;
    using System.Threading.Tasks;
    using Controllers;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.TestHost;
    using Newtonsoft.Json;
    using Xunit;

    public class MvcIntegrationTests {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public MvcIntegrationTests()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        private async Task<string> GetResponse(string url,
            string querystring = "")
        {
            if (!String.IsNullOrEmpty(querystring))
            {
                url += "?" + querystring;
            }

            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<string> PostResponse(string url,
            Dictionary<string,string> form) {

            var c = new FormUrlEncodedContent(form);

            var response = await _client.PostAsync(url, c);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private async Task<List<SimpleError>> GetErrors<T>(string action, Dictionary<string,string> form) {
            string controller = typeof(T).Name.Replace("Controller","");

            var response = await PostResponse($"/{controller}/{action}", form);
            return JsonConvert.DeserializeObject<List<SimpleError>>(response);
        }


        [Fact]
        public async Task Test() {
            var errors = await GetErrors<TestController>("SimpleFailure", new FormData {
                {"Name", "foo"},
                {"Id", "bar"}
            });

            Assert.Equal(1, errors.Count);
            Assert.Equal("The value 'bar' is not valid for Id.", errors[0].Message);
        }
    }
}