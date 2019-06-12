namespace FluentValidation.Tests.AspNetCore {
	using System;
	using System.Globalization;
	using FluentValidation.AspNetCore;
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

			app.UseMvc(routes => {
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}

	public static class WebTestExtensions {

		public static void AddFluentValidationForTesting(this IServiceCollection services, Action<FluentValidationMvcConfiguration> configurator) {
#if NETCOREAPP3_0
			var mvcBuilder = services.AddMvc(mvc => mvc.EnableEndpointRouting = false).AddNewtonsoftJson();
#else
			var mvcBuilder = services.AddMvc();
#endif
			mvcBuilder.AddFluentValidation(configurator);
		}
	}

}
