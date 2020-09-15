namespace FluentValidation.Validators {
	using Results;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	public class OnFailureValidator<T> : NoopPropertyValidator, IChildValidatorAdaptor {
		private readonly IPropertyValidator _innerValidator;
		private readonly Action<T, PropertyValidatorContext, string> _onFailure;

		public OnFailureValidator(IPropertyValidator innerValidator, Action<T, PropertyValidatorContext, string> onFailure) {
			_innerValidator = innerValidator;
			_onFailure = onFailure;
			// Make sure any conditions defined on the wrapped validator are applied
			// to this validator too. They won't be invoked automatically, as conditions
			// are invoked by the parent rule, which means that only *this* validator's
			// conditions will be invoked, not the wrapped validator's conditions.
			if (_innerValidator.Options.HasCondition) {
				Options.ApplyCondition(_innerValidator.Options.InvokeCondition);
			}

			if (_innerValidator.Options.HasAsyncCondition) {
				Options.ApplyAsyncCondition(_innerValidator.Options.InvokeAsyncCondition);
			}
		}

		public override IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context) {
			var results = _innerValidator.Validate(context).ToList();
			if (!results.Any()) return results;
			var errorMessage = results.First().ErrorMessage;
			_onFailure((T) context.InstanceToValidate, context, errorMessage);
			return results;
		}

		public override async Task<IEnumerable<ValidationFailure>> ValidateAsync(PropertyValidatorContext context, CancellationToken cancellation) {
			var results = (await _innerValidator.ValidateAsync(context, cancellation)).ToList();
			if (!results.Any()) return results;
			var errorMessage = results.First().ErrorMessage;
			_onFailure((T) context.InstanceToValidate, context, errorMessage);
			return results;
		}

		public override bool ShouldValidateAsynchronously(IValidationContext context) {
			// If the user has applied an async condition, or the inner validator requires async
			// validation then always go through the async path.
			if (Options.HasAsyncCondition || _innerValidator.ShouldValidateAsynchronously(context)) return true;
			return false;
		}

		public Type ValidatorType {
			get {
				if (_innerValidator is IChildValidatorAdaptor c)
					return c.ValidatorType;

				return _innerValidator.GetType();
			}
		}
	}

}
