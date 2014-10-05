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

	internal class RegularExpressionFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		IRegularExpressionValidator RegexValidator {
			get { return (IRegularExpressionValidator)Validator;}
		}

#if !CoreCLR
        public RegularExpressionFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule rule, IPropertyValidator validator)
			: base(metadata, controllerContext, rule, validator) {
			ShouldValidate = false;
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
#else
        public RegularExpressionFluentValidationPropertyValidator(ModelMetadata metadata, IContextAccessor<ActionContext> actionContext, PropertyRule rule, IPropertyValidator validator)
            : base(metadata, actionContext, rule, validator) {
            IsRequired = false;
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ClientModelValidationContext clientModelValidationContext) {
#endif
            if (!ShouldGenerateClientSideRules()) yield break;

			var formatter = new MessageFormatter().AppendPropertyName(Rule.GetDisplayName());
            string message = formatter.BuildMessage(RegexValidator.ErrorMessageSource.GetString());

            yield return new ModelClientValidationRegexRule(message, RegexValidator.Expression);
        }
    }
}