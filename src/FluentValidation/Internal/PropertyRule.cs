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
		private readonly List<IPropertyValidator> _validators = new List<IPropertyValidator>();
		private Func<CascadeMode> _cascadeModeThunk;
		private string _propertyDisplayName;
		private string _propertyName;
		private string[] _ruleSet = new string[0];
		private Func<IValidationContext, bool> _condition;
		private Func<IValidationContext, CancellationToken, Task<bool>> _asyncCondition;

#pragma warning disable 618
		//TODO: Replace with Func<IValidationContext, string> for FV 10.
		private IStringSource _displayNameSource;
#pragma warning restore 618

		/// <summary>
		/// Condition for all validators in this rule.
		/// </summary>
		public Func<IValidationContext, bool> Condition => _condition;

		/// <summary>
		/// Asynchronous condition for all validators in this rule.
		/// </summary>
		public Func<IValidationContext, CancellationToken, Task<bool>> AsyncCondition => _asyncCondition;

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
		[Obsolete("This property is deprecated and will be removed in FluentValidation 10. Use the GetDisplayName and SetDisplayName instead.")]
		public IStringSource DisplayName {
			get => _displayNameSource;
			set => _displayNameSource = value;
		}

		/// <summary>
		/// Sets the display name for the property.
		/// </summary>
		/// <param name="name">The property's display name</param>
		public void SetDisplayName(string name) {
#pragma warning disable 618
			_displayNameSource = new StaticStringSource(name);
#pragma warning restore 618
		}

		/// <summary>
		/// Sets the display name for the property using a function.
		/// </summary>
		/// <param name="factory">The function for building the display name</param>
		public void SetDisplayName(Func<IValidationContext, string> factory) {
			if (factory == null) throw new ArgumentNullException(nameof(factory));
#pragma warning disable 618
			_displayNameSource = new BackwardsCompatibleStringSource<IValidationContext>(factory);
#pragma warning restore 618
		}

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
			PropertyName = ValidatorOptions.Global.PropertyNameResolver(containerType, member, expression);
#pragma warning disable 618
			_displayNameSource = new BackwardsCompatibleStringSource<IValidationContext>(context => ValidatorOptions.Global.DisplayNameResolver(containerType, member, expression));
