namespace FluentValidation.Mvc {
    using System.Collections.Generic;
    using System.Reflection;
#if !CoreCLR
    using System.Web.Mvc;
#else
    using Microsoft.AspNet.Mvc;
    using Microsoft.AspNet.Mvc.ModelBinding;
    using Microsoft.Framework.DependencyInjection;
    using System.ComponentModel.DataAnnotations;
#endif
    using Internal;
    using Validators;

    internal class EqualToFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		EqualValidator EqualValidator {
			get { return (EqualValidator)Validator; }
		}

#if !CoreCLR
        public EqualToFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule rule, IPropertyValidator validator) : base(metadata, controllerContext, rule, validator) {
			ShouldValidate = false;
		}
        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
#else
        public EqualToFluentValidationPropertyValidator(ModelMetadata metadata, IContextAccessor<ActionContext> actionContext, PropertyRule rule, IPropertyValidator validator) : base(metadata, actionContext, rule, validator) {
            IsRequired = false;
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ClientModelValidationContext clientModelValidationContext) { 
#endif
			if (!ShouldGenerateClientSideRules()) yield break;

			var propertyToCompare = EqualValidator.MemberToCompare as PropertyInfo;
			if(propertyToCompare != null) {
				// If propertyToCompare is not null then we're comparing to another property.
				// If propertyToCompare is null then we're either comparing against a literal value, a field or a method call.
				// We only care about property comparisons in this case.

				var comparisonDisplayName =
					ValidatorOptions.DisplayNameResolver(Rule.TypeToValidate, propertyToCompare, null)
					?? propertyToCompare.Name.SplitPascalCase();

				var formatter = new MessageFormatter()
					.AppendPropertyName(Rule.GetDisplayName())
					.AppendArgument("ComparisonValue", comparisonDisplayName);


				string message = formatter.BuildMessage(EqualValidator.ErrorMessageSource.GetString());
#if !CoreCLR
                yield return new ModelClientValidationEqualToRule(message, CompareAttribute.FormatPropertyForClientValidation(propertyToCompare.Name)) ;
#else
                yield return new ModelClientValidationEqualToRule(message, "*." + propertyToCompare.Name);
#endif
            }
        }
    }
}