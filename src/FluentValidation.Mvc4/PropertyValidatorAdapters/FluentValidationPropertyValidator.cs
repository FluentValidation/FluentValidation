namespace FluentValidation.Mvc {
	using System;
	using System.Collections.Generic;
	using System.Web.Mvc;
	using Internal;
	using Validators;
	using System.Linq;

	public class FluentValidationPropertyValidator : ModelValidator {
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
		protected bool ShouldValidate { get; set; }

		public FluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, PropertyRule rule, IPropertyValidator validator) : base(metadata, controllerContext) {
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

		public override IEnumerable<ModelValidationResult> Validate(object container) {
			if (ShouldValidate) {
				var fakeRule = new PropertyRule(null, x => Metadata.Model, null, null, Metadata.ModelType, null) {
					PropertyName = Metadata.PropertyName,
					DisplayName = Rule == null ? null : Rule.DisplayName,
				};

				var fakeParentContext = new ValidationContext(container);
				var context = new PropertyValidatorContext(fakeParentContext, fakeRule, Metadata.PropertyName);
				var result = Validator.Validate(context);

				foreach (var failure in result) {
					yield return new ModelValidationResult { Message = failure.ErrorMessage };
				}
			}
		}

		protected bool TypeAllowsNullValue(Type type) {
			return (!type.IsValueType || Nullable.GetUnderlyingType(type) != null);
		}

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

			return Enumerable.Empty<ModelClientValidationRule>();
		}
	}
}