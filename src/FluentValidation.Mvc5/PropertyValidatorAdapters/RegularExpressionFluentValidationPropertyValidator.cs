namespace FluentValidation.Mvc {
	using System.Collections.Generic;
	using System.Web.Mvc;
	using Internal;
	using Resources;
	using Validators;

	internal class RegularExpressionFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		IRegularExpressionValidator RegexValidator {
			get { return (IRegularExpressionValidator)Validator;}
		}

		public RegularExpressionFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule rule, IPropertyValidator validator)
			: base(metadata, controllerContext, rule, validator) {
			ShouldValidate = false;
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			if (!ShouldGenerateClientSideRules()) yield break;

			if (string.IsNullOrEmpty(RegexValidator.Expression)) yield break;

			var formatter = ValidatorOptions.MessageFormatterFactory().AppendPropertyName(Rule.GetDisplayName());
			string message;
			try {
				message = RegexValidator.ErrorMessageSource.GetString(null);
			}
			catch (FluentValidationMessageFormatException) {
				// Use provided a message that contains placeholders based on object properties. We can't use that here, so just fall back to the default. 
				message = ValidatorOptions.LanguageManager.GetStringForValidator<RegularExpressionValidator>();
			}
			message = formatter.BuildMessage(message);

			yield return new ModelClientValidationRegexRule(message, RegexValidator.Expression);
		}
	}
}