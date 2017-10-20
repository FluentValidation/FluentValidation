namespace FluentValidation.Mvc {
	using System.Collections.Generic;
	using System.Web.Mvc;
	using Internal;
	using Resources;
	using Validators;

	internal class RequiredFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		public RequiredFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule rule, IPropertyValidator validator) : base(metadata, controllerContext, rule, validator) {
			bool isNonNullableValueType = !TypeAllowsNullValue(metadata.ModelType);
			bool nullWasSpecified = metadata.Model == null;

			ShouldValidate = isNonNullableValueType && nullWasSpecified;
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			if (!ShouldGenerateClientSideRules()) yield break;

			var formatter = ValidatorOptions.MessageFormatterFactory().AppendPropertyName(Rule.GetDisplayName());
			string message;
			try {
				message = Validator.ErrorMessageSource.GetString(null);
			}
			catch (FluentValidationMessageFormatException) {
				// User provided a message that contains placeholders based on object properties. We can't use that here, so just fall back to the default. 
				message = ValidatorOptions.LanguageManager.GetStringForValidator<NotEmptyValidator>();
			}
			message = formatter.BuildMessage(message);
			yield return new ModelClientValidationRequiredRule(message);
		}

		public override bool IsRequired {
			get { return true; }
		}
	}
}