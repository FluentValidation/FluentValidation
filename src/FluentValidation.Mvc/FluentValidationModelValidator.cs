namespace FluentValidation.Mvc {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;
	using Results;

	/// <summary>
	/// ModelValidator implementation that uses FluentValidation.
	/// </summary>
	public class FluentValidationModelValidator : ModelValidator {
		protected IValidatorFactory ValidatorFactory { get; private set; }

		public FluentValidationModelValidator(ModelMetadata metadata, ControllerContext controllerContext, IValidatorFactory factory)
			: base(metadata, controllerContext) {
			this.ValidatorFactory = factory;
		}

		public override IEnumerable<ModelValidationResult> Validate(object container) {
			if (Metadata.Model != null) {

				var validator = CreateValidator(Metadata.ModelType);

				if (validator != null) {
					var result = Validate(validator, Metadata);

					if (!result.IsValid) {
						return ConvertValidationResultToModelValidationResults(result);
					}
				}
			}
			return Enumerable.Empty<ModelValidationResult>();
		}

		protected virtual IEnumerable<ModelValidationResult> ConvertValidationResultToModelValidationResults(ValidationResult result) {
			return result.Errors.Select(x => new ModelValidationResult {
				MemberName = x.PropertyName,
				Message = x.ErrorMessage
			});
		}

		protected virtual ValidationResult Validate(IValidator validator, ModelMetadata metadata) {
			return validator.Validate(Metadata.Model);
		}

		protected virtual IValidator CreateValidator(Type type) {
			return ValidatorFactory.GetValidator(type);
		}
	}
}