namespace FluentValidation.Mvc {
	using System.Collections.Generic;
	using System.Reflection;
	using System.Web.Mvc;
	using Internal;
	using Resources;
	using Validators;

	internal class EqualToFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		EqualValidator EqualValidator {
			get { return (EqualValidator)Validator; }
		}
		
		public EqualToFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule rule, IPropertyValidator validator) : base(metadata, controllerContext, rule, validator) {
			ShouldValidate = false;
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			if (!ShouldGenerateClientSideRules()) yield break;

			var propertyToCompare = EqualValidator.MemberToCompare as PropertyInfo;
			if(propertyToCompare != null) {
				// If propertyToCompare is not null then we're comparing to another property.
				// If propertyToCompare is null then we're either comparing against a literal value, a field or a method call.
				// We only care about property comparisons in this case.

				var comparisonDisplayName =
					ValidatorOptions.DisplayNameResolver(Rule.TypeToValidate, propertyToCompare, null)
					?? propertyToCompare.Name.SplitPascalCase();

				var formatter = ValidatorOptions.MessageFormatterFactory()
					.AppendPropertyName(Rule.GetDisplayName())
					.AppendArgument("ComparisonValue", comparisonDisplayName);


				string message;
				try {
					message = EqualValidator.ErrorMessageSource.GetString(null);
					
				}
				catch (FluentValidationMessageFormatException) {
					// User provided a message that contains placeholders based on object properties. We can't use that here, so just fall back to the default. 
					message = ValidatorOptions.LanguageManager.GetStringForValidator<EqualValidator>();
				}
				message = formatter.BuildMessage(message);
#pragma warning disable 618
				yield return new ModelClientValidationEqualToRule(message, CompareAttribute.FormatPropertyForClientValidation(propertyToCompare.Name)) ;
#pragma warning restore 618
			}
		}
	}
}