#pragma warning restore 618
		}

		/// <summary>
		/// Creates a new property rule from a lambda expression.
		/// </summary>
		public static PropertyRule Create<T, TProperty>(Expression<Func<T, TProperty>> expression) {
			return Create(expression, () => ValidatorOptions.Global.CascadeMode);
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
		public List<IValidationRule> DependentRules { get; }

		public Func<object, object> Transformer { get; set; }

		/// <summary>
		/// Display name for the property.
		/// </summary>
		[Obsolete("Calling GetDisplayName without a context parameter is deprecated and will be removed in FluentValidation 10. If you really need this behaviour, you can call the overload that takes a context but pass in null.")]
		public string GetDisplayName() {
			return GetDisplayName(null);
		}

		/// <summary>
		/// Display name for the property.
		/// </summary>
		public string GetDisplayName(ICommonContext context) {
			//TODO: For FV10, change the parameter from ICommonContext to IValidationContext.
			string result = null;

			if (_displayNameSource != null) {
				result = _displayNameSource.GetString(context);
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
		public virtual IEnumerable<ValidationFailure> Validate(IValidationContext context) {
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
			var accessor = new Lazy<object>(() => GetPropertyValue(context.InstanceToValidate), LazyThreadSafetyMode.None);

			// Invoke each validator and collect its results.
			foreach (var validator in _validators) {
				// TODO: For FV 10 don't store the accessor in the context. Instead add it as an argument to InvokePropertyValidator
				// Do not do this in 9.x as it'd be a breaking change.
				// This must be done *inside* the foreach loop to ensure it's reset for each iteration of the loop.
				// child validators will have replaced it, so ensure it's reset for each iteration.
				context.RootContextData["__FV_CurrentAccessor"] = accessor;

				IEnumerable<ValidationFailure> results;
				if (validator.ShouldValidateAsynchronously(context))
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
#pragma warning disable 618
				if (hasFailure && (cascade == CascadeMode.StopOnFirstFailure || cascade == CascadeMode.Stop)) {
#pragma warning restore 618
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
		public virtual async Task<IEnumerable<ValidationFailure>> ValidateAsync(IValidationContext context, CancellationToken cancellation) {
			if (!context.IsAsync()) {
				context.RootContextData["__FV_IsAsyncExecution"] = true;
			}

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
				return Enumerable.Empty<ValidationFailure>();
			}

			if (_condition != null) {
				if (!_condition(context)) {
					return Enumerable.Empty<ValidationFailure>();
				}
			}

			if (_asyncCondition != null) {
				if (! await _asyncCondition(context, cancellation)) {
					return Enumerable.Empty<ValidationFailure>();
				}
			}

			var cascade = _cascadeModeThunk();
			var failures = new List<ValidationFailure>();
			var accessor = new Lazy<object>(() => GetPropertyValue(context.InstanceToValidate), LazyThreadSafetyMode.None);

			// Invoke each validator and collect its results.
			foreach (var validator in _validators) {
				cancellation.ThrowIfCancellationRequested();

				// TODO: For FV 10 don't store the accessor in the context. Instead add it as an argument to InvokePropertyValidator
				// Do not do this in 9.x as it'd be a breaking change.
				// This must be done *inside* the foreach loop to ensure it's reset for each iteration of the loop.
				// child validators will have replaced it, so ensure it's reset for each iteration.
				context.RootContextData["__FV_CurrentAccessor"] = accessor;

				IEnumerable<ValidationFailure> results;
				if (validator.ShouldValidateAsynchronously(context))
					results = await InvokePropertyValidatorAsync(context, validator, propertyName, cancellation);
				else
					results = InvokePropertyValidator(context, validator, propertyName);

				bool hasFailure = false;

				foreach (var result in results) {
					failures.Add(result);
					hasFailure = true;
				}

				// If there has been at least one failure, and our CascadeMode has been set to StopOnFirst
				// then don't continue to the next rule
#pragma warning disable 618
				if (hasFailure && (cascade == CascadeMode.StopOnFirstFailure || cascade == CascadeMode.Stop)) {
#pragma warning restore 618
					break;
				}
			}

			if (failures.Count > 0) {
				// Callback if there has been at least one property validator failed.
				OnFailure?.Invoke(context.InstanceToValidate, failures);
			}
			else {
				failures.AddRange(await RunDependentRulesAsync(context, cancellation));
			}

			return failures;
		}

		private async Task<IEnumerable<ValidationFailure>> RunDependentRulesAsync(IValidationContext context, CancellationToken cancellation) {
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
		protected virtual async Task<IEnumerable<ValidationFailure>> InvokePropertyValidatorAsync(IValidationContext context, IPropertyValidator validator, string propertyName, CancellationToken cancellation) {
			// TODO: For FV10 accept the accessor as a parameter. Don't change in 9.x as this is a breaking change.
			PropertyValidatorContext propertyContext;
			if (context.RootContextData.TryGetValue("__FV_CurrentAccessor", out var a) && a is Lazy<object> accessor) {
				propertyContext = new PropertyValidatorContext(context, this, propertyName, accessor);
			}
			else {
#pragma warning disable 618
				propertyContext = new PropertyValidatorContext(context, this, propertyName);
#pragma warning restore 618
			}
			if (!validator.Options.InvokeCondition(propertyContext)) return Enumerable.Empty<ValidationFailure>();
			if (!await validator.Options.InvokeAsyncCondition(propertyContext, cancellation)) return Enumerable.Empty<ValidationFailure>();
			return await validator.ValidateAsync(propertyContext, cancellation);
		}

		/// <summary>
		/// Invokes a property validator using the specified validation context.
		/// </summary>
		protected virtual IEnumerable<ValidationFailure> InvokePropertyValidator(IValidationContext context, IPropertyValidator validator, string propertyName) {
			// TODO: For FV10 accept the accessor as a parameter. Don't change in 9.x as this is a breaking change.
			PropertyValidatorContext propertyContext;
			if (context.RootContextData.TryGetValue("__FV_CurrentAccessor", out var a) && a is Lazy<object> accessor) {
				propertyContext = new PropertyValidatorContext(context, this, propertyName, accessor);
			}
			else {
#pragma warning disable 618
				propertyContext = new PropertyValidatorContext(context, this, propertyName);
#pragma warning restore 618
			}
			if (!validator.Options.InvokeCondition(propertyContext)) return Enumerable.Empty<ValidationFailure>();
			return validator.Validate(propertyContext);
		}

		/// <summary>
		/// Gets the property value, including any transformations that need to be applied.
		/// </summary>
		/// <param name="instanceToValidate">The parent object</param>
		/// <returns>The value to be validated</returns>
		internal virtual object GetPropertyValue(object instanceToValidate) {
			var value = PropertyFunc(instanceToValidate);
			if (Transformer != null) value = Transformer(value);
			return value;
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

		public void ApplySharedCondition(Func<IValidationContext, bool> condition) {
			if (_condition == null) {
				_condition = condition;
			}
			else {
				var original = _condition;
				_condition = ctx => condition(ctx) && original(ctx);
			}
		}

		public void ApplySharedAsyncCondition(Func<IValidationContext, CancellationToken, Task<bool>> condition) {
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
