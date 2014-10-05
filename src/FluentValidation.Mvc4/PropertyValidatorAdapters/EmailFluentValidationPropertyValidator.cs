namespace FluentValidation.Mvc
{
    using System.Collections.Generic;
#if !CoreCLR
    using System.Web.Mvc;
#else
    using Microsoft.AspNet.Mvc;
    using Microsoft.AspNet.Mvc.ModelBinding;
    using Microsoft.Framework.DependencyInjection;
    using System.Linq;
    using System;
#endif
    using Internal;
    using Validators;

    internal class EmailFluentValidationPropertyValidator : FluentValidationPropertyValidator {
        private IEmailValidator EmailValidator {
            get { return (IEmailValidator)Validator; }
        }
#if !CoreCLR
        public EmailFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule rule, IPropertyValidator validator) : base(metadata, controllerContext, rule, validator) {
			ShouldValidate=false;
#else
        private IContextAccessor<ActionContext> _actionContext;
        public EmailFluentValidationPropertyValidator(ModelMetadata metadata, IContextAccessor<ActionContext> actionContext, PropertyRule rule, IPropertyValidator validator) : base(metadata, actionContext, rule, validator)
        {
            _actionContext = actionContext;
            IsRequired = false;
#endif
        }

#if !CoreCLR
        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
#else
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ClientModelValidationContext clientModelValidationContext) {
#endif
            if (!ShouldGenerateClientSideRules()) yield break;

            var formatter = new MessageFormatter().AppendPropertyName(Rule.GetDisplayName());
            string message = formatter.BuildMessage(EmailValidator.ErrorMessageSource.GetString());

#if !CoreCLR
			yield return new ModelClientValidationRule {
			                                           	ValidationType = "email",
			                                           	ErrorMessage = message
			                                           };
#else
            yield return new ModelClientValidationRule("email", message);
#endif
        }
    }
}