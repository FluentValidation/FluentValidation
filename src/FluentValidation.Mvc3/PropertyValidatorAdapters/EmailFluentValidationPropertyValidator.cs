namespace FluentValidation.Mvc {
	using System.Collections.Generic;
	using System.Web.Mvc;
	using Internal;
	using Validators;

	internal class EmailFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		private IEmailValidator EmailValidator {
			get { return (IEmailValidator)validator; }
		}

		public EmailFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, string propertyDescription, IPropertyValidator validator) : base(metadata, controllerContext, propertyDescription, validator) {
			ShouldValidate=false;
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			var formatter = new MessageFormatter().AppendPropertyName(propertyDescription);
			string message = formatter.BuildMessage(EmailValidator.ErrorMessageSource.GetString());
			
			yield return new ModelClientValidationRule {
			                                           	ValidationType = "email",
			                                           	ErrorMessage = message
			                                           };
		}
	}
}