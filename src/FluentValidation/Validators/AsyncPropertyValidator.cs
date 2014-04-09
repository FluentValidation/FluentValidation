namespace FluentValidation.Validators {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Results;

    public abstract class AsyncPropertyValidator : PropertyValidator, IAsyncPropertyValidator {
        protected AsyncPropertyValidator(string errorMessageResourceName, Type errorMessageResourceType) 
            : base(errorMessageResourceName, errorMessageResourceType) {
        }

        protected AsyncPropertyValidator(string errorMessage) 
            : base(errorMessage) {
        }

        protected AsyncPropertyValidator(Expression<Func<string>> errorMessageResourceSelector) 
            : base(errorMessageResourceSelector) {
        }

        public virtual Task<IEnumerable<ValidationFailure>> ValidateAsync(PropertyValidatorContext context)
        {
            context.MessageFormatter.AppendPropertyName(context.PropertyDescription);
            context.MessageFormatter.AppendArgument("PropertyValue", context.PropertyValue);

            return
                IsValidAsync(context)
                    .Then(
                          valid => valid ? Enumerable.Empty<ValidationFailure>() : new[] { CreateValidationError(context) }.AsEnumerable(),
                          runSynchronously: true
                    );
        }
    }
}