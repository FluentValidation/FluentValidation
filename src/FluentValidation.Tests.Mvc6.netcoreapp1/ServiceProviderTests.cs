namespace FluentValidation.Tests.AspNetCore {
	using System.Collections.Generic;
	using System.Net.Http;
	using System.Threading.Tasks;
	using Controllers;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.TestHost;
	using Newtonsoft.Json;
	using Xunit;

	public class ServiceProviderTests : IClassFixture<WebAppFixture<StartupWithContainer>> {
		private readonly WebAppFixture<StartupWithContainer> _webApp;


		public ServiceProviderTests(WebAppFixture<StartupWithContainer> webApp)
		{
			_webApp = webApp;
		}


		//these need writing
		[Fact]
        public async Task Gets_validators_from_service_provider() {
			var form = new FormData {
				{ "test.Name", null }
			};

			var result = await _webApp.GetErrors("Test1", form);

			result.IsValidField("test.Name").ShouldBeFalse();
			result.GetError("test.Name").ShouldEqual("Validation Failed");
		}

		[Fact]
		public async Task Validators_should_be_transient() {
			var result = await _webApp.GetErrors("Lifecycle", new FormData());
			var hashCode1 = result.GetError("Foo");

			var result2 = await _webApp.GetErrors("Lifecycle", new FormData());
			var hashCode2 = result2.GetError("Foo");

			Assert.NotNull(hashCode1);
			Assert.NotNull(hashCode2);
			Assert.NotEqual("", hashCode1);
			Assert.NotEqual("", hashCode2);

			Assert.NotEqual(hashCode1, hashCode2);
		}

		[Fact]
		public async Task Gets_validator_for_model_not_underlying_collection_type() {
			var result = await _webApp.GetErrors("ModelThatimplementsIEnumerable", new FormData());
			result.GetError("Name").ShouldEqual("Foo");
		}

	}
}