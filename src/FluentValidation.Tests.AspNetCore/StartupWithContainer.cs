namespace FluentValidation.Tests.AspNetCore {
	using System.Globalization;
	using Controllers;
	using FluentValidation.AspNetCore;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Localization;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Logging;

	public class StartupWithContainer {
		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) {
			services.AddMvc(setup => {
#if NETCOREAPP3_0
					setup.EnableEndpointRouting = false;
#endif
				})
#if NETCOREAPP3_0
				.AddNewtonsoftJson()
#endif
				.AddFluentValidation(cfg => { cfg.RegisterValidatorsFromAssemblyContaining<TestController>(); });
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddScoped<ClientsideScopedDependency>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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

	public class StartupWithContainerWithoutHttpContextAccessor {
		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) {
			services.AddMvc(setup => {
#if NETCOREAPP3_0
				setup.EnableEndpointRouting = false;
#endif
			}).AddFluentValidation(cfg => { cfg.RegisterValidatorsFromAssemblyContaining<TestController>(); });
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
}