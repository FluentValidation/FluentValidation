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
// The latest version of this file can be found at http://www.codeplex.com/FluentValidation
#endregion

namespace FluentValidation.TestHelper {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Internal;
    using Results;
    using Validators;

    public static class ValidationTestExtension {
        public static IEnumerable<ValidationFailure> ShouldHaveValidationErrorFor<T, TValue>(this IValidator<T> validator,
            Expression<Func<T, TValue>> expression, TValue value, string ruleSet = null) where T : class, new() {
            var testValidationResult = validator.TestValidate(expression, value, ruleSet);
            return testValidationResult.ShouldHaveError();
        }

        public static IEnumerable<ValidationFailure> ShouldHaveValidationErrorFor<T, TValue>(this IValidator<T> validator, Expression<Func<T, TValue>> expression, T objectToTest, string ruleSet = null) where T : class {
            var value = expression.Compile()(objectToTest);
            var testValidationResult = validator.TestValidate(expression, value, ruleSet);
            return testValidationResult.ShouldHaveError();
        }

        public static void ShouldNotHaveValidationErrorFor<T, TValue>(this IValidator<T> validator,
            Expression<Func<T, TValue>> expression, TValue value, string ruleSet = null) where T : class, new() {
            var testValidationResult = validator.TestValidate(expression, value, ruleSet);
            testValidationResult.ShouldNotHaveError();
        }

        public static void ShouldNotHaveValidationErrorFor<T, TValue>(this IValidator<T> validator, Expression<Func<T, TValue>> expression, T objectToTest, string ruleSet = null) where T : class {
            var value = expression.Compile()(objectToTest);
            var testValidationResult = validator.TestValidate(expression, value, ruleSet);
            testValidationResult.ShouldNotHaveError();
        }

        public static void ShouldHaveChildValidator<T, TProperty>(this IValidator<T> validator, Expression<Func<T, TProperty>> expression, Type childValidatorType) {
            var descriptor = validator.CreateDescriptor();
            var matchingValidators = descriptor.GetValidatorsForMember(expression.GetMember().Name).ToArray();

            var childValidatorTypes = matchingValidators.OfType<ChildValidatorAdaptor>().Select(x => x.ValidatorType);
            childValidatorTypes = childValidatorTypes.Concat(matchingValidators.OfType<ChildCollectionValidatorAdaptor>().Select(x => x.ChildValidatorType));

            if (childValidatorTypes.All(x => x != childValidatorType)) {
                throw new ValidationTestException(string.Format("Expected property '{0}' to have a child validator of type '{1}.'", expression.GetMember().Name, childValidatorType.Name));
            }
        }

        private static TestValidationResult<T, TValue> TestValidate<T, TValue>(this IValidator<T> validator, Expression<Func<T, TValue>> expression, TValue value, string ruleSet = null) where T : class {
            var instanceToValidate = Activator.CreateInstance<T>();

            var memberAccessor = ((MemberAccessor<T, TValue>) expression);

            memberAccessor.Set(instanceToValidate, value);

            var validationResult = validator.Validate(instanceToValidate, null, ruleSet: ruleSet);

            return new TestValidationResult<T, TValue>(validationResult, memberAccessor);
        }

        public static TestValidationResult<T, T> TestValidate<T>(this IValidator<T> validator, T objectToTest, string ruleSet = null) where T : class {
            var validationResult = validator.Validate(objectToTest, null, ruleSet: ruleSet);

            return new TestValidationResult<T, T>(validationResult, (Expression<Func<T, T>>) (o => o));
        }

        public static IEnumerable<ValidationFailure> ShouldHaveError<T, TValue>(this TestValidationResult<T, TValue> testValidationResult) where T : class {
            return testValidationResult.Which.ShouldHaveError();
        }

        public static void ShouldNotHaveError<T, TValue>(this TestValidationResult<T, TValue> testValidationResult) where T : class {
            testValidationResult.Which.ShouldNotHaveError();
        }

        public static IEnumerable<ValidationFailure> When(this IEnumerable<ValidationFailure> failures, Func<ValidationFailure, bool> failurePredicate)
        {
            if (!failures.Any(failurePredicate))
                throw new ValidationTestException("Expected a validation error is not found");

            return failures;
        }
    }
}