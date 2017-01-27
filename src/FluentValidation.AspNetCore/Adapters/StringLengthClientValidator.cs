namespace FluentValidation.AspNetCore {
	using System.Collections.Generic;
	using Internal;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using Resources;
	using Validators;

	internal class StringLengthClientValidator : ClientValidatorBase {
		public StringLengthClientValidator(PropertyRule rule, IPropertyValidator validator)
			: base(rule, validator) {
		}

		public override void AddValidation(ClientModelValidationContext context) {
			var lengthVal = (LengthValidator)Validator;

			MergeAttribute(context.Attributes, "data-val", "true");
			MergeAttribute(context.Attributes, "data-val-length", GetErrorMessage(lengthVal, context));
			MergeAttribute(context.Attributes, "data-val-length-max", lengthVal.Max.ToString());
			MergeAttribute(context.Attributes, "data-val-length-min", lengthVal.Min.ToString());
		}

		private string GetErrorMessage(LengthValidator lengthVal, ClientModelValidationContext context) {

			var formatter = new MessageFormatter()
				.AppendPropertyName(Rule.GetDisplayName())
				.AppendArgument("MinLength", lengthVal.Min)
				.AppendArgument("MaxLength", lengthVal.Max);

			string message = lengthVal.ErrorMessageSource.GetString();

			if (lengthVal.ErrorMessageSource.ResourceType == typeof(Messages))
			{
				// If we're using the default resources then the mesage for length errors will have two parts, eg:
				// '{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.
				// We can't include the "TotalLength" part of the message because this information isn't available at the time the message is constructed.
				// Instead, we'll just strip this off by finding the index of the period that separates the two parts of the message.

				message = message.Substring(0, message.IndexOf(".") + 1);
			}

			message = formatter.BuildMessage(message);
			return message;
		}
	}
}