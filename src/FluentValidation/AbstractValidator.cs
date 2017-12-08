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

namespace FluentValidation {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Threading;
	using System.Threading.Tasks;
	using Internal;
	using Results;
	using Validators;

	/// <summary>
	/// Base class for entity validator classes.
	/// </summary>
	/// <typeparam name="T">The type of the object being validated</typeparam>
	public abstract class AbstractValidator<T> : IValidator<T>, IEnumerable<IValidationRule> {
		internal TrackingCollection<IValidationRule> NestedValidators { get; } = new TrackingCollection<IValidationRule>();

		// Work-around for reflection bug in .NET 4.5
		static Func<CascadeMode> s_cascadeMode = () => ValidatorOptions.CascadeMode;
		Func<CascadeMode> cascadeMode = s_cascadeMode;

		/// <summary>
		/// Sets the cascade mode for all rules within this validator.
		/// </summary>
		public CascadeMode CascadeMode {
			get => cascadeMode();
			set => cascadeMode = () => value;
		}

		ValidationResult IValidator.Validate(object instance) {
			instance.Guard("Cannot pass null to Validate.");
			if(! ((IValidator)this).CanValidateInstancesOfType(instance.GetType())) {
				throw new InvalidOperationException(string.Format("Cannot validate instances of type '{0}'. This validator can only validate instances of type '{1}'.", instance.GetType().Name, typeof(T).Name));
			}
			
			return Validate((T)instance);
		}

		Task<ValidationResult> IValidator.ValidateAsync(object instance, CancellationToken cancellation) {
			instance.Guard("Cannot pass null to Validate.");
			if (!((IValidator) this).CanValidateInstancesOfType(instance.GetType())) {
				throw new InvalidOperationException(string.Format("Cannot validate instances of type '{0}'. This validator can only validate instances of type '{1}'.", instance.GetType().Name, typeof (T).Name));
			}

			return ValidateAsync((T) instance, cancellation);
		}

		ValidationResult IValidator.Validate(ValidationContext context) {
			context.Guard("Cannot pass null to Validate");

			var newContext = new ValidationContext<T>((T)context.InstanceToValidate, context.PropertyChain, context.Selector) {
				IsChildContext = context.IsChildContext,
				RootContextData = context.RootContextData
			};

			return Validate(newContext);
		}

		Task<ValidationResult> IValidator.ValidateAsync(ValidationContext context, CancellationToken cancellation) {
			context.Guard("Cannot pass null to Validate");

			var newContext = new ValidationContext<T>((T) context.InstanceToValidate, context.PropertyChain, context.Selector) {
				IsChildContext = context.IsChildContext,
				RootContextData = context.RootContextData
			};

			return ValidateAsync(newContext, cancellation);
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
			context.Guard("Cannot pass null to Validate.");
			EnsureInstanceNotNull(context.InstanceToValidate);
			var failures = NestedValidators.SelectMany(x => x.Validate(context));
			return new ValidationResult(failures);
		}

		/// <summary>
		/// Validates the specified instance asynchronously.
		/// </summary>
		/// <param name="context">Validation Context</param>
		/// <param name="cancellation">Cancellation token</param>
		/// <returns>A ValidationResult object containing any validation failures.</returns>
		public virtual Task<ValidationResult> ValidateAsync(ValidationContext<T> context, CancellationToken cancellation = new CancellationToken()) {
			context.Guard("Cannot pass null to Validate");
			EnsureInstanceNotNull(context.InstanceToValidate);

			var failures = new List<ValidationFailure>();
			
			return TaskHelpers.Iterate(
				NestedValidators
				.Select(v => v.ValidateAsync(context, cancellation).Then(fs => failures.AddRange(fs), runSynchronously: true))
			).Then(
				() => new ValidationResult(failures)
			);
		}

		/// <summary>
		/// Adds a rule to the current validator.
		/// </summary>
		/// <param name="rule"></param>
		public void AddRule(IValidationRule rule) {
			NestedValidators.Add(rule);
		}

