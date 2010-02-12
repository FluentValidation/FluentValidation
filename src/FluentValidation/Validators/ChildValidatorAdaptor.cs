namespace FluentValidation.Validators {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using Internal;
	using Results;

	internal class ChildValidatorAdaptor<T> : NoopPropertyValidator {
		readonly IValidator<T> validator;

		public ChildValidatorAdaptor(IValidator<T> validator) {
			this.validator = validator;
		}

		public override IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context) {
			if (context.Member == null) {
				throw new InvalidOperationException(string.Format("Nested validators can only be used with Member Expressions."));
			}

			var instanceToValidate = context.PropertyValue;

			if (instanceToValidate == null) {
				return Enumerable.Empty<ValidationFailure>();
			}

			var propertyChain = new PropertyChain(context.PropertyChain);
			propertyChain.Add(context.Member);

			var newContext = new ValidationContext<T>((T)instanceToValidate, propertyChain, new DefaultValidatorSelector());
			var results = validator.Validate(newContext).Errors;

			return results;
		}

		
		public override void SetErrorMessage(string message) {
			throw new NotSupportedException("Custom error messages are not supported with child validators.");
		}

		public override void SetErrorMessage(Type errorMessageResourceType, string resourceName) {
			throw new NotSupportedException("Custom error messages are not supported with child validators.");
		}

		public override void SetErrorMessage(Expression<Func<string>> resourceSelector) {
			throw new NotSupportedException("Custom error messages are not supported with child validators.");
		}
	}
}