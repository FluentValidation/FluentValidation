namespace FluentValidation.AspNetCore {
	using System.Collections.Generic;
	using Internal;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
			var message = formatter.BuildMessage(Validator.ErrorMessageSource.GetString());
			return message;
		}
	}
}