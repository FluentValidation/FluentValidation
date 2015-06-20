namespace FluentValidation.TestHelper.Fluent
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using Results;
    using TestHelper;

    internal class FluentValidationResultTester<T, TValue> : IFluentValidationResultTester where T : class 
    {
        private readonly FluentValidationResult<T, TValue> fluentValidationResult;

        public FluentValidationResultTester(FluentValidationResult<T, TValue> fluentValidationResult)
        {
            this.fluentValidationResult = fluentValidationResult;
        }

        private string GetPropertyName(IEnumerable<MemberInfo> properties)
        {
            return string.Join(".", new[] { fluentValidationResult.MemberAccessor != null ? fluentValidationResult.MemberAccessor.Member  : null}
                         .Concat(properties)
                         .Where(x => x != null)
                         .Select(x => GetAttributesInternal<DisplayNameAttribute>(x).Select(y => y.DisplayName)
                                                                                    .FirstOrDefault() ?? x.Name));
        }

        private static IEnumerable<TAttr> GetAttributesInternal<TAttr>(MemberInfo member) where TAttr : Attribute
        {
            return member.GetCustomAttributes(typeof(TAttr), false)
                         .Cast<TAttr>();
        }

        public IEnumerable<ValidationFailure> ShouldHaveError(IEnumerable<MemberInfo> properties)
        {
            var propertyName = GetPropertyName(properties);

            var failures = fluentValidationResult.Result
                                                 .Errors
                                                 .Where(x => x.PropertyName == propertyName || string.IsNullOrEmpty(propertyName))
                                                 .ToArray();

            if (!failures.Any())
                throw new ValidationTestException(string.Format("Expected a validation error for property {0}", propertyName));

            return failures;
        }

        public void ShouldNotHaveError(IEnumerable<MemberInfo> properties)
        {
            var propertyName = GetPropertyName(properties);

            if (fluentValidationResult.Result.Errors.Any(x => x.PropertyName == propertyName || string.IsNullOrEmpty(propertyName)))
                throw new ValidationTestException(string.Format("Expected no validation errors for property {0}", propertyName));
        }
    }
}