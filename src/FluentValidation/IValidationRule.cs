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
	using Internal;
	using Results;
	using Validators;

	/// <summary>
	/// Defines a rule associated with a property which can have multiple validators.
	/// </summary>
	public interface IValidationRule {
		/// <summary>
		/// The validators that are grouped under this rule.
		/// </summary>
		IEnumerable<IPropertyValidator> Validators { get; }
		/// <summary>
		/// Name of the rule-set to which this rule belongs.
		/// </summary>
		string RuleSet { get; set; }

		/// <summary>
		/// Performs validation using a validation context and returns a collection of Validation Failures.
		/// </summary>
		/// <param name="context">Validation Context</param>
		/// <returns>A collection of validation failures</returns>
		IEnumerable<ValidationFailure> Validate(ValidationContext context);

		/// <summary>
		/// Performs validation using a validation context and returns a collection of Validation Failures asynchronoulsy.
		/// </summary>
		/// <param name="context">Validation Context</param>
		/// <param name="cancellation">Cancellation token</param>
		/// <returns>A collection of validation failures</returns>
		Task<IEnumerable<ValidationFailure>> ValidateAsync(ValidationContext context, CancellationToken cancellation);

		/// <summary>
		/// Applies a condition to the rule
		/// </summary>
		/// <param name="predicate"></param>
		/// <param name="applyConditionTo"></param>
		void ApplyCondition(Func<object, bool> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators);

		/// <summary>
		/// Applies a condition to the rule asynchronously
		/// </summary>
		/// <param name="predicate"></param>
		/// <param name="applyConditionTo"></param>
		void ApplyAsyncCondition(Func<object, CancellationToken, Task<bool>> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators);
	}
}