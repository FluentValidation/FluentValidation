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

// ReSharper disable MemberCanBePrivate.Global
namespace FluentValidation;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Internal;
using Results;

/// <summary>
/// Base class for object validators.
/// </summary>
/// <typeparam name="T">The type of the object being validated</typeparam>
public abstract partial class AbstractValidator<T> : IValidator<T>, IEnumerable<IValidationRule> {
	internal TrackingCollection<IValidationRuleInternal<T>> Rules { get; } = new();
	private Func<CascadeMode> _classLevelCascadeMode = () => ValidatorOptions.Global.DefaultClassLevelCascadeMode;
	private Func<CascadeMode> _ruleLevelCascadeMode = () => ValidatorOptions.Global.DefaultRuleLevelCascadeMode;

	/// <summary>
	/// <para>
	/// Sets the cascade behaviour <i>in between</i> rules in this validator.
	/// This overrides the default value set in <see cref="ValidatorConfiguration.DefaultClassLevelCascadeMode"/>.
	/// </para>
	/// <para>
	/// If set to <see cref="FluentValidation.CascadeMode.Continue"/> then all rules in the class will execute regardless of failures.
	/// </para>
	/// <para>
	/// If set to <see cref="FluentValidation.CascadeMode.Stop"/> then execution of the validator will stop after any rule fails.
	/// </para>
	/// <para>
	/// Note that cascade behaviour <i>within</i> individual rules is controlled by
	/// <see cref="AbstractValidator{T}.RuleLevelCascadeMode"/>.
	/// </para>
	/// </summary>
	public CascadeMode ClassLevelCascadeMode {
		get => _classLevelCascadeMode();
		set => _classLevelCascadeMode = () => value;
	}

	/// <summary>
	/// <para>
	/// Sets the default cascade behaviour <i>within</i> each rule in this validator.
	/// </para>
	/// <para>
	/// This overrides the default value set in <see cref="ValidatorConfiguration.DefaultRuleLevelCascadeMode"/>.
	/// </para>
	/// <para>
	/// It can be further overridden for specific rules by calling
	/// <see cref="DefaultValidatorOptions.Cascade{T, TProperty}(IRuleBuilderInitial{T, TProperty}, FluentValidation.CascadeMode)"/>.
	/// <seealso cref="RuleBase{T, TProperty, TValue}.CascadeMode"/>.
	/// </para>
	/// <para>
	/// Note that cascade behaviour <i>between</i> rules is controlled by <see cref="AbstractValidator{T}.ClassLevelCascadeMode"/>.
	/// </para>
	/// </summary>
	public CascadeMode RuleLevelCascadeMode {
		get => _ruleLevelCascadeMode();
		set => _ruleLevelCascadeMode = () => value;
	}

	ValidationResult IValidator.Validate(IValidationContext context) {
		ArgumentNullException.ThrowIfNull(context);
		return Validate(ValidationContext<T>.GetFromNonGenericContext(context));
	}

	Task<ValidationResult> IValidator.ValidateAsync(IValidationContext context, CancellationToken cancellation) {
		ArgumentNullException.ThrowIfNull(context);
		return ValidateAsync(ValidationContext<T>.GetFromNonGenericContext(context), cancellation);
	}

	/// <summary>
	/// Validates the specified instance
	/// </summary>
	/// <param name="instance">The object to validate</param>
	/// <returns>A ValidationResult object containing any validation failures</returns>
	public ValidationResult Validate(T instance)
		=> Validate(new ValidationContext<T>(instance, null, ValidatorOptions.Global.ValidatorSelectors.DefaultValidatorSelectorFactory()));

	/// <summary>
	/// Validates the specified instance asynchronously
	/// </summary>
	/// <param name="instance">The object to validate</param>
	/// <param name="cancellation">Cancellation token</param>
	/// <returns>A ValidationResult object containing any validation failures</returns>
	public Task<ValidationResult> ValidateAsync(T instance, CancellationToken cancellation = new())
		=> ValidateAsync(new ValidationContext<T>(instance, null, ValidatorOptions.Global.ValidatorSelectors.DefaultValidatorSelectorFactory()), cancellation);

