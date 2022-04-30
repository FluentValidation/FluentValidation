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
namespace FluentValidation.AspNetCore {
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using System.Linq;
	using System.Reflection;
	using FluentValidation.Internal;
	using FluentValidation.Validators;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc.DataAnnotations;

	public delegate IClientModelValidator FluentValidationClientValidatorFactory(ClientValidatorProviderContext context, IValidationRule rule, IRuleComponent component);

	/// <summary>
	/// Used to generate clientside metadata from FluentValidation's rules.
	/// </summary>
	public class FluentValidationClientModelValidatorProvider : IClientModelValidatorProvider{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ValidatorDescriptorCache _descriptorCache = new ValidatorDescriptorCache();

		public Dictionary<Type, FluentValidationClientValidatorFactory> ClientValidatorFactories { get; } = new() {
			{ typeof(INotNullValidator), (context, rule, component) => new RequiredClientValidator(rule, component) },
			{ typeof(INotEmptyValidator), (context, rule, component) => new RequiredClientValidator(rule, component) },
			{ typeof(IEmailValidator), (context, rule, component) => new EmailClientValidator(rule, component) },
			{ typeof(IRegularExpressionValidator), (context, rule, component) => new RegexClientValidator(rule, component) },
			{ typeof(IMaximumLengthValidator), (context, rule, component) => new MaxLengthClientValidator(rule, component) },
			{ typeof(IMinimumLengthValidator), (context, rule, component) => new MinLengthClientValidator(rule, component) },
			{ typeof(IExactLengthValidator), (context, rule, component) => new StringLengthClientValidator(rule, component)},
			{ typeof(ILengthValidator), (context, rule, component) => new StringLengthClientValidator(rule, component)},
			{ typeof(IInclusiveBetweenValidator), (context, rule, component) => new RangeClientValidator(rule, component) },
			{ typeof(IGreaterThanOrEqualValidator), (context, rule, component) => new RangeMinClientValidator(rule, component) },
			{ typeof(ILessThanOrEqualValidator), (context, rule, component) => new RangeMaxClientValidator(rule, component) },
			{ typeof(IEqualValidator), (context, rule, component) => new EqualToClientValidator(rule, component) },
			{ typeof(ICreditCardValidator), (context, rule, component) => new CreditCardClientValidator(rule, component) },
		};

		public FluentValidationClientModelValidatorProvider(IHttpContextAccessor httpContextAccessor) {
			_httpContextAccessor = httpContextAccessor;
		}

		public void Add(Type validatorType, FluentValidationClientValidatorFactory factory) {
			if (validatorType == null) throw new ArgumentNullException(nameof(validatorType));
			if (factory == null) throw new ArgumentNullException(nameof(factory));

			ClientValidatorFactories[validatorType] = factory;
		}

		public void CreateValidators(ClientValidatorProviderContext context) {
			var descriptor = _descriptorCache.GetCachedDescriptor(context, _httpContextAccessor);

			if (descriptor != null) {
				var propertyName = context.ModelMetadata.PropertyName;

				var validatorsWithRules = from rule in descriptor.GetRulesForMember(propertyName)
					where !rule.HasCondition && !rule.HasAsyncCondition
					let components = rule.Components
					where components.Any()
					from component in components
					where !component.HasCondition && !component.HasAsyncCondition
					let modelValidatorForProperty = GetModelValidator(context, rule, component)
					where modelValidatorForProperty != null
					select modelValidatorForProperty;

				var list = validatorsWithRules.ToList();

				foreach (var propVal in list) {
					context.Results.Add(new ClientValidatorItem {
						Validator = propVal,
						IsReusable = false
					});
				}

				// Must ensure there is at least 1 ClientValidatorItem, set to IsReusable = false
				// otherwise MVC will cache the list of validators, assuming there will always be 0 validators for that property
				// Which isn't true - we may be using the RulesetForClientsideMessages attribute (or some other mechanism) that can change the client validators that are available
				// depending on some context.
				if (list.Count == 0) {
					context.Results.Add(new ClientValidatorItem {IsReusable = false});
				}

				HandleNonNullableValueTypeRequiredRule(context);
			}
		}

		// If the property is a non-nullable value type, then MVC will have already generated a Required rule.
		// If we've provided our own Requried rule, then remove the MVC one.
		protected virtual void HandleNonNullableValueTypeRequiredRule(ClientValidatorProviderContext context) {
			bool isNonNullableValueType = !TypeAllowsNullValue(context.ModelMetadata.ModelType);

			if (isNonNullableValueType) {
				bool fvHasRequiredRule = context.Results.Any(x => x.Validator is RequiredClientValidator);

				if (fvHasRequiredRule) {
					var dataAnnotationsRequiredRule = context.Results
						.FirstOrDefault(x => x.Validator is RequiredAttributeAdapter);
					context.Results.Remove(dataAnnotationsRequiredRule);
				}
			}
		}

		protected virtual IClientModelValidator GetModelValidator(ClientValidatorProviderContext context, IValidationRule rule, IRuleComponent component)	{
			var type = component.Validator.GetType();

			var factory = ClientValidatorFactories
				.Where(x => x.Key.IsAssignableFrom(type))
				.Select(x => x.Value)
				.FirstOrDefault();

			if (factory != null) {
				bool shouldExecute = false;
				var ruleSetToGenerateClientSideRules = RuleSetForClientSideMessagesAttribute.GetRuleSetsForClientValidation(_httpContextAccessor?.HttpContext);

				if (ruleSetToGenerateClientSideRules.Contains(RulesetValidatorSelector.WildcardRuleSetName)) {
					// If RuleSet "*" is specified, include all rules.
					shouldExecute = true;
				}
				else {
					bool executeDefaultRule = ruleSetToGenerateClientSideRules.Contains(RulesetValidatorSelector.DefaultRuleSetName, StringComparer.OrdinalIgnoreCase)
					                          && (rule.RuleSets == null || rule.RuleSets.Length == 0 || rule.RuleSets.Contains(RulesetValidatorSelector.DefaultRuleSetName, StringComparer.OrdinalIgnoreCase));

					shouldExecute = (rule.RuleSets != null && ruleSetToGenerateClientSideRules.Intersect(rule.RuleSets, StringComparer.OrdinalIgnoreCase).Any()) || executeDefaultRule;
				}

				if (shouldExecute) {
					return factory.Invoke(context, rule, component);
				}
			}

			return null;
		}

		private bool TypeAllowsNullValue(Type type) {
			return (!type.IsValueType || Nullable.GetUnderlyingType(type) != null);
		}
	}

}
