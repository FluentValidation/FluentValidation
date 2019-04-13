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

namespace FluentValidation.Internal {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Threading;
	using System.Threading.Tasks;
	using Resources;
	using Results;
	using Validators;

	/// <summary>
	/// Defines a rule associated with a property.
	/// </summary>
	public class PropertyRule : IValidationRule {
		readonly List<IPropertyValidator> _validators = new List<IPropertyValidator>();
		Func<CascadeMode> _cascadeModeThunk = () => ValidatorOptions.CascadeMode;
		string _propertyDisplayName;
		string _propertyName;
		private string[] _ruleSet = new string[0];
		private Func<ValidationContext, bool> _condition;
		private Func<ValidationContext, CancellationToken, Task<bool>> _asyncCondition;

		/// <summary>
		/// Condition for all validators in this rule.
		/// </summary>
		public Func<ValidationContext, bool> Condition => _condition;

		/// <summary>
		/// Asynchronous condition for all validators in this rule.
		/// </summary>
		public Func<ValidationContext, CancellationToken, Task<bool>> AsyncCondition => _asyncCondition;

		/// <summary>
		/// Property associated with this rule.
		/// </summary>
		public MemberInfo Member { get; }

		/// <summary>
		/// Function that can be invoked to retrieve the value of the property.
		/// </summary>
		public Func<object, object> PropertyFunc { get; }

		/// <summary>
		/// Expression that was used to create the rule.
		/// </summary>
		public LambdaExpression Expression { get; }

		/// <summary>
		/// String source that can be used to retrieve the display name (if null, falls back to the property name)
		/// </summary>
		public IStringSource DisplayName { get; set; }

		/// <summary>
		/// Rule set that this rule belongs to (if specified)
		/// </summary>
		public string[] RuleSets {
			get => _ruleSet;
			set => _ruleSet = value ?? new string[0];
		}

		/// <summary>
		/// Function that will be invoked if any of the validators associated with this rule fail.
		/// </summary>
		public Action<object, IEnumerable<ValidationFailure>> OnFailure { get; set; }

		/// <summary>
		/// The current validator being configured by this rule.
		/// </summary>
		public IPropertyValidator CurrentValidator => _validators.LastOrDefault();

		/// <summary>
		/// Type of the property being validated
		/// </summary>
		public Type TypeToValidate { get; }

		/// <summary>
		/// Cascade mode for this rule.
		/// </summary>
		public CascadeMode CascadeMode {
			get => _cascadeModeThunk();
			set => _cascadeModeThunk = () => value;
		}

		/// <summary>
		/// Validators associated with this rule.
		/// </summary>
		public IEnumerable<IPropertyValidator> Validators => _validators;

		/// <summary>
		/// Creates a new property rule.
		/// </summary>
		/// <param name="member">Property</param>
		/// <param name="propertyFunc">Function to get the property value</param>
		/// <param name="expression">Lambda expression used to create the rule</param>
		/// <param name="cascadeModeThunk">Function to get the cascade mode.</param>
		/// <param name="typeToValidate">Type to validate</param>
		/// <param name="containerType">Container type that owns the property</param>
		public PropertyRule(MemberInfo member, Func<object, object> propertyFunc, LambdaExpression expression, Func<CascadeMode> cascadeModeThunk, Type typeToValidate, Type containerType) {
			Member = member;
			PropertyFunc = propertyFunc;
			Expression = expression;
			TypeToValidate = typeToValidate;
			_cascadeModeThunk = cascadeModeThunk;

			DependentRules = new List<IValidationRule>();
			PropertyName = ValidatorOptions.PropertyNameResolver(containerType, member, expression);
			DisplayName = new LazyStringSource(x =>  ValidatorOptions.DisplayNameResolver(containerType, member, expression));
		}

		/// <summary>
		/// Creates a new property rule from a lambda expression.
		/// </summary>
		public static PropertyRule Create<T, TProperty>(Expression<Func<T, TProperty>> expression) {
			return Create(expression, () => ValidatorOptions.CascadeMode);
		}

