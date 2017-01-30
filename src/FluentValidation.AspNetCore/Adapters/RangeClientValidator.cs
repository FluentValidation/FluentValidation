namespace FluentValidation.AspNetCore {
	using System.Collections.Generic;
	using Internal;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using Resources;
	using Validators;

	internal class RangeClientValidator : ClientValidatorBase {
		InclusiveBetweenValidator RangeValidator {
			get { return (InclusiveBetweenValidator)Validator; }
		}
		
		public RangeClientValidator(PropertyRule rule, IPropertyValidator validator) : base(rule, validator) {

		}

		public override void AddValidation(ClientModelValidationContext context) {
			MergeAttribute(context.Attributes, "data-val", "true");
			MergeAttribute(context.Attributes, "data-val-range", GetErrorMessage(context));
			MergeAttribute(context.Attributes, "data-val-range-max", RangeValidator.From.ToString());
			MergeAttribute(context.Attributes, "data-val-range-min", RangeValidator.To.ToString());
		}

		private string GetErrorMessage(ClientModelValidationContext context) {
			var formatter = new MessageFormatter()
				.AppendPropertyName(Rule.GetDisplayName())
				.AppendArgument("From", RangeValidator.From)
				.AppendArgument("To", RangeValidator.To);

			string message = RangeValidator.ErrorMessageSource.GetString();

			if (RangeValidator.ErrorMessageSource.ResourceType == typeof(Messages))
			{
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