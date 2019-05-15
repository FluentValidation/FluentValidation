namespace FluentValidation.Mvc {
	using System.Collections.Generic;
	using System.Web.Mvc;
	using Internal;
	using Resources;
	using Validators;

	internal class RangeFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		InclusiveBetweenValidator RangeValidator {
			get { return (InclusiveBetweenValidator)Validator; }
		}
		
		public RangeFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule propertyDescription, IPropertyValidator validator) : base(metadata, controllerContext, propertyDescription, validator) {
			ShouldValidate=false;
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			if (!ShouldGenerateClientSideRules()) yield break;

			var formatter = ValidatorOptions.MessageFormatterFactory()
				.AppendPropertyName(Rule.GetDisplayName())
				.AppendArgument("From", RangeValidator.From)
				.AppendArgument("To", RangeValidator.To);

			var needsSimplifiedMessage = RangeValidator.Options.ErrorMessageSource is LanguageStringSource;
			
			string message;
			try {
				message = RangeValidator.Options.ErrorMessageSource.GetString(null);
			}
			catch (FluentValidationMessageFormatException){
				// Use provided a message that contains placeholders based on object properties. We can't use that here, so just fall back to the default. 
				message = ValidatorOptions.LanguageManager.GetString("InclusiveBetween_Simple");
				needsSimplifiedMessage = false;
			}

			if (needsSimplifiedMessage && message.Contains("{Value}")) {
				message = ValidatorOptions.LanguageManager.GetString("InclusiveBetween_Simple");
			}

			message = formatter.BuildMessage(message);

			yield return new ModelClientValidationRangeRule(message, RangeValidator.From, RangeValidator.To);
		}
	}
}