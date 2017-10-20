namespace FluentValidation.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Internal;
    using Resources;
    using Validators;

    internal abstract class AbstractComparisonFluentValidationPropertyValidator<TValidator> : FluentValidationPropertyValidator 
        where TValidator: AbstractComparisonValidator {

        protected TValidator AbstractComparisonValidator
        {
            get { return (TValidator)Validator; }
        }

        protected abstract Object MinValue { get; }
        protected abstract Object MaxValue { get; }

        protected AbstractComparisonFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule propertyDescription, IPropertyValidator validator) : base(metadata, controllerContext, propertyDescription, validator) {
            ShouldValidate=false;
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
            if (!ShouldGenerateClientSideRules()) yield break;

            var formatter = ValidatorOptions.MessageFormatterFactory()
                .AppendPropertyName(Rule.GetDisplayName())
                .AppendArgument("ComparisonValue", AbstractComparisonValidator.ValueToCompare);
            string message;
	        try {
		        message = AbstractComparisonValidator.ErrorMessageSource.GetString(null);
	        }
	        catch (FluentValidationMessageFormatException) {
		        message = GetDefaultMessage();
	        }
	        message = formatter.BuildMessage(message);
	        yield return new ModelClientValidationRangeRule(message, MinValue, MaxValue);
        }

	    protected abstract string GetDefaultMessage();
    }
}