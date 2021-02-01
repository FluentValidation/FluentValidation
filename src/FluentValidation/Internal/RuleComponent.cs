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

namespace FluentValidation.Internal {
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using Internal;
	using Validators;

	/// <summary>
	/// Provides additional metadata about a property validator.
	/// </summary>
	public interface IRuleComponent {
		/// <summary>
		/// Whether or not this validator has a condition associated with it.
		/// </summary>
		bool HasCondition { get; }

		/// <summary>
		/// Whether or not this validator has an async condition associated with it.
		/// </summary>
		bool HasAsyncCondition { get; }

		/// <summary>
		/// The validator associated with this component.
		/// </summary>
		IPropertyValidator Validator { get; }

		/// <summary>
		/// Gets the raw unformatted error message. Placeholders will not have been rewritten.
		/// </summary>
		/// <returns></returns>
		string GetUnformattedErrorMessage();
	}

	/// <summary>
	/// An individual component within a rule.
	/// In a rule definition such as RuleFor(x => x.Name).NotNull().NotEqual("Foo")
	/// the NotNull and the NotEqual are both rule steps.
	/// </summary>
	public sealed class RuleComponent<T,TProperty> : IRuleComponent {
		private string _errorMessage;
		private Func<ValidationContext<T>, TProperty, string> _errorMessageFactory;
		private Func<IValidationContext, bool> _condition;
		private Func<IValidationContext, CancellationToken, Task<bool>> _asyncCondition;

		public IPropertyValidator<T,TProperty> PropertyValidator { get; }
		public IAsyncPropertyValidator<T, TProperty> AsyncPropertyValidator { get; }

		internal RuleComponent(IPropertyValidator<T, TProperty> propertyValidator) {
			PropertyValidator = propertyValidator;
		}

		internal RuleComponent(IAsyncPropertyValidator<T, TProperty> asyncPropertyValidator) {
			AsyncPropertyValidator = asyncPropertyValidator;

			// Async validators may support both sync and async execution.
			// Check if we're in that situation.
			if (asyncPropertyValidator is IPropertyValidator<T, TProperty> p) {
				PropertyValidator = p;
			}
		}

		[Obsolete("The Options property will be removed in FluentValidation 11. All properties from Options should be accessed directly on this component instead.")]
		private RuleComponent<T, TProperty> Options => this;

		/// <summary>
		/// Whether or not this validator has a condition associated with it.
		/// </summary>
		public bool HasCondition => _condition != null;

		/// <summary>
		/// Whether or not this validator has an async condition associated with it.
		/// </summary>
		public bool HasAsyncCondition => _asyncCondition != null;

		IPropertyValidator IRuleComponent.Validator
			=> (IPropertyValidator) PropertyValidator ?? AsyncPropertyValidator;

		internal bool ShouldValidateAsynchronously(IValidationContext context) {
			// If ValidateAsync has been invoked on the root validator, then always prefer
			// the asynchronous property validator (if available).
			if (context.IsAsync()) {
				if (AsyncPropertyValidator != null) {
					return true;
				}

				// Fallback to sync if no async validator available.
				return false;
			}

			// If Validate has been invoked on the root validator, then always prefer
			// the synchronous validator.
			if (PropertyValidator != null) {
				return false;
			}

			// Fall back to sync-over-async if only an async validator is available.
			return true;
		}

		/// <summary>
		/// Adds a condition for this validator. If there's already a condition, they're combined together with an AND.
		/// </summary>
		/// <param name="condition"></param>
		public void ApplyCondition(Func<IValidationContext, bool> condition) {
			if (_condition == null) {
				_condition = condition;
			}
			else {
				var original = _condition;
				_condition = ctx => condition(ctx) && original(ctx);
			}
		}

		/// <summary>
		/// Adds a condition for this validator. If there's already a condition, they're combined together with an AND.
		/// </summary>
		/// <param name="condition"></param>
		public void ApplyAsyncCondition(Func<IValidationContext, CancellationToken, Task<bool>> condition) {
			if (_asyncCondition == null) {
				_asyncCondition = condition;
			}
			else {
				var original = _asyncCondition;
				_asyncCondition = async (ctx, ct) => await condition(ctx, ct) && await original(ctx, ct);
			}
		}

		internal bool InvokeCondition(IValidationContext context) {
			if (_condition != null) {
				return _condition(context);
			}

			return true;
		}

		internal async Task<bool> InvokeAsyncCondition(IValidationContext context, CancellationToken token) {
			if (_asyncCondition != null) {
				return await _asyncCondition(context, token);
			}

			return true;
		}

		/// <summary>
		/// Function used to retrieve custom state for the validator
		/// </summary>
		public Func<ValidationContext<T>, TProperty, object> CustomStateProvider { get; set; }

		/// <summary>
		/// Function used to retrieve the severity for the validator
		/// </summary>
		public Func<ValidationContext<T>, TProperty, Severity> SeverityProvider { get; set; }

		/// <summary>
		/// Retrieves the error code.
		/// </summary>
		public string ErrorCode { get; set; }

		/// <summary>
		/// Gets the error message. If a context is supplied, it will be used to format the message if it has placeholders.
		/// If no context is supplied, the raw unformatted message will be returned, containing placeholders.
		/// </summary>
		/// <param name="context">The validation context.</param>
		/// <param name="value">The current property value.</param>
		/// <returns>Either the formatted or unformatted error message.</returns>
		public string GetErrorMessage(ValidationContext<T> context, TProperty value) {
			// Use a custom message if one has been specified.
			string rawTemplate = _errorMessageFactory?.Invoke(context, value) ?? _errorMessage;

			// If no custom message has been supplied, use the default from the synchronous validator
			// if available.
			if (rawTemplate == null && PropertyValidator != null) {
				rawTemplate = PropertyValidator.GetDefaultMessageTemplate();
			}

			// Otherwise try the asynchornous validator.
			if (rawTemplate == null && AsyncPropertyValidator != null) {
				rawTemplate = AsyncPropertyValidator.GetDefaultMessageTemplate();
			}

			if (context == null) {
				return rawTemplate;
			}

			return context.MessageFormatter.BuildMessage(rawTemplate);
		}

		/// <summary>
		/// Gets the raw unformatted error message. Placeholders will not have been rewritten.
		/// </summary>
		/// <returns></returns>
		public string GetUnformattedErrorMessage() {
			string message = _errorMessageFactory?.Invoke(null, default) ?? _errorMessage;

			// If no custom message has been supplied, use the default from the synchronous validator
			// if available.
			if (message == null && PropertyValidator != null) {
				message = PropertyValidator.GetDefaultMessageTemplate();
			}

			// Otherwise try the asynchronous validator.
			if (message == null && AsyncPropertyValidator != null) {
				message = AsyncPropertyValidator.GetDefaultMessageTemplate();
			}

			return message;
		}

		/// <summary>
		/// Sets the overridden error message template for this validator.
		/// </summary>
		/// <param name="errorFactory">A function for retrieving the error message template.</param>
		public void SetErrorMessage(Func<ValidationContext<T>, TProperty, string> errorFactory) {
			_errorMessageFactory = errorFactory;
			_errorMessage = null;
		}

		/// <summary>
		/// Sets the overridden error message template for this validator.
		/// </summary>
		/// <param name="errorMessage">The error message to set</param>
		public void SetErrorMessage(string errorMessage) {
			_errorMessage = errorMessage;
			_errorMessageFactory = null;
		}

		internal Action<T, ValidationContext<T>, TProperty, string> OnFailure { get; set; }
	}

}
