namespace FluentValidation.Validators {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Results;

	public class ChildValidatorAdaptor : NoopPropertyValidator {
		readonly IValidator validator;

		static readonly IEnumerable<ValidationFailure> EmptyResult = Enumerable.Empty<ValidationFailure>();

		static readonly Task<IEnumerable<ValidationFailure>> AsyncEmptyResult = TaskHelpers.FromResult(Enumerable.Empty<ValidationFailure>());

		public IValidator Validator {
			get { return validator; }
		}

		public override bool IsAsync {
			get { return true; }
		}

		public ChildValidatorAdaptor(IValidator validator) {
			this.validator = validator;
		}

		public override IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context) {
			return ValidateInternal(
				context, 
				(ctx, v) => v.Validate(ctx).Errors,
				EmptyResult
			);
		}

		public override Task<IEnumerable<ValidationFailure>> ValidateAsync(PropertyValidatorContext context) {
			return ValidateInternal(
				context, 
				(ctx, v) => v.ValidateAsync(ctx).Then(r => r.Errors.AsEnumerable(), runSynchronously:true),
				AsyncEmptyResult
			);
		}

		private TResult ValidateInternal<TResult>(PropertyValidatorContext context, Func<ValidationContext, IValidator, TResult> validationApplicator, TResult emptyResult) {
			var instanceToValidate = context.PropertyValue;

			if (instanceToValidate == null) {
				return emptyResult;
			}

			var validator = GetValidator(context);

			if (validator == null) {
				return emptyResult;
			}

			var newContext = CreateNewValidationContextForChildValidator(instanceToValidate, context);

			return validationApplicator(newContext, validator);
		}

		protected virtual IValidator GetValidator(PropertyValidatorContext context) {
			return Validator;
		}

		protected ValidationContext CreateNewValidationContextForChildValidator(object instanceToValidate, PropertyValidatorContext context) {
			var newContext = context.ParentContext.CloneForChildValidator(instanceToValidate);
			newContext.PropertyChain.Add(context.Rule.PropertyName);
			return newContext;
		}
	}
}