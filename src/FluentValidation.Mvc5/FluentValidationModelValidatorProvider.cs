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

namespace FluentValidation.Mvc {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;
	using Attributes;
	using Internal;
	using Resources;
	using Validators;

	public delegate ModelValidator FluentValidationModelValidationFactory(ModelMetadata metadata, ControllerContext context, PropertyRule rule, IPropertyValidator validator);

	/// <summary>
	/// Implementation of ModelValidatorProvider that uses FluentValidation.
	/// </summary>
	public class FluentValidationModelValidatorProvider : ModelValidatorProvider {
		public bool AddImplicitRequiredValidator { get; set; }
		public IValidatorFactory ValidatorFactory { get; set; }

		private Dictionary<Type, FluentValidationModelValidationFactory> validatorFactories = new Dictionary<Type, FluentValidationModelValidationFactory>() {
			{ typeof(INotNullValidator), (metadata, context, rule, validator) => new RequiredFluentValidationPropertyValidator(metadata, context, rule, validator) },
			{ typeof(INotEmptyValidator), (metadata, context, rule, validator) => new RequiredFluentValidationPropertyValidator(metadata, context, rule, validator) },
			// email must come before regex.
			{ typeof(IEmailValidator), (metadata, context, rule, validator) => new EmailFluentValidationPropertyValidator(metadata, context, rule, validator) },			
			{ typeof(IRegularExpressionValidator), (metadata, context, rule, validator) => new RegularExpressionFluentValidationPropertyValidator(metadata, context, rule, validator) },
			{ typeof(ILengthValidator), (metadata, context, rule, validator) => new StringLengthFluentValidationPropertyValidator(metadata, context, rule, validator)},
			{ typeof(InclusiveBetweenValidator), (metadata, context, rule, validator) => new RangeFluentValidationPropertyValidator(metadata, context, rule, validator) },
			{ typeof(GreaterThanOrEqualValidator), (metadata, context, rule, validator) => new MinFluentValidationPropertyValidator(metadata, context, rule, validator) },
			{ typeof(LessThanOrEqualValidator), (metadata, context, rule, validator) => new MaxFluentValidationPropertyValidator(metadata, context, rule, validator) },
			{ typeof(EqualValidator), (metadata, context, rule, validator) => new EqualToFluentValidationPropertyValidator(metadata, context, rule, validator) },
			{ typeof(CreditCardValidator), (metadata, context, description, validator) => new CreditCardFluentValidationPropertyValidator(metadata, context, description, validator) }
		};

		public FluentValidationModelValidatorProvider(IValidatorFactory validatorFactory = null) {
			AddImplicitRequiredValidator = true;
			ValidatorFactory = validatorFactory ?? new AttributedValidatorFactory();
		}

		/// <summary>
		/// Initializes the FluentValidationModelValidatorProvider using the default options and adds it in to the ModelValidatorProviders collection.
		/// </summary>
		public static void Configure(Action<FluentValidationModelValidatorProvider> configurationExpression = null) {
			configurationExpression = configurationExpression ?? delegate { };

			var provider = new FluentValidationModelValidatorProvider();
			configurationExpression(provider);

			DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
			ModelValidatorProviders.Providers.Add(provider);
		}

		public void Add(Type validatorType, FluentValidationModelValidationFactory factory) {
			if(validatorType == null) throw new ArgumentNullException("validatorType");
			if(factory == null) throw new ArgumentNullException("factory");

			validatorFactories[validatorType] = factory;
		}

		public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context) {
			IValidator validator = CreateValidator(metadata, context);

			if (IsValidatingProperty(metadata)) {
				return GetValidatorsForProperty(metadata, context, validator);
			}
			return GetValidatorsForModel(metadata, context, validator);
		}

		protected virtual IValidator CreateValidator(ModelMetadata metadata, ControllerContext context) {
			if (IsValidatingProperty(metadata)) {
				return ValidatorFactory.GetValidator(metadata.ContainerType);
			}
			return ValidatorFactory.GetValidator(metadata.ModelType);
		}

		protected IEnumerable<ModelValidator> GetValidatorsForProperty(ModelMetadata metadata, ControllerContext context, IValidator validator) {
			var modelValidators = new List<ModelValidator>();

			if (validator != null) {
				var descriptor = validator.CreateDescriptor();

				var validatorsWithRules = from rule in descriptor.GetRulesForMember(metadata.PropertyName)
										  let propertyRule = (PropertyRule)rule
										  let validators = rule.Validators
										  where validators.Any()
										  from propertyValidator in validators
										  let modelValidatorForProperty = GetModelValidator(metadata, context, propertyRule, propertyValidator)
										  where modelValidatorForProperty != null
										  select modelValidatorForProperty;
					
				modelValidators.AddRange(validatorsWithRules);
			}

			if(validator != null && metadata.IsRequired && AddImplicitRequiredValidator) {
				bool hasRequiredValidators = modelValidators.Any(x => x.IsRequired);

				//If the model is 'Required' then we assume it must have a NotNullValidator. 
				//This is consistent with the behaviour of the DataAnnotationsModelValidatorProvider
				//which silently adds a RequiredAttribute

				if(! hasRequiredValidators) {
					modelValidators.Add(CreateNotNullValidatorForProperty(metadata, context));
				}
			}

			return modelValidators;
		}

		protected virtual ModelValidator GetModelValidator(ModelMetadata meta, ControllerContext context, PropertyRule rule, IPropertyValidator propertyValidator) {
			var type = propertyValidator.GetType();
			
			var factory = validatorFactories
				.Where(x => x.Key.IsAssignableFrom(type))
				.Select(x => x.Value)
				.FirstOrDefault() ?? ((metadata, controllerContext, description, validator) => new FluentValidationPropertyValidator(metadata, controllerContext, description, validator));

			return factory(meta, context, rule, propertyValidator);
		}

		protected virtual ModelValidator CreateNotNullValidatorForProperty(ModelMetadata metadata, ControllerContext cc) {

			var fakeRule = new PropertyRule(null, x => metadata.Model, null, null, metadata.ModelType, null) {
				PropertyName = metadata.PropertyName,
				DisplayName = new StaticStringSource(metadata.GetDisplayName()),
			};
			return new RequiredFluentValidationPropertyValidator(metadata, cc, fakeRule, new NotNullValidator());
		}



		protected virtual IEnumerable<ModelValidator> GetValidatorsForModel(ModelMetadata metadata, ControllerContext context, IValidator validator) {
			if (validator != null) {
				yield return new FluentValidationModelValidator(metadata, context, validator);
			}
		}

		protected virtual bool IsValidatingProperty(ModelMetadata metadata) {
			return metadata.ContainerType != null && !string.IsNullOrEmpty(metadata.PropertyName);
		}
	}
}