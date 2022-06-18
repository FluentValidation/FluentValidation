namespace FluentValidation.Tests;

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Controllers;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class ServiceProviderTests : IClassFixture<WebAppFixture> {
	private readonly HttpClient _client;

	public ServiceProviderTests(WebAppFixture webApp) {

		_client = webApp.CreateClientWithServices(services => {
			services.AddMvc().AddNewtonsoftJson().AddFluentValidation();
			services.AddValidatorsFromAssemblyContaining<TestController>();
		});
	}

	[Fact]
	public async Task Gets_validators_from_service_provider() {
		var form = new Dictionary<string, string> {
			{ "test.Name", null }
		};

		var result = await _client.GetErrors("Test1", form);

		result.IsValidField("test.Name").ShouldBeFalse();
		result.GetError("test.Name").ShouldEqual("Validation Failed");
	}

	[Fact]
	public async Task Validators_should_be_scoped() {
		var result = await _client.GetErrors("Lifecycle");
		var hashCode1 = result.GetError("Foo");

		var result2 = await _client.GetErrors("Lifecycle");
		var hashCode2 = result2.GetError("Foo");

		Assert.NotNull(hashCode1);
		Assert.NotNull(hashCode2);
		Assert.NotEqual("", hashCode1);
		Assert.NotEqual("", hashCode2);

		Assert.NotEqual(hashCode1, hashCode2);
	}

	[Fact]
	public async Task Gets_validator_for_model_not_underlying_collection_type() {
		var result = await _client.GetErrors("ModelThatimplementsIEnumerable");
		result.GetError("Name").ShouldEqual("Foo");
	}
}
