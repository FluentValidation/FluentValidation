namespace FluentValidation.AspNetCore {
	using System.Collections.Generic;
	using Internal;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using Resources;
	using Validators;

	internal class RegexClientValidator : ClientValidatorBase {

		public RegexClientValidator(PropertyRule rule, IPropertyValidator validator)
			: base(rule, validator) {
		}

		public override void AddValidation(ClientModelValidationContext context) {
			var regexVal = (RegularExpressionValidator)Validator;
			var formatter = new MessageFormatter().AppendPropertyName(Rule.GetDisplayName());
			string messageTemplate;
			try {
				messageTemplate = regexVal.ErrorMessageSource.GetString(null);
			}
			catch (FluentValidationMessageFormatException) {
				messageTemplate = Messages.regex_error;
			}
			string message = formatter.BuildMessage(messageTemplate);

			MergeAttribute(context.Attributes, "data-val", "true");
			MergeAttribute(context.Attributes, "data-val-regex", message);
			MergeAttribute(context.Attributes, "data-val-regex-pattern", regexVal.Expression);
		}
	}
}