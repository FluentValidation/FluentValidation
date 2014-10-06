namespace FluentValidation.Mvc {
    using System;
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

    internal abstract class AbstractComparisonFluentValidationPropertyValidator<TValidator> : FluentValidationPropertyValidator 
        where TValidator: AbstractComparisonValidator {

        protected TValidator AbstractComparisonValidator
        {
            get { return (TValidator)Validator; }
        }

        protected abstract Object MinValue { get; }
        protected abstract Object MaxValue { get; }

#if !CoreCLR
        protected AbstractComparisonFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule propertyDescription, IPropertyValidator validator) : base(metadata, controllerContext, propertyDescription, validator) {
            ShouldValidate=false;
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
#else
        protected AbstractComparisonFluentValidationPropertyValidator(ModelMetadata metadata, IContextAccessor<ActionContext> actionContext, PropertyRule propertyDescription, IPropertyValidator validator) : base(metadata, actionContext, propertyDescription, validator) {
            IsRequired = false;
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ClientModelValidationContext clientModelValidationContext) { 
#endif
            if (!ShouldGenerateClientSideRules()) yield break;

            var formatter = new MessageFormatter()
                .AppendPropertyName(Rule.GetDisplayName())
                .AppendArgument("ComparisonValue", AbstractComparisonValidator.ValueToCompare);
            var message = formatter.BuildMessage(AbstractComparisonValidator.ErrorMessageSource.GetString());
            yield return new ModelClientValidationRangeRule(message, MinValue, MaxValue);
        }
    }
}