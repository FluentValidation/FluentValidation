using System.Web.Mvc;
using System.Web.Routing;

namespace FluentValidation.MvcIntegrationDemo {
	using Attributes;
	using Mvc;

	public class MvcApplication : System.Web.HttpApplication {
		public static void RegisterRoutes(RouteCollection routes) {
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new { controller = "People", action = "Index", id = "" } // Parameter defaults
			);


			DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
			ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider(new AttributedValidatorFactory()));

		}

		protected void Application_Start() {
			RegisterRoutes(RouteTable.Routes);
		}
	}
}