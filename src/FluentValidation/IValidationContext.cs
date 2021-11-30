#region License
// Copyright (c) .NET Foundation and contributors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// The latest version of this file can be found at https://github.com/FluentValidation/FluentValidation
#endregion

namespace FluentValidation {
	using System;
	using System.Collections.Generic;
	using Internal;
	using Results;

	public interface IValidationContext {
		/// <summary>
		/// The object currently being validated.
		/// </summary>
		object InstanceToValidate { get; }

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

		/// <summary>
		/// Parent validation context.
		/// </summary>
		IValidationContext ParentContext { get; }

		/// <summary>
		/// Whether this context is async.
		/// </summary>
		bool IsAsync { get; }
	}

	internal interface IHasFailures {
		List<ValidationFailure> Failures { get; }
	}

	//TODO: Temporary interface to avoid introducing a breaking change. Consolidate with IValidationContext in 11.0
	internal interface IThrowOnFailures {
		bool ThrowOnFailures { get; }
	}

	/// <summary>
	/// Validation context
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ValidationContext<T> : IValidationContext, IHasFailures, IThrowOnFailures {
		private IValidationContext _parentContext;

		List<ValidationFailure> IHasFailures.Failures => Failures;
		internal List<ValidationFailure> Failures { get; }

		/// <summary>
		/// The message formatter used to construct error messages.
		/// </summary>
		public MessageFormatter MessageFormatter { get; }

		/// <summary>
		/// Creates a new validation context
		/// </summary>
		/// <param name="instanceToValidate"></param>
		public ValidationContext(T instanceToValidate)
			: this(instanceToValidate, null, ValidatorOptions.Global.ValidatorSelectors.DefaultValidatorSelectorFactory()) {
		}

		/// <summary>
		/// Creates a new validation context with a custom property chain and selector
		/// </summary>
		/// <param name="instanceToValidate"></param>
		/// <param name="propertyChain"></param>
		/// <param name="validatorSelector"></param>
		public ValidationContext(T instanceToValidate, PropertyChain propertyChain, IValidatorSelector validatorSelector)
			: this(instanceToValidate, propertyChain, validatorSelector, new List<ValidationFailure>(), ValidatorOptions.Global.MessageFormatterFactory()) {
		}

		internal ValidationContext(T instanceToValidate, PropertyChain propertyChain, IValidatorSelector validatorSelector, List<ValidationFailure> failures, MessageFormatter messageFormatter) {
			PropertyChain = new PropertyChain(propertyChain);
			InstanceToValidate = instanceToValidate;
			Selector = validatorSelector;
			Failures = failures;
			MessageFormatter = messageFormatter;
		}

		/// <summary>
		/// Creates a new validation context using the specified options.
		/// </summary>
		/// <param name="instanceToValidate">The instance to validate</param>
		/// <param name="options">Callback that allows extra options to be configured.</param>
		public static ValidationContext<T> CreateWithOptions(T instanceToValidate, Action<ValidationStrategy<T>> options) {
			if (options == null) throw new ArgumentNullException(nameof(options));
			var strategy = new ValidationStrategy<T>();
			options(strategy);
			return strategy.BuildContext(instanceToValidate);
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
		object IValidationContext.InstanceToValidate => InstanceToValidate;

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

		// This is the root context so it doesn't have a parent.
		// Explicit implementation so it's not exposed necessarily.
		IValidationContext IValidationContext.ParentContext => _parentContext;

		/// <inheritdoc />
		public bool IsAsync {
			get;
			internal set;
		}

		/// <summary>
		/// Whether the root validator should throw an exception when validation fails.
		/// Defaults to false.
		/// </summary>
		public bool ThrowOnFailures { get; internal set; }


		private Dictionary<string, Dictionary<T, bool>> _sharedConditionCache;

		/// <summary>
		/// Shared condition results cache.
		/// The key of the outer dictionary is the ID of the condition, and its value is the cache for that condition.
		/// The key of the inner dictionary is the instance being validated, and the value is the condition result.
		/// </summary>
		internal Dictionary<string, Dictionary<T, bool>> SharedConditionCache {
			get {
				_sharedConditionCache ??= new();
				return _sharedConditionCache;
			}
		}

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
				var failures = (context is IHasFailures f) ? f.Failures : new List<ValidationFailure>();

				return new ValidationContext<T>(instanceToValidate, context.PropertyChain, context.Selector, failures, ValidatorOptions.Global.MessageFormatterFactory()) {
					IsChildContext = context.IsChildContext,
					RootContextData = context.RootContextData,
					ThrowOnFailures = context is IThrowOnFailures { ThrowOnFailures: true },
					_parentContext = context.ParentContext
				};
			}

			if (context.InstanceToValidate == null) {
				var failures = (context is IHasFailures f) ? f.Failures : new List<ValidationFailure>();

				return new ValidationContext<T>(default, context.PropertyChain, context.Selector, failures, ValidatorOptions.Global.MessageFormatterFactory()) {
					IsChildContext = context.IsChildContext,
					RootContextData = context.RootContextData,
					ThrowOnFailures = context is IThrowOnFailures { ThrowOnFailures: true },
					_parentContext = context.ParentContext,
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
			return new ValidationContext<TChild>(instanceToValidate, PropertyChain, selector ?? Selector, Failures, MessageFormatter) {
				IsChildContext = true,
				RootContextData = RootContextData,
				_parentContext = preserveParentContext ? this : null,
				IsAsync = IsAsync,
			};
		}

		internal void PrepareForChildCollectionValidator() {
			_state ??= new();
			_state.Push((IsChildContext, IsChildCollectionContext, _parentContext, PropertyChain, _sharedConditionCache));
			IsChildContext = true;
			IsChildCollectionContext = true;
			PropertyChain = new PropertyChain();
		}

		internal void RestoreState() {
			var state = _state.Pop();
			IsChildContext = state.IsChildContext;
			IsChildCollectionContext = state.IsChildCollectionContext;
			_parentContext = state.ParentContext;
			PropertyChain = state.Chain;
			_sharedConditionCache = state.SharedConditionCache;
		}


		private Stack<(bool IsChildContext, bool IsChildCollectionContext, IValidationContext ParentContext, PropertyChain Chain, Dictionary<string, Dictionary<T, bool>> SharedConditionCache)> _state;

		/// <summary>
		/// Adds a new validation failure.
		/// </summary>
		/// <param name="failure">The failure to add.</param>
		/// <exception cref="ArgumentNullException"></exception>
		public void AddFailure(ValidationFailure failure) {
			if (failure == null) throw new ArgumentNullException(nameof(failure), "A failure must be specified when calling AddFailure");
			Failures.Add(failure);
		}

		/// <summary>
		/// Adds a new validation failure for the specified property.
		/// </summary>
		/// <param name="propertyName">The property name</param>
		/// <param name="errorMessage">The error message</param>
		public void AddFailure(string propertyName, string errorMessage) {
			errorMessage.Guard("An error message must be specified when calling AddFailure.", nameof(errorMessage));
			errorMessage = MessageFormatter.BuildMessage(errorMessage);
			AddFailure(new ValidationFailure(PropertyChain.BuildPropertyName(propertyName ?? string.Empty), errorMessage));
		}

		/// <summary>
		/// Adds a new validation failure for the specified message.
		/// The failure will be associated with the current property being validated.
		/// </summary>
		/// <param name="errorMessage">The error message</param>
		public void AddFailure(string errorMessage) {
			errorMessage.Guard("An error message must be specified when calling AddFailure.", nameof(errorMessage));
			errorMessage = MessageFormatter.BuildMessage(errorMessage);
			AddFailure(new ValidationFailure(PropertyName, errorMessage));
		}

		private Func<ValidationContext<T>, string> _displayNameFunc;

		/// <summary>
		/// Gets the display name for the current property being validated.
		/// </summary>
		public string DisplayName => _displayNameFunc(this);

		/// <summary>
		/// The full name of the current property being validated.
		/// If accessed inside a child validator, this will include the parent's path too.
		/// </summary>
		public string PropertyName { get; private set; }

		internal string RawPropertyName { get; private set; }

		internal void InitializeForPropertyValidator(string propertyName, Func<ValidationContext<T>, string> displayNameFunc, string rawPropertyName) {
			PropertyName = propertyName;
			_displayNameFunc = displayNameFunc;
			RawPropertyName = rawPropertyName;
		}
	}
}
