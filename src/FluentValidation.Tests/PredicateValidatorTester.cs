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

namespace FluentValidation.Tests {
	using System;
	using System.Globalization;
	using System.Linq;
	using System.Threading;
	using Xunit;
	using Validators;

	
	public class PredicateValidatorTester {
		private TestValidator validator;

		public PredicateValidatorTester() {
           CultureScope.SetDefaultCulture();
            validator = new TestValidator {
				v => v.RuleFor(x => x.Forename).Must(forename => forename == "Jeremy")
			};
		}

		[Fact]
		public void Should_fail_when_predicate_returns_false() {
			var result = validator.Validate(new Person{Forename = "Foo"});
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void Should_succeed_when_predicate_returns_true() {
			var result = validator.Validate(new Person{Forename = "Jeremy"});
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Should_throw_when_predicate_is_null() {
			typeof(ArgumentNullException).ShouldBeThrownBy(() =>
				new TestValidator(v => v.RuleFor(x => x.Surname).Must((Func<string, bool>)null))	
			);
		}

		[Fact]
		public void When_validation_fails_the_default_error_should_be_set() {
			var result = validator.Validate(new Person{Forename = "Foo"});
			result.Errors.Single().ErrorMessage.ShouldEqual("The specified condition was not met for 'Forename'.");
		}

		[Fact]
		public void When_validation_fails_metadata_should_be_set_on_failure() {
			var validator = new TestValidator() {
													v => v.RuleFor(x => x.Forename)
														.Must(forename => forename == "Jeremy")
														.WithLocalizedMessage(()=>TestMessages.ValueOfForPropertyNameIsNotValid, x => x.Forename)
												};

			var result = validator.Validate(new Person() { Forename = "test" });
			var error = result.Errors.Single();

			error.ShouldNotBeNull();
			error.PropertyName.ShouldEqual("Forename");
			error.AttemptedValue.ShouldEqual("test");
			error.ErrorCode.ShouldEqual("PredicateValidator");

			error.FormattedMessageArguments.Length.ShouldEqual(1);
			error.FormattedMessageArguments[0].ShouldEqual("test");

			error.FormattedMessagePlaceholderValues.Count.ShouldEqual(2);
			error.FormattedMessagePlaceholderValues.ContainsKey("PropertyName").ShouldBeTrue();
			error.FormattedMessagePlaceholderValues.ContainsKey("PropertyValue").ShouldBeTrue();

			error.FormattedMessagePlaceholderValues["PropertyName"].ShouldEqual("Forename");
			error.FormattedMessagePlaceholderValues["PropertyValue"].ShouldEqual("test");
		}
	}
}