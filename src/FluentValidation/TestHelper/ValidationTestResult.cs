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
    using System.Collections.Generic;
    using System.Linq;
    using Results;

    public class ValidationTestResult : IValidationTestResult {
        readonly IList<ValidationFailure> failures;

        public ValidationTestResult(IList<ValidationFailure> failures) {
            this.failures = failures;
        }

        public IValidationTestResult WithCustomState<T>(T state) {
            var count = failures.Count(failure => failure.CustomState == (object) state);

            if (count > 0) {
                throw new ValidationTestException(string.Format("Expected custom state of '{0}'.", state));
            }

            return this;
        }

        public IValidationTestResult WithMessage(string message) {
            var count = failures.Count(failure => failure.ErrorMessage == message);

            if (count > 0) {
                throw new ValidationTestException(string.Format("Expected an error message of '{0}'.", message));
            }

            return this;
        }

        public IValidationTestResult WithErrorCode(string errorCode) {
            var count = failures.Count(failure => failure.ErrorCode == errorCode);

            if (count > 0) {
                throw new ValidationTestException(string.Format("Expected an error code of '{0}'.", errorCode));
            }

            return this;
        }
    }
}