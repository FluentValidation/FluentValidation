namespace FluentValidation.Tests.AspNetCore {
	using System.Collections.Generic;
	using System.Net.Http;
	using System.Threading.Tasks;
	using Controllers;
	using FluentValidation.AspNetCore;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.TestHost;
	using Microsoft.Extensions.DependencyInjection;
	using Newtonsoft.Json;
	using Xunit;

	public class ServiceProviderTests : IClassFixture<WebAppFixture> {
		private readonly HttpClient _client;

		public ServiceProviderTests(WebAppFixture webApp) {

			_client = webApp.CreateClientWithServices(services => {
				services.AddMvc().AddNewtonsoftJson().AddFluentValidation(fv => {
					fv.RegisterValidatorsFromAssemblyContaining<TestController>();
				});
			});
		}

		[Fact]
		public async Task Gets_validators_from_service_provider() {
			var form = new FormData {
				{ "test.Name", null }
			};

			var result = await _client.GetErrors("Test1", form);

			result.IsValidField("test.Name").ShouldBeFalse();
			result.GetError("test.Name").ShouldEqual("Validation Failed");
		}

		[Fact]
		public async Task Validators_should_be_scoped() {
			var result = await _client.GetErrors("Lifecycle", new FormData());
			var hashCode1 = result.GetError("Foo");

			var result2 = await _client.GetErrors("Lifecycle", new FormData());
			var hashCode2 = result2.GetError("Foo");

			Assert.NotNull(hashCode1);
			Assert.NotNull(hashCode2);
			Assert.NotEqual("", hashCode1);
			Assert.NotEqual("", hashCode2);

			Assert.NotEqual(hashCode1, hashCode2);
		}

		[Fact]
		public async Task Gets_validator_for_model_not_underlying_collection_type() {
			var result = await _client.GetErrors("ModelThatimplementsIEnumerable", new FormData());
			result.GetError("Name").ShouldEqual("Foo");
		}
	}
}
