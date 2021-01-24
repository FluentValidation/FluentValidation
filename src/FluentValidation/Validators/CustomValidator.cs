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
		private readonly Action<TProperty, ValidationContext<T>> _action;
		private Func<TProperty, ValidationContext<T>, CancellationToken, Task> _asyncAction;
		private readonly bool _isAsync;

		public override string Name => "CustomValidator";

		/// <summary>
		/// Creates a new instance of the CustomValidator
		/// </summary>
		/// <param name="action"></param>
		public CustomValidator(Action<TProperty, ValidationContext<T>> action) {
			_isAsync = false;
			_action = action;

			_asyncAction = (x, ctx, cancel) => Task.Run(() => action(x, ctx), cancel);
		}

		/// <summary>
		/// Creates a new instance of the CustomValidator.
		/// </summary>
		/// <param name="asyncAction"></param>
		public CustomValidator(Func<TProperty, ValidationContext<T>, CancellationToken, Task> asyncAction) {
			_isAsync = true;
			_asyncAction = asyncAction;
			//TODO: For FV 9, throw an exception by default if async validator is being executed synchronously.
			_action = (x, ctx) => Task.Run(() => _asyncAction(x, ctx, new CancellationToken())).GetAwaiter().GetResult();
		}

		public override bool IsValid(ValidationContext<T> context, TProperty value) {
			_action(value, context);
			return true;
		}

		public override async Task<bool> IsValidAsync(ValidationContext<T> context, TProperty value, CancellationToken cancellation) {
			await _asyncAction(value, context, cancellation);
			return true;
		}

		public override bool ShouldValidateAsynchronously(IValidationContext context) {
			return _isAsync && context.IsAsync();
		}
	}
}
