namespace FluentValidation.Validators {
	using FluentValidation.Results;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	public class OnFailureValidator<T, TProperty> : NoopPropertyValidator {
		private readonly IPropertyValidator _innerValidator;
		private readonly Action<T, PropertyValidatorContext, string> _onFailure;

		public OnFailureValidator(IPropertyValidator innerValidator, Action<T, PropertyValidatorContext, string> onFailure) {
			_innerValidator = innerValidator;
			_onFailure = onFailure;
		}

		public override IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context) {
			var results = _innerValidator.Validate(context);
			if (!results.Any()) return results;
			var errorMessage = results.First().ErrorMessage;
			_onFailure((T)context.Instance, context, errorMessage);
			return results;
		}

		public override async Task<IEnumerable<ValidationFailure>> ValidateAsync(PropertyValidatorContext context, CancellationToken cancellation) {
			var results = await _innerValidator.ValidateAsync(context, cancellation);
			if (!results.Any()) return results;
			var errorMessage = results.First().ErrorMessage;
			_onFailure((T)context.Instance, context, errorMessage);
			return results;
		}
	}
}