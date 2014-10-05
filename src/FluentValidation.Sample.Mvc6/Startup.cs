using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Security.Cookies;
using Microsoft.Data.Entity;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using FluentValidation.Sample.Mvc6.Models;
using FluentValidation.Attributes;
using Microsoft.AspNet.Mvc;
using FluentValidation.Mvc;

namespace FluentValidation.Sample.Mvc6
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            // Setup configuration sources
            var configuration = new Configuration();
            configuration.AddJsonFile("config.json");
            configuration.AddEnvironmentVariables();

            // Set up application services
            app.UseServices(services =>
            {
                // Add EF services to the services container
                services.AddEntityFramework()
                    .AddSqlServer();

                // Configure DbContext
                services.SetupOptions<DbContextOptions>(options =>
                {
                    options.UseSqlServer(configuration.Get("Data:DefaultConnection:ConnectionString"));
                });
                
                // Add Identity services to the services container
                services.AddIdentitySqlServer<ApplicationDbContext, ApplicationUser>()
                    .AddAuthentication();

                //TODO: portk - Might want to consider alternatives to keep this as optional
                services.AddTransient<IValidatorFactory, AttributedValidatorFactory>();
                
                // Add MVC services to the services container
                services.AddMvc()
                .SetupOptions<MvcOptions>(options =>
                {
                    options.ModelValidatorProviders.Add(typeof(FluentValidationModelValidatorProvider));
                });
            });

            // Enable Browser Link support
            app.UseBrowserLink();

            // Add static files to the request pipeline
            app.UseStaticFiles();

            // Add cookie-based authentication to the request pipeline
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = ClaimsIdentityOptions.DefaultAuthenticationType,
                LoginPath = new PathString("/Account/Login"),
            });

            // Add MVC to the request pipeline
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default", 
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "People", action = "Index" });

                routes.MapRoute(
                    name: "api",
                    template: "{controller}/{id?}");
            });
        }
    }
}
