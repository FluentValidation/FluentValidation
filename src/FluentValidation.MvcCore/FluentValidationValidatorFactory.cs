using System;
using FluentValidation;

namespace FluentValidation.MvcCore
{
    public class FluentValidationValidatorFactory : ValidatorFactoryBase
    {
       private readonly IServiceProvider _Container;
        public FluentValidationValidatorFactory(IServiceProvider container) {
            _Container = container;
        }
        public override IValidator CreateInstance(Type validatorType) {
            return _Container.GetService(validatorType) as IValidator;
        }
    }
}