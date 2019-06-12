namespace FluentValidation.Tests {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net.Http;
	using System.Text;
	using System.Threading.Tasks;
	using System.Xml.Linq;
	using AspNetCore.Controllers;
	using Newtonsoft.Json;

	public static class HttpClientExtensions {

		public static async Task<string> GetResponse(this HttpClient client, string url,
			string querystring = "") {
			if (!String.IsNullOrEmpty(querystring)) {
				url += "?" + querystring;
			}

			var response = await client.GetAsync(url);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsStringAsync();
		}

		public static async Task<string> PostResponse(this HttpClient client, string url,
			Dictionary<string, string> form) {
			var c = new FormUrlEncodedContent(form);

			var response = await client.PostAsync(url, c);
			response.EnsureSuccessStatusCode();

			return await response.Content.ReadAsStringAsync();
		}

		public static async Task<List<SimpleError>> GetErrors(this HttpClient client, string action, Dictionary<string, string> form) {
			var response = await client.PostResponse($"/Test/{action}", form);
			return JsonConvert.DeserializeObject<List<SimpleError>>(response);
		}

		public static Task<List<SimpleError>> GetErrorsViaJSON<T>(this HttpClient client, string action, T model) {
			return client.GetErrorsViaJSONRaw(action, JsonConvert.SerializeObject(model));
		}

		public static async Task<List<SimpleError>> GetErrorsViaJSONRaw(this HttpClient client, string action, string json) {
			var request = new HttpRequestMessage(HttpMethod.Post, $"/Test/{action}");
			request.Content = new StringContent(json, Encoding.UTF8, "application/json");
			var responseMessage = await client.SendAsync(request);
			responseMessage.EnsureSuccessStatusCode();
			var response = await responseMessage.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<List<SimpleError>>(response);
		}

		public static async Task<XDocument> GetClientsideMessages(this HttpClient client, string action = "/Clientside/Inputs") {
			var output = await client.GetResponse(action);
			return XDocument.Parse(output);
		}

		public static async Task<string> GetClientsideMessage(this HttpClient client, string name, string attribute) {
			var doc = await client.GetClientsideMessages();
			var elem = doc.Root.Elements("input")
				.Where(x => x.Attribute("name").Value == name).SingleOrDefault();

			if (elem == null) {
				throw new Exception("Could not find element with name " + name);
			}

			var attr = elem.Attribute(attribute);

			if (attr == null || string.IsNullOrEmpty(attr.Value)) {
				throw new Exception("Could not find attr " + attribute);
			}

			return attr.Value;
		}

		public static async Task<string[]> RunRulesetAction(this HttpClient client, string action) {

			var doc = await client.GetClientsideMessages(action);

			var elems = doc.Root.Elements("input")
				.Where(x => x.Attribute("name").Value.StartsWith("CustomName"));

			var results = elems.Select(x => x.Attribute("data-val-required"))
				.Where(x => x != null)
				.Select(x => x.Value)
				.ToArray();

			return results;
		}

	}
}
