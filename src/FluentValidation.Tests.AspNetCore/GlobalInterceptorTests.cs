namespace FluentValidation.Tests;

using System.Collections.Generic;
using System.Threading.Tasks;
using Controllers;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

public class GlobalInterceptorTests : IClassFixture<WebAppFixture> {
	private WebAppFixture _app;

	public GlobalInterceptorTests(WebAppFixture app) {
		_app = app;
	}

	[Fact]
	public async Task When_global_action_context_interceptor_specified_Intercepts_validation_for_razor_pages() {
		var form = new Dictionary<string, string> {
			{"Email", "foo"},
			{"Surname", "foo"},
			{"Forename", "foo"},
		};
		var client = _app.CreateClientWithServices(services => {
#pragma warning disable CS0618
			services.AddMvc().AddNewtonsoftJson().AddFluentValidation();
#pragma warning restore CS0618
			services.AddScoped<IValidator<RulesetTestModel>, RulesetTestValidator>();
			services.AddSingleton<IValidatorInterceptor, SimplePropertyInterceptor>();
		});
		var response = await client.PostResponse($"/RulesetTest", form);
		var result = JsonConvert.DeserializeObject<List<SimpleError>>(response);

		result.IsValidField("Forename").ShouldBeFalse();
		result.IsValidField("Surname").ShouldBeFalse();
		result.IsValidField("Email").ShouldBeTrue();
	}

}
