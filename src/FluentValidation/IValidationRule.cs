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
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Threading;
	using System.Threading.Tasks;
	using Internal;
	using Results;
	using Validators;

	public interface ICollectionRule<T, TElement> : IValidationRule<T, TElement> {
		/// <summary>
		/// Filter that should include/exclude items in the collection.
		/// </summary>
		public Func<TElement, bool> Filter { get; set; }

		/// <summary>
		/// Constructs the indexer in the property name associated with the error message.
		/// By default this is "[" + index + "]"
		/// </summary>
		public Func<T, IEnumerable<TElement>, TElement, int, string> IndexBuilder { get; set; }
	}

	public interface IValidationRule<T, TProperty> : IValidationRule<T> {
		/// <summary>
		/// Cascade mode for this rule.
		/// </summary>
		public CascadeMode CascadeMode { get; set; }

		/// <summary>
		/// Function that will be invoked if any of the validators associated with this rule fail.
		/// </summary>
		public Action<T, IEnumerable<ValidationFailure>> OnFailure { get; set; }

		/// <summary>
		/// Sets the display name for the property.
		/// </summary>
		/// <param name="name">The property's display name</param>
		void SetDisplayName(string name);

		/// <summary>
		/// Sets the display name for the property using a function.
		/// </summary>
		/// <param name="factory">The function for building the display name</param>
		void SetDisplayName(Func<ValidationContext<T>, string> factory);
	}

	public interface IValidationRule<T> : IValidationRule {
		void Validate(ValidationContext<T> context);

		Task ValidateAsync(ValidationContext<T> context, CancellationToken cancellation);

		void ApplyCondition(Func<IValidationContext, bool> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators);

		void ApplyAsyncCondition(Func<IValidationContext, CancellationToken, Task<bool>> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators);

		void ApplySharedCondition(Func<ValidationContext<T>, bool> condition);

		void ApplySharedAsyncCondition(Func<ValidationContext<T>, CancellationToken, Task<bool>> condition);

		void AddDependentRules(IEnumerable<IValidationRule<T>> rules);

		//TODO: Make generic
		void AddValidator(IPropertyValidator validator);

		IPropertyValidator CurrentValidator { get; }
	}

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
		/// Gets the display name for the property.
		/// </summary>
		/// <param name="context">Current context</param>
		/// <returns>Display name</returns>
		string GetDisplayName(IValidationContext context);

		/// <summary>
		/// Returns the property name for the property being validated.
		/// Returns null if it is not a property being validated (eg a method call)
		/// </summary>
		public string PropertyName { get; set; }

		/// <summary>
		/// Allows custom creation of an error message
		/// </summary>
		public Func<MessageBuilderContext, string> MessageBuilder { get; set; }

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

		/// <summary>
		/// Expression that was used to create the rule.
		/// </summary>
		public LambdaExpression Expression { get; }
	}
}
