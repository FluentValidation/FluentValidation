namespace FluentValidation.Tests {
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using AspNetCore;
	using AspNetCore.Controllers;
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
		public async ValueTask When_global_action_context_interceptor_specified_Intercepts_validation_for_razor_pages() {
			var form = new FormData {
				{"Email", "foo"},
				{"Surname", "foo"},
				{"Forename", "foo"},
			};
			var client = _app.CreateClientWithServices(services => {
				services.AddMvc().AddNewtonsoftJson().AddFluentValidation();
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
}
