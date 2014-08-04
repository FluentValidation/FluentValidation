namespace FluentValidation.Mvc {
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Internal;
    using Validators;

    internal class MaxFluentValidationPropertyValidator : FluentValidationPropertyValidator {
        LessThanOrEqualValidator RangeValidator
        {
            get { return (LessThanOrEqualValidator)Validator; }
        }
		
        public MaxFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule propertyDescription, IPropertyValidator validator) : base(metadata, controllerContext, propertyDescription, validator) {
            ShouldValidate=false;
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
            if (!ShouldGenerateClientSideRules()) yield break;

            var formatter = new MessageFormatter()
                .AppendPropertyName(Rule.GetDisplayName())
                .AppendArgument("ComparisonValue", RangeValidator.ValueToCompare);
            var message = formatter.BuildMessage(RangeValidator.ErrorMessageSource.GetString());
            yield return new ModelClientValidationRangeRule(message, null, RangeValidator.ValueToCompare);
        }
    }
}