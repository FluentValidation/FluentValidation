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
	using System.Globalization;
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
		readonly List<IPropertyValidator> validators = new List<IPropertyValidator>();
		Func<CascadeMode> cascadeModeThunk = () => ValidatorOptions.CascadeMode;
		string propertyDisplayName;
		string propertyName;

		/// <summary>
		/// Property associated with this rule.
		/// </summary>
		public MemberInfo Member { get; }

		/// <summary>
		/// Function that can be invoked to retrieve the value of the property.
		/// </summary>
		public Func<object, object> PropertyFunc { get; private set; }

		/// <summary>
		/// Expression that was used to create the rule.
		/// </summary>
		public LambdaExpression Expression { get; private set; }

		/// <summary>
		/// String source that can be used to retrieve the display name (if null, falls back to the property name)
		/// </summary>
		public IStringSource DisplayName { get; set; }

		/// <summary>
		/// Rule set that this rule belongs to (if specified)
		/// </summary>
		public string RuleSet { get; set; }

		/// <summary>
		/// Function that will be invoked if any of the validators associated with this rule fail.
		/// </summary>
		public Action<object> OnFailure { get; set; }

		/// <summary>
		/// The current validator being configured by this rule.
		/// </summary>
		public IPropertyValidator CurrentValidator { get; private set; }

		/// <summary>
		/// Type of the property being validated
		/// </summary>
		public Type TypeToValidate { get; private set; }

		/// <summary>
		/// Cascade mode for this rule.
		/// </summary>
		public CascadeMode CascadeMode {
			get { return cascadeModeThunk(); }
			set { cascadeModeThunk = () => value; }
		}

		/// <summary>
		/// Validators associated with this rule.
		/// </summary>
		public IEnumerable<IPropertyValidator> Validators => validators;

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
			OnFailure = x => { };
			TypeToValidate = typeToValidate;
			this.cascadeModeThunk = cascadeModeThunk;
			
			DependentRules = new List<IValidationRule>();
			PropertyName = ValidatorOptions.PropertyNameResolver(containerType, member, expression);
			DisplayName = new LazyStringSource(x => ValidatorOptions.DisplayNameResolver(containerType, member, expression));
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
		public static PropertyRule Create<T, TProperty>(Expression<Func<T, TProperty>> expression, Func<CascadeMode> cascadeModeThunk) {
			var member = expression.GetMember();
			// We can't use the expression tree as a key in the cache, as it doesn't implement GetHashCode/Equals in a useful way.
			// Instead we'll use the MemberInfo as the key, but this only works for member expressions.
			// If this is not a member expression (eg, a RuleFor(x => x) or a RuleFor(x => x.Foo())) then we won't cache the result. 
			// We could probably make the cache more robust in future. 
			var compiled = member == null || ValidatorOptions.DisableAccessorCache ? expression.Compile() : AccessorCache<T>.GetCachedAccessor(member, expression);

			return new PropertyRule(member, compiled.CoerceToNonGeneric(), expression, cascadeModeThunk, typeof(TProperty), typeof(T));
		}

		/// <summary>
		/// Adds a validator to the rule.
		/// </summary>
		public void AddValidator(IPropertyValidator validator) {
			CurrentValidator = validator;
			validators.Add(validator);
		}

		/// <summary>
		/// Replaces a validator in this rule. Used to wrap validators.
		/// </summary>
		public void ReplaceValidator(IPropertyValidator original, IPropertyValidator newValidator) {
			var index = validators.IndexOf(original);

			if (index > -1) {
				validators[index] = newValidator;

				if (ReferenceEquals(CurrentValidator, original)) {
					CurrentValidator = newValidator;
				}
			}
		}

		/// <summary>
		/// Remove a validator in this rule.
		/// </summary>
		public void RemoveValidator(IPropertyValidator original) {
			if (ReferenceEquals(CurrentValidator, original)) {
				CurrentValidator = validators.LastOrDefault(x => x != original);
			}

			validators.Remove(original);
		}

		/// <summary>
		/// Clear all validators from this rule.
		/// </summary>
		public void ClearValidators() {
			CurrentValidator = null;
			validators.Clear();
		}

		/// <summary>
		/// Returns the property name for the property being validated.
		/// Returns null if it is not a property being validated (eg a method call)
		/// </summary>
		public string PropertyName {
			get { return propertyName; }
			set {
				propertyName = value;
				propertyDisplayName = propertyName.SplitPascalCase();
			}
		}

		/// <summary>
		/// Allows custom creation of an error message
		/// </summary>
		public Func<PropertyValidatorContext, string> MessageBuilder { get; set; }

		/// <summary>
		/// Dependent rules
		/// </summary>
		public List<IValidationRule> DependentRules { get; private set; }

		/// <summary>
		/// Display name for the property. 
		/// </summary>
		public string GetDisplayName() {
			string result = null;

			if (DisplayName != null) {
				result = DisplayName.GetString(null /*We don't have a model object at this point*/);
			}

			if (result == null) {
				result = propertyDisplayName;
			}

			return result;
		}

		/// <summary>
		/// Display name for the property. 
		/// </summary>
		public string GetDisplayName(object model) {
			string result = null;

			if (DisplayName != null) {
				result = DisplayName.GetString(model);
			}

			if (result == null) {
				result = propertyDisplayName;
			}

			return result;
		}

		/// <summary>
		/// Performs validation using a validation context and returns a collection of Validation Failures.
		/// </summary>
		/// <param name="context">Validation Context</param>
		/// <returns>A collection of validation failures</returns>
		public virtual IEnumerable<ValidationFailure> Validate(ValidationContext context) {
			string displayName = GetDisplayName(context.InstanceToValidate);

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

			var cascade = cascadeModeThunk();
			bool hasAnyFailure = false;

			// Invoke each validator and collect its results.
			foreach (var validator in validators) {
				var results = InvokePropertyValidator(context, validator, propertyName);

				bool hasFailure = false;

				foreach (var result in results) {
					hasAnyFailure = true;
					hasFailure = true;
					yield return result;
				}

				// If there has been at least one failure, and our CascadeMode has been set to StopOnFirst
				// then don't continue to the next rule
				if (cascade == FluentValidation.CascadeMode.StopOnFirstFailure && hasFailure) {
					break;
				}
			}

			if (hasAnyFailure) {
				// Callback if there has been at least one property validator failed.
				OnFailure(context.InstanceToValidate);
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
		public Task<IEnumerable<ValidationFailure>> ValidateAsync(ValidationContext context, CancellationToken cancellation) {
			try {
				var displayName = GetDisplayName(context.InstanceToValidate);

				if (PropertyName == null && displayName == null)
				{
					//No name has been specified. Assume this is a model-level rule, so we should use empty string instead. 
					displayName = string.Empty;
				}

				// Construct the full name of the property, taking into account overriden property names and the chain (if we're in a nested validator)
				var propertyName = context.PropertyChain.BuildPropertyName(PropertyName ?? displayName);

				// Ensure that this rule is allowed to run. 
				// The validatselector has the opportunity to veto this before any of the validators execute.
				if (!context.Selector.CanExecute(this, propertyName, context)) {
					return TaskHelpers.FromResult(Enumerable.Empty<ValidationFailure>());
				}

				var cascade = cascadeModeThunk();
				var failures = new List<ValidationFailure>();

				var fastExit = false;

				// Firstly, invoke all syncronous validators and collect their results.
				foreach (var validator in validators.Where(v => !v.IsAsync)) {

					if (cancellation.IsCancellationRequested) {
						return TaskHelpers.Canceled<IEnumerable<ValidationFailure>>();
					}

					var results = InvokePropertyValidator(context, validator, propertyName);

					failures.AddRange(results);

					// If there has been at least one failure, and our CascadeMode has been set to StopOnFirst
					// then don't continue to the next rule
					if (fastExit = (cascade == CascadeMode.StopOnFirstFailure && failures.Count > 0)) {
						break;
					}
				}

				//if StopOnFirstFailure triggered then we exit
				if (fastExit && failures.Count > 0) {
					// Callback if there has been at least one property validator failed.
					OnFailure(context.InstanceToValidate);

					return TaskHelpers.FromResult(failures.AsEnumerable());
				}

				var asyncValidators = validators.Where(v => v.IsAsync).ToList();
                
				// if there's no async validators then we exit
				if (asyncValidators.Count == 0) {
					if (failures.Count > 0) {
						// Callback if there has been at least one property validator failed.
						OnFailure(context.InstanceToValidate);
					}
					else
					{
						return RunDependentRulesAsync(failures, context, cancellation)
							.Then(() => failures.AsEnumerable(), cancellationToken: cancellation);
					}

					return TaskHelpers.FromResult(failures.AsEnumerable());
				}

				//Then call asyncronous validators in non-blocking way
				var validations =
					asyncValidators
//						.Select(v => v.ValidateAsync(new PropertyValidatorContext(context, this, propertyName), cancellation)
						.Select(v => InvokePropertyValidatorAsync(context, v, propertyName, cancellation)
							//this is thread safe because tasks are launched sequencially
							.Then(fs => failures.AddRange(fs), runSynchronously: true)
						);

				return
					TaskHelpers.Iterate(
						validations,
						breakCondition: _ => cascade == CascadeMode.StopOnFirstFailure && failures.Count > 0,
						cancellationToken: cancellation
					).Then(async () => {
						if (failures.Count > 0) {
							OnFailure(context.InstanceToValidate);
						}
						else
						{
							await RunDependentRulesAsync(failures, context, cancellation);
						}

						return failures.AsEnumerable();
					},
						runSynchronously: true
					);
			}
			catch (Exception ex) {
				return TaskHelpers.FromError<IEnumerable<ValidationFailure>>(ex);
			}
		}

		private Task RunDependentRulesAsync(List<ValidationFailure> failures, ValidationContext context, CancellationToken cancellation) {
			var validations = DependentRules.Select(v => v.ValidateAsync(context, cancellation).Then(fs => failures.AddRange(fs), runSynchronously: true));
			return TaskHelpers.Iterate(validations, cancellationToken: cancellation);
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
		public void ApplyCondition(Func<object, bool> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators) {
			// Default behaviour for When/Unless as of v1.3 is to apply the condition to all previous validators in the chain.
			if (applyConditionTo == ApplyConditionTo.AllValidators) {
				foreach (var validator in Validators.ToList()) {
					var wrappedValidator = new DelegatingValidator(predicate, validator);
					ReplaceValidator(validator, wrappedValidator);
				}

				foreach (var dependentRule in DependentRules.ToList()) {
					dependentRule.ApplyCondition(predicate, applyConditionTo);
				}
			}
			else {
				var wrappedValidator = new DelegatingValidator(predicate, CurrentValidator);
				ReplaceValidator(CurrentValidator, wrappedValidator);
			}


		}

		/// <summary>
		/// Applies the condition to the rule asynchronously
		/// </summary>
		/// <param name="predicate"></param>
		/// <param name="applyConditionTo"></param>
		public void ApplyAsyncCondition(Func<object, CancellationToken, Task<bool>> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators) {
			// Default behaviour for When/Unless as of v1.3 is to apply the condition to all previous validators in the chain.
			if (applyConditionTo == ApplyConditionTo.AllValidators) {
				foreach (var validator in Validators.ToList()) {
					var wrappedValidator = new DelegatingValidator(predicate, validator);
					ReplaceValidator(validator, wrappedValidator);
				}

				foreach (var dependentRule in DependentRules.ToList()) {
					dependentRule.ApplyAsyncCondition(predicate, applyConditionTo);
				}
			}
			else {
				var wrappedValidator = new DelegatingValidator(predicate, CurrentValidator);
				ReplaceValidator(CurrentValidator, wrappedValidator);
			}
		}
	}

	/// <summary>
	/// Include rule
	/// </summary>
	public class IncludeRule : PropertyRule {
		/// <summary>
		/// Creates a new IncludeRule
		/// </summary>
		/// <param name="validator"></param>
		/// <param name="cascadeModeThunk"></param>
		/// <param name="typeToValidate"></param>
		/// <param name="containerType"></param>
		public IncludeRule(IValidator validator,  Func<CascadeMode> cascadeModeThunk, Type typeToValidate, Type containerType) : base(null, x => x, null, cascadeModeThunk, typeToValidate, containerType) {
			AddValidator(new ChildValidatorAdaptor(validator));
		}

		/// <summary>
		/// Creates a new include rule from an existing validator
		/// </summary>
		/// <param name="validator"></param>
		/// <param name="cascadeModeThunk"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IncludeRule Create<T>(IValidator validator, Func<CascadeMode> cascadeModeThunk) {
			return new IncludeRule(validator, cascadeModeThunk, typeof(T), typeof(T));
		}
	}
}