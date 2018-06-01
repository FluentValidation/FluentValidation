namespace FluentValidation.Tests.WebApi {
	using System.Web.Http;
	using FluentValidation.WebApi;
	using Owin;

	public class StartupRootValidationOnly {
		public void Configuration(IAppBuilder app) {
			var config = new HttpConfiguration();
			config.Routes.MapHttpRoute("Default", "api/{controller}/{action}/{id}", new {id = RouteParameter.Optional});
			config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
			FluentValidationModelValidatorProvider.Configure(config, provider => provider.ImplicitlyValidateChildProperties = false);
			app.UseWebApi(config);
		}
	}
}