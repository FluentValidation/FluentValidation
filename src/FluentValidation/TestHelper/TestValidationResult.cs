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
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Text.RegularExpressions;
	using Internal;
	using Results;

	public class TestValidationResult<T, TValue> : IValidationResultTester where T : class {
		public ValidationResult Result { get; private set; }

		[Obsolete]
		public MemberAccessor<T, TValue> MemberAccessor { get; private set; }

		public TestValidationResult(ValidationResult validationResult, MemberAccessor<T, TValue> memberAccessor) {
			Result = validationResult;
			MemberAccessor = memberAccessor;
		}

		[Obsolete("Call ShouldHaveValidationError/ShouldNotHaveValidationError instead of Which.ShouldHaveValidationError/Which.ShouldNotHaveValidationError")]
		public ITestPropertyChain<TValue> Which {
			get {
				return new TestPropertyChain<TValue, TValue>(this, Enumerable.Empty<MemberInfo>());
			}
		}

		public IEnumerable<ValidationFailure> ShouldHaveValidationErrorFor<TProperty>(Expression<Func<T, TProperty>> memberAccessor) {
			string propertyName = ValidatorOptions.PropertyNameResolver(typeof(T), memberAccessor.GetMember(), memberAccessor);
			return Result.Errors.ShouldHaveValidationError(propertyName);
		}

		public void ShouldNotHaveValidationErrorFor<TProperty>(Expression<Func<T, TProperty>> memberAccessor) {
			string propertyName = ValidatorOptions.PropertyNameResolver(typeof(T), memberAccessor.GetMember(), memberAccessor);
			Result.Errors.ShouldNotHaveValidationError(propertyName);
		}

		[Obsolete]
		IEnumerable<ValidationFailure> IValidationResultTester.ShouldHaveValidationError(IEnumerable<MemberInfo> properties) {
			var propertyName = GetPropertyName(properties);
			return Result.Errors.ShouldHaveValidationError(propertyName);
		}

		[Obsolete]
		void IValidationResultTester.ShouldNotHaveValidationError(IEnumerable<MemberInfo> properties) {
			var propertyName = GetPropertyName(properties);
			Result.Errors.ShouldNotHaveValidationError(propertyName);
		}

		[Obsolete]
		private string GetPropertyName(IEnumerable<MemberInfo> properties) {
			var propertiesList = properties.Where(x => x != null).Select(x => x.Name).ToList();

			if (MemberAccessor != null) {
				string memberName = ValidatorOptions.PropertyNameResolver(typeof(T), MemberAccessor.Member, MemberAccessor);

				if (!string.IsNullOrEmpty(memberName)) {
					propertiesList.Insert(0, memberName);
				}
			}

			return string.Join(".", propertiesList);
		}
	}

}
