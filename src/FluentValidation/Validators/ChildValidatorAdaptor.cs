namespace FluentValidation.Validators {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Internal;
	using Results;

	/// <summary>
	/// Indicates that this validator wraps another validator.
	/// </summary>
	public interface IChildValidatorAdaptor {
		/// <summary>
		/// The type of the underlying validator
		/// </summary>
		Type ValidatorType { get; }
	}

	public class ChildValidatorAdaptor : NoopPropertyValidator, IChildValidatorAdaptor {
		private readonly Func<IValidationContext, IValidator> _validatorProvider;
		private readonly IValidator _validator;

		public Type ValidatorType { get; }

		public string[] RuleSets { get; set; }

		internal bool PassThroughParentContext { get; set; }

		public ChildValidatorAdaptor(IValidator validator, Type validatorType) {
			_validator = validator;
			ValidatorType = validatorType;
		}

		public ChildValidatorAdaptor(Func<IValidationContext, IValidator> validatorProvider, Type validatorType) {
			_validatorProvider = validatorProvider;
			ValidatorType = validatorType;
		}

		public override IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context) {
			if (Options.Condition != null && !Options.Condition(context)) {
				return Enumerable.Empty<ValidationFailure>();
			} 

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
			if (Options.Condition != null && !Options.Condition(context)) {
				return Enumerable.Empty<ValidationFailure>();
			} 
			
			if (Options.AsyncCondition != null && !await Options.AsyncCondition(context, cancellation)) {
				return Enumerable.Empty<ValidationFailure>();
			}
			
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

			return _validatorProvider != null ? _validatorProvider(context) : _validator;
		}

		protected ValidationContext CreateNewValidationContextForChildValidator(object instanceToValidate, PropertyValidatorContext context) {
			var selector = RuleSets?.Length > 0 ? new RulesetValidatorSelector(RuleSets) : null;
			var newContext = context.ParentContext.CloneForChildValidator(instanceToValidate, PassThroughParentContext, selector);

			if(!context.ParentContext.IsChildCollectionContext)
				newContext.PropertyChain.Add(context.Rule.PropertyName);

			return newContext;
		}

		public override bool ShouldValidateAsynchronously(ValidationContext context) {
			return context.IsAsync() || Options.AsyncCondition != null;
		}
	}
}