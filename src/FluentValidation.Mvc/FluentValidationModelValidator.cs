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
		readonly IValidator validator;

		public FluentValidationModelValidator(ModelMetadata metadata, ControllerContext controllerContext, IValidator validator)
			: base(metadata, controllerContext) {
			this.validator = validator;
		}

		public override IEnumerable<ModelValidationResult> Validate(object container) {
			if (Metadata.Model != null) {

				var result = validator.Validate(Metadata.Model);

				if (!result.IsValid) {
					return ConvertValidationResultToModelValidationResults(result);
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
	}
}