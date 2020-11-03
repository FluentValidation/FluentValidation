namespace FluentValidation.Tests {
	using System.Net.Http;
	using System.Threading.Tasks;
	using AspNetCore;
	using AspNetCore.Controllers;
	using Microsoft.AspNetCore.Http;
	using Microsoft.Extensions.DependencyInjection;
	using Xunit;
	using Xunit.Abstractions;

	public class DependencyInjectionTests : IClassFixture<WebAppFixture> {
		private readonly ITestOutputHelper _output;
		private readonly HttpClient _client;

		public DependencyInjectionTests(ITestOutputHelper output, WebAppFixture webApp) {
			CultureScope.SetDefaultCulture();

			_output = output;
			_client = webApp.WithWebHostBuilder(webHostBuilder => {
					webHostBuilder.ConfigureServices(services => {
						services.AddFluentValidationForTesting(fv => {
							fv.ImplicitlyValidateChildProperties = false;
						});
						services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
						services.AddScoped<IValidator<ParentModel>, InjectsExplicitChildValidator>();
						services.AddScoped<IValidator<ChildModel>, InjectedChildValidator>();
						services.AddScoped<IValidator<ParentModel6>, InjectsExplicitChildValidatorCollection>();
					});
				})
				.CreateClient();
		}

		[Fact]
		public async Task Resolves_explicit_child_validator() {
			var result = await _client.GetErrors("InjectsExplicitChildValidator", new FormData());
			result.IsValidField("Child.Name").ShouldBeFalse();
			result.GetError("Child.Name").ShouldEqual("NotNullInjected");
		}

		[Fact]
		public async Task Resolves_explicit_child_validator_for_collection() {
			var formData = new FormData();
			formData.Add("Children[0].Name", null);
			var result = await _client.GetErrors("InjectsExplicitChildValidatorCollection", formData);
			result.IsValidField("Children[0].Name").ShouldBeFalse();
			result.GetError("Children[0].Name").ShouldEqual("NotNullInjected");
		}
	}
}
