namespace FluentValidation.AspNetCore {
	using System.Collections.Generic;
	using Internal;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using Validators;

	internal class EmailClientValidator : ClientValidatorBase {
		private IEmailValidator EmailValidator {
			get { return (IEmailValidator)Validator; }
		}

		public EmailClientValidator(PropertyRule rule, IPropertyValidator validator) : base(rule, validator) {
		}

		public override void AddValidation(ClientModelValidationContext context) {
			var formatter = new MessageFormatter().AppendPropertyName(Rule.GetDisplayName());
			string message = formatter.BuildMessage(EmailValidator.ErrorMessageSource.GetString());
			MergeAttribute(context.Attributes, "data-val", "true");
			MergeAttribute(context.Attributes, "email", message);
		}
	}
}