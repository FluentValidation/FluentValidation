namespace FluentValidation.Tests {
	using System.Net.Http;
	using AspNetCore;
	using AspNetCore.Controllers;
	using FluentValidation.AspNetCore;
	using Microsoft.Extensions.DependencyInjection;
	using Xunit;

	public class ImplicitRootCollectionTests : IClassFixture<WebAppFixture> {
		private readonly WebAppFixture _app;

		public ImplicitRootCollectionTests(WebAppFixture app) {
			_app = app;
		}

		private HttpClient CreateClient(bool implicitCollectionValidationEnabled) {
			return _app.CreateClientWithServices(services => {
				services.AddMvc().AddNewtonsoftJson().AddFluentValidation(fv => {
					fv.ImplicitlyValidateRootCollectionElements = implicitCollectionValidationEnabled;
				});
				services.AddScoped<IValidator<ParentModel>, ParentModelValidator>();
				services.AddScoped<IValidator<ChildModel>, ChildModelValidator>();
			});
		}

		[Fact]
		public async void Does_not_implicitly_run_root_collection_element_validator_when_disabled() {
			var client = CreateClient(false);
			var result = await client.GetErrorsViaJSON(
				nameof(TestController.ImplicitRootCollectionElementValidator),
				new[] { new ChildModel() });

			result.Count.ShouldEqual(0);
		}

		[Fact]
		public async void Does_not_implicitly_run_child_validator_when_root_collection_element_validation_enabled() {
			var client = CreateClient(true);
			var result = await client.GetErrorsViaJSON(
				nameof(TestController.ImplicitRootCollectionElementValidationEnabled),
				new ParentModel());

			result.Count.ShouldEqual(0);
		}

		[Fact]
		public async void Executes_implicit_root_collection_element_validator_when_enabled() {
			var client = CreateClient(true);
			var result = await client.GetErrorsViaJSON(
				nameof(TestController.ImplicitRootCollectionElementValidator),
				new[] { new ChildModel() });

			result.Count.ShouldEqual(1);
			result[0].Name.ShouldEqual("[0].Name");
		}

	}
}