		/// <summary>
		/// Creates a new property rule from a lambda expression.
		/// </summary>
		public static PropertyRule Create<T, TProperty>(Expression<Func<T, TProperty>> expression, Func<CascadeMode> cascadeModeThunk, bool bypassCache = false) {
			var member = expression.GetMember();
			var compiled = AccessorCache<T>.GetCachedAccessor(member, expression, bypassCache);
			return new PropertyRule(member, compiled.CoerceToNonGeneric(), expression, cascadeModeThunk, typeof(TProperty), typeof(T));
		}

		/// <summary>
		/// Adds a validator to the rule.
		/// </summary>
		public void AddValidator(IPropertyValidator validator) {
			_validators.Add(validator);
		}

		/// <summary>
		/// Replaces a validator in this rule. Used to wrap validators.
		/// </summary>
		public void ReplaceValidator(IPropertyValidator original, IPropertyValidator newValidator) {
			var index = _validators.IndexOf(original);

			if (index > -1) {
				_validators[index] = newValidator;
			}
		}

		/// <summary>
		/// Remove a validator in this rule.
		/// </summary>
		public void RemoveValidator(IPropertyValidator original) {
			_validators.Remove(original);
		}

		/// <summary>
		/// Clear all validators from this rule.
		/// </summary>
		public void ClearValidators() {
			_validators.Clear();
		}

		/// <summary>
		/// Returns the property name for the property being validated.
		/// Returns null if it is not a property being validated (eg a method call)
		/// </summary>
		public string PropertyName {
			get { return _propertyName; }
			set {
				_propertyName = value;
				_propertyDisplayName = _propertyName.SplitPascalCase();
			}
		}

		/// <summary>
		/// Allows custom creation of an error message
		/// </summary>
		public Func<MessageBuilderContext, string> MessageBuilder { get; set; }

		/// <summary>
		/// Dependent rules
		/// </summary>
		public List<IValidationRule> DependentRules { get; private set; }

		public Func<object, object> Transformer { get; set; }

		/// <summary>
		/// Display name for the property.
		/// </summary>
		public string GetDisplayName() {
			string result = null;

			if (DisplayName != null) {
				result = DisplayName.GetString(null /*We don't have a model object at this point*/);
			}

			if (result == null) {
				result = _propertyDisplayName;
			}

			return result;
		}

		/// <summary>
		/// Display name for the property.
		/// </summary>
		public string GetDisplayName(IValidationContext context) {
			string result = null;

			if (DisplayName != null) {
				result = DisplayName.GetString(context);
			}

			if (result == null) {
				result = _propertyDisplayName;
			}

			return result;
		}

		/// <summary>
		/// Performs validation using a validation context and returns a collection of Validation Failures.
		/// </summary>
		/// <param name="context">Validation Context</param>
		/// <returns>A collection of validation failures</returns>
		public virtual IEnumerable<ValidationFailure> Validate(ValidationContext context) {
			string displayName = GetDisplayName(context);

			if (PropertyName == null && displayName == null) {
				//No name has been specified. Assume this is a model-level rule, so we should use empty string instead.
				displayName = string.Empty;
			}

			// Construct the full name of the property, taking into account overriden property names and the chain (if we're in a nested validator)
			string propertyName = context.PropertyChain.BuildPropertyName(PropertyName ?? displayName);

			// Ensure that this rule is allowed to run.
			// The validatselector has the opportunity to veto this before any of the validators execute.
			if (!context.Selector.CanExecute(this, propertyName, context)) {
				yield break;
			}
			
			if (_condition != null) {
				if (!_condition(context)) {
					yield break;
				}
			}
			
			// TODO: For FV 9, throw an exception by default if synchronous validator has async condition.
			if (_asyncCondition != null) {
				if (!_asyncCondition(context, default).GetAwaiter().GetResult()) {
					yield break;
				}
			}

			var cascade = _cascadeModeThunk();
			var failures = new List<ValidationFailure>();

			// Invoke each validator and collect its results.
			foreach (var validator in _validators) {
				IEnumerable<ValidationFailure> results;
				if (validator.ShouldValidateAsync(context))
					//TODO: For FV 9 by default disallow invocation of async validators when running synchronously.
					results = InvokePropertyValidatorAsync(context, validator, propertyName, default).GetAwaiter().GetResult();
				else
					results = InvokePropertyValidator(context, validator, propertyName);

				bool hasFailure = false;

				foreach (var result in results) {
					failures.Add(result);
					hasFailure = true;
					yield return result;
				}

				// If there has been at least one failure, and our CascadeMode has been set to StopOnFirst
				// then don't continue to the next rule
				if (cascade == FluentValidation.CascadeMode.StopOnFirstFailure && hasFailure) {
					break;
				}
			}

			if (failures.Count > 0) {
				// Callback if there has been at least one property validator failed.
				OnFailure?.Invoke(context.InstanceToValidate, failures);
			}
			else {
				foreach (var dependentRule in DependentRules) {
					foreach (var failure in dependentRule.Validate(context)) {
						yield return failure;
					}
				}
			}
		}

