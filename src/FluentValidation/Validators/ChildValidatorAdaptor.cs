namespace FluentValidation.Validators {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Internal;
	using Results;

	public class ChildValidatorAdaptor : NoopPropertyValidator {
		static readonly IEnumerable<ValidationFailure> EmptyResult = Enumerable.Empty<ValidationFailure>();
		static readonly Task<IEnumerable<ValidationFailure>> AsyncEmptyResult = TaskHelpers.FromResult(Enumerable.Empty<ValidationFailure>());

		readonly Func<IValidationContext, IValidator> _validatorProvider;

		public Type ValidatorType { get; protected set; }
		
		internal bool PassThroughParentContext { get; set; }

		public ChildValidatorAdaptor(IValidator validator) : this(_ => validator, validator.GetType()) {
		}

		public ChildValidatorAdaptor(Func<IValidationContext, IValidator> validatorProvider, Type validatorType) {
			_validatorProvider = validatorProvider;
			ValidatorType = validatorType;
		}

		public override IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context) {
			return ValidateInternal(
				context, 
				(ctx, v) => v.Validate(ctx).Errors,
				EmptyResult
			);
		}

		public override Task<IEnumerable<ValidationFailure>> ValidateAsync(PropertyValidatorContext context, CancellationToken cancellation) {
			return ValidateInternal(
				context, 
				async (ctx, v) => (await v.ValidateAsync(ctx, cancellation)).Errors.AsEnumerable(),
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

		public virtual IValidator GetValidator(PropertyValidatorContext context) {
			context.Guard("Cannot pass a null context to GetValidator", nameof(context));
			return _validatorProvider(context);
		}

		protected ValidationContext CreateNewValidationContextForChildValidator(object instanceToValidate, PropertyValidatorContext context) {
			var newContext = context.ParentContext.CloneForChildValidator(instanceToValidate, PassThroughParentContext);
			
			if(!context.ParentContext.IsChildCollectionContext)
				newContext.PropertyChain.Add(context.Rule.PropertyName);

			return newContext;
		}

		public override bool ShouldValidateAsync(ValidationContext context) {
			return context.IsAsync();
		}
	}
}