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

			var messageNeedsSplitting = RangeValidator.ErrorMessageSource.ResourceType == typeof(LanguageManager);
			
			string message;
			try {
				message = RangeValidator.ErrorMessageSource.GetString(null);
			}
			catch (FluentValidationMessageFormatException){
				// Use provided a message that contains placeholders based on object properties. We can't use that here, so just fall back to the default. 
				message = ValidatorOptions.LanguageManager.GetStringForValidator<InclusiveBetweenValidator>();
				messageNeedsSplitting = true;
			}

			if (messageNeedsSplitting) {
				// If we're using the default resources then the mesage for length errors will have two parts, eg:
				// '{PropertyName}' must be between {From} and {To}. You entered {Value}.
				// We can't include the "Value" part of the message because this information isn't available at the time the message is constructed.
				// Instead, we'll just strip this off by finding the index of the period that separates the two parts of the message.

				message = message.Substring(0, message.IndexOf(".") + 1);
			}

			message = formatter.BuildMessage(message);

			yield return new ModelClientValidationRangeRule(message, RangeValidator.From, RangeValidator.To);
		}
	}
}