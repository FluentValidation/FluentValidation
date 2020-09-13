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
		public Func<PropertyValidatorContext, bool> Condition { get; private set; }

		/// <summary>
		/// Async condition associated with the validator. If the condition fails, the validator will not run.
		/// </summary>
		public Func<PropertyValidatorContext, CancellationToken, Task<bool>> AsyncCondition { get; private set; }

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

		/// <summary>
		/// Function used to retrieve custom state for the validator
		/// </summary>
		public Func<PropertyValidatorContext, object> CustomStateProvider { get; set; }

		/// <summary>
		/// Function used to retrieve the severity for the validator
		/// </summary>
		public Func<PropertyValidatorContext, Severity> SeverityProvider { get; set; }

		/// <summary>
		/// Factory for retrieving the unformatted error message template.
		/// </summary>
#pragma warning disable 618
		public Func<PropertyValidatorContext, string> ErrorMessageFactory {
			get {
				// TODO: For FV10, change the backing field type to Func and get rid of the
				// IStringSource backing.
				if (_errorSource is BackwardsCompatibleStringSource<PropertyValidatorContext> s) {
					return s.Factory;
				}

				if (_errorSource != null) {
					return context => _errorSource.GetString(context);
				}

				return null;
			}
			set {
				if (value == null) throw new ArgumentNullException(nameof(value));
				_errorSource = new BackwardsCompatibleStringSource<PropertyValidatorContext>(value);
			}
		}
#pragma warning restore 618

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
		[Obsolete("ErrorMessageSource is deprecated and will be removed in FluentValidation 10. Please use the ErrorMessageFactory property instead, which takes a Func<PropertyValidatorContext, string>")]
		public IStringSource ErrorMessageSource {
			get => _errorSource;
			set => _errorSource = value ?? throw new ArgumentNullException(nameof(value));
		}

		/// <summary>
		/// Retrieves the error code.
		/// </summary>
		[Obsolete("ErrorCodeSource is deprecated and will be FluentValidation 10. Please use the ErrorCode property instead.")]
		public IStringSource ErrorCodeSource {
			get => _errorCodeSource;
			set => _errorCodeSource = value ?? throw new ArgumentNullException(nameof(value));
		}
	}
}
