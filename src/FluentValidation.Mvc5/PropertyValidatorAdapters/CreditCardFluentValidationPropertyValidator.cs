namespace FluentValidation.Mvc {
	using System.Collections.Generic;
	using System.Web.Mvc;
	using Internal;
	using Resources;
	using Validators;

	internal class CreditCardFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		public CreditCardFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule rule, IPropertyValidator validator) : base(metadata, controllerContext, rule, validator) {
			ShouldValidate=false;
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			if (!ShouldGenerateClientSideRules()) yield break;

			var formatter = ValidatorOptions.MessageFormatterFactory().AppendPropertyName(Rule.GetDisplayName());
			string message;
			try {
				message = Validator.ErrorMessageSource.GetString(null);
			}
			catch (FluentValidationMessageFormatException) {
				message = ValidatorOptions.LanguageManager.GetStringForValidator<CreditCardValidator>();
			}
			message = formatter.BuildMessage(message);


			yield return new ModelClientValidationRule {
			                                           	ValidationType = "creditcard",
			                                           	ErrorMessage = message
			                                           };
		}
	}
}