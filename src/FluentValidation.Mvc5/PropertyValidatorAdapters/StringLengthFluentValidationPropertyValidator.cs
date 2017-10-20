namespace FluentValidation.Mvc {
	using System.Collections.Generic;
	using System.Web.Mvc;
	using Internal;
	using Resources;
	using Validators;

	internal class StringLengthFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		private ILengthValidator LengthValidator {
			get { return (ILengthValidator)Validator; }
		}

		public StringLengthFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule rule, IPropertyValidator validator)
			: base(metadata, controllerContext, rule, validator) {
			ShouldValidate = false;
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			if(!ShouldGenerateClientSideRules()) yield break;

			// Don't generate clientside rules if min/max are lazily loaded. 
			var lengthVal = LengthValidator as LengthValidator;

			if (lengthVal != null && lengthVal.MaxFunc != null && lengthVal.MinFunc != null) {
				yield break;
			}

			var formatter = ValidatorOptions.MessageFormatterFactory()
				.AppendPropertyName(Rule.GetDisplayName())
				.AppendArgument("MinLength", LengthValidator.Min)
				.AppendArgument("MaxLength", LengthValidator.Max);

			var messageNeedsSplitting = LengthValidator.ErrorMessageSource.ResourceType == typeof(LanguageManager);
			string message;

			try {
				message = LengthValidator.ErrorMessageSource.GetString(null);
			}
			catch (FluentValidationMessageFormatException) {
				// Use provided a message that contains placeholders based on object properties. We can't use that here, so just fall back to the default. 
				if (lengthVal is MinimumLengthValidator) {
					message = ValidatorOptions.LanguageManager.GetStringForValidator<MinimumLengthValidator>();
				} else if (lengthVal is MaximumLengthValidator) {
					message = ValidatorOptions.LanguageManager.GetStringForValidator<MaximumLengthValidator>();
				} else if (lengthVal is ExactLengthValidator) {
					message = ValidatorOptions.LanguageManager.GetStringForValidator<ExactLengthValidator>();
				} else {
					message = ValidatorOptions.LanguageManager.GetStringForValidator<LengthValidator>();
				}
				messageNeedsSplitting = true;
			}

			if(messageNeedsSplitting) {
				// If we're using the default resources then the mesage for length errors will have two parts, eg:
				// '{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.
				// We can't include the "TotalLength" part of the message because this information isn't available at the time the message is constructed.
				// Instead, we'll just strip this off by finding the index of the period that separates the two parts of the message.

				message = message.Substring(0, message.IndexOf(".") + 1);
			}

			message = formatter.BuildMessage(message);

			ModelClientValidationRule rule;
			if (lengthVal is MinimumLengthValidator)
				rule = new ModelClientValidationMinLengthRule(message, LengthValidator.Min);
			else if (lengthVal is MaximumLengthValidator)
				rule = new ModelClientValidationMaxLengthRule(message, LengthValidator.Max);
			else
				rule = new ModelClientValidationStringLengthRule(message, LengthValidator.Min, LengthValidator.Max);
			yield return rule;
		}
	}
}