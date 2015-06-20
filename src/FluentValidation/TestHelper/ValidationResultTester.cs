namespace FluentValidation.TestHelper {
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Results;

    class ValidationResultTester<T, TValue> : IValidationResultTester where T : class {
        readonly TestValidationResult<T, TValue> testValidationResult;

        public ValidationResultTester(TestValidationResult<T, TValue> testValidationResult) {
            this.testValidationResult = testValidationResult;
        }

        string GetPropertyName(IEnumerable<MemberInfo> properties) {
            return string.Join(".", new[] {testValidationResult.MemberAccessor != null ? testValidationResult.MemberAccessor.Member : null}
                .Concat(properties)
                .Where(x => x != null)
                .Select(x => x.Name));
        }

        public IEnumerable<ValidationFailure> ShouldHaveError(IEnumerable<MemberInfo> properties) {
            var propertyName = GetPropertyName(properties);

            var failures = testValidationResult.Result
                .Errors
                .Where(x => x.PropertyName == propertyName || string.IsNullOrEmpty(propertyName))
                .ToArray();

            if (!failures.Any())
                throw new ValidationTestException(string.Format("Expected a validation error for property {0}", propertyName));

            return failures;
        }

        public void ShouldNotHaveError(IEnumerable<MemberInfo> properties) {
            var propertyName = GetPropertyName(properties);

            if (testValidationResult.Result.Errors.Any(x => x.PropertyName == propertyName || string.IsNullOrEmpty(propertyName)))
                throw new ValidationTestException(string.Format("Expected no validation errors for property {0}", propertyName));
        }
    }
}