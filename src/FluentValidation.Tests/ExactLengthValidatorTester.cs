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
	using System.Globalization;
	using System.Linq;
	using System.Threading;
	using Xunit;
	using Validators;

	
	public class ExactLengthValidatorTester {

		public ExactLengthValidatorTester() {
			CultureScope.SetDefaultCulture();
		}

		[Fact]
		public void When_the_text_is_an_exact_length_the_validator_should_pass() {
			var validator = new TestValidator { v => v.RuleFor(x => x.Surname).Length(4) };
			var result = validator.Validate(new Person { Surname = "test" });
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void When_the_text_length_is_smaller_the_validator_should_fail() {
			var validator = new TestValidator {v => v.RuleFor(x => x.Surname).Length(10) };
			var result = validator.Validate(new Person { Surname = "test" });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_the_text_length_is_larger_the_validator_should_fail() {
			var validator = new TestValidator { v => v.RuleFor(x => x.Surname).Length(1) };
			var result = validator.Validate(new Person { Surname = "test" });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_the_validator_fails_the_error_message_should_be_set() {
			var validator = new TestValidator { v => v.RuleFor(x => x.Surname).Length(2) };
			var result = validator.Validate(new Person() { Surname = "test"});
			result.Errors.Single().ErrorMessage.ShouldEqual("'Surname' must be 2 characters in length. You entered 4 characters.");
		}

		[Fact]
		public void Min_and_max_properties_should_be_set() {
			var validator = new ExactLengthValidator(5);
			validator.Min.ShouldEqual(5);
			validator.Max.ShouldEqual(5);
		}
		
		[Fact]
		public void When_exact_length_rule_failes_error_should_have_exact_length_error_errorcode() {
			var validator = new TestValidator { v => v.RuleFor(x => x.Surname).Length(2) };
			
			var result = validator.Validate(new Person() { Surname = "test" });
			var error = result.Errors.SingleOrDefault(e => e.ErrorCode == "ExactLengthValidator");

			error.ShouldNotBeNull();
			error.PropertyName.ShouldEqual("Surname");
			error.AttemptedValue.ShouldEqual("test");
			error.FormattedMessageArguments.Length.ShouldEqual(0);

			error.FormattedMessagePlaceholderValues.Count.ShouldEqual(5);
			error.FormattedMessagePlaceholderValues.ContainsKey("PropertyName").ShouldBeTrue();
			error.FormattedMessagePlaceholderValues.ContainsKey("PropertyValue").ShouldBeTrue();
			error.FormattedMessagePlaceholderValues.ContainsKey("MinLength").ShouldBeTrue();
			error.FormattedMessagePlaceholderValues.ContainsKey("MaxLength").ShouldBeTrue();
			error.FormattedMessagePlaceholderValues.ContainsKey("TotalLength").ShouldBeTrue();

			error.FormattedMessagePlaceholderValues["PropertyName"].ShouldEqual("Surname");
			error.FormattedMessagePlaceholderValues["PropertyValue"].ShouldEqual("test");
			error.FormattedMessagePlaceholderValues["MinLength"].ShouldEqual(2);
			error.FormattedMessagePlaceholderValues["MaxLength"].ShouldEqual(2);
			error.FormattedMessagePlaceholderValues["TotalLength"].ShouldEqual(4);
		}
	}
}