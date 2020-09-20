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

	public class ChildValidatorAdaptor<T,TProperty> : NoopPropertyValidator, IChildValidatorAdaptor {
		private readonly Func<ICommonContext, IValidator<TProperty>> _validatorProvider;
		private readonly IValidator<TProperty> _validator;

		public Type ValidatorType { get; }

		public string[] RuleSets { get; set; }

		internal bool PassThroughParentContext { get; set; }

		public ChildValidatorAdaptor(IValidator<TProperty> validator, Type validatorType) {
			_validator = validator;
			ValidatorType = validatorType;
		}

		public ChildValidatorAdaptor(Func<ICommonContext, IValidator<TProperty>> validatorProvider, Type validatorType) {
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

			var newContext = CreateNewValidationContextForChildValidator(context, validator);

			// If we're inside a collection with RuleForEach, then preserve the CollectionIndex placeholder
			// and pass it down to child validator by caching it in the RootContextData which flows through to
			// the child validator. PropertyValidator.PrepareMessageFormatterForValidationError handles extracting this.
			HandleCollectionIndex(context, out object originalIndex, out object currentIndex);

			var results = validator.Validate(newContext).Errors;

			// Reset the collection index
			ResetCollectionIndex(context, originalIndex, currentIndex);

			return results;
		}

		public override async Task<IEnumerable<ValidationFailure>> ValidateAsync(PropertyValidatorContext context, CancellationToken cancellation) {
			if (context.PropertyValue == null) {
				return Enumerable.Empty<ValidationFailure>();
			}

			var validator = GetValidator(context);

			if (validator == null) {
				return Enumerable.Empty<ValidationFailure>();
			}

			var newContext = CreateNewValidationContextForChildValidator(context, validator);

			// If we're inside a collection with RuleForEach, then preserve the CollectionIndex placeholder
			// and pass it down to child validator by caching it in the RootContextData which flows through to
			// the child validator. PropertyValidator.PrepareMessageFormatterForValidationError handles extracting this.
			HandleCollectionIndex(context, out object originalIndex, out object currentIndex);

			var result = await validator.ValidateAsync(newContext, cancellation);

			ResetCollectionIndex(context, originalIndex, currentIndex);

			return result.Errors;
		}

		public virtual IValidator GetValidator(PropertyValidatorContext context) {
			context.Guard("Cannot pass a null context to GetValidator", nameof(context));

			return _validatorProvider != null ? _validatorProvider(context) : _validator;
		}

		protected virtual IValidationContext CreateNewValidationContextForChildValidator(PropertyValidatorContext context, IValidator validator) {
			var selector = RuleSets?.Length > 0 ? new RulesetValidatorSelector(RuleSets) : null;
			var parentContext = ValidationContext<T>.GetFromNonGenericContext(context.ParentContext);
			var newContext = parentContext.CloneForChildValidator((TProperty)context.PropertyValue, PassThroughParentContext, selector);

			if(!parentContext.IsChildCollectionContext)
				newContext.PropertyChain.Add(context.Rule.PropertyName);

			return newContext;
		}

		public override bool ShouldValidateAsynchronously(IValidationContext context) {
			return context.IsAsync() || Options.HasAsyncCondition;
		}

		private void HandleCollectionIndex(PropertyValidatorContext context, out object originalIndex, out object index) {
			originalIndex = null;
			if (context.MessageFormatter.PlaceholderValues.TryGetValue("CollectionIndex", out index)) {
				context.ParentContext.RootContextData.TryGetValue("__FV_CollectionIndex", out originalIndex);
				context.ParentContext.RootContextData["__FV_CollectionIndex"] = index;
			}
		}

		private void ResetCollectionIndex(PropertyValidatorContext context, object originalIndex, object index) {
			// Reset the collection index
			if (index != null) {
				if (originalIndex != null) {
					context.ParentContext.RootContextData["__FV_CollectionIndex"] = originalIndex;
				}
				else {
					context.ParentContext.RootContextData.Remove("__FV_CollectionIndex");
				}
			}
		}
	}
}
