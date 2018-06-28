namespace FluentValidation.Tests.AspNetCore {
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Logging;
	using FluentValidation.AspNetCore;
	using FluentValidation.Attributes;
	using System.Globalization;
	using Microsoft.AspNetCore.Localization;

	public class StartupWithMvcValidationDisabled
    {
        public StartupWithMvcValidationDisabled(IHostingEnvironment env)
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
	            cfg.ValidatorFactoryType = typeof(AttributedValidatorFactory);
	            cfg.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
            });
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
}