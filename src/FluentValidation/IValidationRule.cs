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
	using FluentValidation.Resources;
	using Internal;
	using Results;
	using Validators;


	/// <summary>
	/// Defines a generic rule associated with a generic propery which can have multiple validators.
	/// There is no need to be implemented in the user code.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="Propery"></typeparam>
	public interface IValidationRule<T, out TProperty> : IValidationRule<T> {
	}

	/// <summary>
	/// Defines a generic rule associated with a propery which can have multiple validators.
	/// There is no need to be implemented in the user code.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="Propery"></typeparam>
	public interface IValidationRule<T> : IValidationRule {
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
		/// Performs validation using a validation context and returns a collection of Validation Failures.
		/// </summary>
		/// <param name="context">Validation Context</param>
		/// <returns>A collection of validation failures</returns>
		IEnumerable<ValidationFailure> Validate(IValidationContext context);

		/// <summary>
		/// Performs validation using a validation context and returns a collection of Validation Failures asynchronously.
		/// </summary>
		/// <param name="context">Validation Context</param>
		/// <param name="cancellation">Cancellation token</param>
		/// <returns>A collection of validation failures</returns>
		Task<IEnumerable<ValidationFailure>> ValidateAsync(IValidationContext context, CancellationToken cancellation);

		/// <summary>
		/// Applies a condition to either all the validators in the rule, or the most recent validator in the rule chain.
		/// </summary>
		/// <param name="predicate">The condition to apply</param>
		/// <param name="applyConditionTo">Indicates whether the condition should be applied to all validators in the rule, or only the current one</param>
		void ApplyCondition(Func<PropertyValidatorContext, bool> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators);

		/// <summary>
		/// Applies an asynchronous condition to either all the validators in the rule, or the most recent validator in the rule chain.
		/// </summary>
		/// <param name="predicate">The condition to apply</param>
		/// <param name="applyConditionTo">Indicates whether the condition should be applied to all validators in the rule, or only the current one</param>
		void ApplyAsyncCondition(Func<PropertyValidatorContext, CancellationToken, Task<bool>> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators);

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
		/// Condition for all validators in this rule.
		/// </summary>
		Func<IValidationContext, bool> Condition { get; }

		/// <summary>
		/// Asynchronous condition for all validators in this rule.
		/// </summary>
		Func<IValidationContext, CancellationToken, Task<bool>> AsyncCondition { get; }

		/// <summary>
		/// Property associated with this rule.
		/// </summary>
		MemberInfo Member { get; }

		/// <summary>
		/// Function that can be invoked to retrieve the value of the property.
		/// </summary>
		Func<object, object> PropertyFunc { get; }

		/// <summary>
		/// Expression that was used to create the rule.
		/// </summary>
		LambdaExpression Expression { get; }

		/// <summary>
		/// String source that can be used to retrieve the display name (if null, falls back to the property name)
		/// </summary>
		[Obsolete("This property is deprecated and will be removed in FluentValidation 10. Use the GetDisplayName and SetDisplayName instead.")]
		IStringSource DisplayName { get; }

		/// <summary>
		/// Sets the display name for the property.
		/// </summary>
		/// <param name="name">The property's display name</param>
		void SetDisplayName(string name);

		/// <summary>
		/// Sets the display name for the property using a function.
		/// </summary>
		/// <param name="factory">The function for building the display name</param>
		void SetDisplayName(Func<IValidationContext, string> factory);

		/// <summary>
		/// Function that will be invoked if any of the validators associated with this rule fail.
		/// </summary>
		Action<object, IEnumerable<ValidationFailure>> OnFailure { get; set; }

		/// <summary>
		/// The current validator being configured by this rule.
		/// </summary>
		IPropertyValidator CurrentValidator { get; }

		/// <summary>
		/// Type of the property being validated
		/// </summary>
		Type TypeToValidate { get; }

		/// <summary>
		/// Cascade mode for this rule.
		/// </summary>
		CascadeMode CascadeMode { get; set; }

		/// <summary>
		/// Adds a validator to the rule.
		/// </summary>
		void AddValidator(IPropertyValidator validator);


		/// <summary>
		/// Replaces a validator in this rule. Used to wrap validators.
		/// </summary>
		void ReplaceValidator(IPropertyValidator original, IPropertyValidator newValidator);

		/// <summary>
		/// Remove a validator in this rule.
		/// </summary>
		void RemoveValidator(IPropertyValidator original);

		/// <summary>
		/// Clear all validators from this rule.
		/// </summary>
		void ClearValidators();

		/// <summary>
		/// Returns the property name for the property being validated.
		/// Returns null if it is not a property being validated (eg a method call)
		/// </summary>
		string PropertyName { get; set; }

		/// <summary>
		/// Allows custom creation of an error message
		/// </summary>
		Func<MessageBuilderContext, string> MessageBuilder { get; set; }

		/// <summary>
		/// Dependent rules
		/// </summary>
		List<IValidationRule> DependentRules { get; }

		Func<object, object> Transformer { get; set; }

		/// <summary>
		/// Display name for the property.
		/// </summary>
		[Obsolete("Calling GetDisplayName without a context parameter is deprecated and will be removed in FluentValidation 10. If you really need this behaviour, you can call the overload that takes a context but pass in null.")]
		string GetDisplayName();

		/// <summary>
		/// Display name for the property.
		/// </summary>
		string GetDisplayName(ICommonContext context);

		/// <summary>
		/// Gets the property value, including any transformations that need to be applied.
		/// </summary>
		/// <param name="instanceToValidate">The parent object</param>
		/// <returns>The value to be validated</returns>
		object GetPropertyValue(object instanceToValidate);
	}
}
