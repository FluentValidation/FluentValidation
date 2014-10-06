namespace FluentValidation.Mvc {
	using System;
	using System.Collections.Generic;
	using System.Linq;
#if !CoreCLR
    using System.Web.Mvc;
#else
    using Microsoft.AspNet.Mvc;
    using Microsoft.AspNet.Mvc.ModelBinding;
    using Microsoft.Framework.DependencyInjection;
#endif
    using Internal;
	using Results;

    /// <summary>
    /// ModelValidator implementation that uses FluentValidation.
    /// </summary>
#if !CoreCLR
	internal class FluentValidationModelValidator : ModelValidator {
#else
    internal class FluentValidationModelValidator : IModelValidator {
        private IContextAccessor<ActionContext> _actionContext;
#endif
        readonly IValidator validator;
		readonly CustomizeValidatorAttribute customizations;

#if CoreCLR
        public bool IsRequired { get; set; }
#endif

#if !CoreCLR
        public FluentValidationModelValidator(ModelMetadata metadata, ControllerContext _context, IValidator validator)
			: base(metadata, _context) {
#else
        public FluentValidationModelValidator(ModelMetadata metadata, IContextAccessor<ActionContext> _context, IValidator validator) {
            this._actionContext = _context;
#endif
            this.validator = validator;

#if !CoreCLR
			this.customizations = CustomizeValidatorAttribute.GetFromControllerContext(_context) 
#else
            this.customizations = CustomizeValidatorAttribute.GetFromActionContext(_context)
#endif
            ?? new CustomizeValidatorAttribute();
		}

#if !CoreCLR
        public override IEnumerable<ModelValidationResult> Validate(object container) {
#else
        public IEnumerable<ModelValidationResult> Validate(ModelValidationContext validationContext) {
            ModelMetadata Metadata = validationContext.ModelMetadata;
#endif
            if (Metadata.Model != null) {
				var selector = customizations.ToValidatorSelector();
				var interceptor = customizations.GetInterceptor() ?? (validator as IValidatorInterceptor);
				var context = new ValidationContext(Metadata.Model, new PropertyChain(), selector);

				if(interceptor != null) {
                    // Allow the user to provide a customized context
                    // However, if they return null then just use the original context.
#if !CoreCLR
					context = interceptor.BeforeMvcValidation(ControllerContext, context) ?? context;
#else
                    context = interceptor.BeforeMvcValidation(_actionContext, context) ?? context;
#endif
                }

                var result = validator.Validate(context);

				if(interceptor != null) {
                    // allow the user to provice a custom collection of failures, which could be empty.
                    // However, if they return null then use the original collection of failures. 
#if !CoreCLR
					result = interceptor.AfterMvcValidation(ControllerContext, context, result) ?? result;
#else
                    result = interceptor.AfterMvcValidation(_actionContext, context, result) ?? result;
#endif
                }

                if (!result.IsValid) {
					return ConvertValidationResultToModelValidationResults(result);
				}
			}
			return Enumerable.Empty<ModelValidationResult>();
		}

		protected virtual IEnumerable<ModelValidationResult> ConvertValidationResultToModelValidationResults(ValidationResult result) {
#if !CoreCLR
            return result.Errors.Select(x => new ModelValidationResult {
				MemberName = x.PropertyName,
				Message = x.ErrorMessage
			});
#else
            return result.Errors.Select(x => new ModelValidationResult(x.PropertyName, x.ErrorMessage));
#endif
        }
    }
}