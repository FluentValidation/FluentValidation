namespace FluentValidation.Mvc {
#if !CoreCLR
    using System.Web.Mvc;
#else
    using Microsoft.AspNet.Mvc;
    using Microsoft.AspNet.Mvc.ModelBinding;
    using Microsoft.Framework.DependencyInjection;
#endif
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
#if !CoreCLR
		ValidationContext BeforeMvcValidation(ControllerContext controllerContext, ValidationContext validationContext);
#else
        ValidationContext BeforeMvcValidation(IContextAccessor<ActionContext> actionContext, ValidationContext validationContext);
#endif

        /// <summary>
        /// Invoked after MVC validation takes place which allows the result to be customized.
        /// It should return a ValidationResult.
        /// </summary>
        /// <param name="controllerContext">Controller Context</param>
        /// <param name="validationContext">Validation Context</param>
        /// <param name="result">The result of validation.</param>
        /// <returns>Validation Context</returns>
#if !CoreCLR
		ValidationResult AfterMvcValidation(ControllerContext controllerContext, ValidationContext validationContext, ValidationResult result);
#else
        ValidationResult AfterMvcValidation(IContextAccessor<ActionContext> actionContext, ValidationContext validationContext, ValidationResult result);
#endif
    }
}