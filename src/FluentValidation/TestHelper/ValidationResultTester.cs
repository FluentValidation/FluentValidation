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

namespace FluentValidation.TestHelper {
    using System;
    using System.Collections.Generic;
	using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
	using Results;

	class ValidationResultTester<T, TValue> : IValidationResultTester where T : class {
		readonly TestValidationResult<T, TValue> testValidationResult;

		public ValidationResultTester(TestValidationResult<T, TValue> testValidationResult) {
			this.testValidationResult = testValidationResult;
		}

		string GetPropertyName(IEnumerable<MemberInfo> properties) {
			return string.Join(".", 
                GetMemberNames().Concat(properties
				.Where(x => x != null)
				.Select(x => x.Name)));
		}

	    private IEnumerable<string> GetMemberNames() {
	        if (testValidationResult.MemberAccessor == null) {
	            return Enumerable.Empty<string>();
	        }

            var memberNames = new Stack<string>();

            var getMemberExp = new Func<Expression, MemberExpression>(toUnwrap => {

                if (toUnwrap is UnaryExpression)
                {
                    return ((UnaryExpression)toUnwrap).Operand as MemberExpression;
                }
                else if (toUnwrap is LambdaExpression) {
                    return ((LambdaExpression)toUnwrap).Body as MemberExpression;
                }

                return toUnwrap as MemberExpression;
            });

            var memberExp = getMemberExp(this.testValidationResult.MemberAccessor);

            while (memberExp != null)
            {
                memberNames.Push(memberExp.Member.Name);
                memberExp = getMemberExp(memberExp.Expression);
            }
	        return memberNames;
	    }

        public IEnumerable<ValidationFailure> ShouldHaveValidationError(IEnumerable<MemberInfo> properties) {
			var propertyName = GetPropertyName(properties);

			var failures = testValidationResult.Result.Errors.Where(x => x.PropertyName == propertyName || string.IsNullOrEmpty(propertyName)).ToArray();

			if (!failures.Any())
				throw new ValidationTestException(string.Format("Expected a validation error for property {0}", propertyName));

			return failures;
		}

		public void ShouldNotHaveValidationError(IEnumerable<MemberInfo> properties) {
			var propertyName = GetPropertyName(properties);

			if (testValidationResult.Result.Errors.Any(x => x.PropertyName == propertyName || string.IsNullOrEmpty(propertyName)))
				throw new ValidationTestException(string.Format("Expected no validation errors for property {0}", propertyName));
		}
	}
}