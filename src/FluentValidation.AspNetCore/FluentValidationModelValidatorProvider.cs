namespace FluentValidation.AspNetCore {
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.DataAnnotations.Internal;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;


/*
	/// <summary>
	/// ModelValidatorProvider implementation only used for child properties.
	/// </summary>
	internal class FluentValidationModelValidatorProvider : IModelValidatorProvider {

		private IModelValidatorProvider _inner;
		private readonly bool _shouldExecute;
		private IValidatorFactory _validatorFactory;
		private CustomizeValidatorAttribute _customizations;

		public FluentValidationModelValidatorProvider(IValidatorFactory validatorFactory, CustomizeValidatorAttribute customizations, IModelValidatorProvider inner) {
			_validatorFactory = validatorFactory;
			_customizations = customizations;
			_inner = inner;
		}

		public void CreateValidators(ModelValidatorProviderContext context) {

				var validator = _validatorFactory.GetValidator(context.ModelMetadata.ModelType);

				if (validator != null) {
					context.Results.Add(new ValidatorItem {
						IsReusable = false,
						Validator = new FluentValidationModelValidator(validator, _customizations)
					});
				}
			

			_inner.CreateValidators(context);
		}
	}
*/

	internal class FluentValidationModelValidator : IModelValidator {
		private IValidator _validator;
		private CustomizeValidatorAttribute _customizations;

		public FluentValidationModelValidator(IValidator validator, CustomizeValidatorAttribute customizations) {
			_validator = validator;
			_customizations = customizations;
		}

		public IEnumerable<ModelValidationResult> Validate(ModelValidationContext mvContext) {
			var selector = _customizations.ToValidatorSelector();
			var interceptor = _customizations.GetInterceptor() ?? (_validator as IValidatorInterceptor);
			var context = new FluentValidation.ValidationContext(mvContext.Model, new FluentValidation.Internal.PropertyChain(), selector);

			if (interceptor != null) {
				// Allow the user to provide a customized context
				// However, if they return null then just use the original context.
				context = interceptor.BeforeMvcValidation((ControllerContext)mvContext.ActionContext, context) ?? context;
			}

			var result = _validator.Validate(context);

			if (interceptor != null) {
				// allow the user to provice a custom collection of failures, which could be empty.
				// However, if they return null then use the original collection of failures. 
				result = interceptor.AfterMvcValidation((ControllerContext)mvContext.ActionContext, context, result) ?? result;
			}

			return result.Errors.Select(x => new ModelValidationResult(x.PropertyName, x.ErrorMessage));
		}
	}
}