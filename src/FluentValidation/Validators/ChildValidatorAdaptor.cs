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

	public class ChildValidatorAdaptor<T,TProperty> : NoopPropertyValidator<T,TProperty>, IChildValidatorAdaptor {
		private readonly Func<PropertyValidatorContext<T, TProperty>, IValidator<TProperty>> _validatorProvider;
		private readonly IValidator<TProperty> _validator;

		public override string Name => "ChildValidatorAdaptor";

		public Type ValidatorType { get; }

		public string[] RuleSets { get; set; }

		internal bool PassThroughParentContext { get; set; }

		public ChildValidatorAdaptor(IValidator<TProperty> validator, Type validatorType) {
			_validator = validator;
			ValidatorType = validatorType;
		}

		public ChildValidatorAdaptor(Func<PropertyValidatorContext<T, TProperty>, IValidator<TProperty>> validatorProvider, Type validatorType) {
			_validatorProvider = validatorProvider;
			ValidatorType = validatorType;
		}

		public override void Validate(PropertyValidatorContext<T,TProperty> context) {
			if (context.PropertyValue == null) {
				return;
			}

			var validator = GetValidator(context);

			if (validator == null) {
				return;
			}

			var newContext = CreateNewValidationContextForChildValidator(context);
			var totalFailures = ValidationContext<T>.GetFromNonGenericContext(context.ParentContext).Failures.Count;

			// If we're inside a collection with RuleForEach, then preserve the CollectionIndex placeholder
			// and pass it down to child validator by caching it in the RootContextData which flows through to
			// the child validator. PropertyValidator.PrepareMessageFormatterForValidationError handles extracting this.
			HandleCollectionIndex(context, out object originalIndex, out object currentIndex);

			var result = validator.Validate(newContext);

			// Reset the collection index
			ResetCollectionIndex(context, originalIndex, currentIndex);

			if (result.Errors.Count > totalFailures && OnFailure != null) {
				// Errors collection will contain all the errors from the validation run, not just those for the child validator.
				var firstError = result.Errors.Skip(totalFailures).First().ErrorMessage;
				OnFailure(context.InstanceToValidate, context, firstError);
			}
		}

		public override async Task ValidateAsync(PropertyValidatorContext<T,TProperty> context, CancellationToken cancellation) {
			if (context.PropertyValue == null) {
				return;
			}

			var validator = GetValidator(context);

			if (validator == null) {
				return;
			}

			var newContext = CreateNewValidationContextForChildValidator(context);
			var totalFailures = ValidationContext<T>.GetFromNonGenericContext(context.ParentContext).Failures.Count;

			// If we're inside a collection with RuleForEach, then preserve the CollectionIndex placeholder
			// and pass it down to child validator by caching it in the RootContextData which flows through to
			// the child validator. PropertyValidator.PrepareMessageFormatterForValidationError handles extracting this.
			HandleCollectionIndex(context, out object originalIndex, out object currentIndex);

			var result = await validator.ValidateAsync(newContext, cancellation);

			ResetCollectionIndex(context, originalIndex, currentIndex);

			if (result.Errors.Count > totalFailures && OnFailure != null) {
				// Errors collection will contain all the errors from the validation run, not just those for the child validator.
				var firstError = result.Errors.Skip(totalFailures).First().ErrorMessage;
				OnFailure(context.InstanceToValidate, context, firstError);
			}
		}

		public virtual IValidator GetValidator(PropertyValidatorContext<T,TProperty> context) {
			context.Guard("Cannot pass a null context to GetValidator", nameof(context));

			return _validatorProvider != null ? _validatorProvider(context) : _validator;
		}

		protected virtual IValidationContext CreateNewValidationContextForChildValidator(PropertyValidatorContext<T,TProperty> context) {
			var selector = GetSelector(context);
			var newContext = context.ParentContext.CloneForChildValidator(context.PropertyValue, PassThroughParentContext, selector);

			if(!context.ParentContext.IsChildCollectionContext)
				newContext.PropertyChain.Add(context.Rule.PropertyName);

			return newContext;
		}

		private protected virtual IValidatorSelector GetSelector(PropertyValidatorContext<T,TProperty> context) {
			return RuleSets?.Length > 0 ? new RulesetValidatorSelector(RuleSets) : null;
		}

		public override bool ShouldValidateAsynchronously(IValidationContext context) {
			return context.IsAsync() || HasAsyncCondition;
		}

		private void HandleCollectionIndex(PropertyValidatorContext<T,TProperty> context, out object originalIndex, out object index) {
			originalIndex = null;
			if (context.MessageFormatter.PlaceholderValues.TryGetValue("CollectionIndex", out index)) {
				context.ParentContext.RootContextData.TryGetValue("__FV_CollectionIndex", out originalIndex);
				context.ParentContext.RootContextData["__FV_CollectionIndex"] = index;
			}
		}

		private void ResetCollectionIndex(PropertyValidatorContext<T,TProperty> context, object originalIndex, object index) {
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
