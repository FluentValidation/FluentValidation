namespace FluentValidation.Mvc {
    using System.Collections.Generic;
#if !CoreCLR
    using System.Web.Mvc;
#else
    using Microsoft.AspNet.Mvc;
    using Microsoft.AspNet.Mvc.ModelBinding;
    using Microsoft.Framework.DependencyInjection;
#endif
    using Internal;
    using Validators;

    internal class CreditCardFluentValidationPropertyValidator : FluentValidationPropertyValidator {
#if !CoreCLR
		public CreditCardFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule rule, IPropertyValidator validator) : base(metadata, controllerContext, rule, validator) {
			ShouldValidate=false;
		}
        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			if (!ShouldGenerateClientSideRules()) yield break;
#else
        public CreditCardFluentValidationPropertyValidator(ModelMetadata metadata, IContextAccessor<ActionContext> actionContext, PropertyRule rule, IPropertyValidator validator) : base(metadata, actionContext, rule, validator) { 
            IsRequired = false;
        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
#endif
            var formatter = new MessageFormatter().AppendPropertyName(Rule.GetDisplayName());
			string message = formatter.BuildMessage(Validator.ErrorMessageSource.GetString());
#if !CoreCLR
            yield return new ModelClientValidationRule {
			                                           	ValidationType = "creditcard",
			                                           	ErrorMessage = message
			                                           };
#else
            yield return new ModelClientValidationRule("creditcard", message);
#endif
        }
    }
}