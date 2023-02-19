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

namespace FluentValidation;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Internal;

/// <summary>
/// Default options that can be used to configure a validator.
/// </summary>
public static class DefaultValidatorOptions {
	/// <summary>
	/// Configures the rule.
	/// </summary>
	/// <param name="ruleBuilder"></param>
	/// <param name="configurator">Action to configure the object.</param>
	/// <returns></returns>
	public static IRuleBuilderInitial<T, TProperty> Configure<T, TProperty>(this IRuleBuilderInitial<T, TProperty> ruleBuilder, Action<IValidationRule<T, TProperty>> configurator) {
		configurator(Configurable(ruleBuilder));
		return ruleBuilder;
	}

	/// <summary>
	/// Configures the current object.
	/// </summary>
	/// <param name="ruleBuilder"></param>
	/// <param name="configurator">Action to configure the object.</param>
	/// <returns></returns>
	public static IRuleBuilderOptions<T, TProperty> Configure<T, TProperty>(this IRuleBuilderOptions<T, TProperty> ruleBuilder, Action<IValidationRule<T, TProperty>> configurator) {
		configurator(Configurable(ruleBuilder));
		return ruleBuilder;
	}

	/// <summary>
	/// Configures the rule object.
	/// </summary>
	/// <param name="ruleBuilder"></param>
	/// <param name="configurator">Action to configure the object.</param>
	/// <returns></returns>
	public static IRuleBuilderInitialCollection<T, TElement> Configure<T, TElement>(this IRuleBuilderInitialCollection<T, TElement> ruleBuilder, Action<ICollectionRule<T, TElement>> configurator) {
		configurator(Configurable(ruleBuilder));
		return ruleBuilder;
	}

	/// <summary>
	/// Gets the configurable rule instance from a rule builder.
	/// </summary>
	/// <param name="ruleBuilder">The rule builder.</param>
	/// <returns>A configurable IValidationRule instance.</returns>
	public static IValidationRule<T, TProperty> Configurable<T, TProperty>(IRuleBuilder<T, TProperty> ruleBuilder)
		=> ((IRuleBuilderInternal<T, TProperty>) ruleBuilder).Rule;

	/// <summary>
	/// Gets the configurable rule instance from a rule builder.
	/// </summary>
	/// <param name="ruleBuilder">The rule builder.</param>
	/// <returns>A configurable IValidationRule instance.</returns>
	public static ICollectionRule<T, TCollectionElement> Configurable<T, TCollectionElement>(IRuleBuilderInitialCollection<T, TCollectionElement> ruleBuilder)
		=> (ICollectionRule<T, TCollectionElement>) ((IRuleBuilderInternal<T, TCollectionElement>) ruleBuilder).Rule;

	/// <summary>
	/// <para>
	/// Specifies the cascade mode for failures.
	/// </para>
	/// <para>
	/// If set to <see cref="CascadeMode.Stop"/> then execution of the rule will stop once the first validator in the chain fails.
	/// </para>
	/// <para>
	/// If set to <see cref="CascadeMode.Continue"/> then all validators in the chain will execute regardless of failures.
	/// </para>
	/// </summary>
	public static IRuleBuilderInitial<T, TProperty> Cascade<T, TProperty>(this IRuleBuilderInitial<T, TProperty> ruleBuilder, CascadeMode cascadeMode) {
		Configurable(ruleBuilder).CascadeMode = cascadeMode;
		return ruleBuilder;
	}

	/// <summary>
	/// <para>
	/// Specifies the cascade mode for failures.
	/// </para>
	/// <para>
	/// If set to <see cref="CascadeMode.Stop"/> then execution of the rule will stop once the first validator in the chain fails.
	/// </para>
	/// <para>
	/// If set to <see cref="CascadeMode.Continue"/> then all validators in the chain will execute regardless of failures.
	/// </para>
	/// </summary>
	public static IRuleBuilderInitialCollection<T, TProperty> Cascade<T, TProperty>(this IRuleBuilderInitialCollection<T, TProperty> ruleBuilder, CascadeMode cascadeMode) {
		Configurable(ruleBuilder).CascadeMode = cascadeMode;
		return ruleBuilder;
	}

