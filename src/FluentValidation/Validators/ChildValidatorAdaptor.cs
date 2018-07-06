namespace FluentValidation.Validators {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Internal;
	using Results;

	public class ChildValidatorAdaptor : NoopPropertyValidator {
		readonly Func<IValidationContext, IValidator> _validatorProvider;

		public Type ValidatorType { get; }
		
		internal bool PassThroughParentContext { get; set; }

		public ChildValidatorAdaptor(IValidator validator) : this(_ => validator, validator.GetType()) {
		}

		public ChildValidatorAdaptor(Func<IValidationContext, IValidator> validatorProvider, Type validatorType) {
			_validatorProvider = validatorProvider;
			ValidatorType = validatorType;
		}

		public override IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context) {
			if (context.PropertyValue == null) {
				return Enumerable.Empty<ValidationFailure>();
			}

			var validator = GetValidator(context);

			if (validator == null) {
				return Enumerable.Empty<ValidationFailure>();
			}

			var newContext = CreateNewValidationContextForChildValidator(context.PropertyValue, context);
			return validator.Validate(newContext).Errors;
		}

		public override async Task<IEnumerable<ValidationFailure>> ValidateAsync(PropertyValidatorContext context, CancellationToken cancellation) {
			if (context.PropertyValue == null) {
				return Enumerable.Empty<ValidationFailure>();
			}

			var validator = GetValidator(context);

			if (validator == null) {
				return Enumerable.Empty<ValidationFailure>();
			}

			var newContext = CreateNewValidationContextForChildValidator(context.PropertyValue, context);
			var result = await validator.ValidateAsync(newContext, cancellation);
			return result.Errors;
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