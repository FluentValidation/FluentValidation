namespace FluentValidation.Validators
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using FluentValidation.Internal;
	using FluentValidation.Resources;

	public delegate Task<bool> AsyncPredicate(object instance, object property, PropertyValidatorContext propertyValidatorContext, CancellationToken cancelToken);

	/// <summary>
	/// Asynchronous custom validator
	/// </summary>
	public class AsyncPredicateValidator : AsyncValidatorBase
	{
		private readonly AsyncPredicate predicate;

		/// <summary>
		/// Creates a new ASyncPredicateValidator
		/// </summary>
		/// <param name="predicate"></param>
		public AsyncPredicateValidator(AsyncPredicate predicate)
			: base(nameof(Messages.predicate_error), typeof(Messages))
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