		/// <summary>
		/// Performs asynchronous validation using a validation context and returns a collection of Validation Failures.
		/// </summary>
		/// <param name="context">Validation Context</param>
		/// <param name="cancellation"></param>
		/// <returns>A collection of validation failures</returns>
		public virtual async Task<IEnumerable<ValidationFailure>> ValidateAsync(ValidationContext context, CancellationToken cancellation) {
			if (!context.IsAsync()) {
				context.RootContextData["__FV_IsAsyncExecution"] = true;
			}

			var displayName = GetDisplayName(context);

			if (PropertyName == null && displayName == null) {
				//No name has been specified. Assume this is a model-level rule, so we should use empty string instead.
				displayName = string.Empty;
			}

			// Construct the full name of the property, taking into account overriden property names and the chain (if we're in a nested validator)
			var propertyName = context.PropertyChain.BuildPropertyName(PropertyName ?? displayName);

			// Ensure that this rule is allowed to run.
			// The validatselector has the opportunity to veto this before any of the validators execute.
			if (!context.Selector.CanExecute(this, propertyName, context)) {
				return Enumerable.Empty<ValidationFailure>();
			}
			
			if (_condition != null) {
				if (!_condition(context)) {
					return Enumerable.Empty<ValidationFailure>();
				}
			}
			
			// TODO: For FV 9, throw an exception by default if synchronous validator has async condition.
			if (_asyncCondition != null) {
				if (! await _asyncCondition(context, cancellation)) {
					return Enumerable.Empty<ValidationFailure>();
				}
			}

			var cascade = _cascadeModeThunk();
			var failures = new List<ValidationFailure>();

			var fastExit = false;

			// Firstly, invoke all synchronous validators and collect their results.
			foreach (var validator in _validators.Where(v => !v.ShouldValidateAsync(context))) {
				cancellation.ThrowIfCancellationRequested();
				failures.AddRange(InvokePropertyValidator(context, validator, propertyName));

				// If there has been at least one failure, and our CascadeMode has been set to StopOnFirst
				// then don't continue to the next rule
				fastExit = cascade == CascadeMode.StopOnFirstFailure && failures.Count > 0;

				if (fastExit) {
					break;
				}
			}

			//if StopOnFirstFailure triggered then we exit
			if (fastExit && failures.Count > 0) {
				// Callback if there has been at least one property validator failed.
				OnFailure?.Invoke(context.InstanceToValidate, failures);
				return failures;
			}

			var asyncValidators = _validators.Where(v => v.ShouldValidateAsync(context)).ToList();

			// if there's no async validators then we exit
			if (asyncValidators.Count == 0) {
				if (failures.Count > 0) {
					// Callback if there has been at least one property validator failed.
					OnFailure?.Invoke(context.InstanceToValidate, failures);
				}
				else {
					failures.AddRange(await RunDependentRulesAsync(context, cancellation));
				}

				return failures;
			}

			foreach (var asyncValidator in asyncValidators) {
				cancellation.ThrowIfCancellationRequested();

				var propertyFailures = await InvokePropertyValidatorAsync(context, asyncValidator, propertyName, cancellation);
				failures.AddRange(propertyFailures);

				if (cascade == CascadeMode.StopOnFirstFailure && failures.Count > 0) {
					break;
				}
			}

			if (failures.Count > 0) {
				OnFailure?.Invoke(context.InstanceToValidate, failures);
			}
			else {
				failures.AddRange(await RunDependentRulesAsync(context, cancellation));
			}

			return failures;
		}

