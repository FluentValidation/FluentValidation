namespace FluentValidation.Tests.AspNetCore {
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Logging;
	using FluentValidation.AspNetCore;
	using FluentValidation.Attributes;

	public class StartupWithImplicitValidationDisabled
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(setup => {
#if NETCOREAPP3_0
		            setup.EnableEndpointRouting = false;
#endif
            })
#if NETCOREAPP3_0
	        .AddNewtonsoftJson()
#endif           
			.AddFluentValidation(cfg => {
	            cfg.ValidatorFactoryType = typeof(AttributedValidatorFactory);
	            cfg.ImplicitlyValidateChildProperties = false;
            });
	        

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}