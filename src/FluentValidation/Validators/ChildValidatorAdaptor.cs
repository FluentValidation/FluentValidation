namespace FluentValidation.Validators {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Results;

	public class ChildValidatorAdaptor : NoopPropertyValidator {
		readonly IValidator validator;

		public IValidator Validator {
			get { return validator; }
		}

		public ChildValidatorAdaptor(IValidator validator) {
			this.validator = validator;
		}

		public override IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context) {
			return ValidateInternal(
				context, 
				(ctx, v) => v.Validate(ctx).Errors
			);
		}

		public override Task<IEnumerable<ValidationFailure>> ValidateAsync(PropertyValidatorContext context) {
			return ValidateInternal(
				context, 
				(ctx, v) => v.ValidateAsync(ctx).Then(r => r.Errors.AsEnumerable(), runSynchronously:true)
			);
		}

		TResult ValidateInternal<TResult>(PropertyValidatorContext context, Func<ValidationContext, IValidator, TResult> validationApplicator) {
			if (context.Rule.Member == null) {
				throw new InvalidOperationException(string.Format("Nested validators can only be used with Member Expressions."));
			}

			var instanceToValidate = context.PropertyValue;

			if (instanceToValidate == null) {
				return (TResult) Enumerable.Empty<ValidationFailure>();
			}

			var validator = GetValidator(context);

			if (validator == null) {
				return (TResult) Enumerable.Empty<ValidationFailure>();
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