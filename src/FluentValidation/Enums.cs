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

	/// <summary>
	/// Specifies how rules should cascade when one fails.
	/// </summary>
	public enum CascadeMode {
		/// <summary>
		/// When a rule/validator fails, execution continues to the next rule/validator. 
		/// For more information, see the methods/properties that accept this enum as a parameter.
		/// </summary>
		Continue,
		/// <summary>
		/// For more information, see the methods/properties that accept this enum as a parameter.
		/// </summary>
		[Obsolete(
			$"The behaviour of {nameof(StopOnFirstFailure)} has been replaced by use of the " +
				$"separate validator-level properties {nameof(AbstractValidator<object>.ClassLevelCascadeMode)} " +
				$"and {nameof(AbstractValidator<object>.RuleLevelCascadeMode)}, " +
				$"and their global default equivalents. {nameof(StopOnFirstFailure)} will be removed in a later release. " +
				"For more details, see https://docs.fluentvalidation.net/en/latest/conditions.html#setting-the-cascade-mode .")]
		StopOnFirstFailure,

		/// <summary>
		/// When a rule/validator fails, validation is stopped for the current rule/validator.
		/// For more information, see the methods/properties that accept this enum as a parameter.
		/// </summary>
		Stop,
	}

	/// <summary>
	/// Specifies where a When/Unless condition should be applied
	/// </summary>
	public enum ApplyConditionTo {
		/// <summary>
		/// Applies the condition to all validators declared so far in the chain.
		/// </summary>
		AllValidators,
		/// <summary>
		/// Applies the condition to the current validator only.
		/// </summary>
		CurrentValidator
	}

     /// <summary>
     /// Specifies the severity of a rule.
     /// </summary>
	public enum Severity {
		/// <summary>
		/// Error
		/// </summary>
		Error,
		/// <summary>
		/// Warning
		/// </summary>
		Warning,
		/// <summary>
		/// Info
		/// </summary>
		Info
	}
}
