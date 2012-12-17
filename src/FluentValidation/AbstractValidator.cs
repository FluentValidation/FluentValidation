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

namespace FluentValidation {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using Internal;
	using Results;
	using Validators;

	/// <summary>
	/// Base class for entity validator classes.
	/// </summary>
	/// <typeparam name="T">The type of the object being validated</typeparam>
	public abstract class AbstractValidator<T> : IValidator<T>, IEnumerable<IValidationRule> {
		readonly TrackingCollection<IValidationRule> nestedValidators = new TrackingCollection<IValidationRule>();

        // Work-around for reflection bug in .NET 4.5
        static Func<CascadeMode> s_cascadeMode = () => ValidatorOptions.CascadeMode;
        Func<CascadeMode> cascadeMode = s_cascadeMode;

		/// <summary>
		/// Sets the cascade mode for all rules within this validator.
		/// </summary>
		public CascadeMode CascadeMode {
			get { return cascadeMode(); }
			set { cascadeMode = () => value; }
		}

		ValidationResult IValidator.Validate(object instance) {
			instance.Guard("Cannot pass null to Validate.");
			if(! ((IValidator)this).CanValidateInstancesOfType(instance.GetType())) {
				throw new InvalidOperationException(string.Format("Cannot validate instances of type '{0}'. This validator can only validate instances of type '{1}'.", instance.GetType().Name, typeof(T).Name));
			}
			
			return Validate((T)instance);
		}

		ValidationResult IValidator.Validate(ValidationContext context) {
			context.Guard("Cannot pass null to Validate");

			var newContext = new ValidationContext<T>((T)context.InstanceToValidate, context.PropertyChain, context.Selector) {
				IsChildContext = context.IsChildContext
			};

			return Validate(newContext);
		}

		/// <summary>
		/// Validates the specified instance
		/// </summary>
		/// <param name="instance">The object to validate</param>
		/// <returns>A ValidationResult object containing any validation failures</returns>
		public virtual ValidationResult Validate(T instance) {
			return Validate(new ValidationContext<T>(instance, new PropertyChain(), new DefaultValidatorSelector()));
		}
		
		/// <summary>
		/// Validates the specified instance.
		/// </summary>
		/// <param name="context">Validation Context</param>
		/// <returns>A ValidationResult object containing any validation failures.</returns>
		public virtual ValidationResult Validate(ValidationContext<T> context) {
			context.Guard("Cannot pass null to Validate");
			var failures = nestedValidators.SelectMany(x => x.Validate(context)).ToList();
			return new ValidationResult(failures);
		}

		/// <summary>
		/// Adds a rule to the current validator.
		/// </summary>
		/// <param name="rule"></param>
		public void AddRule(IValidationRule rule) {
			nestedValidators.Add(rule);
		}

		/// <summary>
		/// Creates a <see cref="IValidatorDescriptor" /> that can be used to obtain metadata about the current validator.
		/// </summary>
		public virtual IValidatorDescriptor CreateDescriptor() {
			return new ValidatorDescriptor<T>(nestedValidators);
		}

		bool IValidator.CanValidateInstancesOfType(Type type) {
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
			expression.Guard("Cannot pass null to RuleFor");
			var rule = PropertyRule.Create(expression, () => CascadeMode);
			AddRule(rule);
			var ruleBuilder = new RuleBuilder<T, TProperty>(rule);
			return ruleBuilder;
		}

		public IRuleBuilderInitial<T, TProperty> RuleForEach<TProperty>(Expression<Func<T, IEnumerable<TProperty>>> expression) {
			expression.Guard("Cannot pass null to RuleForEach");
			var rule = CollectionPropertyRule<TProperty>.Create(expression, () => CascadeMode);
			AddRule(rule);
			var ruleBuilder = new RuleBuilder<T, TProperty>(rule);
			return ruleBuilder;
		} 

		/// <summary>
		/// Defines a custom validation rule using a lambda expression.
		/// If the validation rule fails, it should return a instance of a <see cref="ValidationFailure">ValidationFailure</see>
		/// If the validation rule succeeds, it should return null.
		/// </summary>
		/// <param name="customValidator">A lambda that executes custom validation rules.</param>
		public void Custom(Func<T, ValidationFailure> customValidator) {
			customValidator.Guard("Cannot pass null to Custom");
			AddRule(new DelegateValidator<T>(x => new[] { customValidator(x) }));
		}

		/// <summary>
		/// Defines a custom validation rule using a lambda expression.
		/// If the validation rule fails, it should return an instance of <see cref="ValidationFailure">ValidationFailure</see>
		/// If the validation rule succeeds, it should return null.
		/// </summary>
		/// <param name="customValidator">A lambda that executes custom validation rules</param>
		public void Custom(Func<T, ValidationContext<T>, ValidationFailure> customValidator) {
			customValidator.Guard("Cannot pass null to Custom");
			AddRule(new DelegateValidator<T>((x, ctx) => new[] { customValidator(x, ctx) }));
		}

		/// <summary>
		/// Defines a RuleSet that can be used to group together several validators.
		/// </summary>
		/// <param name="ruleSetName">The name of the ruleset.</param>
		/// <param name="action">Action that encapsulates the rules in the ruleset.</param>
		public void RuleSet(string ruleSetName, Action action) {
			ruleSetName.Guard("A name must be specified when calling RuleSet.");
			action.Guard("A ruleset definition must be specified when calling RuleSet.");

			using (nestedValidators.OnItemAdded(r => r.RuleSet = ruleSetName)) {
				action();
			}
		}

		/// <summary>
		/// Defines a condition that applies to several rules
		/// </summary>
		/// <param name="predicate">The condition that should apply to multiple rules</param>
		/// <param name="action">Action that encapsulates the rules.</param>
		/// <returns></returns>
		public void When(Func<T, bool> predicate, Action action) {
			var propertyRules = new List<IValidationRule>();

			Action<IValidationRule> onRuleAdded = propertyRules.Add;

			using(nestedValidators.OnItemAdded(onRuleAdded)) {
				action();
			}

			// Must apply the predictae after the rule has been fully created to ensure any rules-specific conditions have already been applied.
			propertyRules.ForEach(x => x.ApplyCondition(predicate.CoerceToNonGeneric()));
		}

		/// <summary>
		/// Defiles an inverse condition that applies to several rules
		/// </summary>
		/// <param name="predicate">The condition that should be applied to multiple rules</param>
		/// <param name="action">Action that encapsulates the rules</param>
		public void Unless(Func<T, bool> predicate, Action action) {
			When(x => !predicate(x), action);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection of validation rules.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public IEnumerator<IValidationRule> GetEnumerator() {
			return nestedValidators.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}