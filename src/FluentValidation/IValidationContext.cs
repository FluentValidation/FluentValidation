namespace FluentValidation {
	using System;
	using System.Collections.Generic;
	using Internal;

	/// <summary>
	/// Defines a validation context.
	/// </summary>
	public interface ICommonContext {
		/// <summary>
		/// The object currently being validated.
		/// </summary>
		object InstanceToValidate { get; }

		/// <summary>
		/// The value of the property being validated.
		/// </summary>
		object PropertyValue { get; }

		/// <summary>
		/// Parent validation context.
		/// </summary>
		ICommonContext ParentContext { get; }
	}

	public interface IValidationContext : ICommonContext {
		/// <summary>
		/// Additional data associated with the validation request.
		/// </summary>
		IDictionary<string, object> RootContextData { get; }

		/// <summary>
		/// Property chain
		/// </summary>
		PropertyChain PropertyChain { get; }

		/// <summary>
		/// Selector
		/// </summary>
		IValidatorSelector Selector { get; }

		/// <summary>
		/// Whether this is a child context
		/// </summary>
		bool IsChildContext { get; }

		/// <summary>
		/// Whether this is a child collection context.
		/// </summary>
		bool IsChildCollectionContext { get; }
	}

	/// <summary>
	/// Validation context
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ValidationContext<T> : IValidationContext {
		private ICommonContext _parentContext;

		/// <summary>
		/// Creates a new validation context
		/// </summary>
		/// <param name="instanceToValidate"></param>
		public ValidationContext(T instanceToValidate) : this(instanceToValidate, new PropertyChain(), ValidatorOptions.ValidatorSelectors.DefaultValidatorSelectorFactory()) {
		}

		/// <summary>
		/// Creates a new validation context with a custom property chain and selector
		/// </summary>
		/// <param name="instanceToValidate"></param>
		/// <param name="propertyChain"></param>
		/// <param name="validatorSelector"></param>
		public ValidationContext(T instanceToValidate, PropertyChain propertyChain, IValidatorSelector validatorSelector) {
			PropertyChain = new PropertyChain(propertyChain);
			InstanceToValidate = instanceToValidate;
			Selector = validatorSelector;
		}

		/// <summary>
		/// The object to validate
		/// </summary>
		public T InstanceToValidate { get; private set; }

		/// <summary>
		/// Additional data associated with the validation request.
		/// </summary>
		public IDictionary<string, object> RootContextData { get; private protected set; } = new Dictionary<string, object>();


		/// <summary>
		/// Property chain
		/// </summary>
		public PropertyChain PropertyChain { get; private set; }

		/// <summary>
		/// Object being validated
		/// </summary>
		object ICommonContext.InstanceToValidate => InstanceToValidate;

		/// <summary>
		/// Selector
		/// </summary>
		public IValidatorSelector Selector { get; private set; }

		/// <summary>
		/// Whether this is a child context
		/// </summary>
		public virtual bool IsChildContext { get; internal set; }

		/// <summary>
		/// Whether this is a child collection context.
		/// </summary>
		public virtual bool IsChildCollectionContext { get; internal set; }

		// root level context doesn't know about properties.
		object ICommonContext.PropertyValue => null;

		// This is the root context so it doesn't have a parent.
		// Explicit implementation so it's not exposed necessarily.
		ICommonContext ICommonContext.ParentContext => _parentContext;


		/// <summary>
		/// Gets or creates generic validation context from non-generic validation context.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="NotSupportedException"></exception>
		public static ValidationContext<T> GetFromNonGenericContext(IValidationContext context) {
			if (context == null) throw new ArgumentNullException(nameof(context));

			// Already of the correct type.
			if (context is ValidationContext<T> c) {
				return c;
			}

			// Parameters match
			if (context.InstanceToValidate is T instanceToValidate) {
				return new ValidationContext<T>(instanceToValidate, context.PropertyChain, context.Selector) {
					IsChildContext = context.IsChildContext,
					RootContextData = context.RootContextData,
					_parentContext = context.ParentContext
				};
			}

			if (context.InstanceToValidate == null) {
				return new ValidationContext<T>(default, context.PropertyChain, context.Selector) {
					IsChildContext = context.IsChildContext,
					RootContextData = context.RootContextData,
					_parentContext = context.ParentContext
				};
			}

			throw new InvalidOperationException($"Cannot validate instances of type '{context.InstanceToValidate.GetType().Name}'. This validator can only validate instances of type '{typeof(T).Name}'.");
		}

		/// <summary>
		/// Creates a new validation context for use with a child validator
		/// </summary>
		/// <param name="instanceToValidate"></param>
		/// <param name="preserveParentContext"></param>
		/// <param name="selector"></param>
		/// <returns></returns>
		public ValidationContext<TChild> CloneForChildValidator<TChild>(TChild instanceToValidate, bool preserveParentContext = false, IValidatorSelector selector = null) {
			return new ValidationContext<TChild>(instanceToValidate, PropertyChain, selector ?? Selector) {
				IsChildContext = true,
				RootContextData = RootContextData,
				_parentContext = preserveParentContext ? this : null
			};
		}

		/// <summary>
		/// Creates a new validation context for use with a child collection validator
		/// </summary>
		/// <param name="instanceToValidate"></param>
		/// <param name="preserveParentContext"></param>
		/// <returns></returns>
		public ValidationContext<TNew> CloneForChildCollectionValidator<TNew>(TNew instanceToValidate, bool preserveParentContext = false) {
			return new ValidationContext<TNew>(instanceToValidate, null, Selector) {
				IsChildContext = true,
				IsChildCollectionContext = true,
				RootContextData = RootContextData,
				_parentContext = preserveParentContext ? this : null
			};
		}
	}
}
