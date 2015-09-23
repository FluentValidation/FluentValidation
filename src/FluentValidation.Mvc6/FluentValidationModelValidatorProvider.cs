using System.Collections.Generic;
using Microsoft.AspNet.Mvc.ModelBinding.Validation;
using FluentValidation;
using System.Linq;

namespace FluentValidation.Mvc6
{
	//TODO: Need support for CustomizeValidatorAttribute and client-side

	public class FluentValidationModelValidatorProvider : IModelValidatorProvider {
		public IValidatorFactory ValidatorFactory { get; private set; }

		public void GetValidators(ModelValidatorProviderContext context)
		{
			IValidator validator = CreateValidator(context);

			if (! IsValidatingProperty(context))
			{
				context.Validators.Add(new FluentValidationModelValidator(validator));
			}
		}

		protected virtual IValidator CreateValidator(ModelValidatorProviderContext context) {
			if (IsValidatingProperty(context)) {
				return ValidatorFactory.GetValidator(context.ModelMetadata.ContainerType);
			}
			return ValidatorFactory.GetValidator(context.ModelMetadata.ModelType);
		}

		protected virtual bool IsValidatingProperty(ModelValidatorProviderContext context) {
			return context.ModelMetadata.ContainerType != null && !string.IsNullOrEmpty(context.ModelMetadata.PropertyName);
		}
	}

	public class FluentValidationModelValidator : IModelValidator {
		private IValidator _validator;

		public FluentValidationModelValidator(IValidator validator) {
			_validator = validator;
		}

		public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context) {
			var modelExplorer = context.ModelExplorer;
			var metadata = modelExplorer.Metadata;

			var memberName = metadata.PropertyName ?? metadata.ModelType.Name;
			var containerExplorer = modelExplorer.Container;

			var container = containerExplorer?.Model;
			var model = container ?? modelExplorer.Model;

			var result = _validator.Validate(model);

			return from error in result.Errors
				select new ModelValidationResult(error.PropertyName, error.ErrorMessage);

		}

		public bool IsRequired {
			get { return false; }
		}
	}
}