		/// <summary>
		/// Creates a <see cref="IValidatorDescriptor" /> that can be used to obtain metadata about the current validator.
		/// </summary>
		public virtual IValidatorDescriptor CreateDescriptor() {
			return new ValidatorDescriptor<T>(NestedValidators);
		}

		bool IValidator.CanValidateInstancesOfType(Type type) {
			return typeof(T).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
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
			var ruleBuilder = new RuleBuilder<T, TProperty>(rule, this);
			return ruleBuilder;
		}

		/// <summary>
		/// Invokes a rule for each item in the collection
		/// </summary>
		/// <typeparam name="TProperty">Type of property</typeparam>
		/// <param name="expression">Expression representing the collection to validate</param>
		/// <returns>An IRuleBuilder instance on which validators can be defined</returns>
		public IRuleBuilderInitial<T, TProperty> RuleForEach<TProperty>(Expression<Func<T, IEnumerable<TProperty>>> expression) {
			expression.Guard("Cannot pass null to RuleForEach");
			var rule = CollectionPropertyRule<TProperty>.Create(expression, () => CascadeMode);
			AddRule(rule);
			var ruleBuilder = new RuleBuilder<T, TProperty>(rule, this);
			return ruleBuilder;
		} 

		/// <summary>
		/// Defines a custom validation rule using a lambda expression.
		/// If the validation rule fails, it should return a instance of a <see cref="ValidationFailure">ValidationFailure</see>
		/// If the validation rule succeeds, it should return null.
		/// </summary>
		/// <param name="customValidator">A lambda that executes custom validation rules.</param>
		[Obsolete("Use model-level RuleFor(x => x).Custom((x, context) => {}) instead")]
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
		[Obsolete("Use model-level RuleFor(x => x).Custom((x, context) => {}) instead")]
		public void Custom(Func<T, ValidationContext<T>, ValidationFailure> customValidator) {
			customValidator.Guard("Cannot pass null to Custom");
			AddRule(new DelegateValidator<T>((x, ctx) => new[] { customValidator(x, ctx) }));
		}

		/// <summary>
		/// Defines a custom asynchronous validation rule using a lambda expression.
		/// If the validation rule fails, it should asynchronously return a instance of a <see cref="ValidationFailure">ValidationFailure</see>
		/// If the validation rule succeeds, it should return null.
		/// </summary>
		/// <param name="customValidator">A lambda that executes custom validation rules.</param>
		[Obsolete("Use model-level RuleFor(x => x).CustomAsync(await (x,context,cancellation) => {}) instead")]
		public void CustomAsync(Func<T, Task<ValidationFailure>> customValidator) {
			customValidator.Guard("Cannot pass null to Custom");
			AddRule(new DelegateValidator<T>(x => customValidator(x).Then(f => new[] {f}.AsEnumerable(), runSynchronously: true)));
		}

		/// <summary>
		/// Defines a custom asynchronous validation rule using a lambda expression.
		/// If the validation rule fails, it should asynchronously return an instance of <see cref="ValidationFailure">ValidationFailure</see>
		/// If the validation rule succeeds, it should return null.
		/// </summary>
		/// <param name="customValidator">A lambda that executes custom validation rules</param>
		[Obsolete("Use model-level RuleFor(x => x).CustomAsync(await (x,context,cancellation) => {}) instead")]
		public void CustomAsync(Func<T, ValidationContext<T>, CancellationToken, Task<ValidationFailure>> customValidator) {
			customValidator.Guard("Cannot pass null to Custom");
			AddRule(new DelegateValidator<T>((x, ctx, cancel) => customValidator(x, ctx, cancel).Then(f => new[] {f}.AsEnumerable(), runSynchronously: true)));
		}

