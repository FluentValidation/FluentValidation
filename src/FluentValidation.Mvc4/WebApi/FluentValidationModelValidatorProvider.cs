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

	using WebApi.PropertyValidatorAdapters;

	public delegate ModelValidator FluentValidationModelValidationFactory(ModelMetadata metadata, IEnumerable<ModelValidatorProvider> validatorProviders, PropertyRule rule, IPropertyValidator validator);


	public class FluentValidationModelValidatorProvider : ModelValidatorProvider {
		public IValidatorFactory ValidatorFactory { get; set; }

		private Dictionary<Type, FluentValidationModelValidationFactory> validatorFactories = new Dictionary<Type, FluentValidationModelValidationFactory>() {
			{ typeof(INotNullValidator), (metadata, context, rule, validator) => new RequiredFluentValidationPropertyValidator(metadata, context, rule, validator) },
			{ typeof(INotEmptyValidator), (metadata, context, rule, validator) => new RequiredFluentValidationPropertyValidator(metadata, context, rule, validator) },
			// email must come before regex.
			{ typeof(IEmailValidator), (metadata, context, rule, validator) => new FluentValidationPropertyValidator(metadata, context, rule, validator)  },			
			{ typeof(IRegularExpressionValidator), (metadata, context, rule, validator) => new FluentValidationPropertyValidator(metadata, context, rule, validator) },
			{ typeof(ILengthValidator), (metadata, context, rule, validator) => new FluentValidationPropertyValidator(metadata, context, rule, validator)},
			{ typeof(InclusiveBetweenValidator), (metadata, context, rule, validator) => new FluentValidationPropertyValidator(metadata, context, rule, validator) },
			{ typeof(EqualValidator), (metadata, context, rule, validator) => new FluentValidationPropertyValidator(metadata, context, rule, validator) },
			{ typeof(CreditCardValidator), (metadata, context, description, validator) => new FluentValidationPropertyValidator(metadata, context, description, validator) }
		};

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

		public void Add(Type validatorType, FluentValidationModelValidationFactory factory)
		{
			if(validatorType == null) throw new ArgumentNullException("validatorType");
			if(factory == null) throw new ArgumentNullException("factory");

			validatorFactories[validatorType] = factory;
		}

		public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, IEnumerable<ModelValidatorProvider> validatorProviders) {
			IValidator validator = CreateValidator(metadata, validatorProviders);

			if (IsValidatingProperty(metadata)) {
				return GetValidatorsForProperty(metadata, validatorProviders, validator);
			}
			return GetValidatorsForModel(validatorProviders, validator);
		}

		protected virtual IValidator CreateValidator(ModelMetadata metadata, IEnumerable<ModelValidatorProvider> validatorProviders)
		{
			if (IsValidatingProperty(metadata)) {
				return ValidatorFactory.GetValidator(metadata.ContainerType);
			}
			return ValidatorFactory.GetValidator(metadata.ModelType);
		}

		IEnumerable<ModelValidator> GetValidatorsForProperty(ModelMetadata metadata, IEnumerable<ModelValidatorProvider> validatorProviders, IValidator validator)
		{
			var modelValidators = new List<ModelValidator>();

			if (validator != null) {
				var descriptor = validator.CreateDescriptor();

				var validatorsWithRules = from rule in descriptor.GetRulesForMember(metadata.PropertyName)
										  let propertyRule = (PropertyRule)rule
										  let validators = rule.Validators
										  where validators.Any()
										  from propertyValidator in validators
										  let modelValidatorForProperty = GetModelValidator(metadata, validatorProviders, propertyRule, propertyValidator)
										  where modelValidatorForProperty != null
										  select modelValidatorForProperty;
					
				modelValidators.AddRange(validatorsWithRules);
			}

			return modelValidators;
		}

		private ModelValidator GetModelValidator(ModelMetadata meta, IEnumerable<ModelValidatorProvider> providers, PropertyRule rule, IPropertyValidator propertyValidator) {
			var type = propertyValidator.GetType();
			
			var factory = validatorFactories
				.Where(x => x.Key.IsAssignableFrom(type))
				.Select(x => x.Value)
				.FirstOrDefault() ?? ((metadata, validatorProviders, description, validator) => new FluentValidationPropertyValidator(metadata, validatorProviders, description, validator));

			return factory(meta, providers, rule, propertyValidator);
		}

		IEnumerable<ModelValidator> GetValidatorsForModel(IEnumerable<ModelValidatorProvider> validatorProviders, IValidator validator) {
			if (validator != null) {
				yield return new FluentValidationModelValidator(validatorProviders, validator);
			}
		}

		protected virtual bool IsValidatingProperty(ModelMetadata metadata) {
			return metadata.ContainerType != null && !string.IsNullOrEmpty(metadata.PropertyName);
		}
	}
}