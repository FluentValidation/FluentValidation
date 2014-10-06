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
    using Resources;
    using Validators;

    internal class RangeFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		InclusiveBetweenValidator RangeValidator {
			get { return (InclusiveBetweenValidator)Validator; }
		}

#if !CoreCLR
        public RangeFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule propertyDescription, IPropertyValidator validator) : base(metadata, controllerContext, propertyDescription, validator) {
			ShouldValidate=false;
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
#else
        public RangeFluentValidationPropertyValidator(ModelMetadata metadata, IContextAccessor<ActionContext> actionContext, PropertyRule propertyDescription, IPropertyValidator validator) : base(metadata, actionContext, propertyDescription, validator) {
            IsRequired = false;
        }

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ClientModelValidationContext clientModelValidationContext) {
#endif
			if (!ShouldGenerateClientSideRules()) yield break;

			var formatter = new MessageFormatter()
                .AppendPropertyName(Rule.GetDisplayName())
                .AppendArgument("From", RangeValidator.From)
                .AppendArgument("To", RangeValidator.To);

            string message = RangeValidator.ErrorMessageSource.GetString();
            
            if (RangeValidator.ErrorMessageSource.ResourceType == typeof(Messages)) {
				// If we're using the default resources then the mesage for length errors will have two parts, eg:
				// '{PropertyName}' must be between {From} and {To}. You entered {Value}.
				// We can't include the "Value" part of the message because this information isn't available at the time the message is constructed.
				// Instead, we'll just strip this off by finding the index of the period that separates the two parts of the message.

				message = message.Substring(0, message.IndexOf(".") + 1);
			}

            message = formatter.BuildMessage(message);

			yield return new ModelClientValidationRangeRule(message, RangeValidator.From, RangeValidator.To);
		}
    }
}