#region License
// Copyright (c) Jeremy Skinner (http://www.jeremyskinner.co.uk)
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
// The latest version of this file can be found at https://github.com/jeremyskinner/FluentValidation
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
		private IStringSource _errorSource;
		private IStringSource _errorCodeSource;
		
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
		/// Retrieves the unformatted error message template.
		/// </summary>
		public IStringSource ErrorMessageSource {
			get => _errorSource;
			set => _errorSource = value ?? throw new ArgumentNullException(nameof(value));
		}
		
		/// <summary>
		/// Retrieves the error code.
		/// </summary>
		public IStringSource ErrorCodeSource {
			get => _errorCodeSource;
			set => _errorCodeSource = value ?? throw new ArgumentNullException(nameof(value));
		}
	}
}