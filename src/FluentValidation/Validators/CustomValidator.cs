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

		public override string Name => "CustomValidator";

		/// <summary>
		/// Creates a new instance of the CustomValidator
		/// </summary>
		/// <param name="action"></param>
		public CustomValidator(Action<TProperty, ValidationContext<T>> action) {
			_action = action;
		}

		public override bool IsValid(ValidationContext<T> context, TProperty value) {
			_action(value, context);
			return true;
		}
	}

	/// <summary>
	/// Custom validator that allows for manual/direct creation of ValidationFailure instances.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TProperty"></typeparam>
	public class AsyncCustomValidator<T, TProperty> : AsyncPropertyValidator<T,TProperty> {
		private Func<TProperty, ValidationContext<T>, CancellationToken, Task> _asyncAction;

		public override string Name => "CustomValidator";

		/// <summary>
		/// Creates a new instance of the CustomValidator.
		/// </summary>
		/// <param name="asyncAction"></param>
		public AsyncCustomValidator(Func<TProperty, ValidationContext<T>, CancellationToken, Task> asyncAction) {
			_asyncAction = asyncAction;
		}

		public override async Task<bool> IsValidAsync(ValidationContext<T> context, TProperty value, CancellationToken cancellation) {
			await _asyncAction(value, context, cancellation);
			return true;
		}
	}

}
