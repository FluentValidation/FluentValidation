namespace FluentValidation.AspNetCore {
	using Internal;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using Resources;
	using Validators;

	internal class RangeMaxClientValidator : ClientValidatorBase {
		LessThanOrEqualValidator RangeValidator {
			get { return (LessThanOrEqualValidator)Validator; }
		}

		public RangeMaxClientValidator(PropertyRule rule, IPropertyValidator validator) : base(rule, validator) {

		}

		public override void AddValidation(ClientModelValidationContext context) {
			var compareValue = RangeValidator.ValueToCompare;

			if (compareValue != null) {
				MergeAttribute(context.Attributes, "data-val", "true");
				MergeAttribute(context.Attributes, "data-val-range", GetErrorMessage(context));
				MergeAttribute(context.Attributes, "data-val-range-max", compareValue.ToString());
				MergeAttribute(context.Attributes, "data-val-range-min", "0");
			}
		}

		private string GetErrorMessage(ClientModelValidationContext context) {
			
			var formatter = ValidatorOptions.MessageFormatterFactory()
				.AppendPropertyName(Rule.GetDisplayName())
				.AppendArgument("ComparisonValue", RangeValidator.ValueToCompare);

			string message;

			try {
				message = RangeValidator.Options.ErrorMessageSource.GetString(null);
			} catch (FluentValidationMessageFormatException) {
				message = ValidatorOptions.LanguageManager.GetStringForValidator<LessThanOrEqualValidator>();
			}

			message = formatter.BuildMessage(message);

			return message;
		}
	}
}