namespace FluentValidation.Mvc.WebApi
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Http;
	using System.Web.Http.Metadata;
	using System.Web.Http.Validation;

	using FluentValidation.Attributes;
	using FluentValidation.Internal;
	using FluentValidation.Validators;

	public delegate ModelValidator FluentValidationModelValidationFactory(ModelMetadata metadata, IEnumerable<ModelValidatorProvider> validatorProviders, PropertyRule rule, IPropertyValidator validator);


	public class FluentValidationModelValidatorProvider : ModelValidatorProvider {
		public IValidatorFactory ValidatorFactory { get; set; }

		public FluentValidationModelValidatorProvider(IValidatorFactory validatorFactory = null) {
			ValidatorFactory = validatorFactory ?? new AttributedValidatorFactory();
		}

		/// <summary>
		/// Initializes the FluentValidationModelValidatorProvider using the default options and adds it in to the ModelValidatorProviders collection.
		/// </summary>
		public static void Configure(Action<FluentValidationModelValidatorProvider> configurationExpression = null) {
			configurationExpression = configurationExpression ?? delegate { };

			var provider = new FluentValidationModelValidatorProvider();
			configurationExpression(provider);
			
			GlobalConfiguration.Configuration.Services.Add(typeof(ModelValidatorProvider), provider);
		}

        public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, IEnumerable<ModelValidatorProvider> validatorProviders)
        {
            IValidator validator = CreateValidator(metadata, validatorProviders);

            if (IsValidatingProperty(metadata))
            {
                return GetValidatorsForProperty(metadata, validatorProviders, validator);
            }
            return GetValidatorsForModel(validatorProviders, validator);
        }

        protected virtual IValidator CreateValidator(ModelMetadata metadata, IEnumerable<ModelValidatorProvider> validatorProviders)
        {
            if (IsValidatingProperty(metadata))
            {
                return ValidatorFactory.GetValidator(metadata.ContainerType);
            }
            return ValidatorFactory.GetValidator(metadata.ModelType);
        }

        IEnumerable<ModelValidator> GetValidatorsForProperty(ModelMetadata metadata, IEnumerable<ModelValidatorProvider> validatorProviders, IValidator validator)
        {
            var modelValidators = new List<ModelValidator>();

            if (validator != null)
            {
                var descriptor = validator.CreateDescriptor();

                var validatorsWithRules = from rule in descriptor.GetRulesForMember(metadata.PropertyName)
                                          let propertyRule = (PropertyRule)rule
                                          let validators = rule.Validators
                                          where validators.Any()
                                          from propertyValidator in validators
                                          let modelValidatorForProperty = new FluentValidationPropertyValidator(metadata, validatorProviders, propertyRule, propertyValidator)
                                          where modelValidatorForProperty != null
                                          select modelValidatorForProperty;

                modelValidators.AddRange(validatorsWithRules);
            }

            return modelValidators;
        }

        IEnumerable<ModelValidator> GetValidatorsForModel(IEnumerable<ModelValidatorProvider> validatorProviders, IValidator validator)
        {
            if (validator != null)
            {
                yield return new FluentValidationModelValidator(validatorProviders, validator);
            }
        }

        protected virtual bool IsValidatingProperty(ModelMetadata metadata)
        {
            return metadata.ContainerType != null && !string.IsNullOrEmpty(metadata.PropertyName);
        }
	}
}