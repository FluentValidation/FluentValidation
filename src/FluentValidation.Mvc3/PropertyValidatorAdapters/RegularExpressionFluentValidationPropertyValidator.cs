namespace FluentValidation.Mvc {
	using System.Collections.Generic;
	using System.Web.Mvc;
	using Internal;
	using Validators;

	internal class RegularExpressionFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		IRegularExpressionValidator RegexValidator {
			get { return (IRegularExpressionValidator)validator;}
		}

		public RegularExpressionFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, string propertyDescription, IPropertyValidator validator)
			: base(metadata, controllerContext, propertyDescription, validator) {
			ShouldValidate = false;
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			var formatter = new MessageFormatter().AppendPropertyName(propertyDescription);
			string message = formatter.BuildMessage(RegexValidator.ErrorMessageSource.GetString());
			yield return new ModelClientValidationRegexRule(message, RegexValidator.Expression);
		}
	}
}