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
namespace FluentValidation.AspNetCore {
	using System;
	using System.Collections.Generic;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using System.Linq;
	using System.Reflection;
	using FluentValidation.Internal;
	using FluentValidation.Validators;
	using Microsoft.AspNetCore.Http;

	public delegate IClientModelValidator FluentValidationClientValidatorFactory(ClientValidatorProviderContext context, PropertyRule rule, IPropertyValidator validator);

	public class FluentValidationClientModelValidatorProvider : IClientModelValidatorProvider{

		public Dictionary<Type, FluentValidationClientValidatorFactory> ClientValidatorFactories => _validatorFactories;

		private readonly Dictionary<Type, FluentValidationClientValidatorFactory> _validatorFactories = new Dictionary<Type, FluentValidationClientValidatorFactory>() {
			{ typeof(INotNullValidator), (context, rule, validator) => new RequiredClientValidator(rule, validator) },
			{ typeof(INotEmptyValidator), (context, rule, validator) => new RequiredClientValidator(rule, validator) },
			// email must come before regex.
			{ typeof(IEmailValidator), (context, rule, validator) => new EmailClientValidator(rule, validator) },
			{ typeof(IRegularExpressionValidator), (context, rule, validator) => new RegexClientValidator(rule, validator) },
			{ typeof(MaximumLengthValidator), (context, rule, validator) => new MaxLengthClientValidator(rule, validator) },
			{ typeof(MinimumLengthValidator), (context, rule, validator) => new MinLengthClientValidator(rule, validator) },
			{ typeof(LengthValidator), (context, rule, validator) => new StringLengthClientValidator(rule, validator)},
			{ typeof(ExactLengthValidator), (context, rule, validator) => new StringLengthClientValidator(rule, validator)},
			{ typeof(InclusiveBetweenValidator), (context, rule, validator) => new RangeClientValidator(rule, validator) },
			{ typeof(GreaterThanOrEqualValidator), (context, rule, validator) => new RangeMinClientValidator(rule, validator) },
			{ typeof(LessThanOrEqualValidator), (context, rule, validator) => new RangeMaxClientValidator(rule, validator) },
			{ typeof(EqualValidator), (context, rule, validator) => new EqualToClientValidator(rule, validator) },
			{ typeof(CreditCardValidator), (context, rule, validator) => new CreditCardClientValidator(rule, validator) },
			

		};

		IHttpContextAccessor _httpContextAccessor;

		public FluentValidationClientModelValidatorProvider(IHttpContextAccessor httpContextAccessor) {
			_httpContextAccessor = httpContextAccessor;
		}

		public void Add(Type validatorType, FluentValidationClientValidatorFactory factory) {
			if (validatorType == null) throw new ArgumentNullException(nameof(validatorType));
			if (factory == null) throw new ArgumentNullException(nameof(factory));

			_validatorFactories[validatorType] = factory;
		}

		public void CreateValidators(ClientValidatorProviderContext context) {
			var modelType = context.ModelMetadata.ContainerType;

			if (_httpContextAccessor == null) {
				throw new InvalidOperationException("Cannot use clientside validation unless the IHttpContextAccessor is registered with the service provider. Make sure the provider is registered by calling services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); in your Startup class's ConfigureServices method");
			}

			var validatorFactory = (IValidatorFactory)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IValidatorFactory));

			if (modelType != null ) {
				var validator = validatorFactory.GetValidator(modelType);

				if (validator != null) {

					var descriptor = validator.CreateDescriptor();
					var propertyName = context.ModelMetadata.PropertyName;

					var validatorsWithRules = from rule in descriptor.GetRulesForMember(propertyName)
						let propertyRule = (PropertyRule) rule
						let validators = rule.Validators
						where validators.Any()
						from propertyValidator in validators
						let modelValidatorForProperty = GetModelValidator(context, propertyRule, propertyValidator)
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
						context.Results.Add(new ClientValidatorItem { IsReusable = false });
					}
					
					HandleNonNullableValueTypeRequiredRule(context);

				}
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
						.FirstOrDefault(x => x.Validator is Microsoft.AspNetCore.Mvc.DataAnnotations.Internal.RequiredAttributeAdapter);
					context.Results.Remove(dataAnnotationsRequiredRule);
				}
			}
		}

		protected virtual IClientModelValidator GetModelValidator(ClientValidatorProviderContext context, PropertyRule rule, IPropertyValidator propertyValidator)
		{
			var type = propertyValidator.GetType();

			var factory = _validatorFactories
				.Where(x => x.Key.IsAssignableFrom(type))
				.Select(x => x.Value)
				.FirstOrDefault();

			if (factory != null) {
				var ruleSetToGenerateClientSideRules = RuleSetForClientSideMessagesAttribute.GetRuleSetsForClientValidation(_httpContextAccessor?.HttpContext);
				bool executeDefaultRule = (ruleSetToGenerateClientSideRules.Contains("default", StringComparer.OrdinalIgnoreCase) && string.IsNullOrEmpty(rule.RuleSet));
				bool shouldExecute = ruleSetToGenerateClientSideRules.Contains(rule.RuleSet) || executeDefaultRule;

				if (shouldExecute) {
					return factory.Invoke(context, rule, propertyValidator);
				}
			}

			return null;
		}

		private bool TypeAllowsNullValue(Type type) {
			return (!type.GetTypeInfo().IsValueType || Nullable.GetUnderlyingType(type) != null);
		}
	}

}