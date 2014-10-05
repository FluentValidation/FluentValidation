namespace FluentValidation.Validators {
	using System;
	using System.Threading.Tasks;
	using FluentValidation.Internal;
	using FluentValidation.Resources;

	public class AsyncPredicateValidator : AsyncValidatorBase {
		private readonly Func<object, object, PropertyValidatorContext, Task<bool>> predicate;
		public AsyncPredicateValidator(Func<object, object, PropertyValidatorContext, Task<bool>> predicate)
			: base(() => Messages.predicate_error) {
			predicate.Guard("A predicate must be specified.");
			this.predicate = predicate;
		}

		protected override Task<bool> IsValidAsync(PropertyValidatorContext context) {
			return predicate(context.Instance, context.PropertyValue, context);
		}
	}
}