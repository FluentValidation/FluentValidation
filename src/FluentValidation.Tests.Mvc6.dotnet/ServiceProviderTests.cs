namespace FluentValidation.Tests.AspNetCore {
	using System.Collections.Generic;
	using System.Net.Http;
	using System.Threading.Tasks;
	using Controllers;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.TestHost;
	using Newtonsoft.Json;
	using Xunit;

	public class ServiceProviderTests {

		private readonly TestServer _server;
		private readonly HttpClient _client;

		public ServiceProviderTests()
		{
			_server = new TestServer(new WebHostBuilder().UseStartup<StartupWithContainer>());
			_client = _server.CreateClient();
		}


		//these need writing
		[Fact]
        public async Task Gets_validators_from_service_provider() {
			var form = new FormData {
				{ "test.Name", null }
			};

			var result = await GetErrors("Test1", form);

			result.IsValidField("test.Name").ShouldBeFalse();
			result.GetError("test.Name").ShouldEqual("Validation Failed");
		}

		[Fact]
		public async Task Validators_should_be_transient() {
			var result = await GetErrors("Lifecycle", new FormData());
			var hashCode1 = result.GetError("Foo");

			var result2 = await GetErrors("Lifecycle", new FormData());
			var hashCode2 = result2.GetError("Foo");

			Assert.NotNull(hashCode1);
			Assert.NotNull(hashCode2);
			Assert.NotEqual("", hashCode1);
			Assert.NotEqual("", hashCode2);

			Assert.NotEqual(hashCode1, hashCode2);
		}

		[Fact]
		public async Task Gets_validator_for_model_not_underlying_collection_type() {
			var result = await GetErrors("ModelThatimplementsIEnumerable", new FormData());
			result.GetError("Name").ShouldEqual("Foo");
		}

		private async Task<List<SimpleError>> GetErrors(string action, Dictionary<string, string> form)
		{

			var response = await PostResponse($"/Test/{action}", form);
			return JsonConvert.DeserializeObject<List<SimpleError>>(response);
		}


		private async Task<string> PostResponse(string url,
		  Dictionary<string, string> form)
		{

			var c = new FormUrlEncodedContent(form);

			var response = await _client.PostAsync(url, c);
			response.EnsureSuccessStatusCode();

			return await response.Content.ReadAsStringAsync();
		}
	}
}