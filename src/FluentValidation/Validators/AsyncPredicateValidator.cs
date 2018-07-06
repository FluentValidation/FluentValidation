namespace FluentValidation.Validators {
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using FluentValidation.Internal;
	using FluentValidation.Resources;

	/// <summary>
	/// Asynchronous custom validator
	/// </summary>
	public class AsyncPredicateValidator : PropertyValidator {
		private readonly Func<object, object, PropertyValidatorContext, CancellationToken, Task<bool>> _predicate;

		/// <summary>
		/// Creates a new AsyncPredicateValidator
		/// </summary>
		/// <param name="predicate"></param>
		public AsyncPredicateValidator(Func<object, object, PropertyValidatorContext, CancellationToken, Task<bool>> predicate) : base(new LanguageStringSource(nameof(AsyncPredicateValidator))) {
			predicate.Guard("A predicate must be specified.", nameof(predicate));
			this._predicate = predicate;
		}

		protected override Task<bool> IsValidAsync(PropertyValidatorContext context, CancellationToken cancellation) {
			return _predicate(context.Instance, context.PropertyValue, context, cancellation);
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			return Task.Run(() => IsValidAsync(context, new CancellationToken())).GetAwaiter().GetResult();
		}

		public override bool ShouldValidateAsync(ValidationContext context) {
			return context.IsAsync();
		}
	}
}