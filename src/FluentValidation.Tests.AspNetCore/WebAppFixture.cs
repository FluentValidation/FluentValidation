namespace FluentValidation.Tests {
	using System;
	using AspNetCore;
	using AspNetCore.Controllers;
	using Attributes;
	using FluentValidation.AspNetCore;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc.Testing;
	using Microsoft.Extensions.DependencyInjection;

	public class WebAppFixture : WebApplicationFactory<Startup> {
		protected override void ConfigureWebHost(IWebHostBuilder builder) {
			builder.UseContentRoot(".");
		}

		protected override IWebHostBuilder CreateWebHostBuilder() {
			return new WebHostBuilder()
				.UseDefaultServiceProvider((context, options) => options.ValidateScopes = true)
				.UseStartup<Startup>();
		}

		public WebApplicationFactory<Startup> WithContainer(bool registerContextAccessor = true, bool enableLocalization = false) {
			return WithWebHostBuilder(cfg => {
				cfg.ConfigureServices(services => {

					if (enableLocalization) {
						services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });
					}

					services.AddFluentValidationForTesting(fv => {
						fv.RegisterValidatorsFromAssemblyContaining<TestController>();
					});
					if (registerContextAccessor) {
						services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
					}

					services.AddScoped<ClientsideScopedDependency>();
				});
			});
		}

		public WebApplicationFactory<Startup> WithDataAnnotationsDisabled() {
			return WithFluentValidation(fv => {
				fv.ValidatorFactoryType = typeof(AttributedValidatorFactory);
				fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
			});
		}

		public WebApplicationFactory<Startup> WithImplicitValidationEnabled(bool enabled) {
			return WithFluentValidation(fv => {
				fv.ValidatorFactoryType = typeof(AttributedValidatorFactory);
				fv.ImplicitlyValidateChildProperties = enabled;
			});
		}

		public WebApplicationFactory<Startup> WithImplicitCollectionValidationEnabled(bool enabled) {
			return WithFluentValidation(fv => {
				fv.ValidatorFactoryType = typeof(AttributedValidatorFactory);
				fv.ImplicitlyValidateChildProperties = false;
				fv.ImplicitlyValidateRootCollectionElements = enabled;
			});
		}

		public WebApplicationFactory<Startup> WithFluentValidation(Action<FluentValidationMvcConfiguration> config) {
			return WithWebHostBuilder(cfg => {
				cfg.ConfigureServices(services => {
					services.AddFluentValidationForTesting(config);
				});
			});
		}
	}
}
