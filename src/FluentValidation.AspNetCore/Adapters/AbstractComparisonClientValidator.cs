namespace FluentValidation.AspNetCore {
    using System;
    using System.Collections.Generic;
    using Internal;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
    using Validators;

    internal abstract class AbstractComparisonClientValidator<TValidator> : ClientValidatorBase 
        where TValidator: AbstractComparisonValidator {

        protected TValidator AbstractComparisonValidator
        {
            get { return (TValidator)Validator; }
        }

        protected abstract Object MinValue { get; }
        protected abstract Object MaxValue { get; }

        protected AbstractComparisonClientValidator(PropertyRule rule, IPropertyValidator validator) : base(rule, validator) {
        }

		protected string GetErrorMessage(ClientModelValidationContext context) {
			var formatter = new MessageFormatter()
			  .AppendPropertyName(Rule.GetDisplayName())
			  .AppendArgument("ComparisonValue", AbstractComparisonValidator.ValueToCompare);
			var message = formatter.BuildMessage(AbstractComparisonValidator.ErrorMessageSource.GetString());

			return message;
		}
	}
}