		/// <summary>
		/// Defines a RuleSet that can be used to group together several validators.
		/// </summary>
		/// <param name="ruleSetName">The name of the ruleset.</param>
		/// <param name="action">Action that encapsulates the rules in the ruleset.</param>
		public void RuleSet(string ruleSetName, Action action) {
			ruleSetName.Guard("A name must be specified when calling RuleSet.");
			action.Guard("A ruleset definition must be specified when calling RuleSet.");

			using (NestedValidators.OnItemAdded(r => r.RuleSet = ruleSetName)) {
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

			using(NestedValidators.OnItemAdded(onRuleAdded)) {
				action(); 
			}

			// Must apply the predicate after the rule has been fully created to ensure any rules-specific conditions have already been applied.
			propertyRules.ForEach(x => x.ApplyCondition(predicate.CoerceToNonGeneric()));
		}

		/// <summary>
		/// Defines an inverse condition that applies to several rules
		/// </summary>
		/// <param name="predicate">The condition that should be applied to multiple rules</param>
		/// <param name="action">Action that encapsulates the rules</param>
		public void Unless(Func<T, bool> predicate, Action action) {
			When(x => !predicate(x), action);
		}

		/// <summary>
		/// Defines an asynchronous condition that applies to several rules
		/// </summary>
		/// <param name="predicate">The asynchronous condition that should apply to multiple rules</param>
		/// <param name="action">Action that encapsulates the rules.</param>
		/// <returns></returns>
		[Obsolete("Use the overload of WhenAsync that takes a CancellationToken")]
		public void WhenAsync(Func<T, Task<bool>> predicate, Action action) {
			var newPredicate = new Func<T, CancellationToken, Task<bool>>((x, ct) => predicate(x));
			WhenAsync(newPredicate, action);
		}

		/// <summary>
		/// Defines an asynchronous condition that applies to several rules
		/// </summary>
		/// <param name="predicate">The asynchronous condition that should apply to multiple rules</param>
		/// <param name="action">Action that encapsulates the rules.</param>
		/// <returns></returns>
		public void WhenAsync(Func<T, CancellationToken, Task<bool>> predicate, Action action) {
			var propertyRules = new List<IValidationRule>();

			Action<IValidationRule> onRuleAdded = propertyRules.Add;

			using (NestedValidators.OnItemAdded(onRuleAdded)) {
				action();
			}

			// Must apply the predicate after the rule has been fully created to ensure any rules-specific conditions have already been applied.
			propertyRules.ForEach(x => x.ApplyAsyncCondition(predicate.CoerceToNonGeneric()));
		}

		/// <summary>
		/// Defines an inverse asynchronous condition that applies to several rules
		/// </summary>
		/// <param name="predicate">The asynchronous condition that should be applied to multiple rules</param>
		/// <param name="action">Action that encapsulates the rules</param>
		[Obsolete("Use the overload of UnlessAsync that takes a CancellationToken")]
		public void UnlessAsync(Func<T, Task<bool>> predicate, Action action) {
			WhenAsync(x => predicate(x).Then(y => !y), action);
		}

		/// <summary>
		/// Defines an inverse asynchronous condition that applies to several rules
		/// </summary>
		/// <param name="predicate">The asynchronous condition that should be applied to multiple rules</param>
		/// <param name="action">Action that encapsulates the rules</param>
		public void UnlessAsync(Func<T, CancellationToken, Task<bool>> predicate, Action action) {
			WhenAsync((x, ct) => predicate(x, ct).Then(y => !y, ct), action);
		}

		/// <summary>
		/// Includes the rules from the specified validator
		/// </summary>
		public void Include(IValidator<T> rulesToInclude) {
			rulesToInclude.Guard("Cannot pass null to Include");
			var rule = IncludeRule.Create<T>(rulesToInclude, () => CascadeMode);
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
			return NestedValidators.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		/// <summary>
		/// Throws an exception if the instance being validated is null.
		/// </summary>
		/// <param name="instanceToValidate"></param>
		protected virtual void EnsureInstanceNotNull(object instanceToValidate) {
			instanceToValidate.Guard("Cannot pass null model to Validate.");
		}
	}

	/// <summary>
	/// Container class for dependent rule definitions
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class	DependentRules<T> : AbstractValidator<T> {
	}
}
