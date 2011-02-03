namespace FluentValidation.Mvc {
	using System.Web.Mvc;
	using Results;

	/// <summary>
	/// Specifies an interceptor that can be used to provide hooks that will be called before and after MVC validation occurs.
	/// </summary>
	public interface IValidatorInterceptor {
		/// <summary>
		/// Invoked before MVC validation takes place which allows the ValidationContext to be customized prior to validation.
		/// It should return a ValidationContext object.
		/// </summary>
		/// <param name="controllerContext">Controller Context</param>
		/// <param name="validationContext">Validation Context</param>
		/// <returns>Validation Context</returns>
		ValidationContext BeforeMvcValidation(ControllerContext controllerContext, ValidationContext validationContext);

		/// <summary>
		/// Invoked after MVC validation takes place which allows the result to be customized.
		/// It should return a ValidationResult.
		/// </summary>
		/// <param name="controllerContext">Controller Context</param>
		/// <param name="validationContext">Validation Context</param>
		/// <param name="result">The result of validation.</param>
		/// <returns>Validation Context</returns>
		ValidationResult AfterMvcValidation(ControllerContext controllerContext, ValidationContext validationContext, ValidationResult result);
	}
}