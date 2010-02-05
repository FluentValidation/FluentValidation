namespace FluentValidation.Validators {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using Internal;
	using Results;

	internal class ChildValidatorAdaptor<T> : IPropertyValidator {
		readonly IValidator<T> validator;

		public ChildValidatorAdaptor(IValidator<T> validator) {
			this.validator = validator;
		}

		public IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context) {
			/*if (context.Member == null) {
				throw new InvalidOperationException(string.Format("Nested validators can only be used with Member Expressions."));
			}*/

			var instanceToValidate = context.PropertyValue;

			if (instanceToValidate == null) {
				return Enumerable.Empty<ValidationFailure>();
			}

			var propertyChain = new PropertyChain(context.PropertyChain);
			propertyChain.Add(context.PropertyName);

			var newContext = new ValidationContext<T>((T)instanceToValidate, propertyChain, new DefaultValidatorSelector());
			var results = validator.SelectMany(x => x.Validate(newContext));

			return results;
		}

		public string ErrorMessageTemplate {
			get { return null; }
		}

		public ICollection<Func<object, object>> CustomMessageFormatArguments {
			get { return new List<Func<object, object>>(); }
		}

		public bool SupportsStandaloneValidation {
			get { return false; }
		}

		public Type ErrorMessageResourceType {
			get { return null; }
		}

		public string ErrorMessageResourceName {
			get { return null; }
		}

		public Func<object, object> CustomStateProvider {
			get { return null; }
			set {/*TODO: Should this be a no-op*/  }
		}

		public void SetErrorMessage(string message) {
			throw new NotSupportedException("Custom error messages are not supported with child validators.");
		}

		public void SetErrorMessage(Type errorMessageResourceType, string resourceName) {
			throw new NotSupportedException("Custom error messages are not supported with child validators.");
		}

		public void SetErrorMessage(Expression<Func<string>> resourceSelector) {
			throw new NotSupportedException("Custom error messages are not supported with child validators.");
		}
	}
}