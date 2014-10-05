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

    internal class StringLengthFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		private ILengthValidator LengthValidator {
			get { return (ILengthValidator)Validator; }
		}

#if !CoreCLR
        public StringLengthFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule rule, IPropertyValidator validator)
			: base(metadata, controllerContext, rule, validator) {
			ShouldValidate = false;
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
#else
        public StringLengthFluentValidationPropertyValidator(ModelMetadata metadata, IContextAccessor<ActionContext> actionContext, PropertyRule rule, IPropertyValidator validator)
            : base(metadata, actionContext, rule, validator) {
            IsRequired = false;
        }

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ClientModelValidationContext clientModelValidationContext) {
#endif
			if(!ShouldGenerateClientSideRules()) yield break;

			var formatter = new MessageFormatter()
                .AppendPropertyName(Rule.GetDisplayName())
                .AppendArgument("MinLength", LengthValidator.Min)
                .AppendArgument("MaxLength", LengthValidator.Max);

            string message = LengthValidator.ErrorMessageSource.GetString();

			if(LengthValidator.ErrorMessageSource.ResourceType == typeof(Messages)) {
				// If we're using the default resources then the mesage for length errors will have two parts, eg:
				// '{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.
				// We can't include the "TotalLength" part of the message because this information isn't available at the time the message is constructed.
				// Instead, we'll just strip this off by finding the index of the period that separates the two parts of the message.

				message = message.Substring(0, message.IndexOf(".") + 1);
			}

            message = formatter.BuildMessage(message);

			yield return new ModelClientValidationStringLengthRule(message, LengthValidator.Min, LengthValidator.Max);
        }
    }
}