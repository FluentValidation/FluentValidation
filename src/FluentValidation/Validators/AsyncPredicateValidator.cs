namespace FluentValidation.Validators
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using FluentValidation.Internal;
	using FluentValidation.Resources;

	/// <summary>
	/// Asynchronous custom validator
	/// </summary>
	public class AsyncPredicateValidator : AsyncValidatorBase
	{
		private readonly Func<object, object, PropertyValidatorContext, CancellationToken, Task<bool>> predicate;

		/// <summary>
		/// Creates a new ASyncPredicateValidator
		/// </summary>
		/// <param name="predicate"></param>
		public AsyncPredicateValidator(Func<object, object, PropertyValidatorContext, CancellationToken, Task<bool>> predicate) : base(new LanguageStringSource(nameof(AsyncPredicateValidator)))
		{
			predicate.Guard("A predicate must be specified.");
			this.predicate = predicate;
		}

		/// <summary>
		/// Runs the validation check
		/// </summary>
		/// <param name="context"></param>
		/// <param name="cancellation"></param>
		/// <returns></returns>
		protected override Task<bool> IsValidAsync(PropertyValidatorContext context, CancellationToken cancellation)
		{
			return predicate(context.Instance, context.PropertyValue, context, cancellation);
		}
	}
}