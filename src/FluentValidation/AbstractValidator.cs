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
	public abstract class AbstractValidator<T> : IValidator<T>, IEnumerable<IValidationRule> {
		internal TrackingCollection<IValidationRule> Rules { get; } = new TrackingCollection<IValidationRule>();
		private Func<CascadeMode> _cascadeMode = () => ValidatorOptions.CascadeMode;

		/// <summary>
		/// Sets the cascade mode for all rules within this validator.
		/// </summary>
		public CascadeMode CascadeMode {
			get => _cascadeMode();
			set => _cascadeMode = () => value;
		}

		ValidationResult IValidator.Validate(object instance) {
			return ((IValidator) this).Validate(new ValidationContext(instance));
		}

		Task<ValidationResult> IValidator.ValidateAsync(object instance, CancellationToken cancellation) {
			return ((IValidator)this).ValidateAsync(new ValidationContext(instance), cancellation);
		}

		ValidationResult IValidator.Validate(ValidationContext context) {
			context.Guard("Cannot pass null to Validate", nameof(context));
			return Validate(ValidationContext<T>.GetFromNonGenericContext(context));
		}

		Task<ValidationResult> IValidator.ValidateAsync(ValidationContext context, CancellationToken cancellation) {
			context.Guard("Cannot pass null to Validate", nameof(context));
			return ValidateAsync(ValidationContext<T>.GetFromNonGenericContext(context), cancellation);
		}

		/// <summary>
		/// Validates the specified instance
		/// </summary>
		/// <param name="instance">The object to validate</param>
		/// <returns>A ValidationResult object containing any validation failures</returns>
		public ValidationResult Validate(T instance) {
			return Validate(new ValidationContext<T>(instance, new PropertyChain(), ValidatorOptions.ValidatorSelectors.DefaultValidatorSelectorFactory()));
		}

		/// <summary>
		/// Validates the specified instance asynchronously
		/// </summary>
		/// <param name="instance">The object to validate</param>
		/// <param name="cancellation">Cancellation token</param>
		/// <returns>A ValidationResult object containing any validation failures</returns>
		public Task<ValidationResult> ValidateAsync(T instance, CancellationToken cancellation = new CancellationToken()) {
			return ValidateAsync(new ValidationContext<T>(instance, new PropertyChain(), ValidatorOptions.ValidatorSelectors.DefaultValidatorSelectorFactory()), cancellation);
		}

		/// <summary>
		/// Validates the specified instance.
		/// </summary>
		/// <param name="context">Validation Context</param>
		/// <returns>A ValidationResult object containing any validation failures.</returns>
		public virtual ValidationResult Validate(ValidationContext<T> context) {
			context.Guard("Cannot pass null to Validate.", nameof(context));

			var result = new ValidationResult();
			bool shouldContinue = PreValidate(context, result);

			if (!shouldContinue) {
				return result;
			}

			EnsureInstanceNotNull(context.InstanceToValidate);

			var failures = Rules.SelectMany(x => x.Validate(context));

			foreach (var validationFailure in failures.Where(failure => failure != null)) {
				result.Errors.Add(validationFailure);
			}

			SetExecutedRulesets(result, context);

			return result;
		}

		/// <summary>
		/// Validates the specified instance asynchronously.
		/// </summary>
		/// <param name="context">Validation Context</param>
		/// <param name="cancellation">Cancellation token</param>
		/// <returns>A ValidationResult object containing any validation failures.</returns>
		public async virtual Task<ValidationResult> ValidateAsync(ValidationContext<T> context, CancellationToken cancellation = new CancellationToken()) {
			context.Guard("Cannot pass null to Validate", nameof(context));
			context.RootContextData["__FV_IsAsyncExecution"] = true;

			var result = new ValidationResult();

			bool shouldContinue = PreValidate(context, result);

			if (!shouldContinue) {
				return result;
			}

			EnsureInstanceNotNull(context.InstanceToValidate);

			foreach (var rule in Rules) {
				cancellation.ThrowIfCancellationRequested();
				var failures = await rule.ValidateAsync(context, cancellation);

				foreach (var failure in failures.Where(f => f != null)) {
					result.Errors.Add(failure);
				}
			}

			SetExecutedRulesets(result, context);

			return result;
		}

		private void SetExecutedRulesets(ValidationResult result, ValidationContext<T> context) {
			var executed = context.RootContextData.GetOrAdd("_FV_RuleSetsExecuted", () => new HashSet<string>{"default"});
			result.RuleSetsExecuted = executed.ToArray();
		}

		/// <summary>
		/// Adds a rule to the current validator.
		/// </summary>
		/// <param name="rule"></param>
		protected void AddRule(IValidationRule rule) {
			Rules.Add(rule);
		}

		/// <summary>
		/// Creates a <see cref="IValidatorDescriptor" /> that can be used to obtain metadata about the current validator.
		/// </summary>
		public virtual IValidatorDescriptor CreateDescriptor() {
			return new ValidatorDescriptor<T>(Rules);
		}

		bool IValidator.CanValidateInstancesOfType(Type type) {
			if (type == null) throw new ArgumentNullException(nameof(type));
			return typeof(T).IsAssignableFrom(type);
		}

		/// <summary>
		/// Defines a validation rule for a specify property.
		/// </summary>
		/// <example>
		/// RuleFor(x => x.Surname)...
		/// </example>
		/// <typeparam name="TProperty">The type of property being validated</typeparam>
		/// <param name="expression">The expression representing the property to validate</param>
		/// <returns>an IRuleBuilder instance on which validators can be defined</returns>
		public IRuleBuilderInitial<T, TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty>> expression) {
			expression.Guard("Cannot pass null to RuleFor", nameof(expression));
			// If rule-level caching is enabled, then bypass the expression-level cache.
			// Otherwise we essentially end up caching expressions twice unnecessarily.
			var rule = PropertyRule.Create(expression, () => CascadeMode);
			AddRule(rule);
			var ruleBuilder = new RuleBuilder<T, TProperty>(rule, this);
			return ruleBuilder;
		}

		/// <summary>
		/// Invokes a rule for each item in the collection
		/// </summary>
		/// <typeparam name="TProperty">Type of property</typeparam>
		/// <param name="expression">Expression representing the collection to validate</param>
		/// <returns>An IRuleBuilder instance on which validators can be defined</returns>
		public IRuleBuilderInitialCollection<T, TProperty> RuleForEach<TProperty>(Expression<Func<T, IEnumerable<TProperty>>> expression) {
			expression.Guard("Cannot pass null to RuleForEach", nameof(expression));
			var rule = CollectionPropertyRule<TProperty>.Create(expression, () => CascadeMode);
			AddRule(rule);
			var ruleBuilder = new RuleBuilder<T, TProperty>(rule, this);
			return ruleBuilder;
		}

		/// <summary>
		/// Defines a RuleSet that can be used to group together several validators.
		/// </summary>
		/// <param name="ruleSetName">The name of the ruleset.</param>
		/// <param name="action">Action that encapsulates the rules in the ruleset.</param>
		public void RuleSet(string ruleSetName, Action action) {
			ruleSetName.Guard("A name must be specified when calling RuleSet.", nameof(ruleSetName));
			action.Guard("A ruleset definition must be specified when calling RuleSet.", nameof(action));

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
			return When((x, ctx) => predicate(x), action);
		}

		/// <summary>
		/// Defines a condition that applies to several rules
		/// </summary>
		/// <param name="predicate">The condition that should apply to multiple rules</param>
		/// <param name="action">Action that encapsulates the rules.</param>
		/// <returns></returns>
		public IConditionBuilder When(Func<T, ValidationContext<T>, bool> predicate, Action action) {
			return new ConditionBuilder<T>(Rules).When(predicate, action);
		}

		/// <summary>
		/// Defines an inverse condition that applies to several rules
		/// </summary>
		/// <param name="predicate">The condition that should be applied to multiple rules</param>
		/// <param name="action">Action that encapsulates the rules</param>
		public IConditionBuilder Unless(Func<T, bool> predicate, Action action) {
			return Unless((x, ctx) => predicate(x), action);
		}

		/// <summary>
		/// Defines an inverse condition that applies to several rules
		/// </summary>
		/// <param name="predicate">The condition that should be applied to multiple rules</param>
		/// <param name="action">Action that encapsulates the rules</param>
		public IConditionBuilder Unless(Func<T, ValidationContext<T>, bool> predicate, Action action) {
			return new ConditionBuilder<T>(Rules).Unless(predicate, action);
		}

		/// <summary>
		/// Defines an asynchronous condition that applies to several rules
		/// </summary>
		/// <param name="predicate">The asynchronous condition that should apply to multiple rules</param>
		/// <param name="action">Action that encapsulates the rules.</param>
		/// <returns></returns>
		public IConditionBuilder WhenAsync(Func<T, CancellationToken, Task<bool>> predicate, Action action) {
			return WhenAsync((x, ctx, cancel) => predicate(x, cancel), action);
		}

		/// <summary>
		/// Defines an asynchronous condition that applies to several rules
		/// </summary>
		/// <param name="predicate">The asynchronous condition that should apply to multiple rules</param>
		/// <param name="action">Action that encapsulates the rules.</param>
		/// <returns></returns>
		public IConditionBuilder WhenAsync(Func<T, ValidationContext<T>, CancellationToken, Task<bool>> predicate, Action action) {
			return new AsyncConditionBuilder<T>(Rules).WhenAsync(predicate, action);
		}

		/// <summary>
		/// Defines an inverse asynchronous condition that applies to several rules
		/// </summary>
		/// <param name="predicate">The asynchronous condition that should be applied to multiple rules</param>
		/// <param name="action">Action that encapsulates the rules</param>
		public IConditionBuilder UnlessAsync(Func<T, CancellationToken, Task<bool>> predicate, Action action) {
			return UnlessAsync((x, ctx, cancel) => predicate(x, cancel), action);
		}

		/// <summary>
		/// Defines an inverse asynchronous condition that applies to several rules
		/// </summary>
		/// <param name="predicate">The asynchronous condition that should be applied to multiple rules</param>
		/// <param name="action">Action that encapsulates the rules</param>
		public IConditionBuilder UnlessAsync(Func<T, ValidationContext<T>, CancellationToken, Task<bool>> predicate, Action action) {
			return new AsyncConditionBuilder<T>(Rules).UnlessAsync(predicate, action);
		}

		/// <summary>
		/// Includes the rules from the specified validator
		/// </summary>
		public void Include(IValidator<T> rulesToInclude) {
			rulesToInclude.Guard("Cannot pass null to Include", nameof(rulesToInclude));
			var rule = IncludeRule.Create<T>(rulesToInclude, () => CascadeMode);
			AddRule(rule);
		}

		/// <summary>
		/// Includes the rules from the specified validator
		/// </summary>
		public void Include<TValidator>(Func<T, TValidator> rulesToInclude) where TValidator : IValidator<T> {
			rulesToInclude.Guard("Cannot pass null to Include", nameof(rulesToInclude));
			var rule = IncludeRule.Create(rulesToInclude, () => CascadeMode);
			AddRule(rule);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection of validation rules.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public IEnumerator<IValidationRule> GetEnumerator() {
			return Rules.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		/// <summary>
		/// Throws an exception if the instance being validated is null.
		/// </summary>
		/// <param name="instanceToValidate"></param>
		protected virtual void EnsureInstanceNotNull(object instanceToValidate) {
			instanceToValidate.Guard("Cannot pass null model to Validate.", nameof(instanceToValidate));
		}

		/// <summary>
		/// Determines if validation should occur and provides a means to modify the context and ValidationResult prior to execution.
		/// If this method returns false, then the ValidationResult is immediately returned from Validate/ValidateAsync.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		protected virtual bool PreValidate(ValidationContext<T> context, ValidationResult result) {
			return true;
		}
	}
}
