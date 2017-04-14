namespace FluentValidation.Mvc {
    using System.Web.Mvc;
    using Internal;
    using Resources;
    using Validators;

    internal class MinFluentValidationPropertyValidator : AbstractComparisonFluentValidationPropertyValidator<GreaterThanOrEqualValidator> {

        protected override object MinValue {
            get { return AbstractComparisonValidator.ValueToCompare;  }
        }

        protected override object MaxValue {
            get { return null; }
        }

        public MinFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule propertyDescription, IPropertyValidator validator)
            : base(metadata, controllerContext, propertyDescription, validator) {
        }

	    protected override string GetDefaultMessage() {
		    return ValidatorOptions.LanguageManager.GetStringForValidator<GreaterThanOrEqualValidator>();
	    }
    }
}