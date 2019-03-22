namespace FluentValidation.Tests {
	using System;
	using System.Collections.Generic;
	using System.Net.Http;
	using System.Text;
	using System.Threading.Tasks;
	using AspNetCore.Controllers;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.TestHost;
	using Newtonsoft.Json;

	public class WebAppFixture<TStartup> where TStartup : class {
		public TestServer Server { get; }
		public HttpClient Client { get; }

		public WebAppFixture() {
			Server = BuildTestServer<TStartup>();
			Client = Server.CreateClient();
		}

		public static TestServer BuildTestServer<T>() where T : class {
			return new TestServer(new WebHostBuilder()
				.UseDefaultServiceProvider((context, options) => options.ValidateScopes = true)
				.UseStartup<T>());
		}

		public async Task<string> GetResponse(string url,
			string querystring = "") {
			if (!String.IsNullOrEmpty(querystring)) {
				url += "?" + querystring;
			}

			var response = await Client.GetAsync(url);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsStringAsync();
		}

		public async Task<string> PostResponse(string url,
			Dictionary<string, string> form) {
			var c = new FormUrlEncodedContent(form);

			var response = await Client.PostAsync(url, c);
			response.EnsureSuccessStatusCode();

			return await response.Content.ReadAsStringAsync();
		}

		public async Task<List<SimpleError>> GetErrors(string action, Dictionary<string, string> form) {
			var response = await PostResponse($"/Test/{action}", form);
				return JsonConvert.DeserializeObject<List<SimpleError>>(response);
		}

		public Task<List<SimpleError>> GetErrorsViaJSON<T>(string action, T model) {
			return GetErrorsViaJSONRaw(action, JsonConvert.SerializeObject(model));
		}

		public async Task<List<SimpleError>> GetErrorsViaJSONRaw(string action, string json) {
			var request = new HttpRequestMessage(HttpMethod.Post, $"/Test/{action}");
			request.Content = new StringContent(json, Encoding.UTF8, "application/json");
			var responseMessage = await Client.SendAsync(request);
			responseMessage.EnsureSuccessStatusCode();
			var response = await responseMessage.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<List<SimpleError>>(response);
		}

	}
}