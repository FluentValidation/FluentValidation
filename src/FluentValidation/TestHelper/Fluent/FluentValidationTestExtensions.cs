namespace FluentValidation.TestHelper.Fluent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Results;

    public static class FluentValidationTestExtensions
    {
        public static void When(this IEnumerable<ValidationFailure> failures, params Expression<Func<ValidationFailure, bool>>[] failurePredicates) 
        {
            failurePredicates = failurePredicates ?? new Expression<Func<ValidationFailure, bool>>[0];

            var typeParam = Expression.Parameter(typeof(ValidationFailure));

            var andExpression = failurePredicates.Select(x => Expression.Invoke(x, typeParam))
                                                 .Aggregate(Expression.Constant(true) as Expression, Expression.AndAlso);

            var lambda = Expression.Lambda<Func<ValidationFailure, bool>>(andExpression, typeParam);

            var compiledLambda = lambda.Compile();

            var filteredFailures = failures.Where(compiledLambda);

            if(!filteredFailures.Any())
                throw new ValidationTestException("Expected a validation error is not found");
        }

        public static FluentValidationResult<T, TValue> TestValidate<T, TValue>(this IValidator<T> validator, Expression<Func<T, TValue>> expression, TValue value, string ruleSet = null) where T : class
        {
            var instanceToValidate = Activator.CreateInstance<T>();

            var memberAccessor = ((MemberAccessor<T, TValue>) expression);

            memberAccessor.Set(instanceToValidate, value);

            var validationResult = validator.Validate(instanceToValidate, null, ruleSet: ruleSet);

            return new FluentValidationResult<T, TValue>(validationResult, memberAccessor);
        }

        public static FluentValidationResult<T, T> TestValidate<T>(this IValidator<T> validator, T objectToTest, string ruleSet = null) where T : class
        {
            var validationResult = validator.Validate(objectToTest, null, ruleSet: ruleSet);

            return new FluentValidationResult<T, T>(validationResult, (Expression<Func<T, T>>)(o => o));
        }

        public static IEnumerable<ValidationFailure> ShouldHaveError<T, TValue>(this FluentValidationResult<T, TValue> fluentValidationResult) where T : class
        {
            return fluentValidationResult.Which.ShouldHaveError();
        }

        public static void ShouldNotHaveError<T, TValue>(this FluentValidationResult<T, TValue> fluentValidationResult) where T : class
        {
            fluentValidationResult.Which.ShouldNotHaveError();
        }
    }
}