	/// <summary>
	/// Validates the specified instance.
	/// </summary>
	/// <param name="context">Validation Context</param>
	/// <returns>A ValidationResult object containing any validation failures.</returns>
	public virtual ValidationResult Validate(ValidationContext<T> context) {
		ArgumentNullException.ThrowIfNull(context);

		try {
			return ValidateInternal(context);
		}
		catch (AsyncValidatorInvokedSynchronouslyException) {
			// If we attempted to execute an async validator, re-create the exception with more useful info.
			bool wasInvokedByMvc = context.RootContextData.ContainsKey("InvokedByMvc");
			throw new AsyncValidatorInvokedSynchronouslyException(GetType(), wasInvokedByMvc);
		}
	}

	/// <summary>
	/// Validates the specified instance asynchronously.
	/// </summary>
	/// <param name="context">Validation Context</param>
	/// <param name="cancellation">Cancellation token</param>
	/// <returns>A ValidationResult object containing any validation failures.</returns>
	public virtual async Task<ValidationResult> ValidateAsync(ValidationContext<T> context, CancellationToken cancellation = default) {
		ArgumentNullException.ThrowIfNull(context);
		context.IsAsync = true;
		return await ValidateInternalAsync(context, cancellation);
	}

	[Zomp.SyncMethodGenerator.CreateSyncVersion(OmitNullableDirective = true)]
	private async ValueTask<ValidationResult> ValidateInternalAsync(ValidationContext<T> context, CancellationToken cancellation) {
		var result = new ValidationResult(context.Failures);
		bool shouldContinue = PreValidate(context, result);

		if (!shouldContinue) {
			if (!result.IsValid && context.ThrowOnFailures) {
				RaiseValidationException(context, result);
			}

			return result;
		}

		if (context.InstanceToValidate == null) {
			throw new InvalidOperationException("Cannot pass a null model to Validate/ValidateAsync. The root model must be non-null.");
		}

		int count = Rules.Count;

		// Performance: Use for loop rather than foreach to reduce allocations.
		for (int i = 0; i < count; i++) {
			cancellation.ThrowIfCancellationRequested();
			var totalFailures = context.Failures.Count;

			await Rules[i].ValidateAsync(context, cancellation);

			if (ClassLevelCascadeMode == CascadeMode.Stop && result.Errors.Count > totalFailures) {
				// Bail out if we're "failing-fast". Check to see if the number of failures
				// has been increased by this rule (which could've generated 1 or more failures).
				break;
			}
		}

		SetExecutedRuleSets(result, context);

		if (!result.IsValid && context.ThrowOnFailures) {
			RaiseValidationException(context, result);
		}

		return result;
	}

	private void SetExecutedRuleSets(ValidationResult result, ValidationContext<T> context) {
		if (context.RootContextData.TryGetValue("_FV_RuleSetsExecuted", out var obj) && obj is HashSet<string> set) {
			result.RuleSetsExecuted = set.ToArray();
		}
		else {
			result.RuleSetsExecuted = RulesetValidatorSelector.DefaultRuleSetNameInArray;
		}
	}

	/// <summary>
	/// Creates a <see cref="IValidatorDescriptor" /> that can be used to obtain metadata about the current validator.
	/// </summary>
	public virtual IValidatorDescriptor CreateDescriptor() => new ValidatorDescriptor<T>(Rules);

	bool IValidator.CanValidateInstancesOfType(Type type) {
		ArgumentNullException.ThrowIfNull(type);
		return typeof(T).IsAssignableFrom(type);
	}

