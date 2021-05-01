namespace FluentValidation.Tests.AspNetCore {
	using System.Globalization;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Localization;
	using Microsoft.Extensions.DependencyInjection;

	public class Startup {
		public void ConfigureServices(IServiceCollection services) {
			// Intentionally not implemented - each test fixture should configure services explicitly.
		}

		public void Configure(IApplicationBuilder app) {
			CultureInfo cultureInfo = new CultureInfo("en-US");
			app.UseRequestLocalization(options => {
				options.DefaultRequestCulture = new RequestCulture(cultureInfo);
				options.SupportedCultures = new[] {cultureInfo};
				options.SupportedUICultures = new[] {cultureInfo};
			});

			app
				.UseRouting()
				.UseEndpoints(endpoints => {
					endpoints.MapRazorPages();
					endpoints.MapDefaultControllerRoute();
				});
		}
	}
}
