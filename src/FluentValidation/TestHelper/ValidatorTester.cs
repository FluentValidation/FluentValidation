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
    using Results;

    public class ValidatorTester<T, TValue> where T : class {
        readonly MemberAccessor<T, TValue> accessor;
        readonly string ruleSet;
        readonly IValidator<T> validator;
        readonly TValue value;

        public ValidatorTester(Expression<Func<T, TValue>> expression, IValidator<T> validator, TValue value, string ruleSet = null) {
            this.validator = validator;
            this.value = value;
            accessor = expression;
            this.ruleSet = ruleSet;
        }

        public IList<ValidationFailure> ValidateNoError(T instanceToValidate) {
            accessor.Set(instanceToValidate, value);
            var failures = validator.Validate(instanceToValidate, ruleSet: ruleSet).Errors;
            var count = failures.Count(x => x.PropertyName == accessor.Member.Name);

            if (count > 0) {
                throw new ValidationTestException(string.Format("Expected no validation errors for property {0}", accessor.Member.Name));
            }

            return failures;
        }

        public IList<ValidationFailure> ValidateError(T instanceToValidate) {
            accessor.Set(instanceToValidate, value);
            var failures = validator.Validate(instanceToValidate, ruleSet: ruleSet).Errors;
            var count = failures.Count(x => x.PropertyName == accessor.Member.Name);

            if (count == 0) {
                throw new ValidationTestException(string.Format("Expected a validation error for property {0}", accessor.Member.Name));
            }

            return failures;
        }
    }
}