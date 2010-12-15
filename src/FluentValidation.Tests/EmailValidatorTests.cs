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
	using NUnit.Framework;
	using Validators;

	[TestFixture]
	public class EmailValidatorTests {
		TestValidator validator;

		[SetUp]
		public void Setup() {
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

			validator = new TestValidator {
				v => v.RuleFor(x => x.Email).EmailAddress()
			};
		}

		[Test]
		public void When_the_text_is_a_valid_email_address_then_the_validator_should_pass() {
			string email = "testperson@gmail.com";
			var result = validator.Validate(new Person { Email = email });
			result.IsValid.ShouldBeTrue();
		}

		[Test]
		public void When_the_text_is_a_valid_email_address_including_plus_validator_should_pass() {
			string email = "testperson+label@gmail.com";
			var result = validator.Validate(new Person { Email = email });
			result.IsValid.ShouldBeTrue();
		}

		[Test]
		public void When_the_text_is_null_then_the_validator_should_pass() {
			string email = null;
			var result = validator.Validate(new Person { Email = email });
			result.IsValid.ShouldBeTrue();
		}

		[Test]
		public void When_the_text_is_empty_then_the_validator_should_fail() {
			string email = String.Empty;
			var result = validator.Validate(new Person { Email = email });
			result.IsValid.ShouldBeFalse();
		}

		[Test]
		public void When_the_text_is_not_a_valid_email_address_then_the_validator_should_fail() {
			string email = "testperso";
			var result = validator.Validate(new Person { Email = email });
			result.IsValid.ShouldBeFalse();
		}

		[Test]
		public void When_validation_fails_the_default_error_should_be_set() {
			string email = "testperso";
		var result = validator.Validate(new Person { Email = email });
			result.Errors.Single().ErrorMessage.ShouldEqual("'Email' is not a valid email address.");
		}

		[Test]
		public void This_should_not_hang() {
			string email = "thisisaverylongstringcodeplex.com";
			var result = validator.Validate(new Person { Email = email });
			result.IsValid.ShouldBeFalse();
		}

		[Test]
		public void When_email_address_contains_upper_cases_then_the_validator_should_pass() {
			string email = "testperson@gmail.com";
			var result = validator.Validate(new Person { Email = email });
			result.IsValid.ShouldBeTrue();

			email = "TestPerson@gmail.com";
			result = validator.Validate(new Person { Email = email });
			result.IsValid.ShouldBeTrue();
		}
	}
}