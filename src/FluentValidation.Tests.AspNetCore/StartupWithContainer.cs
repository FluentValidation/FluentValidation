namespace FluentValidation.Tests.AspNetCore {
	using Controllers;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Logging;
	using FluentValidation.AspNetCore;
	using Microsoft.AspNetCore.Http;
	using System.Globalization;
	using Microsoft.AspNetCore.Localization;

	public class StartupWithContainer
    {

        public StartupWithContainer(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(setup => {
                
            }).AddFluentValidation(cfg => {
	            cfg.RegisterValidatorsFromAssemblyContaining<TestController>();
            });
	        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
	        services.AddScoped<ClientsideScopedDependency>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            CultureInfo cultureInfo = new CultureInfo("en-US");
            app.UseRequestLocalization(options => {
                options.DefaultRequestCulture = new RequestCulture(cultureInfo);
                options.SupportedCultures = new []{ cultureInfo };
                options.SupportedUICultures = new []{ cultureInfo };
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

	public class StartupWithContainerWithoutHttpContextAccessor {

		public StartupWithContainerWithoutHttpContextAccessor(IHostingEnvironment env) {
			var builder = new ConfigurationBuilder();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) {
			services.AddMvc(setup => {

			}).AddFluentValidation(cfg => {
				cfg.RegisterValidatorsFromAssemblyContaining<TestController>();
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
			CultureInfo cultureInfo = new CultureInfo("en-US");
			app.UseRequestLocalization(options => {
				options.DefaultRequestCulture = new RequestCulture(cultureInfo);
				options.SupportedCultures = new []{ cultureInfo };
				options.SupportedUICultures = new []{ cultureInfo };
			});

			app.UseMvc(routes => {
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}