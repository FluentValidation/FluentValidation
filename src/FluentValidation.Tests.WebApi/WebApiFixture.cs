namespace FluentValidation.Tests.WebApi {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net.Http;
	using System.Text;
	using System.Threading.Tasks;
	using Microsoft.Owin.Testing;
	using Newtonsoft.Json;

	public class WebApiFixture<TStartup> {
		private TestServer _server;

		public WebApiFixture() {
			_server = TestServer.Create<TStartup>();
		}

		public Task<List<SimpleError>> InvokeTest<T>(string input, string contentType = "application/x-www-form-urlencoded") {
			string className = typeof(T).Name;

			return PostForm("/api/Test/" + className, input, contentType);
		}

		
		public async Task<string> GetResult(string url) {
			return await _server.HttpClient.GetStringAsync(url);
		}

		public Task<List<SimpleError>> PostForm(string url, Dictionary<string, string> formData, string contentType = "application/x-www-form-urlencoded") {
			return PostForm(url, ConvertToFormData(formData), contentType);
		}

		public async Task<List<SimpleError>> PostForm(string url, string formData, string contentType = "application/x-www-form-urlencoded") {
			var response = await _server.HttpClient.PostAsync(url, new StringContent(formData, Encoding.UTF8, contentType));
			string responseStr = await response.Content.ReadAsStringAsync();
			try {
				var errors = response.Content.ReadAsAsync<List<SimpleError>>().Result;
				return errors;
			}
			catch (AggregateException e) {
				var json = e.InnerExceptions.OfType<JsonSerializationException>().Any();
				if (json) {
					throw new Exception("Could not deserialize JSON. Response was " + responseStr);
				}
				else throw;
			}
		}
		
		public string ConvertToFormData(Dictionary<string, string> dict) {
			return string.Join("&", dict.Select((x) => x.Key + "=" + x.Value.ToString()));
		}

	}
}