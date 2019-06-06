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

	// TODO: Remove the TValue generic and the IValidationResultTester interface from this for 9.0.
	public class TestValidationResult<T, TValue> : ValidationResult, IValidationResultTester where T : class {

		[Obsolete("Use properties on the parent class itself")]
		public ValidationResult Result { get; private set; }

		[Obsolete]
		public MemberAccessor<T, TValue> MemberAccessor { get; private set; }

		public TestValidationResult(ValidationResult validationResult, MemberAccessor<T, TValue> memberAccessor) : base(validationResult.Errors){
			Result = validationResult;
			MemberAccessor = memberAccessor;
			RuleSetsExecuted = validationResult.RuleSetsExecuted;
		}

		[Obsolete("Call ShouldHaveValidationError/ShouldNotHaveValidationError instead of Which.ShouldHaveValidationError/Which.ShouldNotHaveValidationError")]
		public ITestPropertyChain<TValue> Which {
			get {
				return new TestPropertyChain<TValue, TValue>(this, Enumerable.Empty<MemberInfo>());
			}
		}

		public IEnumerable<ValidationFailure> ShouldHaveValidationErrorFor<TProperty>(Expression<Func<T, TProperty>> memberAccessor) {
			string propertyName = ValidatorOptions.PropertyNameResolver(typeof(T), memberAccessor.GetMember(), memberAccessor);
			return ValidationTestExtension.ShouldHaveValidationError(Errors, propertyName);
		}

		public void ShouldNotHaveValidationErrorFor<TProperty>(Expression<Func<T, TProperty>> memberAccessor) {
			string propertyName = ValidatorOptions.PropertyNameResolver(typeof(T), memberAccessor.GetMember(), memberAccessor);
			ValidationTestExtension.ShouldNotHaveValidationError(Errors, propertyName);
		}

		public IEnumerable<ValidationFailure> ShouldHaveValidationErrorFor(string propertyName) {
			return ValidationTestExtension.ShouldHaveValidationError(Errors, propertyName);
		}

		public void ShouldNotHaveValidationErrorFor(string propertyName) {
			ValidationTestExtension.ShouldNotHaveValidationError(Errors, propertyName);
		}

		[Obsolete]
		IEnumerable<ValidationFailure> IValidationResultTester.ShouldHaveValidationError(IEnumerable<MemberInfo> properties) {
			var propertyName = properties.Any() ? GetPropertyName(properties) : ValidationTestExtension.MatchAnyFailure;
			return ValidationTestExtension.ShouldHaveValidationError(Errors, propertyName);
		}

		[Obsolete]
		void IValidationResultTester.ShouldNotHaveValidationError(IEnumerable<MemberInfo> properties) {
			var propertyName = properties.Any() ? GetPropertyName(properties) : ValidationTestExtension.MatchAnyFailure;
			ValidationTestExtension.ShouldNotHaveValidationError(Errors, propertyName);
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
