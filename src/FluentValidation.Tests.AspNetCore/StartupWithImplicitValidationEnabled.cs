namespace FluentValidation.Tests.AspNetCore {
/*
	public class StartupWithImplicitValidationEnabled
    {
        public StartupWithImplicitValidationEnabled(IHostingEnvironment env)
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
	            cfg.ImplicitlyValidateChildProperties = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
*/
}