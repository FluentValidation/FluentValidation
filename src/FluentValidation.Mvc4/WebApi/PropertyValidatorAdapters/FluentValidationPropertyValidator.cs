namespace FluentValidation.Mvc.WebApi.PropertyValidatorAdapters
{
	using System;
	using System.Collections.Generic;
	using System.Web.Http.Metadata;
	using System.Web.Http.Validation;

	using FluentValidation.Internal;
	using FluentValidation.Validators;

	public class FluentValidationPropertyValidator : ModelValidator {
		public IPropertyValidator Validator { get; private set; }
		public PropertyRule Rule { get; private set; }

		public FluentValidationPropertyValidator(ModelMetadata metadata, IEnumerable<ModelValidatorProvider> validatorProviders, PropertyRule rule, IPropertyValidator validator) : base(validatorProviders) {
			this.Validator = validator;

			// Build a new rule instead of the one passed in.
			// We do this as the rule passed in will not have the correct properties defined for standalone validation.
			// We also want to ensure we copy across the CustomPropertyName and RuleSet, if specified. 
			Rule = new PropertyRule(null, x => metadata.Model, null, null, metadata.ModelType, null)
			{
				PropertyName = metadata.PropertyName,
				DisplayName = rule == null ? null : rule.DisplayName,
				RuleSet = rule == null ? null : rule.RuleSet
			};
		}
		
		public override IEnumerable<ModelValidationResult> Validate(ModelMetadata metadata, object container) {
			var fakeRule = new PropertyRule(null, x => metadata.Model, null, null, metadata.ModelType, null)
			{
				PropertyName = metadata.PropertyName,
				DisplayName = Rule == null ? null : Rule.DisplayName,
			};

			var fakeParentContext = new ValidationContext(container);
			var context = new PropertyValidatorContext(fakeParentContext, fakeRule, metadata.PropertyName);
			var result = Validator.Validate(context);

			foreach (var failure in result) {
				yield return new ModelValidationResult { Message = failure.ErrorMessage };
			}
		}

		protected bool TypeAllowsNullValue(Type type) {
			return (!type.IsValueType || Nullable.GetUnderlyingType(type) != null);
		}
	}
}