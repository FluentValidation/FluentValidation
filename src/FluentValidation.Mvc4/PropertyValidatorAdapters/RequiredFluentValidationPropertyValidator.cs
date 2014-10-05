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

    internal class RequiredFluentValidationPropertyValidator : FluentValidationPropertyValidator {
#if !CoreCLR
        public RequiredFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule rule, IPropertyValidator validator) : base(metadata, controllerContext, rule, validator) {
#else
        public RequiredFluentValidationPropertyValidator(ModelMetadata metadata, IContextAccessor<ActionContext> actionContext, PropertyRule rule, IPropertyValidator validator) : base(metadata, actionContext, rule, validator) {
#endif
            bool isNonNullableValueType = !TypeAllowsNullValue(metadata.ModelType);
			bool nullWasSpecified = metadata.Model == null;

#if !CoreCLR
            ShouldValidate = isNonNullableValueType && nullWasSpecified;
#else
            IsRequired = isNonNullableValueType && nullWasSpecified;
#endif
        }

#if !CoreCLR
        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
#else
        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ClientModelValidationContext clientModelValidationContext) {
#endif
            if (!ShouldGenerateClientSideRules()) yield break;

            var formatter = new MessageFormatter().AppendPropertyName(Rule.GetDisplayName());
            var message = formatter.BuildMessage(Validator.ErrorMessageSource.GetString());
            yield return new ModelClientValidationRequiredRule(message);
        }

        public override bool IsRequired {
            get { return true; }
        }
    }
}