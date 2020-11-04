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

		public override void Validate(PropertyValidatorContext context) {
			int count = context.Result.Errors.Count;
			_innerValidator.Validate(context);

			if (context.Result.Errors.Count > count) {
				var firstNewMessage = context.Result.Errors[count].ErrorMessage;
				_onFailure((T) context.InstanceToValidate, context, firstNewMessage);
			}
		}

		public override async Task ValidateAsync(PropertyValidatorContext context, CancellationToken cancellation) {
			int count = context.Result.Errors.Count;
			await _innerValidator.ValidateAsync(context, cancellation);

			if (context.Result.Errors.Count > count) {
				var firstNewMessage = context.Result.Errors[count].ErrorMessage;
				_onFailure((T) context.InstanceToValidate, context, firstNewMessage);
			}
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
