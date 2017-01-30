namespace FluentValidation.AspNetCore
{
	using System.Collections.Generic;
	using Internal;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using Validators;

	internal class CreditCardClientValidator : ClientValidatorBase {
		public CreditCardClientValidator(PropertyRule rule, IPropertyValidator validator) : base(rule, validator) {
		}

		public override void AddValidation(ClientModelValidationContext context) {
			var formatter = new MessageFormatter().AppendPropertyName(Rule.GetDisplayName());
			string message = formatter.BuildMessage(Validator.ErrorMessageSource.GetString());
			MergeAttribute(context.Attributes, "data-val", "true");
			MergeAttribute(context.Attributes, "creditcard", message);
		}
	}
}