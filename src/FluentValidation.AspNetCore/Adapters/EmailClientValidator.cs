namespace FluentValidation.AspNetCore {
	using System.Collections.Generic;
	using Internal;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using Resources;
	using Validators;

	internal class EmailClientValidator : ClientValidatorBase {
		private IEmailValidator EmailValidator {
			get { return (IEmailValidator)Validator; }
		}

		public EmailClientValidator(PropertyRule rule, IPropertyValidator validator) : base(rule, validator) {
		}

		public override void AddValidation(ClientModelValidationContext context) {
			var formatter = new MessageFormatter().AppendPropertyName(Rule.GetDisplayName());

			string messageTemplate;
			try {
				messageTemplate = EmailValidator.ErrorMessageSource.GetString(null);
			}
			catch (FluentValidationMessageFormatException) {
				messageTemplate = Messages.email_error;
			}

			string message = formatter.BuildMessage(messageTemplate);
			MergeAttribute(context.Attributes, "data-val", "true");
			MergeAttribute(context.Attributes, "email", message);
		}
	}
}