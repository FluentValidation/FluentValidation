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
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading;
	using System.Threading.Tasks;
	using Internal;
	using Resources;
	using Validators;

	/// <summary>
	/// Default options that can be used to configure a validator.
	/// </summary>
	public static class DefaultValidatorOptions {

		/// <summary>
		/// Specifies the cascade mode for failures. 
		/// If set to 'Stop' then execution of the rule will stop once the first validator in the chain fails.
		/// If set to 'Continue' then all validators in the chain will execute regardless of failures.
		/// </summary>
		public static IRuleBuilderInitial<T, TProperty> Cascade<T, TProperty>(this IRuleBuilderInitial<T, TProperty> ruleBuilder, CascadeMode cascadeMode) {
			return ruleBuilder.Configure(cfg => {
				cfg.CascadeMode = cascadeMode;
			});
		}

		/// <summary>
		/// Transforms the property value before validation occurs. The transformed value must be of the same type as the input value. 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="transformationFunc"></param>
		/// <returns></returns>
		public static IRuleBuilderInitial<T, TProperty> Transform<T, TProperty>(this IRuleBuilderInitial<T, TProperty> ruleBuilder, Func<TProperty, TProperty> transformationFunc) {
			return ruleBuilder.Configure(cfg => {
				cfg.Transformer = transformationFunc.CoerceToNonGeneric();
			});
		}

		/// <summary>
		/// Specifies a custom action to be invoked when the validator fails. 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="rule"></param>
		/// <param name="onFailure"></param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> OnAnyFailure<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Action<T> onFailure) {
			return rule.Configure(config => {
				config.OnFailure = onFailure.CoerceToNonGeneric();
			});
		}

		/// <summary>
		/// Specifies a custom error message to use if validation fails.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="errorMessage">The error message to use</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> WithMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, string errorMessage) {
			errorMessage.Guard("A message must be specified when calling WithMessage.", nameof(errorMessage));
			return rule.Configure(config => {
				config.CurrentValidator.GetMetadata().ErrorMessageSource = new StaticStringSource(errorMessage);
			});
		}

		/// <summary>
		/// Specifies a custom error message to use when validation fails.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="messageProvider">Delegate that will be invoked to retrieve the localized message. </param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> WithMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<T, string> messageProvider) {
			messageProvider.Guard("A messageProvider must be provided.", nameof(messageProvider));
			return rule.Configure(config => {
				config.CurrentValidator.GetMetadata().ErrorMessageSource = new LazyStringSource(ctx => messageProvider((T)ctx.InstanceToValidate));
			});
		}

		/// <summary>
		/// Specifies a custom error message to use when validation fails.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="messageProvider">Delegate that will be invoked.Uses_localized_name to retrieve the localized message. </param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> WithMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<T, TProperty, string> messageProvider) {
			messageProvider.Guard("A messageProvider must be provided.", nameof(messageProvider));

			return rule.Configure(config => {
				config.CurrentValidator.GetMetadata().ErrorMessageSource 
					= new LazyStringSource(context => messageProvider((T)context.InstanceToValidate, (TProperty)context.PropertyValue));
			});
		}

		/// <summary>
		/// Specifies a custom error code to use if validation fails.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="errorCode">The error code to use</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> WithErrorCode<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, string errorCode) {
			errorCode.Guard("A error code must be specified when calling WithErrorCode.", nameof(errorCode));

			return rule.Configure(config => {
				config.CurrentValidator.GetMetadata().ErrorCodeSource = new StaticStringSource(errorCode);
			});
		}

		/// <summary>
		/// Specifies a custom error message resource to use when validation fails.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="resourceType">Resource type representing a resx file</param>
		/// <param name="resourceName">Name of resource</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> WithLocalizedMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Type resourceType, string resourceName) {
			resourceType.Guard("A resource type must be provided.", nameof(resourceType));
			resourceName.Guard("A resource name must be provided.", nameof(resourceName));

			return rule.Configure(config => {
				config.CurrentValidator.GetMetadata().ErrorMessageSource = new LocalizedStringSource(resourceType, resourceName);
			});
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
			predicate.Guard("A predicate must be specified when calling When.", nameof(predicate));
			// Default behaviour for When/Unless as of v1.3 is to apply the condition to all previous validators in the chain.
			return rule.Configure(config => {
				config.ApplyCondition(ctx => predicate((T)ctx.InstanceToValidate), applyConditionTo);
			});
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
			predicate.Guard("A predicate must be specified when calling Unless", nameof(predicate));
			return rule.When(x => !predicate(x), applyConditionTo);
		}


		/// <summary>
		/// Specifies an asynchronous condition limiting when the validator should run. 
		/// The validator will only be executed if the result of the lambda returns true.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="predicate">A lambda expression that specifies a condition for when the validator should run</param>
		/// <param name="applyConditionTo">Whether the condition should be applied to the current rule or all rules in the chain</param>
		/// <returns></returns>
		[Obsolete("Use the overload of WhenAsync that takes a CancellationToken")]
		public static IRuleBuilderOptions<T, TProperty> WhenAsync<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<T, Task<bool>> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators)
		{
			predicate.Guard("A predicate must be specified when calling WhenAsync.", nameof(predicate));

			var newPredicate = new Func<ValidationContext, CancellationToken, Task<bool>>((ctx, ct) => predicate((T) ctx.InstanceToValidate));

			return rule.Configure(config => {
				config.ApplyAsyncCondition(newPredicate, applyConditionTo);
			});
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
			predicate.Guard("A predicate must be specified when calling WhenAsync.", nameof(predicate));
			// Default behaviour for When/Unless as of v1.3 is to apply the condition to all previous validators in the chain.
			return rule.Configure(config => {
				config.ApplyAsyncCondition((ctx, ct) => predicate((T)ctx.InstanceToValidate, ct), applyConditionTo);
			});
		}

		/// <summary>
		/// Specifies an asynchronous condition limiting when the validator should not run. 
		/// The validator will only be executed if the result of the lambda returns false.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="predicate">A lambda expression that specifies a condition for when the validator should not run</param>
		/// <param name="applyConditionTo">Whether the condition should be applied to the current rule or all rules in the chain</param>
		/// <returns></returns>
		[Obsolete("Use the overload of UnlessAsync that takes a CancellationToken")]
		public static IRuleBuilderOptions<T, TProperty> UnlessAsync<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<T, Task<bool>> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators) {
			predicate.Guard("A predicate must be specified when calling UnlessAsync", nameof(predicate));
			return rule.WhenAsync(x => predicate(x).Then(y => !y), applyConditionTo);
		}

		/// <summary>
		/// Specifies an asynchronous condition limiting when the validator should not run. 
		/// The validator will only be executed if the result of the lambda returns false.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="predicate">A lambda expression that specifies a condition for when the validator should not run</param>
		/// <param name="applyConditionTo">Whether the condition should be applied to the current rule or all rules in the chain</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> UnlessAsync<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<T, CancellationToken, Task<bool>> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators)
		{
			predicate.Guard("A predicate must be specified when calling UnlessAsync", nameof(predicate));
			return rule.WhenAsync((x, ct) => predicate(x, ct).Then(y => !y, ct), applyConditionTo);
		}

		/// <summary>
		/// Triggers an action when the rule passes. Typically used to configure dependent rules. This applies to all preceding rules in the chain. 
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="action">An action to be invoked if the rule is valid</param>
		/// <returns></returns>
		[Obsolete("You no longer need to call RuleFor on the DependentRulesBuilder. Use the other overload of DependentRules that takes an Action and just call the root RuleFor method instead.")]
		public static IRuleBuilderOptions<T, TProperty> DependentRules<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Action<InlineValidator<T>>  action) {

			var dependencyContainer = new InlineValidator<T>();

			if (rule is IExposesParentValidator<T> exposesParentValidator) {
				if (exposesParentValidator.ParentValidator is AbstractValidator<T> parent) {
					// Capture any rules added to the parent validator inside this delegate. 
					using (parent.NestedValidators.Capture(dependencyContainer.AddRule)) {
						action(dependencyContainer);
					}
				}
			}
			else {
				action(dependencyContainer);
			}


			rule.Configure(cfg => {

				if (cfg.RuleSets.Length > 0) {
					foreach (var dependentRule in dependencyContainer) {
						if (dependentRule is PropertyRule propRule && propRule.RuleSets.Length == 0) {
							propRule.RuleSets = cfg.RuleSets;
						}
					}
				}

				cfg.DependentRules.AddRange(dependencyContainer);
			});
			return rule;
		}


		/// <summary>
		/// Triggers an action when the rule passes. Typically used to configure dependent rules. This applies to all preceding rules in the chain. 
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="action">An action to be invoked if the rule is valid</param>
		/// <returns></returns>

		public static IRuleBuilderOptions<T, TProperty> DependentRules<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Action action) {

			var dependencyContainer = new List<IValidationRule>();

			if (rule is IExposesParentValidator<T> exposesParentValidator) {
				if (exposesParentValidator.ParentValidator is AbstractValidator<T> parent) {
					// Capture any rules added to the parent validator inside this delegate. 
					using (parent.NestedValidators.Capture(dependencyContainer.Add)) {
						action();
					}
				}
			}
			else {
				throw new NotSupportedException("DependentRules can only be called as part of classes that inherit from AbstractValidator");
			}

			rule.Configure(cfg => {

				if (cfg.RuleSets.Length > 0) {
					foreach (var dependentRule in dependencyContainer) {
						if (dependentRule is PropertyRule propRule && propRule.RuleSets.Length == 0) {
							propRule.RuleSets = cfg.RuleSets;
						}
					}
				}

				cfg.DependentRules.AddRange(dependencyContainer);
			});
			return rule;
		}


		/// <summary>
		/// Specifies a custom property name to use within the error message.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="overridePropertyName">The property name to use</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> WithName<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, string overridePropertyName) {
			overridePropertyName.Guard("A property name must be specified when calling WithName.", nameof(overridePropertyName));
			return rule.Configure(config => {
				config.DisplayName = new StaticStringSource(overridePropertyName);	
			});
		}

		/// <summary>
		/// Specifies a custom property name to use within the error message.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="nameProvider">Func used to retrieve the property's display name</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> WithName<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<T, string> nameProvider) {
			nameProvider.Guard("A nameProvider WithName.", nameof(nameProvider));
			return rule.Configure(config => {
				// Must use null propogation here.
				// The MVC clientside validation will try and retrieve the name, but won't 
				// be able to to so if we've used this overload of withname.
				config.DisplayName = new LazyStringSource(ctx => nameProvider((T)ctx?.InstanceToValidate));
			});
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
			return rule.Configure(config => config.PropertyName = propertyName);
		}

		/// <summary>
		/// Overrides the name of the property associated with this rule.
		/// NOTE: This is a considered to be an advanced feature. Most of the time that you use this, you actually meant to use WithName.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="expr">An expression referencing another property</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> OverridePropertyName<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Expression<Func<T, TProperty>> expr) {
			if (expr == null) throw new ArgumentNullException(nameof(expr));
			var member = expr.GetMember(); // Need the FluentValidation.Internal namespace imported for this extension method to show up. 
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
			stateProvider.Guard("A lambda expression must be passed to WithState", nameof(stateProvider));
			var wrapper = new Func<PropertyValidatorContext, object>(ctx => stateProvider((T) ctx.Instance));
			return rule.Configure(config => config.CurrentValidator.GetMetadata().CustomStateProvider = wrapper);
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
			stateProvider.Guard("A lambda expression must be passed to WithState", nameof(stateProvider));

			var wrapper = new Func<PropertyValidatorContext, object>(ctx => {
				return stateProvider((T) ctx.Instance, (TProperty) ctx.PropertyValue);
			});

			return rule.Configure(config => config.CurrentValidator.GetMetadata().CustomStateProvider = wrapper);
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
			return rule.Configure(config => config.CurrentValidator.GetMetadata().Severity = severity);
		}

		/// <summary>
		/// Gets the default message for a property validato
		/// </summary>
		/// <typeparam name="T">The validator type</typeparam>
		/// <returns>The translated string</returns>
		public static string GetStringForValidator<T>(this ILanguageManager languageManager) {
			return languageManager.GetString(typeof(T).Name);
		}

		internal static Func<T, object>[] ConvertArrayOfObjectsToArrayOfDelegates<T>(object[] objects) {
			if(objects == null || objects.Length == 0) {
				return new Func<T, object>[0];
			}
			return objects.Select(obj => new Func<T, object>(x => obj)).ToArray();
		} 
	}
}