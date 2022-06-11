namespace FluentValidation.Tests;

using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class DisableDataAnnotationsTests : IClassFixture<WebAppFixture> {
	private readonly WebAppFixture _app;

	public DisableDataAnnotationsTests(WebAppFixture app) {
		_app = app;
	}

	[Fact]
	public async Task Disables_data_annotations() {
		var client = _app.CreateClientWithServices(services => {
			services.AddMvc().AddNewtonsoftJson().AddFluentValidation(fv => {
				fv.DisableDataAnnotationsValidation = true;
			});
			services.AddScoped<IValidator<MultiValidationModel>, MultiValidationValidator>();
		});

		var result = await client.GetErrors("MultipleValidationStrategies");
		result.Count.ShouldEqual(1);
		result[0].Message.ShouldEqual("'Some Other Property' must not be empty.");
	}

}
