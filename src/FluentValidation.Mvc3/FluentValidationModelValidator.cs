namespace FluentValidation.Mvc {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;
	using Internal;
	using Results;

	/// <summary>
	/// ModelValidator implementation that uses FluentValidation.
	/// </summary>
	internal class FluentValidationModelValidator : ModelValidator {
		readonly IValidator validator;
		readonly CustomizeValidatorAttribute customizations;

		public FluentValidationModelValidator(ModelMetadata metadata, ControllerContext controllerContext, IValidator validator)
			: base(metadata, controllerContext) {
			this.validator = validator;
			
			this.customizations = CustomizeValidatorAttribute.GetFromControllerContext(controllerContext) 
				?? new CustomizeValidatorAttribute();
		}

		public override IEnumerable<ModelValidationResult> Validate(object container) {
			if (Metadata.Model != null) {
				IValidatorSelector selector = new DefaultValidatorSelector();
				
				if(! string.IsNullOrEmpty(customizations.RuleSet)) {
					selector = new RulesetValidatorSelector(customizations.RuleSet);
				}
				else {
					var props = customizations.GetProperties();
					if(props.Length > 0) {
						selector = new MemberNameValidatorSelector(props);
					}
				}

				var context = new ValidationContext(Metadata.Model, new PropertyChain(), selector);

				var result = validator.Validate(context);

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