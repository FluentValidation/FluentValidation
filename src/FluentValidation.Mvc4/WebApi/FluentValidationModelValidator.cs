namespace FluentValidation.Mvc.WebApi
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Http.Metadata;
	using System.Web.Http.Validation;

	using FluentValidation.Internal;
	using FluentValidation.Results;

	public class FluentValidationModelValidator : ModelValidator {
		readonly IValidator validator;

		public FluentValidationModelValidator(IEnumerable<ModelValidatorProvider> validatorProviders, IValidator validator)
			: base(validatorProviders) {
			this.validator = validator;
		}

		public override IEnumerable<ModelValidationResult> Validate(ModelMetadata metadata, object container) {
			if (metadata.Model != null) {
				var selector = new DefaultValidatorSelector();
				var context = new ValidationContext(metadata.Model, new PropertyChain(), selector);

				var result = validator.Validate(context);

				if (!result.IsValid) {
					return ConvertValidationResultToModelValidationResults(result);
				}
			}
			return Enumerable.Empty<ModelValidationResult>();
		}

		protected virtual IEnumerable<ModelValidationResult> ConvertValidationResultToModelValidationResults(ValidationResult result) {
			return result.Errors.Select(x => new ModelValidationResult
			{
				MemberName = x.PropertyName,
				Message = x.ErrorMessage
			});
		}
	}
}