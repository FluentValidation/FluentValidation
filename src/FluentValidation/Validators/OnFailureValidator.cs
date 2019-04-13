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

		public Type ValidatorType {
			get {
				if (_innerValidator is IChildValidatorAdaptor c)
					return c.ValidatorType;
				
				return _innerValidator.GetType();
			}
		}
	}

}