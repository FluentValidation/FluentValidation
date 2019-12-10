#region License

// Copyright (c) Jeremy Skinner (http://www.jeremyskinner.co.uk)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// You may not use this file except in compliance with the License.
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
// The latest version of this file can be found at http://github.com/JeremySkinner/FluentValidation

#endregion

namespace FluentValidation.TestHelper {
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using Internal;
	using Results;

	public class TestValidationResult<T> : ValidationResult where T : class {

		public TestValidationResult(ValidationResult validationResult) : base(validationResult.Errors){
			RuleSetsExecuted = validationResult.RuleSetsExecuted;
		}

		public IEnumerable<ValidationFailure> ShouldHaveValidationErrorFor<TProperty>(Expression<Func<T, TProperty>> memberAccessor) {
			string propertyName = ValidatorOptions.Global.PropertyNameResolver(typeof(T), memberAccessor.GetMember(), memberAccessor);
			return ValidationTestExtension.ShouldHaveValidationError(Errors, propertyName, true);
		}

		public void ShouldNotHaveValidationErrorFor<TProperty>(Expression<Func<T, TProperty>> memberAccessor) {
			string propertyName = ValidatorOptions.Global.PropertyNameResolver(typeof(T), memberAccessor.GetMember(), memberAccessor);
			ValidationTestExtension.ShouldNotHaveValidationError(Errors, propertyName, true);
		}

		public IEnumerable<ValidationFailure> ShouldHaveValidationErrorFor(string propertyName) {
			return ValidationTestExtension.ShouldHaveValidationError(Errors, propertyName, false);
		}

		public void ShouldNotHaveValidationErrorFor(string propertyName) {
			ValidationTestExtension.ShouldNotHaveValidationError(Errors, propertyName, false);
		}
	}

}
