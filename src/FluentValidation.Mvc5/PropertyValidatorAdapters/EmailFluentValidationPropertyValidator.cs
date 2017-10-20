namespace FluentValidation.Mvc {
	using System.Collections.Generic;
	using System.Web.Mvc;
	using Internal;
	using Resources;
	using Validators;

	internal class EmailFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		private IEmailValidator EmailValidator {
			get { return (IEmailValidator)Validator; }
		}

		public EmailFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule rule, IPropertyValidator validator) : base(metadata, controllerContext, rule, validator) {
			ShouldValidate=false;
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			if (!ShouldGenerateClientSideRules()) yield break;

			var formatter = ValidatorOptions.MessageFormatterFactory().AppendPropertyName(Rule.GetDisplayName());
			string message;
			try {
				message = EmailValidator.ErrorMessageSource.GetString(null);
			}
			catch (FluentValidationMessageFormatException) {
				// User provided a message that contains placeholders based on object properties. We can't use that here, so just fall back to the default. 
				message = ValidatorOptions.LanguageManager.GetStringForValidator<EmailValidator>();
			}
			message = formatter.BuildMessage(message);
			
			yield return new ModelClientValidationRule {
			                                           	ValidationType = "email",
			                                           	ErrorMessage = message
			                                           };
		}
	}
}