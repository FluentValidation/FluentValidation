namespace FluentValidation.AspNetCore {
	using System.Collections.Generic;
	using Internal;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using Resources;
	using Validators;

	internal class RequiredClientValidator : ClientValidatorBase{
		public RequiredClientValidator(PropertyRule rule, IPropertyValidator validator) : base(rule, validator) {

		}

		public override void AddValidation(ClientModelValidationContext context) {
			MergeAttribute(context.Attributes, "data-val", "true");
			MergeAttribute(context.Attributes, "data-val-required", GetErrorMessage(context));
		}

		private string GetErrorMessage(ClientModelValidationContext context) {
			var formatter = new MessageFormatter().AppendPropertyName(Rule.GetDisplayName());
			string messageTemplate;
			try {
				messageTemplate = Validator.ErrorMessageSource.GetString(null);
			}
			catch (FluentValidationMessageFormatException) {
				messageTemplate = Messages.notempty_error;
			}
			var message = formatter.BuildMessage(messageTemplate);
			return message;
		}
	}
}