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
	using System.Linq;
	using System.Linq.Expressions;
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
		/// Specifies a custom action to be invoked when the validator fails. 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="rule"></param>
		/// <param name="onFailure"></param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> OnAnyFailure<T, TProperty>(this IRuleBuilderOptions<T,TProperty> rule, Action<T> onFailure) {
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
			return rule.WithMessage(errorMessage, null as object[]);
		}

		/// <summary>
		/// Specifies a custom error message to use if validation fails.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="errorMessage">The error message to use</param>
		/// <param name="formatArgs">Additional arguments to be specified when formatting the custom error message.</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> WithMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, string errorMessage, params object[] formatArgs) {
			var funcs = ConvertArrayOfObjectsToArrayOfDelegates<T>(formatArgs);
			return rule.WithMessage(errorMessage, funcs);
		}

		/// <summary>
		/// Specifies a custom error message to use if validation fails.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="errorMessage">The error message to use</param>
		/// <param name="funcs">Additional property values to be included when formatting the custom error message.</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> WithMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, string errorMessage, params Func<T, object>[] funcs) {
			errorMessage.Guard("A message must be specified when calling WithMessage.");

			return rule.Configure(config => {
				config.CurrentValidator.ErrorMessageSource = new StaticStringSource(errorMessage);

				funcs
					.Select(func => new Func<object, object, object>((instance, value) => func((T)instance)))
					.ForEach(config.CurrentValidator.CustomMessageFormatArguments.Add);
			});
		}

		/// <summary>
		/// Specifies a custom error message to use if validation fails.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="errorMessage">The error message to use</param>
		/// <param name="funcs">Additional property values to use when formatting the custom error message.</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> WithMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, string errorMessage, params Func<T, TProperty, object>[] funcs) {
			errorMessage.Guard("A message must be specified when calling WithMessage.");

			return rule.Configure(config => {
				config.CurrentValidator.ErrorMessageSource = new StaticStringSource(errorMessage);

				funcs
					.Select(func => new Func<object, object, object>((instance, value) => func((T)instance, (TProperty)value)))
					.ForEach(config.CurrentValidator.CustomMessageFormatArguments.Add);
			});
		} 

		/// <summary>
		/// Specifies a custom error message resource to use when validation fails.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="resourceSelector">The resource to use as an expression, eg () => Messages.MyResource</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T,TProperty> WithLocalizedMessage<T,TProperty>(this IRuleBuilderOptions<T,TProperty> rule, Expression<Func<string>> resourceSelector) {
			// We use the StaticResourceAccessorBuilder here because we don't want calls to WithLocalizedMessage to be overriden by the ResourceProviderType.
			return rule.WithLocalizedMessage(resourceSelector, new StaticResourceAccessorBuilder());
		}

		/// <summary>
		/// Specifies a custom error message resource to use when validation fails.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="resourceSelector">The resource to use as an expression, eg () => Messages.MyResource</param>
		/// <param name="formatArgs">Custom message format args</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> WithLocalizedMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Expression<Func<string>> resourceSelector, params object[] formatArgs) {
			var funcs = ConvertArrayOfObjectsToArrayOfDelegates<T>(formatArgs);
			return rule.WithLocalizedMessage(resourceSelector, funcs);
		}

		/// <summary>
		/// Specifies a custom error message resource to use when validation fails.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="resourceSelector">The resource to use as an expression, eg () => Messages.MyResource</param>
		/// <param name="formatArgs">Custom message format args</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> WithLocalizedMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Expression<Func<string>> resourceSelector, params Func<T, object>[] formatArgs) {
			// We use the StaticResourceAccessorBuilder here because we don't want calls to WithLocalizedMessage to be overriden by the ResourceProviderType.
			return rule.WithLocalizedMessage(resourceSelector, new StaticResourceAccessorBuilder())
				.Configure(cfg => {
					formatArgs
						.Select(func => new Func<object, object, object>((instance, value) => func((T)instance)))
						.ForEach(cfg.CurrentValidator.CustomMessageFormatArguments.Add);
				});
			
		}

		/// <summary>
		/// Specifies a custom error message resource to use when validation fails.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="resourceSelector">The resource to use as an expression, eg () => Messages.MyResource</param>
		/// <param name="resourceAccessorBuilder">The resource accessor builder to use. </param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T,TProperty> WithLocalizedMessage<T,TProperty>(this IRuleBuilderOptions<T,TProperty> rule, Expression<Func<string>> resourceSelector, IResourceAccessorBuilder resourceAccessorBuilder) {
			resourceSelector.Guard("An expression must be specified when calling WithLocalizedMessage, eg .WithLocalizedMessage(() => Messages.MyResource)");
		
			return rule.Configure(config => {
				config.CurrentValidator.ErrorMessageSource = LocalizedStringSource.CreateFromExpression(resourceSelector, resourceAccessorBuilder);
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
			predicate.Guard("A predicate must be specified when calling When.");
			// Default behaviour for When/Unless as of v1.3 is to apply the condition to all previous validators in the chain.
			return rule.Configure(config => {
				config.ApplyCondition(predicate.CoerceToNonGeneric(), applyConditionTo);
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
			predicate.Guard("A predicate must be specified when calling Unless");
			return rule.When(x => !predicate(x), applyConditionTo);
		}


		/// <summary>
		/// Specifies a custom property name to use within the error message.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="overridePropertyName">The property name to use</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> WithName<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, string overridePropertyName) {
			overridePropertyName.Guard("A property name must be specified when calling WithName.");
			return rule.Configure(config => {
				config.DisplayName = new StaticStringSource(overridePropertyName);	
			});
		}

		/// <summary>
		/// Specifies a localized name for the error message. 
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="resourceSelector">The resource to use as an expression, eg () => Messages.MyResource</param>
		/// <param name="resourceAccessorBuilder">Resource accessor builder to use</param>
		public static IRuleBuilderOptions<T, TProperty> WithLocalizedName<T,TProperty>(this IRuleBuilderOptions<T,TProperty> rule, Expression<Func<string>> resourceSelector, IResourceAccessorBuilder resourceAccessorBuilder = null) {
			resourceSelector.Guard("A resource selector must be specified.");
			// default to the static resource accessor builder - explicit resources configured with WithLocalizedName should take precedence over ResourceProviderType.
			resourceAccessorBuilder = resourceAccessorBuilder ?? new StaticResourceAccessorBuilder();
			
			return rule.Configure(config => {
				config.DisplayName = LocalizedStringSource.CreateFromExpression(resourceSelector, resourceAccessorBuilder);
			});
		}

		/// <summary>
		/// Overrides the name of the property associated with this rule.
		/// NOTE: This is a considered to be an advanced feature. 99% of the time that you use this, you actually meant to use WithName.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="propertyName">The property name to use</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> OverridePropertyName<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, string propertyName) {
			propertyName.Guard("A property name must be specified when calling WithNamePropertyName.");
			return rule.Configure(config => config.PropertyName = propertyName);
		}

		/// <summary>
		/// Overrides the name of the property associated with this rule.
		/// NOTE: This is a considered to be an advanced feature. 99% of the time that you use this, you actually meant to use WithName.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="propertyName">The property name to use</param>
		/// <returns></returns>
		[Obsolete("WithPropertyName has been deprecated. If you wish to set the name of the property within the error message, use 'WithName'. If you actually intended to change which property this rule was declared against, use 'OverridePropertyName' instead.")]
		public static IRuleBuilderOptions<T, TProperty> WithPropertyName<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, string propertyName) {
			return rule.OverridePropertyName(propertyName);
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
			stateProvider.Guard("A lambda expression must be passed to WithState");
			return rule.Configure(config => config.CurrentValidator.CustomStateProvider = stateProvider.CoerceToNonGeneric());
		}

		static Func<T, object>[] ConvertArrayOfObjectsToArrayOfDelegates<T>(object[] objects) {
			if(objects == null || objects.Length == 0) {
				return new Func<T, object>[0];
			}
			return objects.Select(obj => new Func<T, object>(x => obj)).ToArray();
		} 
	}
}