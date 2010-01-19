namespace FluentValidation.Mvc {
	using System.Collections.Generic;
	using System.Web.Mvc;
	using Validators;

	internal class FluentValidationPropertyValidator : ModelValidator {
		readonly IPropertyValidator validator;

		public FluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, IPropertyValidator validator) : base(metadata, controllerContext) {
			this.validator = validator;
		}

		public override IEnumerable<ModelValidationResult> Validate(object container) {
			var context = new PropertyValidatorContext(Metadata.PropertyName, container, Metadata.Model, Metadata.PropertyName);
			var result = validator.Validate(context);

			if(! result.IsValid) {
				yield return new ModelValidationResult {
                    Message = result.Error
				};
			}
		}


	}
}