	/// <summary>
	/// Defines a validation rule for a specific property.
	/// </summary>
	/// <example>
	/// RuleFor(x => x.Surname)...
	/// </example>
	/// <typeparam name="TProperty">The type of property being validated</typeparam>
	/// <param name="expression">The expression representing the property to validate</param>
	/// <returns>an IRuleBuilder instance on which validators can be defined</returns>
	public IRuleBuilderInitial<T, TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty>> expression) {
		ArgumentNullException.ThrowIfNull(expression);
		var rule = PropertyRule<T, TProperty>.Create(expression, () => RuleLevelCascadeMode);
		Rules.Add(rule);
		OnRuleAdded(rule);
		return new RuleBuilder<T, TProperty>(rule, this);
	}

	/// <summary>
	/// Invokes a rule for each item in the collection.
	/// </summary>
	/// <typeparam name="TElement">Type of property</typeparam>
	/// <param name="expression">Expression representing the collection to validate</param>
	/// <returns>An IRuleBuilder instance on which validators can be defined</returns>
	public IRuleBuilderInitialCollection<T, TElement> RuleForEach<TElement>(Expression<Func<T, IEnumerable<TElement>>> expression) {
		ArgumentNullException.ThrowIfNull(expression);
		var rule = CollectionPropertyRule<T, TElement>.Create(expression, () => RuleLevelCascadeMode);
		Rules.Add(rule);
		OnRuleAdded(rule);
		return new RuleBuilder<T, TElement>(rule, this);
	}

	/// <summary>
	/// Defines a RuleSet that can be used to group together several validators.
	/// </summary>
	/// <param name="ruleSetName">The name of the ruleset.</param>
	/// <param name="action">Action that encapsulates the rules in the ruleset.</param>
	public void RuleSet(string ruleSetName, Action action) {
		ExtensionsInternal.ThrowIfNullOrEmpty(ruleSetName);
		ArgumentNullException.ThrowIfNull(action);

		var ruleSetNames = ruleSetName.Split(',', ';')
			.Select(x => x.Trim())
			.ToArray();

		using (Rules.OnItemAdded(r => r.RuleSets = ruleSetNames)) {
			action();
		}
	}

	/// <summary>
	/// Defines a condition that applies to several rules
	/// </summary>
	/// <param name="predicate">The condition that should apply to multiple rules</param>
	/// <param name="action">Action that encapsulates the rules.</param>
	/// <returns></returns>
	public IConditionBuilder When(Func<T, bool> predicate, Action action) {
		ArgumentNullException.ThrowIfNull(predicate);
		ArgumentNullException.ThrowIfNull(action);
		return When((x, _) => predicate(x), action);
	}

	/// <summary>
	/// Defines a condition that applies to several rules
	/// </summary>
	/// <param name="predicate">The condition that should apply to multiple rules</param>
	/// <param name="action">Action that encapsulates the rules.</param>
	/// <returns></returns>
	public IConditionBuilder When(Func<T, ValidationContext<T>, bool> predicate, Action action) {
		ArgumentNullException.ThrowIfNull(predicate);
		ArgumentNullException.ThrowIfNull(action);
		return new ConditionBuilder<T>(Rules).When(predicate, action);
	}

	/// <summary>
	/// Defines an inverse condition that applies to several rules
	/// </summary>
	/// <param name="predicate">The condition that should be applied to multiple rules</param>
	/// <param name="action">Action that encapsulates the rules</param>
	public IConditionBuilder Unless(Func<T, bool> predicate, Action action) {
		ArgumentNullException.ThrowIfNull(predicate);
		ArgumentNullException.ThrowIfNull(action);
		return Unless((x, _) => predicate(x), action);
	}

	/// <summary>
	/// Defines an inverse condition that applies to several rules
	/// </summary>
	/// <param name="predicate">The condition that should be applied to multiple rules</param>
	/// <param name="action">Action that encapsulates the rules</param>
	public IConditionBuilder Unless(Func<T, ValidationContext<T>, bool> predicate, Action action) {
		ArgumentNullException.ThrowIfNull(predicate);
		ArgumentNullException.ThrowIfNull(action);
		return new ConditionBuilder<T>(Rules).Unless(predicate, action);
	}

	/// <summary>
	/// Defines an asynchronous condition that applies to several rules
	/// </summary>
	/// <param name="predicate">The asynchronous condition that should apply to multiple rules</param>
	/// <param name="action">Action that encapsulates the rules.</param>
	/// <returns></returns>
	public IConditionBuilder WhenAsync(Func<T, CancellationToken, Task<bool>> predicate, Action action) {
		ArgumentNullException.ThrowIfNull(predicate);
		ArgumentNullException.ThrowIfNull(action);
		return WhenAsync((x, _, cancel) => predicate(x, cancel), action);
	}

	/// <summary>
	/// Defines an asynchronous condition that applies to several rules
	/// </summary>
	/// <param name="predicate">The asynchronous condition that should apply to multiple rules</param>
	/// <param name="action">Action that encapsulates the rules.</param>
	/// <returns></returns>
	public IConditionBuilder WhenAsync(Func<T, ValidationContext<T>, CancellationToken, Task<bool>> predicate, Action action) {
		ArgumentNullException.ThrowIfNull(predicate);
		ArgumentNullException.ThrowIfNull(action);
		return new AsyncConditionBuilder<T>(Rules).WhenAsync(predicate, action);
	}

	/// <summary>
	/// Defines an inverse asynchronous condition that applies to several rules
	/// </summary>
	/// <param name="predicate">The asynchronous condition that should be applied to multiple rules</param>
	/// <param name="action">Action that encapsulates the rules</param>
	public IConditionBuilder UnlessAsync(Func<T, CancellationToken, Task<bool>> predicate, Action action) {
		ArgumentNullException.ThrowIfNull(predicate);
		ArgumentNullException.ThrowIfNull(action);
		return UnlessAsync((x, _, cancel) => predicate(x, cancel), action);
	}

	/// <summary>
	/// Defines an inverse asynchronous condition that applies to several rules
	/// </summary>
	/// <param name="predicate">The asynchronous condition that should be applied to multiple rules</param>
	/// <param name="action">Action that encapsulates the rules</param>
	public IConditionBuilder UnlessAsync(Func<T, ValidationContext<T>, CancellationToken, Task<bool>> predicate, Action action) {
		ArgumentNullException.ThrowIfNull(predicate);
		ArgumentNullException.ThrowIfNull(action);
		return new AsyncConditionBuilder<T>(Rules).UnlessAsync(predicate, action);
	}

	/// <summary>
	/// Includes the rules from the specified validator
	/// </summary>
	public void Include(IValidator<T> rulesToInclude) {
		ArgumentNullException.ThrowIfNull(rulesToInclude);
		var rule = IncludeRule<T>.Create(rulesToInclude, () => RuleLevelCascadeMode);
		Rules.Add(rule);
		OnRuleAdded(rule);
	}

	/// <summary>
	/// Includes the rules from the specified validator
	/// </summary>
	public void Include<TValidator>(Func<T, TValidator> rulesToInclude) where TValidator : IValidator<T> {
		ArgumentNullException.ThrowIfNull(rulesToInclude);
		var rule = IncludeRule<T>.Create(rulesToInclude, () => RuleLevelCascadeMode);
		Rules.Add(rule);
		OnRuleAdded(rule);
	}

	/// <summary>
	/// Returns an enumerator that iterates through the collection of validation rules.
	/// </summary>
	/// <returns>
	/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
	/// </returns>
	/// <filterpriority>1</filterpriority>
	public IEnumerator<IValidationRule> GetEnumerator() => Rules.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	/// <summary>
	/// Determines if validation should occtur and provides a means to modify the context and ValidationResult prior to execution.
	/// If this method returns false, then the ValidationResult is immediately returned from Validate/ValidateAsync.
	/// </summary>
	/// <param name="context"></param>
	/// <param name="result"></param>
	/// <returns></returns>
	protected virtual bool PreValidate(ValidationContext<T> context, ValidationResult result) => true;

	/// <summary>
	/// Throws a ValidationException. This method will only be called if the validator has been configured
	/// to throw exceptions if validation fails. The default behaviour is not to throw an exception.
	/// </summary>
	/// <param name="context"></param>
	/// <param name="result"></param>
	/// <exception cref="ValidationException"></exception>
	protected virtual void RaiseValidationException(ValidationContext<T> context, ValidationResult result)
		=> throw new ValidationException(result.Errors);

	/// <summary>
	/// This method is invoked when a rule has been created (via RuleFor/RuleForEach) and has been added to the validator.
	/// You can override this method to provide customizations to all rule instances.
	/// </summary>
	/// <param name="rule"></param>
	protected virtual void OnRuleAdded(IValidationRule<T> rule) { }
}