	/// <summary>
	/// Specifies a custom error message to use when validation fails. Only applies to the rule that directly precedes it.
	/// </summary>
	/// <param name="rule">The current rule</param>
	/// <param name="errorMessage">The error message to use</param>
	/// <returns></returns>
	public static IRuleBuilderOptions<T, TProperty> WithMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, string errorMessage) {
		ExtensionsInternal.ThrowIfNullOrEmpty(errorMessage);
		Configurable(rule).Current.SetErrorMessage(errorMessage);
		return rule;
	}

	/// <summary>
	/// Specifies a custom error message to use when validation fails. Only applies to the rule that directly precedes it.
	/// </summary>
	/// <param name="rule">The current rule</param>
	/// <param name="messageProvider">Delegate that will be invoked to retrieve the localized message. </param>
	/// <returns></returns>
	public static IRuleBuilderOptions<T, TProperty> WithMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<T, string> messageProvider) {
		ArgumentNullException.ThrowIfNull(messageProvider);
		Configurable(rule).Current.SetErrorMessage((ctx, val) => {
			return messageProvider(ctx == null ? default : ctx.InstanceToValidate);
		});
		return rule;
	}

	/// <summary>
	/// Specifies a custom error message to use when validation fails. Only applies to the rule that directly precedes it.
	/// </summary>
	/// <param name="rule">The current rule</param>
	/// <param name="messageProvider">Delegate that will be invoked.Uses_localized_name to retrieve the localized message. </param>
	/// <returns></returns>
	public static IRuleBuilderOptions<T, TProperty> WithMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<T, TProperty, string> messageProvider) {
		ArgumentNullException.ThrowIfNull(messageProvider);
		Configurable(rule).Current.SetErrorMessage((context, value) => {
			return messageProvider(context == null ? default : context.InstanceToValidate, value);
		});
		return rule;
	}

	/// <summary>
	/// Specifies a custom error code to use if validation fails.
	/// </summary>
	/// <param name="rule">The current rule</param>
	/// <param name="errorCode">The error code to use</param>
	/// <returns></returns>
	public static IRuleBuilderOptions<T, TProperty> WithErrorCode<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, string errorCode) {
		ExtensionsInternal.ThrowIfNullOrEmpty(errorCode);
		Configurable(rule).Current.ErrorCode = errorCode;
		return rule;
	}

	/// <summary>
	/// Specifies a condition limiting when the validator should run.
	/// The validator will only be executed if the result of the lambda returns true.
	/// </summary>
	/// <param name="rule">The current rule</param>
	/// <param name="predicate">A lambda expression that specifies a condition for when the validator should run</param>
	/// <param name="applyConditionTo">Whether the condition should be applied to the current rule or all rules in the chain</param>
	/// <returns></returns>
	public static IRuleBuilderOptions<T, TProperty> When<T,TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<T, bool> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators) {
		ArgumentNullException.ThrowIfNull(predicate);
		return rule.When((x, ctx) => predicate(x), applyConditionTo);
	}

	/// <summary>
	/// Specifies a condition limiting when the validator should run.
	/// The validator will only be executed if the result of the lambda returns true.
	/// </summary>
	/// <param name="rule">The current rule</param>
	/// <param name="predicate">A lambda expression that specifies a condition for when the validator should run</param>
	/// <param name="applyConditionTo">Whether the condition should be applied to the current rule or all rules in the chain</param>
	/// <returns></returns>
	public static IRuleBuilderOptionsConditions<T, TProperty> When<T,TProperty>(this IRuleBuilderOptionsConditions<T, TProperty> rule, Func<T, bool> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators) {
		ArgumentNullException.ThrowIfNull(predicate);
		return rule.When((x, ctx) => predicate(x), applyConditionTo);
	}

	/// <summary>
	/// Specifies a condition limiting when the validator should run.
	/// The validator will only be executed if the result of the lambda returns true.
	/// </summary>
	/// <param name="rule">The current rule</param>
	/// <param name="predicate">A lambda expression that specifies a condition for when the validator should run</param>
	/// <param name="applyConditionTo">Whether the condition should be applied to the current rule or all rules in the chain</param>
	/// <returns></returns>
	public static IRuleBuilderOptions<T, TProperty> When<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<T, ValidationContext<T>, bool> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators) {
		ArgumentNullException.ThrowIfNull(predicate);
		// Default behaviour for When/Unless as of v1.3 is to apply the condition to all previous validators in the chain.
		Configurable(rule).ApplyCondition(ctx => predicate((T)ctx.InstanceToValidate, ValidationContext<T>.GetFromNonGenericContext(ctx)), applyConditionTo);
		return rule;
	}

	/// <summary>
	/// Specifies a condition limiting when the validator should run.
	/// The validator will only be executed if the result of the lambda returns true.
	/// </summary>
	/// <param name="rule">The current rule</param>
	/// <param name="predicate">A lambda expression that specifies a condition for when the validator should run</param>
	/// <param name="applyConditionTo">Whether the condition should be applied to the current rule or all rules in the chain</param>
	/// <returns></returns>
	public static IRuleBuilderOptionsConditions<T, TProperty> When<T, TProperty>(this IRuleBuilderOptionsConditions<T, TProperty> rule, Func<T, ValidationContext<T>, bool> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators) {
		ArgumentNullException.ThrowIfNull(predicate);
		// Default behaviour for When/Unless as of v1.3 is to apply the condition to all previous validators in the chain.
		Configurable(rule).ApplyCondition(ctx => predicate((T)ctx.InstanceToValidate, ValidationContext<T>.GetFromNonGenericContext(ctx)), applyConditionTo);
		return rule;
	}

	/// <summary>
	/// Specifies a condition limiting when the validator should not run.
	/// The validator will only be executed if the result of the lambda returns false.
	/// </summary>
	/// <param name="rule">The current rule</param>
	/// <param name="predicate">A lambda expression that specifies a condition for when the validator should not run</param>
	/// <param name="applyConditionTo">Whether the condition should be applied to the current rule or all rules in the chain</param>
	/// <returns></returns>
	public static IRuleBuilderOptions<T, TProperty> Unless<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<T, bool> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators) {
		ArgumentNullException.ThrowIfNull(predicate);
		return rule.Unless((x, ctx) => predicate(x), applyConditionTo);
	}

	/// <summary>
	/// Specifies a condition limiting when the validator should not run.
	/// The validator will only be executed if the result of the lambda returns false.
	/// </summary>
	/// <param name="rule">The current rule</param>
	/// <param name="predicate">A lambda expression that specifies a condition for when the validator should not run</param>
	/// <param name="applyConditionTo">Whether the condition should be applied to the current rule or all rules in the chain</param>
	/// <returns></returns>
	public static IRuleBuilderOptionsConditions<T, TProperty> Unless<T, TProperty>(this IRuleBuilderOptionsConditions<T, TProperty> rule, Func<T, bool> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators) {
		ArgumentNullException.ThrowIfNull(predicate);
		return rule.Unless((x, ctx) => predicate(x), applyConditionTo);
	}

	/// <summary>
	/// Specifies a condition limiting when the validator should not run.
	/// The validator will only be executed if the result of the lambda returns false.
	/// </summary>
	/// <param name="rule">The current rule</param>
	/// <param name="predicate">A lambda expression that specifies a condition for when the validator should not run</param>
	/// <param name="applyConditionTo">Whether the condition should be applied to the current rule or all rules in the chain</param>
	/// <returns></returns>
	public static IRuleBuilderOptions<T, TProperty> Unless<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<T, ValidationContext<T>, bool> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators) {
		ArgumentNullException.ThrowIfNull(predicate);
		return rule.When((x, ctx) => !predicate(x, ctx), applyConditionTo);
	}

	/// <summary>
	/// Specifies a condition limiting when the validator should not run.
	/// The validator will only be executed if the result of the lambda returns false.
	/// </summary>
	/// <param name="rule">The current rule</param>
	/// <param name="predicate">A lambda expression that specifies a condition for when the validator should not run</param>
	/// <param name="applyConditionTo">Whether the condition should be applied to the current rule or all rules in the chain</param>
	/// <returns></returns>
	public static IRuleBuilderOptionsConditions<T, TProperty> Unless<T, TProperty>(this IRuleBuilderOptionsConditions<T, TProperty> rule, Func<T, ValidationContext<T>, bool> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators) {
		ArgumentNullException.ThrowIfNull(predicate);
		return rule.When((x, ctx) => !predicate(x, ctx), applyConditionTo);
	}

	/// <summary>
	/// Specifies an asynchronous condition limiting when the validator should run.
	/// The validator will only be executed if the result of the lambda returns true.
	/// </summary>
	/// <param name="rule">The current rule</param>
	/// <param name="predicate">A lambda expression that specifies a condition for when the validator should run</param>
	/// <param name="applyConditionTo">Whether the condition should be applied to the current rule or all rules in the chain</param>
	/// <returns></returns>
	public static IRuleBuilderOptions<T, TProperty> WhenAsync<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<T, CancellationToken, Task<bool>> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators) {
		ArgumentNullException.ThrowIfNull(predicate);
		return rule.WhenAsync((x, ctx, ct) => predicate(x, ct), applyConditionTo);
	}

	/// <summary>
	/// Specifies an asynchronous condition limiting when the validator should run.
	/// The validator will only be executed if the result of the lambda returns true.
	/// </summary>
	/// <param name="rule">The current rule</param>
	/// <param name="predicate">A lambda expression that specifies a condition for when the validator should run</param>
	/// <param name="applyConditionTo">Whether the condition should be applied to the current rule or all rules in the chain</param>
	/// <returns></returns>
	public static IRuleBuilderOptionsConditions<T, TProperty> WhenAsync<T, TProperty>(this IRuleBuilderOptionsConditions<T, TProperty> rule, Func<T, CancellationToken, Task<bool>> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators) {
		ArgumentNullException.ThrowIfNull(predicate);
		return rule.WhenAsync((x, ctx, ct) => predicate(x, ct), applyConditionTo);
	}

	/// <summary>
	/// Specifies an asynchronous condition limiting when the validator should run.
	/// The validator will only be executed if the result of the lambda returns true.
	/// </summary>
	/// <param name="rule">The current rule</param>
	/// <param name="predicate">A lambda expression that specifies a condition for when the validator should run</param>
	/// <param name="applyConditionTo">Whether the condition should be applied to the current rule or all rules in the chain</param>
	/// <returns></returns>
	public static IRuleBuilderOptions<T, TProperty> WhenAsync<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<T, ValidationContext<T>, CancellationToken, Task<bool>> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators) {
		ArgumentNullException.ThrowIfNull(predicate);
		// Default behaviour for When/Unless as of v1.3 is to apply the condition to all previous validators in the chain.
		Configurable(rule).ApplyAsyncCondition((ctx, ct) => predicate((T)ctx.InstanceToValidate, ValidationContext<T>.GetFromNonGenericContext(ctx), ct), applyConditionTo);
		return rule;
	}

	/// <summary>
	/// Specifies an asynchronous condition limiting when the validator should run.
	/// The validator will only be executed if the result of the lambda returns true.
	/// </summary>
	/// <param name="rule">The current rule</param>
	/// <param name="predicate">A lambda expression that specifies a condition for when the validator should run</param>
	/// <param name="applyConditionTo">Whether the condition should be applied to the current rule or all rules in the chain</param>
	/// <returns></returns>
	public static IRuleBuilderOptionsConditions<T, TProperty> WhenAsync<T, TProperty>(this IRuleBuilderOptionsConditions<T, TProperty> rule, Func<T, ValidationContext<T>, CancellationToken, Task<bool>> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators) {
		ArgumentNullException.ThrowIfNull(predicate);
		// Default behaviour for When/Unless as of v1.3 is to apply the condition to all previous validators in the chain.
		Configurable(rule).ApplyAsyncCondition((ctx, ct) => predicate((T)ctx.InstanceToValidate, ValidationContext<T>.GetFromNonGenericContext(ctx), ct), applyConditionTo);
		return rule;
	}

	/// <summary>
	/// Specifies an asynchronous condition limiting when the validator should not run.
	/// The validator will only be executed if the result of the lambda returns false.
	/// </summary>
	/// <param name="rule">The current rule</param>
	/// <param name="predicate">A lambda expression that specifies a condition for when the validator should not run</param>
	/// <param name="applyConditionTo">Whether the condition should be applied to the current rule or all rules in the chain</param>
	/// <returns></returns>
	public static IRuleBuilderOptions<T, TProperty> UnlessAsync<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<T, CancellationToken, Task<bool>> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators) {
		ArgumentNullException.ThrowIfNull(predicate);
		return rule.UnlessAsync((x, ctx, ct) => predicate(x, ct), applyConditionTo);
	}

	/// <summary>
	/// Specifies an asynchronous condition limiting when the validator should not run.
	/// The validator will only be executed if the result of the lambda returns false.
	/// </summary>
	/// <param name="rule">The current rule</param>
	/// <param name="predicate">A lambda expression that specifies a condition for when the validator should not run</param>
	/// <param name="applyConditionTo">Whether the condition should be applied to the current rule or all rules in the chain</param>
	/// <returns></returns>
	public static IRuleBuilderOptionsConditions<T, TProperty> UnlessAsync<T, TProperty>(this IRuleBuilderOptionsConditions<T, TProperty> rule, Func<T, CancellationToken, Task<bool>> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators) {
		ArgumentNullException.ThrowIfNull(predicate);
		return rule.UnlessAsync((x, ctx, ct) => predicate(x, ct), applyConditionTo);
	}

	/// <summary>
	/// Specifies an asynchronous condition limiting when the validator should not run.
	/// The validator will only be executed if the result of the lambda returns false.
	/// </summary>
	/// <param name="rule">The current rule</param>
	/// <param name="predicate">A lambda expression that specifies a condition for when the validator should not run</param>
	/// <param name="applyConditionTo">Whether the condition should be applied to the current rule or all rules in the chain</param>
	/// <returns></returns>
	public static IRuleBuilderOptions<T, TProperty> UnlessAsync<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<T, ValidationContext<T>, CancellationToken, Task<bool>> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators) {
		ArgumentNullException.ThrowIfNull(predicate);
		return rule.WhenAsync(async (x, ctx, ct) => !await predicate(x, ctx, ct), applyConditionTo);
	}

	/// <summary>
	/// Specifies an asynchronous condition limiting when the validator should not run.
	/// The validator will only be executed if the result of the lambda returns false.
	/// </summary>
	/// <param name="rule">The current rule</param>
	/// <param name="predicate">A lambda expression that specifies a condition for when the validator should not run</param>
	/// <param name="applyConditionTo">Whether the condition should be applied to the current rule or all rules in the chain</param>
	/// <returns></returns>
	public static IRuleBuilderOptionsConditions<T, TProperty> UnlessAsync<T, TProperty>(this IRuleBuilderOptionsConditions<T, TProperty> rule, Func<T, ValidationContext<T>, CancellationToken, Task<bool>> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators) {
		ArgumentNullException.ThrowIfNull(predicate);
		return rule.WhenAsync(async (x, ctx, ct) => !await predicate(x, ctx, ct), applyConditionTo);
	}

	/// <summary>
	/// Applies a filter to a collection property.
	/// </summary>
	/// <param name="rule">The current rule</param>
	/// <param name="predicate">The condition</param>
	/// <returns></returns>
	public static IRuleBuilderInitialCollection<T, TCollectionElement> Where<T, TCollectionElement>(this IRuleBuilderInitialCollection<T, TCollectionElement> rule, Func<TCollectionElement, bool> predicate) {
		// This overload supports RuleFor().SetCollectionValidator() (which returns IRuleBuilderOptions<T, IEnumerable<TElement>>)
		ArgumentNullException.ThrowIfNull(predicate);
		Configurable(rule).Filter = predicate;
		return rule;
	}

	/// <summary>
	/// Specifies a custom property name to use within the error message.
	/// </summary>
	/// <param name="rule">The current rule</param>
	/// <param name="overridePropertyName">The property name to use</param>
	/// <returns></returns>
	public static IRuleBuilderOptions<T, TProperty> WithName<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, string overridePropertyName) {
		ExtensionsInternal.ThrowIfNullOrEmpty(overridePropertyName);
		Configurable(rule).SetDisplayName(overridePropertyName);
		return rule;
	}

	/// <summary>
	/// Specifies a custom property name to use within the error message.
	/// </summary>
	/// <param name="rule">The current rule</param>
	/// <param name="nameProvider">Func used to retrieve the property's display name</param>
	/// <returns></returns>
	public static IRuleBuilderOptions<T, TProperty> WithName<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<T, string> nameProvider) {
		ArgumentNullException.ThrowIfNull(nameProvider);
		// Must use null propagation here.
		// The MVC clientside validation will try and retrieve the name, but won't
		// be able to to so if we've used this overload of WithName.
		Configurable(rule).SetDisplayName((context => {
			T instance = context == null ? default : context.InstanceToValidate;
			return nameProvider(instance);
		}));
		return rule;
	}

	/// <summary>
	/// Overrides the name of the property associated with this rule.
	/// NOTE: This is a considered to be an advanced feature. Most of the time that you use this, you actually meant to use WithName.
	/// </summary>
	/// <param name="rule">The current rule</param>
	/// <param name="propertyName">The property name to use</param>
	/// <returns></returns>
	public static IRuleBuilderOptions<T, TProperty> OverridePropertyName<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, string propertyName) {
		// Allow string.Empty as this could be a model-level rule.
		if (propertyName == null) throw new ArgumentNullException(nameof(propertyName), "A property name must be specified when calling OverridePropertyName.");
		Configurable(rule).PropertyName = propertyName;
		return rule;
	}

	/// <summary>
	/// Overrides the name of the property associated with this rule.
	/// NOTE: This is a considered to be an advanced feature. Most of the time that you use this, you actually meant to use WithName.
	/// </summary>
	/// <param name="rule">The current rule</param>
	/// <param name="expr">An expression referencing another property</param>
	/// <returns></returns>
	public static IRuleBuilderOptions<T, TProperty> OverridePropertyName<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Expression<Func<T, object>> expr) {
		if (expr == null) throw new ArgumentNullException(nameof(expr));
		var member = expr.GetMember();
		if (member == null) throw new NotSupportedException("Must supply a MemberExpression when calling OverridePropertyName");
		return rule.OverridePropertyName(member.Name);
	}

	/// <summary>
	/// Specifies custom state that should be stored alongside the validation message when validation fails for this rule.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TProperty"></typeparam>
	/// <param name="rule"></param>
	/// <param name="stateProvider"></param>
	/// <returns></returns>
	public static IRuleBuilderOptions<T, TProperty> WithState<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<T, object> stateProvider) {
		ArgumentNullException.ThrowIfNull(stateProvider);
		var wrapper = new Func<ValidationContext<T>, TProperty, object>((ctx, _) => stateProvider(ctx.InstanceToValidate));
		Configurable(rule).Current.CustomStateProvider = wrapper;
		return rule;
	}

	/// <summary>
	/// Specifies custom state that should be stored alongside the validation message when validation fails for this rule.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TProperty"></typeparam>
	/// <param name="rule"></param>
	/// <param name="stateProvider"></param>
	/// <returns></returns>
	public static IRuleBuilderOptions<T, TProperty> WithState<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<T, TProperty, object> stateProvider) {
		ArgumentNullException.ThrowIfNull(stateProvider);

		var wrapper = new Func<ValidationContext<T>, TProperty, object>((ctx, val) => {
			return stateProvider(ctx.InstanceToValidate, val);
		});

		Configurable(rule).Current.CustomStateProvider = wrapper;
		return rule;
	}

	///<summary>
	/// Specifies custom severity that should be stored alongside the validation message when validation fails for this rule.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TProperty"></typeparam>
	/// <param name="rule"></param>
	/// <param name="severity"></param>
	/// <returns></returns>
	public static IRuleBuilderOptions<T, TProperty> WithSeverity<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Severity severity) {
		Configurable(rule).Current.SeverityProvider = (_, _) => severity;
		return rule;
	}

	/// <summary>
	/// Specifies custom severity that should be stored alongside the validation message when validation fails for this rule.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TProperty"></typeparam>
	/// <param name="rule"></param>
	/// <param name="severityProvider"></param>
	/// <returns></returns>
	public static IRuleBuilderOptions<T, TProperty> WithSeverity<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<T, Severity> severityProvider) {
		ArgumentNullException.ThrowIfNull(severityProvider);

		Severity SeverityProvider(ValidationContext<T> ctx, TProperty value) {
			return severityProvider(ctx.InstanceToValidate);
		}

		Configurable(rule).Current.SeverityProvider = SeverityProvider;
		return rule;
	}

	/// <summary>
	/// Specifies custom severity that should be stored alongside the validation message when validation fails for this rule.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TProperty"></typeparam>
	/// <param name="rule"></param>
	/// <param name="severityProvider"></param>
	/// <returns></returns>
	public static IRuleBuilderOptions<T, TProperty> WithSeverity<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<T, TProperty, Severity> severityProvider) {
		ArgumentNullException.ThrowIfNull(severityProvider);

		Severity SeverityProvider(ValidationContext<T> ctx, TProperty value) {
			return severityProvider(ctx.InstanceToValidate, value);
		}

		Configurable(rule).Current.SeverityProvider = SeverityProvider;
		return rule;
	}

	/// <summary>
	/// Allows the generated indexer to be overridden for collection rules.
	/// </summary>
	/// <param name="rule">The current rule</param>
	/// <param name="callback">The callback. Receives the model, the collection, the current element and the current index as parameters. Should return a string representation of the indexer. The default is "[" + index + "]"</param>
	/// <returns></returns>
	public static IRuleBuilderInitialCollection<T, TCollectionElement> OverrideIndexer<T, TCollectionElement>(this IRuleBuilderInitialCollection<T, TCollectionElement> rule, Func<T, IEnumerable<TCollectionElement>, TCollectionElement, int, string> callback) {
		// This overload supports RuleFor().SetCollectionValidator() (which returns IRuleBuilderOptions<T, IEnumerable<TElement>>)
		ArgumentNullException.ThrowIfNull(callback);
		Configurable(rule).IndexBuilder = callback;
		return rule;
	}
}
