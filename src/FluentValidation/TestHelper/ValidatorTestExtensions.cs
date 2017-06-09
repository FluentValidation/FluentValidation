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

#pragma warning disable 1591
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
			var instanceToValidate = Activator.CreateInstance<T>();
			var testValidationResult = validator.TestValidate(expression, instanceToValidate, value, ruleSet);
			return testValidationResult.ShouldHaveError();
		}

		//This one
		public static IEnumerable<ValidationFailure> ShouldHaveValidationErrorFor<T, TValue>(this IValidator<T> validator, Expression<Func<T, TValue>> expression, T objectToTest, string ruleSet = null) where T : class {
			var value = expression.Compile()(objectToTest);
			var testValidationResult = validator.TestValidate(expression, objectToTest, value, ruleSet);
			return testValidationResult.ShouldHaveError();
		}

		public static void ShouldNotHaveValidationErrorFor<T, TValue>(this IValidator<T> validator,
			Expression<Func<T, TValue>> expression, TValue value, string ruleSet = null) where T : class, new() {
				var instanceToValidate = Activator.CreateInstance<T>();
			var testValidationResult = validator.TestValidate(expression, instanceToValidate, value, ruleSet);
			testValidationResult.ShouldNotHaveError();
		}

		//This one
		public static void ShouldNotHaveValidationErrorFor<T, TValue>(this IValidator<T> validator, Expression<Func<T, TValue>> expression, T objectToTest, string ruleSet = null) where T : class {
			var value = expression.Compile()(objectToTest);
			var testValidationResult = validator.TestValidate(expression, objectToTest, value, ruleSet);
			testValidationResult.ShouldNotHaveError();
		}

		public static void ShouldHaveChildValidator<T, TProperty>(this IValidator<T> validator, Expression<Func<T, TProperty>> expression, Type childValidatorType) {
			var descriptor = validator.CreateDescriptor();
			var expressionMemberName = expression.GetMember().Name;
			var matchingValidators = descriptor.GetValidatorsForMember(expressionMemberName).ToArray();
			var childValidatorTypes = matchingValidators.OfType<ChildValidatorAdaptor>().Select(x => x.ValidatorType);
			childValidatorTypes = childValidatorTypes.Concat(matchingValidators.OfType<ChildCollectionValidatorAdaptor>().Select(x => x.ChildValidatorType));

			if (childValidatorTypes.All(x => x != childValidatorType)) {
				var childValidatorNames = childValidatorTypes.Any() ? string.Join(", ", childValidatorTypes.Select(x => x.Name)) : "none";
				throw new ValidationTestException(string.Format("Expected property '{0}' to have a child validator of type '{1}.'. Instead found '{2}'", expressionMemberName, childValidatorType.Name, childValidatorNames));
			}
		}

		private static TestValidationResult<T, TValue> TestValidate<T, TValue>(this IValidator<T> validator, Expression<Func<T, TValue>> expression, T instanceToValidate, TValue value, string ruleSet = null) where T : class {
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
			return testValidationResult.Which.ShouldHaveValidationError();
		}

		public static void ShouldNotHaveError<T, TValue>(this TestValidationResult<T, TValue> testValidationResult) where T : class {
			testValidationResult.Which.ShouldNotHaveValidationError();
		}

		public static IEnumerable<ValidationFailure> When(this IEnumerable<ValidationFailure> failures, Func<ValidationFailure, bool> failurePredicate, string exceptionMessage = null) {
			if (!failures.Any(failurePredicate))
				throw new ValidationTestException(exceptionMessage ?? "Expected a validation error is not found");

			return failures;
		}

		public static IEnumerable<ValidationFailure> WithCustomState(this IEnumerable<ValidationFailure> failures, object expectedCustomState) {
			return failures.When(failure => failure.CustomState == expectedCustomState, string.Format("Expected custom state of '{0}'.", expectedCustomState));
		}

		public static IEnumerable<ValidationFailure> WithErrorMessage(this IEnumerable<ValidationFailure> failures, string expectedErrorMessage) {
			return failures.When(failure => failure.ErrorMessage == expectedErrorMessage, string.Format("Expected an error message of '{0}'.", expectedErrorMessage));
		}

		public static IEnumerable<ValidationFailure> WithErrorCode(this IEnumerable<ValidationFailure> failures, string expectedErrorCode) {
			return failures.When(failure => failure.ErrorCode == expectedErrorCode, string.Format("Expected an error code of '{0}'.", expectedErrorCode));
		}
	}
}