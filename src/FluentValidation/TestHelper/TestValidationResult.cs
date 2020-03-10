#region License

// Copyright (c) Jeremy Skinner (http://www.jeremyskinner.co.uk) and contributors.
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
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using Internal;
	using Results;

	public class TestValidationResult<T> : ValidationResult where T : class {

		public TestValidationResult(ValidationResult validationResult) : base(validationResult.Errors){
			RuleSetsExecuted = validationResult.RuleSetsExecuted;
		}

		/// <summary>
		///  Whether this set of assertions set the property value inline (instead of supplying a pre-populated instance).
		/// </summary>
		internal bool ValueSetInline { get; set; }

		public IEnumerable<ValidationFailure> ShouldHaveValidationErrorFor<TProperty>(Expression<Func<T, TProperty>> memberAccessor) {
			// If the property we're asserting against is a collection (any IEnumerable except string)
			// then allow assertions against the property without the indexer.
			// Eg if the error is for "Addresses[0]" then allow assertions to pass if they're against "Addresses".
			// Note that this is only allowed when using the test extension that sets the property at the same time, as it's
			// not possible to pass in a collection value and specify the error is against a specific index.
			string propertyName = PropertyChain.FromExpression(memberAccessor, true).ToString();
			bool shouldStripFinalIndexer = ValueSetInline && typeof(IEnumerable).IsAssignableFrom(typeof(TProperty)) && typeof(TProperty) != typeof(string);
			return ValidationTestExtension.ShouldHaveValidationError(Errors, propertyName, shouldStripFinalIndexer);
		}

		public void ShouldNotHaveValidationErrorFor<TProperty>(Expression<Func<T, TProperty>> memberAccessor) {
			string propertyName = PropertyChain.FromExpression(memberAccessor, true).ToString();
			bool shouldStripFinalIndexer = ValueSetInline && typeof(IEnumerable).IsAssignableFrom(typeof(TProperty)) && typeof(TProperty) != typeof(string);
			ValidationTestExtension.ShouldNotHaveValidationError(Errors, propertyName, shouldStripFinalIndexer);
		}

		public IEnumerable<ValidationFailure> ShouldHaveValidationErrorFor(string propertyName) {
			return ValidationTestExtension.ShouldHaveValidationError(Errors, propertyName, false);
		}

		public void ShouldNotHaveValidationErrorFor(string propertyName) {
			ValidationTestExtension.ShouldNotHaveValidationError(Errors, propertyName, false);
		}
	}

}
