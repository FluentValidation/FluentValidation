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
	using System.Reflection;
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
		string[] RuleSets { get; set; }

		/// <summary>
		/// Applies a condition to either all the validators in the rule, or the most recent validator in the rule chain.
		/// </summary>
		/// <param name="predicate">The condition to apply</param>
		/// <param name="applyConditionTo">Indicates whether the condition should be applied to all validators in the rule, or only the current one</param>
		void ApplyCondition(Func<IValidationContext, bool> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators);

		/// <summary>
		/// Applies an asynchronous condition to either all the validators in the rule, or the most recent validator in the rule chain.
		/// </summary>
		/// <param name="predicate">The condition to apply</param>
		/// <param name="applyConditionTo">Indicates whether the condition should be applied to all validators in the rule, or only the current one</param>
		void ApplyAsyncCondition(Func<IValidationContext, CancellationToken, Task<bool>> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators);

		/// <summary>
		/// Applies a condition that wraps the entire rule.
		/// </summary>
		/// <param name="condition">The condition to apply.</param>
		void ApplySharedCondition(Func<IValidationContext, bool> condition);

		/// <summary>
		/// Applies an asynchronous condition that wraps the entire rule.
		/// </summary>
		/// <param name="condition">The condition to apply.</param>
		void ApplySharedAsyncCondition(Func<IValidationContext, CancellationToken, Task<bool>> condition);

		/// <summary>
		/// Gets the display name for the property.
		/// </summary>
		/// <param name="context">Current context</param>
		/// <returns>Display name</returns>
		string GetDisplayName(IValidationContext context);

		/// <summary>
		/// Returns the property name for the property being validated.
		/// Returns null if it is not a property being validated (eg a method call)
		/// </summary>
		public string PropertyName { get; }

		/// <summary>
		/// Allows custom creation of an error message
		/// </summary>
		public Func<MessageBuilderContext, string> MessageBuilder { get; }

		/// <summary>
		/// Property associated with this rule.
		/// </summary>
		public MemberInfo Member { get; }

		/// <summary>
		/// Type of the property being validated
		/// </summary>
		public Type TypeToValidate { get; }

		/// <summary>
		/// Whether the rule has a condition defined.
		/// </summary>
		bool HasCondition { get; }

		/// <summary>
		/// Whether the rule has an async condition defined.
		/// </summary>
		bool HasAsyncCondition { get; }
	}
}
