namespace FluentValidation.Mvc {
	using System;
	using System.Collections.Generic;
	using System.Web.Mvc;
	using Internal;
	using Validators;
	using System.Linq;

	public class FluentValidationPropertyValidator : ModelValidator {
		protected readonly string propertyDescription;
		protected readonly IPropertyValidator validator;

		/*
		 This might seem a bit strange, but we do *not* want to do any work in these validators.
		 They should only be used for metadata purposes.
		 This is so that the validation can be left to the actual FluentValidationModelValidator.
		 The exception to this is the Required validator - these *do* need to run standalone
		 in order to bypass MVC's "A value is required" message which cannot be turned off.
		 Basically, this is all just to bypass the bad design in ASP.NET MVC. Boo, hiss. 
		*/
		protected bool ShouldValidate { get; set; }

		public FluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, string propertyDescription, IPropertyValidator validator) : base(metadata, controllerContext) {
			this.propertyDescription = propertyDescription;
			this.validator = validator;
		}

		public override IEnumerable<ModelValidationResult> Validate(object container) {
			if (ShouldValidate) {
				var fakeRule = new PropertyRule(null, x => Metadata.Model, null, null, Metadata.ModelType, null) {
					PropertyName = Metadata.PropertyName
				};

				var fakeParentContext = new ValidationContext(container);
				var context = new PropertyValidatorContext(fakeParentContext, fakeRule, Metadata.PropertyName);
				var result = validator.Validate(context);

				foreach (var failure in result) {
					yield return new ModelValidationResult { Message = failure.ErrorMessage };
				}
			}
		}

		protected bool TypeAllowsNullValue(Type type) {
			return (!type.IsValueType || Nullable.GetUnderlyingType(type) != null);
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			var supportsClientValidation = validator as IClientValidatable;
			
			if(supportsClientValidation != null) {
				return supportsClientValidation.GetClientValidationRules(Metadata, ControllerContext);
			}

			return Enumerable.Empty<ModelClientValidationRule>();
		}
	}
}