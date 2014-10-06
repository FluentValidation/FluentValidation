namespace FluentValidation.Mvc {
    using System;
    using System.Reflection;
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
    using System.Linq;

#if !CoreCLR
    public class FluentValidationPropertyValidator : ModelValidator {
#else
    public class FluentValidationPropertyValidator : IModelValidator, IClientModelValidator {
#endif
        public IPropertyValidator Validator { get; private set; }
		public PropertyRule Rule { get; private set; }

        /*
		 This might seem a bit strange, but we do *not* want to do any work in these validators.
		 They should only be used for metadata purposes.
		 This is so that the validation can be left to the actual FluentValidationModelValidator.
		 The exception to this is the Required validator - these *do* need to run standalone
		 in order to bypass MVC's "A value is required" message which cannot be turned off.
		 Basically, this is all just to bypass the bad design in ASP.NET MVC. Boo, hiss. 
		*/
#if !CoreCLR
		protected bool ShouldValidate { get; set; }
        
        public FluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule rule, IPropertyValidator validator) : base(metadata, controllerContext) {
#else
        private IContextAccessor<ActionContext> _actionContext; 
        public virtual bool IsRequired { get; set; }

        public FluentValidationPropertyValidator(ModelMetadata metadata, IContextAccessor<ActionContext> actionContext, PropertyRule rule, IPropertyValidator validator) {
            _actionContext = actionContext;
#endif
            this.Validator = validator;

			// Build a new rule instead of the one passed in.
			// We do this as the rule passed in will not have the correct properties defined for standalone validation.
			// We also want to ensure we copy across the CustomPropertyName and RuleSet, if specified. 
			Rule = new PropertyRule(null, x => metadata.Model, null, null, metadata.ModelType, null) {
				PropertyName = metadata.PropertyName,
				DisplayName = rule == null ? null : rule.DisplayName,
				RuleSet = rule == null ? null : rule.RuleSet
			};
        }

#if !CoreCLR
		public override IEnumerable<ModelValidationResult> Validate(object container) {
                if (ShouldValidate)
                {
#else
        public virtual IEnumerable<ModelValidationResult> Validate(ModelValidationContext validationContext) { 
                if (IsRequired)
                {
                    var Metadata = validationContext.ModelMetadata;
#endif
                    var fakeRule = new PropertyRule(null, x => Metadata.Model, null, null, Metadata.ModelType, null) { 
                        PropertyName = Metadata.PropertyName,
                        DisplayName = Rule == null ? null : Rule.DisplayName,
                };

#if !CoreCLR
				var fakeParentContext = new ValidationContext(container);
#else
                var fakeParentContext = new ValidationContext(validationContext.ContainerMetadata);
#endif
                var context = new PropertyValidatorContext(fakeParentContext, fakeRule, Metadata.PropertyName);
                var result = Validator.Validate(context);

                foreach (var failure in result) {
#if !CoreCLR
                    yield return new ModelValidationResult { Message = failure.ErrorMessage };
#else
                    yield return new ModelValidationResult(failure.PropertyName, failure.ErrorMessage);
#endif
                }
            }
        }

		protected bool TypeAllowsNullValue(Type type) {
#if !CoreCLR
            return (!type.IsValueType || Nullable.GetUnderlyingType(type) != null);
#else
            return (!type.GetTypeInfo().IsValueType || Nullable.GetUnderlyingType(type) != null);
#endif
        }

#if !CoreCLR
       protected virtual bool ShouldGenerateClientSideRules() {
			var ruleSetToGenerateClientSideRules = RuleSetForClientSideMessagesAttribute.GetRuleSetsForClientValidation(ControllerContext.HttpContext);
			bool executeDefaultRule = (ruleSetToGenerateClientSideRules.Contains("default", StringComparer.OrdinalIgnoreCase) && string.IsNullOrEmpty(Rule.RuleSet));
			return ruleSetToGenerateClientSideRules.Contains(Rule.RuleSet) || executeDefaultRule ;
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
            if (!ShouldGenerateClientSideRules()) return Enumerable.Empty<ModelClientValidationRule>();

			var supportsClientValidation = Validator as IClientValidatable;
			
			if(supportsClientValidation != null) {
				return supportsClientValidation.GetClientValidationRules(Metadata, ControllerContext);
			}
#else
       protected virtual bool ShouldGenerateClientSideRules() {
            var ruleSetToGenerateClientSideRules = RuleSetForClientSideMessagesAttribute.GetRuleSetsForClientValidation(_actionContext.Value.HttpContext);
            bool executeDefaultRule = (ruleSetToGenerateClientSideRules.Contains("default", StringComparer.OrdinalIgnoreCase) && string.IsNullOrEmpty(Rule.RuleSet));
            return ruleSetToGenerateClientSideRules.Contains(Rule.RuleSet) || executeDefaultRule ;
		}

        public virtual IEnumerable<ModelClientValidationRule> GetClientValidationRules(ClientModelValidationContext clientModelValidationContext) {
#endif
            return Enumerable.Empty<ModelClientValidationRule>();
        }
    }
}