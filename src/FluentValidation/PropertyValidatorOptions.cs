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
	using System.Threading;
	using System.Threading.Tasks;
	using Resources;
	using Validators;

	/// <summary>
	/// Validator metadata.
	/// </summary>
	public class PropertyValidatorOptions {
#pragma warning disable 618
		private IStringSource _errorSource;
		private IStringSource _errorCodeSource;
#pragma warning restore 618

		/// <summary>
		/// Condition associated with the validator. If the condition fails, the validator will not run.
		/// </summary>
		[Obsolete("The Condition property will not be accessible in FluentValidation 10. Use the HasCondition property to check if a condition is set.")]
		public Func<PropertyValidatorContext, bool> Condition { get; private set; }

		/// <summary>
		/// Async condition associated with the validator. If the condition fails, the validator will not run.
		/// </summary>
		[Obsolete("The AsyncCondition property will not be accessible in FluentValidation 10. Use the HasAsyncCondition property to check if a condition is set.")]
		public Func<PropertyValidatorContext, CancellationToken, Task<bool>> AsyncCondition { get; private set; }

#pragma warning disable 618
		/// <summary>
		/// Whether or not this validator has a condition associated with it.
		/// </summary>
		public bool HasCondition => Condition != null;

		/// <summary>
		/// Whether or not this validator has an async condition associated with it.
		/// </summary>
		public bool HasAsyncCondition => AsyncCondition != null;

		/// <summary>
		/// Adds a condition for this validator. If there's already a condition, they're combined together with an AND.
		/// </summary>
		/// <param name="condition"></param>
		public void ApplyCondition(Func<PropertyValidatorContext, bool> condition) {
			if (Condition == null) {
				Condition = condition;
			}
			else {
				var original = Condition;
				Condition = ctx => condition(ctx) && original(ctx);
			}
		}

		/// <summary>
		/// Adds a condition for this validator. If there's already a condition, they're combined together with an AND.
		/// </summary>
		/// <param name="condition"></param>
		public void ApplyAsyncCondition(Func<PropertyValidatorContext, CancellationToken, Task<bool>> condition) {
			if (AsyncCondition == null) {
				AsyncCondition = condition;
			}
			else {
				var original = AsyncCondition;
				AsyncCondition = async (ctx, ct) => await condition(ctx, ct) && await original(ctx, ct);
			}
		}

		internal bool InvokeCondition(PropertyValidatorContext context) {
			if (Condition != null) {
				return Condition(context);
			}

			return true;
		}

		internal async Task<bool> InvokeAsyncCondition(PropertyValidatorContext context, CancellationToken token) {
			if (AsyncCondition != null) {
				return await AsyncCondition(context, token);
			}

			return true;
		}
#pragma warning restore 618

		/// <summary>
		/// Function used to retrieve custom state for the validator
		/// </summary>
		public Func<PropertyValidatorContext, object> CustomStateProvider { get; set; }

		/// <summary>
		/// Function used to retrieve the severity for the validator
		/// </summary>
		public Func<PropertyValidatorContext, Severity> SeverityProvider { get; set; }

		/// <summary>
		/// Retrieves the error code.
		/// </summary>
#pragma warning disable 618
		public string ErrorCode {
			get {
				// TODO: For FV10, change the backing field type to string and get rid of the
				// IStringSource backing.
				if (_errorCodeSource is StaticStringSource s) {
					return s.String;
				}

				return _errorCodeSource?.GetString(null);
			}
			set {
				if (value == null) throw new ArgumentNullException(nameof(value));
				_errorCodeSource = new StaticStringSource(value);
			}
		}
#pragma warning restore 618

		/// <summary>
		/// Retrieves the unformatted error message template.
		/// </summary>
		[Obsolete("ErrorMessageSource is deprecated and will be removed in FluentValidation 10. Please use SetErrorMessage and GetErrorMessageTemplate instead.")]
		public IStringSource ErrorMessageSource {
			get => _errorSource;
			set => _errorSource = value ?? throw new ArgumentNullException(nameof(value));
		}

		/// <summary>
		/// Retrieves the error code.
		/// </summary>
		[Obsolete("ErrorCodeSource is deprecated and will be removed FluentValidation 10. Please use the ErrorCode property instead.")]
		public IStringSource ErrorCodeSource {
			get => _errorCodeSource;
			set => _errorCodeSource = value ?? throw new ArgumentNullException(nameof(value));
		}

		/// <summary>
		/// Returns the default error message template for this validator, when not overridden.
		/// </summary>
		/// <returns></returns>
		protected virtual string GetDefaultMessageTemplate() => "No default error message has been specified";

		/// <summary>
		/// Gets the error message. If a context is supplied, it will be used to format the message if it has placeholders.
		/// If no context is supplied, the raw unformatted message will be returned, containing placeholders.
		/// </summary>
		/// <param name="context">The current property validator context.</param>
		/// <returns>Either the formatted or unformatted error message.</returns>
		public string GetErrorMessageTemplate(PropertyValidatorContext context) {
			string rawTemplate = _errorSource?.GetString(context) ?? GetDefaultMessageTemplate();

			if (context == null) {
				return rawTemplate;
			}

			return context.MessageFormatter.BuildMessage(rawTemplate);
		}

		/// <summary>
		/// Sets the overridden error message template for this validator.
		/// </summary>
		/// <param name="errorFactory">A function for retrieving the error message template.</param>
		public void SetErrorMessage(Func<PropertyValidatorContext, string> errorFactory) {
			//TODO: For FV10 use a backing field of the correct type.
#pragma warning disable 618
			_errorSource = new BackwardsCompatibleStringSource<PropertyValidatorContext>(errorFactory);
#pragma warning restore 618
		}

		/// <summary>
		/// Sets the overridden error message template for this validator.
		/// </summary>
		/// <param name="errorMessage">The error message to set</param>
		public void SetErrorMessage(string errorMessage) {
			// TODO: For FV10 use a backing field of the correct type.
#pragma warning disable 618
			_errorSource = new StaticStringSource(errorMessage);
#pragma warning restore 618
		}
	}
}
