namespace FluentValidation.Mvc {
	using System.Collections.Generic;
	using System.Web.Mvc;
	using Internal;
	using Validators;

	internal class CreditCardFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		public CreditCardFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule rule, IPropertyValidator validator) : base(metadata, controllerContext, rule, validator) {
			ShouldValidate=false;
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			var formatter = new MessageFormatter().AppendPropertyName(Rule.PropertyDescription);
			string message = formatter.BuildMessage(Validator.ErrorMessageSource.GetString());
			
			yield return new ModelClientValidationRule {
			                                           	ValidationType = "creditcard",
			                                           	ErrorMessage = message
			                                           };
		}
	}
}