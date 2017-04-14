namespace FluentValidation.Mvc {
    using System.Web.Mvc;
    using Internal;
    using Resources;
    using Validators;

    internal class MaxFluentValidationPropertyValidator : AbstractComparisonFluentValidationPropertyValidator<LessThanOrEqualValidator> {

        protected override object MinValue {
            get { return null; }
        }

        protected override object MaxValue {
            get { return AbstractComparisonValidator.ValueToCompare; }
        }

        public MaxFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule propertyDescription, IPropertyValidator validator)
            : base(metadata, controllerContext, propertyDescription, validator) {
        }

	    protected override string GetDefaultMessage() {
		    return ValidatorOptions.LanguageManager.GetStringForValidator<LessThanOrEqualValidator>() ;
	    }
    }
}