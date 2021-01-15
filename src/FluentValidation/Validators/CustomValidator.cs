namespace FluentValidation.Validators {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Internal;
	using Results;

	/// <summary>
	/// Custom validator that allows for manual/direct creation of ValidationFailure instances.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TProperty"></typeparam>
	public class CustomValidator<T, TProperty> : PropertyValidator<T,TProperty> {
		private readonly Action<TProperty, PropertyValidatorContext<T,TProperty>> _action;
		private Func<TProperty, PropertyValidatorContext<T,TProperty>, CancellationToken, Task> _asyncAction;
		private readonly bool _isAsync;

		/// <summary>
		/// Creates a new instance of the CustomValidator
		/// </summary>
		/// <param name="action"></param>
		public CustomValidator(Action<TProperty, PropertyValidatorContext<T,TProperty>> action) {
			_isAsync = false;
			_action = action;

			_asyncAction = (x, ctx, cancel) => Task.Run(() => action(x, ctx), cancel);
		}

		/// <summary>
		/// Creates a new instance of the CustomValidator.
		/// </summary>
		/// <param name="asyncAction"></param>
		public CustomValidator(Func<TProperty, PropertyValidatorContext<T,TProperty>, CancellationToken, Task> asyncAction) {
			_isAsync = true;
			_asyncAction = asyncAction;
			//TODO: For FV 9, throw an exception by default if async validator is being executed synchronously.
			_action = (x, ctx) => Task.Run(() => _asyncAction(x, ctx, new CancellationToken())).GetAwaiter().GetResult();
		}

		public override void Validate(PropertyValidatorContext<T,TProperty> context) {
			_action((TProperty) context.PropertyValue, context);
		}

		public override async Task ValidateAsync(PropertyValidatorContext<T,TProperty> context, CancellationToken cancellation) {
			await _asyncAction((TProperty)context.PropertyValue, context, cancellation);
		}

		protected sealed override bool IsValid(PropertyValidatorContext<T,TProperty> context) {
			throw new NotImplementedException();
		}

		public override bool ShouldValidateAsynchronously(IValidationContext context) {
			return _isAsync && context.IsAsync();
		}
	}
}
