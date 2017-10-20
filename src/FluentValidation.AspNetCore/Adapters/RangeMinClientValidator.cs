namespace FluentValidation.AspNetCore {
	using Internal;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using Resources;
	using Validators;

	internal class RangeMinClientValidator : ClientValidatorBase {
		GreaterThanOrEqualValidator RangeValidator {
			get { return (GreaterThanOrEqualValidator)Validator; }
		}

		public RangeMinClientValidator(PropertyRule rule, IPropertyValidator validator) : base(rule, validator) {

		}

		public override void AddValidation(ClientModelValidationContext context) {
			var compareValue = RangeValidator.ValueToCompare;

			if (compareValue != null) {
				MergeAttribute(context.Attributes, "data-val", "true");
				MergeAttribute(context.Attributes, "data-val-range", GetErrorMessage(context));
//				MergeAttribute(context.Attributes, "data-val-range-max", "-1");
				MergeAttribute(context.Attributes, "data-val-range-min", compareValue.ToString());
			}
		}

		private string GetErrorMessage(ClientModelValidationContext context) {
			var formatter = ValidatorOptions.MessageFormatterFactory()
				.AppendPropertyName(Rule.GetDisplayName())
				.AppendArgument("ComparisonValue", RangeValidator.ValueToCompare);

			var messageNeedsSplitting = RangeValidator.ErrorMessageSource.ResourceType == typeof(LanguageManager);

			string message;

			try {
				message = RangeValidator.ErrorMessageSource.GetString(null);
			} catch (FluentValidationMessageFormatException) {
				message = ValidatorOptions.LanguageManager.GetStringForValidator<GreaterThanOrEqualValidator>();
				messageNeedsSplitting = true;
			}

			if (messageNeedsSplitting) {
				// If we're using the default resources then the mesage for length errors will have two parts, eg:
				// '{PropertyName}' must be between {From} and {To}. You entered {Value}.
				// We can't include the "Value" part of the message because this information isn't available at the time the message is constructed.
				// Instead, we'll just strip this off by finding the index of the period that separates the two parts of the message.

				message = message.Substring(0, message.IndexOf(".") + 1);
			}
			message = formatter.BuildMessage(message);

			return message;
		}
	}
}