namespace FluentValidation.Mvc {
	using System.Web.Mvc;
	using Results;

	public interface IValidatorInterceptor {
		ValidationContext BeforeMvcValidation(ControllerContext controllerContext, ValidationContext validationContext);
		ValidationResult AfterMvcValidation(ControllerContext controllerContext, ValidationContext validationContext, ValidationResult result);
	}
}