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
	using System.Text.RegularExpressions;
	using System.Threading;
	using Xunit;
	using Validators;

	
	public class RegularExpressionValidatorTests {
		TestValidator validator;
		TestValidator validator2;
		TestValidator validator3;

		public  RegularExpressionValidatorTests() {
			CultureScope.SetDefaultCulture();
			validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).Matches(@"^\w\d$")
			};

			validator2 = new TestValidator {
				v => v.RuleFor(x => x.Surname).Matches(x => x.Regex)
			};

			validator3 = new TestValidator {
				v => v.RuleFor(x => x.Surname).Matches(x => x.AnotherRegex)
			};
		}

		[Fact]
		public void When_the_text_matches_the_regular_expression_then_the_validator_should_pass() {
			string input = "S3";
			var result = validator.Validate(new Person{Surname = input });
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void When_the_text_does_not_match_the_regular_expression_then_the_validator_should_fail() {
			var result = validator.Validate(new Person{Surname = "S33"});
			result.IsValid.ShouldBeFalse();

			result = validator.Validate(new Person{Surname = " 5"});
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_the_text_is_empty_then_the_validator_should_fail() {
			var result = validator.Validate(new Person{Surname = ""});
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_the_text_is_null_then_the_validator_should_pass() {
			var result = validator.Validate(new Person{Surname = null});
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void When_validation_fails_the_default_error_should_be_set() {
			var result = validator.Validate(new Person{Surname = "S33"});
			result.Errors.Single().ErrorMessage.ShouldEqual("'Surname' is not in the correct format.");
		}

		[Fact]
		public void When_the_text_matches_the_lambda_regular_expression_then_the_validator_should_pass()
		{
			string input = "S3";
			var result = validator2.Validate(new Person { Surname = input, Regex = @"^\w\d$" });
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void When_the_text_matches_the_lambda_regex_regular_expression_then_the_validator_should_pass()
		{
			string input = "S3";
			var result = validator3.Validate(new Person { Surname = input, AnotherRegex = new System.Text.RegularExpressions.Regex(@"^\w\d$") });
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void When_the_text_does_not_match_the_lambda_regular_expression_then_the_validator_should_fail()
		{
			var result = validator2.Validate(new Person { Surname = "S33", Regex = @"^\w\d$" });
			result.IsValid.ShouldBeFalse();

			result = validator2.Validate(new Person { Surname = " 5", Regex = @"^\w\d$" });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_the_text_does_not_match_the_lambda_regex_regular_expression_then_the_validator_should_fail()
		{
			var result = validator3.Validate(new Person { Surname = "S33", AnotherRegex = new System.Text.RegularExpressions.Regex(@"^\w\d$") });
			result.IsValid.ShouldBeFalse();

			result = validator3.Validate(new Person { Surname = " 5", AnotherRegex = new System.Text.RegularExpressions.Regex(@"^\w\d$") });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void Can_access_expression_in_message() {
			var v = new TestValidator();
			v.RuleFor(x => x.Forename).Matches(@"^\w\d$").WithMessage("test {RegularExpression}");

			var result = v.Validate(new Person {Forename = ""});
			result.Errors.Single().ErrorMessage.ShouldEqual(@"test ^\w\d$");
		}

		[Fact]
		public void Can_access_expression_in_message_lambda()
		{
			var v = new TestValidator();
			v.RuleFor(x => x.Forename).Matches(x => x.Regex).WithMessage("test {RegularExpression}");

			var result = v.Validate(new Person { Forename = "", Regex = @"^\w\d$" });
			result.Errors.Single().ErrorMessage.ShouldEqual(@"test ^\w\d$");
		}

		[Fact]
		public void Can_access_expression_in_message_lambda_regex()
		{
			var v = new TestValidator();
			v.RuleFor(x => x.Forename).Matches(x => x.AnotherRegex).WithMessage("test {RegularExpression}");

			var result = v.Validate(new Person { Forename = "", AnotherRegex = new System.Text.RegularExpressions.Regex(@"^\w\d$") });
			result.Errors.Single().ErrorMessage.ShouldEqual(@"test ^\w\d$");
		}


		[Fact]
		public void Uses_regex_object()
		{
			var validator = new TestValidator(v => v.RuleFor(x => x.Surname).Matches(new Regex(@"^\w\d$")));
			string input = "S3";
			var result = validator.Validate(new Person { Surname = input });
			result.IsValid.ShouldBeTrue();
		}


		[Fact]
		public void Uses_lazily_loaded_expression() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Surname).Matches(x => @"^\w\d$"));
			string input = "S3";
			var result = validator.Validate(new Person { Surname = input });
			result.IsValid.ShouldBeTrue();
		}


		[Fact]
		public void Uses_lazily_loaded_expression_with_options()
		{
			var validator = new TestValidator(v => v.RuleFor(x => x.Surname).Matches(@"^\w\d$", RegexOptions.Compiled));
			string input = "S3";
			var result = validator.Validate(new Person { Surname = input });
			result.IsValid.ShouldBeTrue();
		}
	}
}