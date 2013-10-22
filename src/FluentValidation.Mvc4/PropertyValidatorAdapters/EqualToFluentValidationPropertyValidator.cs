namespace FluentValidation.Mvc {
	using System.Collections.Generic;
	using System.Reflection;
	using System.Web.Mvc;
	using Internal;
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

				var formatter = new MessageFormatter()
					.AppendPropertyName(Rule.GetDisplayName())
					.AppendArgument("ComparisonValue", propertyToCompare.Name);


				string message = formatter.BuildMessage(EqualValidator.ErrorMessageSource.GetString());
				yield return new ModelClientValidationEqualToRule(message, CompareAttribute.FormatPropertyForClientValidation(propertyToCompare.Name)) ;
			}
		}
	}
}