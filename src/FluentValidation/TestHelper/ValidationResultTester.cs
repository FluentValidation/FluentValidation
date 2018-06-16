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
    using System.Text.RegularExpressions;
    using Results;
	using FluentValidation.Internal;
	
	class ValidationResultTester<T, TValue> : IValidationResultTester where T : class {
		readonly TestValidationResult<T, TValue> _testValidationResult;

		public ValidationResultTester(TestValidationResult<T, TValue> testValidationResult) {
			this._testValidationResult = testValidationResult;
		}

		string GetPropertyName(IEnumerable<MemberInfo> properties) {
			return string.Join(".", 
                GetMemberNames().Concat(properties
				.Where(x => x != null)
				.Select(x => x.Name)));
		}

	    private IEnumerable<string> GetMemberNames() {
	        if (_testValidationResult.MemberAccessor == null) {
		        yield break;
	        }

		    string memberName = ValidatorOptions.PropertyNameResolver(
			    typeof(T), 
			    _testValidationResult.MemberAccessor.Member,  
			    _testValidationResult.MemberAccessor
			);
		    
		    yield return memberName;
	    }

        public IEnumerable<ValidationFailure> ShouldHaveValidationError(IEnumerable<MemberInfo> properties) {
			var propertyName = GetPropertyName(properties);

			var failures = _testValidationResult.Result.Errors.Where(x => NormalizePropertyName(x.PropertyName) == propertyName || string.IsNullOrEmpty(propertyName)).ToArray();

			if (!failures.Any())
				throw new ValidationTestException(string.Format("Expected a validation error for property {0}", propertyName));

			return failures;
		}

		public void ShouldNotHaveValidationError(IEnumerable<MemberInfo> properties) {
			var propertyName = GetPropertyName(properties);

			var failures = _testValidationResult.Result.Errors.Where(x => NormalizePropertyName(x.PropertyName) == propertyName || string.IsNullOrEmpty(propertyName)).ToList();

			if (failures.Any())
				throw new ValidationTestException(string.Format("Expected no validation errors for property {0}", propertyName), failures);
		}

		private string NormalizePropertyName(string propertyName) {
			return Regex.Replace(propertyName, @"\[.*\]", string.Empty);
		}
	}
}