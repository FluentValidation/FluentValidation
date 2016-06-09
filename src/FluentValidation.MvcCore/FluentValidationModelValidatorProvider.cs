using System.Collections.Generic;
using FluentValidation;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;

namespace FluentValidation.MvcCore
{
    //TODO: Need support for CustomizeValidatorAttribute and client-side

    public class FluentValidationModelValidatorProvider : IModelValidatorProvider
    {
        private IValidatorFactory _ValidatorFactory;

        public FluentValidationModelValidatorProvider(IValidatorFactory validatorFactory) {
            if (validatorFactory == null) 
                throw new ArgumentNullException(nameof(validatorFactory));
                
            _ValidatorFactory = validatorFactory;
        }

        public void CreateValidators(ModelValidatorProviderContext context)
        {
            var validator = CreateValidator(context);
            if (! IsValidatingProperty(context) && validator != null) 
 			{ 
 				context.Results.Add(new ValidatorItem 
                 { 
                     Validator = new FluentValidationModelValidator(validator), 
                     IsReusable = false 
                 }); 
 			} 
        }

        protected virtual IValidator CreateValidator(ModelValidatorProviderContext context)
        {
            if (IsValidatingProperty(context))
            {
                return _ValidatorFactory.GetValidator(context.ModelMetadata.ContainerType);
            }
            return _ValidatorFactory.GetValidator(context.ModelMetadata.ModelType);
        }

        protected virtual bool IsValidatingProperty(ModelValidatorProviderContext context)
        {
            return context.ModelMetadata.ContainerType != null && !string.IsNullOrEmpty(context.ModelMetadata.PropertyName);
        }
    }

    public class FluentValidationModelValidator : IModelValidator
    {
        private IValidator _validator;

        public FluentValidationModelValidator(IValidator validator)
        {
            _validator = validator;
        }

        public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context)
        {
            var model = context.Model;

            var result = _validator.Validate(model);

            return from error in result.Errors
                   select new ModelValidationResult(error.PropertyName, error.ErrorMessage);

        }

        public bool IsRequired
        {
            get { return false; }
        }
    }
}
