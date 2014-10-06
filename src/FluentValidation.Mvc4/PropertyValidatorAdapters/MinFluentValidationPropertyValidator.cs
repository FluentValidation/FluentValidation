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

    internal class MinFluentValidationPropertyValidator : AbstractComparisonFluentValidationPropertyValidator<GreaterThanOrEqualValidator> {

        protected override object MinValue {
            get { return AbstractComparisonValidator.ValueToCompare;  }
        }

        protected override object MaxValue {
            get { return null; }
        }

#if !CoreCLR
        public MinFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule propertyDescription, IPropertyValidator validator)
            : base(metadata, controllerContext, propertyDescription, validator) {
        }
#else
        public MinFluentValidationPropertyValidator(ModelMetadata metadata, IContextAccessor<ActionContext> actionContext, PropertyRule propertyDescription, IPropertyValidator validator)
           : base(metadata, actionContext, propertyDescription, validator) {
        }
#endif
    }
}