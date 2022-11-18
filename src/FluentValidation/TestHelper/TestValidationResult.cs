#region License

// Copyright (c) .NET Foundation and contributors.
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

namespace FluentValidation.TestHelper;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Internal;
using Results;

public class TestValidationResult<T> : ValidationResult {

	public TestValidationResult(ValidationResult validationResult) : base(validationResult.Errors){
		RuleSetsExecuted = validationResult.RuleSetsExecuted;
	}

	public ITestValidationWith ShouldHaveValidationErrorFor<TProperty>(Expression<Func<T, TProperty>> memberAccessor) {
		string propertyName = ValidatorOptions.Global.PropertyNameResolver(typeof(T), memberAccessor.GetMember(), memberAccessor);
		return ShouldHaveValidationError(propertyName, true);
	}

	public void ShouldNotHaveValidationErrorFor<TProperty>(Expression<Func<T, TProperty>> memberAccessor) {
		string propertyName = ValidatorOptions.Global.PropertyNameResolver(typeof(T), memberAccessor.GetMember(), memberAccessor);
		ShouldNotHaveValidationError(propertyName, true);
	}

	public void ShouldNotHaveAnyValidationErrors() {
		ShouldNotHaveValidationError(ValidationTestExtension.MatchAnyFailure, true);
	}

	public ITestValidationContinuation ShouldHaveAnyValidationError() {
		if (!Errors.Any())
			throw new ValidationTestException($"Expected at least one validation error, but none were found.");

		return new TestValidationContinuation(Errors);
	}

	public ITestValidationWith ShouldHaveValidationErrorFor(string propertyName) {
		return ShouldHaveValidationError(propertyName, false);
	}

	public void ShouldNotHaveValidationErrorFor(string propertyName) {
		ShouldNotHaveValidationError(propertyName, false);
	}

	private ITestValidationWith ShouldHaveValidationError(string propertyName, bool shouldNormalizePropertyName) {
		var result = new TestValidationContinuation(Errors);
		result.ApplyPredicate(x => (shouldNormalizePropertyName ?  NormalizePropertyName(x.PropertyName) == propertyName : x.PropertyName == propertyName)
		                           || (string.IsNullOrEmpty(x.PropertyName) && string.IsNullOrEmpty(propertyName))
		                           || propertyName == ValidationTestExtension.MatchAnyFailure);

		if (result.Any()) {
			return result;
		}

		// We expected an error but failed to match it.
		var errorMessageBanner = $"Expected a validation error for property {propertyName}";

		string errorMessage = "";

		if (Errors?.Any() == true) {
			string errorMessageDetails = "";
			for (int i = 0; i < Errors.Count; i++) {
				errorMessageDetails += $"[{i}]: {Errors[i].PropertyName}\n";
			}
			errorMessage = $"{errorMessageBanner}\n----\nProperties with Validation Errors:\n{errorMessageDetails}";
		}
		else {
			errorMessage = $"{errorMessageBanner}";
		}

		throw new ValidationTestException(errorMessage);
	}

	private void ShouldNotHaveValidationError(string propertyName, bool shouldNormalizePropertyName) {
		var failures = Errors.Where(x => (shouldNormalizePropertyName ? NormalizePropertyName(x.PropertyName) == propertyName : x.PropertyName == propertyName)
		                                 || (string.IsNullOrEmpty(x.PropertyName) && string.IsNullOrEmpty(propertyName))
		                                 || propertyName == ValidationTestExtension.MatchAnyFailure
		).ToList();

		if (failures.Any()) {
			var errorMessageBanner = $"Expected no validation errors for property {propertyName}";
			if (propertyName == ValidationTestExtension.MatchAnyFailure) {
				errorMessageBanner = "Expected no validation errors";
			}
			string errorMessageDetails = "";
			for (int i = 0; i < failures.Count; i++) {
				errorMessageDetails += $"[{i}]: {failures[i].ErrorMessage}\n";
			}
			var errorMessage = $"{errorMessageBanner}\n----\nValidation Errors:\n{errorMessageDetails}";
			throw new ValidationTestException(errorMessage, failures);
		}
	}

	private static string NormalizePropertyName(string propertyName) {
		return Regex.Replace(propertyName, @"\[.*\]", string.Empty);
	}
}