		private async Task<IEnumerable<ValidationFailure>> RunDependentRulesAsync(ValidationContext context, CancellationToken cancellation) {
			var failures = new List<ValidationFailure>();

			foreach (var rule in DependentRules) {
				cancellation.ThrowIfCancellationRequested();
				failures.AddRange(await rule.ValidateAsync(context, cancellation));
			}

			return failures;
		}

		/// <summary>
		/// Invokes the validator asynchronously
		/// </summary>
		/// <param name="context"></param>
		/// <param name="validator"></param>
		/// <param name="propertyName"></param>
		/// <param name="cancellation"></param>
		/// <returns></returns>
		protected virtual Task<IEnumerable<ValidationFailure>> InvokePropertyValidatorAsync(ValidationContext context, IPropertyValidator validator, string propertyName, CancellationToken cancellation) {
			return validator.ValidateAsync(new PropertyValidatorContext(context, this, propertyName), cancellation);
		}

		/// <summary>
		/// Invokes a property validator using the specified validation context.
		/// </summary>
		protected virtual IEnumerable<ValidationFailure> InvokePropertyValidator(ValidationContext context, IPropertyValidator validator, string propertyName) {
			var propertyContext = new PropertyValidatorContext(context, this, propertyName);
			return validator.Validate(propertyContext);
		}
		
		/// <summary>
		/// Applies a condition to the rule
		/// </summary>
		/// <param name="predicate"></param>
		/// <param name="applyConditionTo"></param>
		public void ApplyCondition(Func<PropertyValidatorContext, bool> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators) {
			// Default behaviour for When/Unless as of v1.3 is to apply the condition to all previous validators in the chain.
			if (applyConditionTo == ApplyConditionTo.AllValidators) {
				foreach (var validator in Validators) {
					validator.Options.ApplyCondition(predicate);
				}

				foreach (var dependentRule in DependentRules) {
					dependentRule.ApplyCondition(predicate, applyConditionTo);
				}
			}
			else {
				CurrentValidator.Options.ApplyCondition(predicate);
			}
		}

		/// <summary>
		/// Applies the condition to the rule asynchronously
		/// </summary>
		/// <param name="predicate"></param>
		/// <param name="applyConditionTo"></param>
		public void ApplyAsyncCondition(Func<PropertyValidatorContext, CancellationToken, Task<bool>> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators) {
			// Default behaviour for When/Unless as of v1.3 is to apply the condition to all previous validators in the chain.
			if (applyConditionTo == ApplyConditionTo.AllValidators) {
				foreach (var validator in Validators) {
					validator.Options.ApplyAsyncCondition(predicate);
				}

				foreach (var dependentRule in DependentRules) {
					dependentRule.ApplyAsyncCondition(predicate, applyConditionTo);
				}
			}
			else {
				CurrentValidator.Options.ApplyAsyncCondition(predicate);
			}
		}

		// TODO: Consider making these public and part of the interface for FV 9.
		internal void ApplySharedCondition(Func<ValidationContext, bool> condition) {
			if (_condition == null) {
				_condition = condition;
			}
			else {
				var original = _condition;
				_condition = ctx => condition(ctx) && original(ctx);
			}
		}

		internal void ApplySharedAsyncCondition(Func<ValidationContext, CancellationToken, Task<bool>> condition) {
			if (_asyncCondition == null) {
				_asyncCondition = condition;
			}
			else {
				var original = _asyncCondition;
				_asyncCondition = async (ctx, ct) => await condition(ctx, ct) && await original(ctx, ct);
			}
			
		}
	}
}