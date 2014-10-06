namespace FluentValidation.Mvc {
#if !CoreCLR
    using System.Web.Mvc;
#else
    using Microsoft.AspNet.Mvc;
    using Microsoft.AspNet.Mvc.ModelBinding;
    using Microsoft.Framework.DependencyInjection;
#endif
    using Internal;
    using Validators;

    internal class MaxFluentValidationPropertyValidator : AbstractComparisonFluentValidationPropertyValidator<LessThanOrEqualValidator> {

        protected override object MinValue {
            get { return null; }
        }

        protected override object MaxValue {
            get { return AbstractComparisonValidator.ValueToCompare; }
        }

#if !CoreCLR
        public MaxFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule propertyDescription, IPropertyValidator validator)
            : base(metadata, controllerContext, propertyDescription, validator) {
        }
#else
        public MaxFluentValidationPropertyValidator(ModelMetadata metadata, IContextAccessor<ActionContext> actionContext, PropertyRule propertyDescription, IPropertyValidator validator)
            : base(metadata, actionContext, propertyDescription, validator) {
        }
#endif
    }
}