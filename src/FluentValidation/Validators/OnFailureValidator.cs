namespace FluentValidation.Validators {
	using FluentValidation.Internal;
	using FluentValidation.Results;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	public class OnFailureValidator<T, TProperty> : NoopPropertyValidator {
		private readonly IPropertyValidator _innerValidator;
		private readonly Action<T> _onFailureSuperSimple;
		private readonly Action<T, PropertyValidatorContext> _onFailureSimple;
		private readonly Action<T, PropertyValidatorContext, string> _onFailureComplex;

		private readonly Action<PropertyValidatorContext> _onFailure;

		public OnFailureValidator(IPropertyValidator innerValidator, Action<T> onFailure) {

			onFailure.Guard("onFailure action must be specified.", nameof(onFailure));
			_innerValidator = innerValidator;
			_onFailureSuperSimple = onFailure;
		}

		public OnFailureValidator(IPropertyValidator innerValidator, Action<T, PropertyValidatorContext> onFailure) {
			onFailure.Guard("onFailure action must be specified.", nameof(onFailure));
			_innerValidator = innerValidator;
			_onFailureSimple = onFailure;
		}

		public OnFailureValidator(IPropertyValidator innerValidator, Action<T, PropertyValidatorContext, string> onFailure) {
			onFailure.Guard("onFailure action must be specified.", nameof(onFailure));
			_innerValidator = innerValidator;
			_onFailureComplex = onFailure;
		}

		public override IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context) {
			var results = _innerValidator.Validate(context);
			if (!results.Any()) return results;

			if (_onFailureSuperSimple != null) {
				_onFailureSuperSimple((T)context.Instance);
			}

			else if (_onFailureSimple != null) {
				_onFailureSimple((T)context.Instance, context);
			}

			else {
				var errorMessage = results.First().ErrorMessage;
				_onFailureComplex((T)context.Instance, context, errorMessage);
			}

			return results;
		}

		public override Task<IEnumerable<ValidationFailure>> ValidateAsync(PropertyValidatorContext context, CancellationToken cancellation) {
			var results = _innerValidator.ValidateAsync(context, cancellation);
			if (!results.Result.Any()) return results;

			if (_onFailureSuperSimple != null) {
				_onFailureSuperSimple((T)context.Instance);
			}

			else if (_onFailureSimple != null) {
				_onFailureSimple((T)context.Instance, context);
			}

			else {
				var errorMessage = results.Result.First().ErrorMessage;
				_onFailureComplex((T)context.Instance, context, errorMessage);
			}

			return results;
		}
	}
}