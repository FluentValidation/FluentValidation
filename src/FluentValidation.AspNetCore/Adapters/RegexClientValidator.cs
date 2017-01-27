namespace FluentValidation.AspNetCore {
	using System.Collections.Generic;
	using Internal;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using Validators;

	internal class RegexClientValidator : ClientValidatorBase {

		public RegexClientValidator(PropertyRule rule, IPropertyValidator validator)
			: base(rule, validator) {
		}

		public override void AddValidation(ClientModelValidationContext context) {
			var regexVal = Validator as RegularExpressionValidator;
			var formatter = new MessageFormatter().AppendPropertyName(Rule.GetDisplayName());
			string message = formatter.BuildMessage(regexVal.ErrorMessageSource.GetString());

			MergeAttribute(context.Attributes, "data-val", "true");
			MergeAttribute(context.Attributes, "data-val-regex", message);
			MergeAttribute(context.Attributes, "data-val-regex-pattern", regexVal.Expression);
		}
	}
}