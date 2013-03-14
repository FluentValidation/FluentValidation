namespace FluentValidation.Validators {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Internal;
	using Results;

	public class ChildValidatorAdaptor : NoopPropertyValidator {
		readonly IValidator validator;

		public IValidator Validator {
			get { return validator; }
		}

		public ChildValidatorAdaptor(IValidator validator) {
			this.validator = validator;
		}

		public override IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context) {
			if (context.Rule.Member == null) {
				throw new InvalidOperationException(string.Format("Nested validators can only be used with Member Expressions."));
			}

			var instanceToValidate = context.PropertyValue;

			if (instanceToValidate == null) {
				return Enumerable.Empty<ValidationFailure>();
			}

			var validator = GetValidator(context);

			if(validator == null) {
				return Enumerable.Empty<ValidationFailure>();
			}

			var newContext = CreateNewValidationContextForChildValidator(instanceToValidate, context);
			var results = validator.Validate(newContext).Errors;

			return results;
		}

		protected virtual IValidator GetValidator(PropertyValidatorContext context) {
			return Validator;
		}

		protected ValidationContext CreateNewValidationContextForChildValidator(object instanceToValidate, PropertyValidatorContext context) {
			var newContext = context.ParentContext.CloneForChildValidator(instanceToValidate);
			newContext.PropertyChain.Add(context.Rule.PropertyName);
			return newContext;
		}
	}
}