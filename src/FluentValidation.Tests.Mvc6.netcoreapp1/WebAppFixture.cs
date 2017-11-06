namespace FluentValidation.Tests {
	using System;
	using System.Collections.Generic;
	using System.Net.Http;
	using System.Threading.Tasks;
	using AspNetCore;
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
#if NETCOREAPP2_0
					.UseDefaultServiceProvider((context, options) => options.ValidateScopes = true)
#endif
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
	}
}