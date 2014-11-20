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

namespace FluentValidation.Tests {
	using System;
	using System.Globalization;
	using System.Linq;
	using System.Threading;
	using Xunit;
	using Validators;

	
	public class EmailValidatorTests {
		TestValidator validator;

		public EmailValidatorTests() {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            validator = new TestValidator {
				v => v.RuleFor(x => x.Email).EmailAddress()
			};
		}

		[Fact]
		public void When_the_text_is_empty_then_the_validator_should_fail() {
			string email = String.Empty;
			var result = validator.Validate(new Person { Email = email });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_the_text_is_not_a_valid_email_address_then_the_validator_should_fail() {
			string email = "testperso";
			var result = validator.Validate(new Person { Email = email });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_validation_fails_the_default_error_should_be_set() {
			string email = "testperso";
		var result = validator.Validate(new Person { Email = email });
			result.Errors.Single().ErrorMessage.ShouldEqual("'Email' is not a valid email address.");
		}

		[Fact]
		public void This_should_not_hang() {
			string email = "thisisaverylongstringcodeplex.com";
			var result = validator.Validate(new Person { Email = email });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void double_period_with_uk_Domain() {
			string email = "first.last@test..co.uk";
			var result = validator.Validate(new Person {Email = email});
			result.IsValid.ShouldBeFalse();
		}

		[Xunit.Extensions.Theory]
		[Xunit.Extensions.InlineData((string)null)]
		[Xunit.Extensions.InlineData("testperson@gmail.com")]
		[Xunit.Extensions.InlineData("TestPerson@gmail.com")]
		[Xunit.Extensions.InlineData("testperson+label@gmail.com")]
		[Xunit.Extensions.InlineData("\"Abc\\@def\"@example.com")]
		[Xunit.Extensions.InlineData("\"Fred Bloggs\"@example.com")]
		[Xunit.Extensions.InlineData("\"Joe\\Blow\"@example.com")]
		[Xunit.Extensions.InlineData("\"Abc@def\"@example.com")]
		[Xunit.Extensions.InlineData("customer/department=shipping@example.com")]
		[Xunit.Extensions.InlineData("$A12345@example.com")]
		[Xunit.Extensions.InlineData("!def!xyz%abc@example.com")]
		[Xunit.Extensions.InlineData("__somename@example.com")]
		[Xunit.Extensions.InlineData("first.last@test.co.uk")]
		public void Valid_email_addresses(string email) {
				var result = validator.Validate(new Person {Email = email});
				result.IsValid.ShouldBeTrue(string.Format("The email address {0} should be valid", email));
		}
	}
}