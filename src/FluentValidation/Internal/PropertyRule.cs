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
// The latest version of this file can be found at http://www.codeplex.com/FluentValidation
#endregion

namespace FluentValidation.Internal {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
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
		public MemberInfo Member { get; private set; }

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
		public IEnumerable<IPropertyValidator> Validators {
			get { return validators; }
		}

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

			PropertyName = ValidatorOptions.PropertyNameResolver(containerType, member, expression);
			DisplayName = new LazyStringSource(() => ValidatorOptions.DisplayNameResolver(containerType, member, expression));
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
			var compiled = expression.Compile();

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
		/// Display name for the property. 
		/// </summary>
		public string GetDisplayName() {
			string result = null;

			if (DisplayName != null) {
				result = DisplayName.GetString();
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
			string displayName = GetDisplayName();

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
		}

		/// <summary>
		/// Performs asynchronous validation using a validation context and returns a collection of Validation Failures.
		/// </summary>
		/// <param name="context">Validation Context</param>
		/// <returns>A collection of validation failures</returns>
		public Task<IEnumerable<ValidationFailure>> ValidateAsync(ValidationContext context) {
			try {
				var displayName = GetDisplayName();

				if (PropertyName == null && displayName == null) {
					return
						TaskHelpers.FromError<IEnumerable<ValidationFailure>>(
							new InvalidOperationException(
								string.Format(
									"Property name could not be automatically determined for expression {0}. Please specify either a custom property name by calling 'WithName'.",
									Expression)));
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
					var results = InvokePropertyValidator(context, validator, propertyName);

					failures.AddRange(results);

					// If there has been at least one failure, and our CascadeMode has been set to StopOnFirst
					// then don't continue to the next rule
					if (fastExit = (cascade == CascadeMode.StopOnFirstFailure && failures.Count > 0)) {
						break;
					}
				}

				var asyncValidators = validators.Where(v => v.IsAsync).ToList();

				//if there's no async validators or StopOnFirstFailure triggered then we exit
				if (asyncValidators.Count == 0 || fastExit) {
					if (failures.Count > 0) {
						// Callback if there has been at least one property validator failed.
						OnFailure(context.InstanceToValidate);
					}

					return TaskHelpers.FromResult(failures.AsEnumerable());
				}

				//Then call asyncronous validators in non-blocking way
				var validations =
					asyncValidators
						.Select(v => v
							.ValidateAsync(new PropertyValidatorContext(context, this, propertyName))
							//this is thread safe because tasks are launched sequencially
							.Then(fs => failures.AddRange(fs), runSynchronously: true)
						);

				return
					TaskHelpers.Iterate(
						validations,
						breakCondition: _ => cascade == CascadeMode.StopOnFirstFailure && failures.Count > 0
					).Then(() => {
						if (failures.Count > 0) {
							OnFailure(context.InstanceToValidate);
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

		/// <summary>
		/// Invokes a property validator using the specified validation context.
		/// </summary>
		protected virtual IEnumerable<ValidationFailure> InvokePropertyValidator(ValidationContext context, IPropertyValidator validator, string propertyName) {
			var propertyContext = new PropertyValidatorContext(context, this, propertyName);
			return validator.Validate(propertyContext);
		}

		public void ApplyCondition(Func<object, bool> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators) {
			// Default behaviour for When/Unless as of v1.3 is to apply the condition to all previous validators in the chain.
			if (applyConditionTo == ApplyConditionTo.AllValidators) {
				foreach (var validator in Validators.ToList()) {
					var wrappedValidator = new DelegatingValidator(predicate, validator);
					ReplaceValidator(validator, wrappedValidator);
				}
			}
			else {
				var wrappedValidator = new DelegatingValidator(predicate, CurrentValidator);
				ReplaceValidator(CurrentValidator, wrappedValidator);
			}
		}
	}
}