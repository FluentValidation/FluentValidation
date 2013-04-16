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
	public class LengthValidatorTests {
		[SetUp]
		public void Setup() {
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
		}

		[Test]
		public void When_the_text_is_between_the_range_specified_then_the_validator_should_pass() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Surname).Length(1, 10));
			var result = validator.Validate(new Person { Surname = "Test"});
			result.IsValid.ShouldBeTrue();
		}

		[Test]
		public void When_the_text_is_smaller_than_the_range_then_the_validator_should_fail() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Surname).Length(5, 10));
			var result = validator.Validate(new Person { Surname = "Test" });
			result.IsValid.ShouldBeFalse();
		}

		[Test]
		public void When_the_text_is_larger_than_the_range_then_the_validator_should_fail() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Surname).Length(1, 2));
			var result = validator.Validate(new Person { Surname = "Test" });
			result.IsValid.ShouldBeFalse();
		}

		[Test]
		public void When_the_text_is_exactly_the_size_of_the_upper_bound_then_the_validator_should_pass() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Surname).Length(1, 4));
			var result = validator.Validate(new Person{Surname = "Test"});
			result.IsValid.ShouldBeTrue();
		}

		[Test]
		public void When_the_text_is_exactly_the_size_of_the_lower_bound_then_the_validator_should_pass() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Surname).Length(4, 5));
			var result = validator.Validate(new Person { Surname = "Test" });
			result.IsValid.ShouldBeTrue();
		}

		[Test]
		public void When_the_max_is_smaller_than_the_min_then_the_validator_should_throw() {
			typeof(ArgumentOutOfRangeException).ShouldBeThrownBy(() => 
				new TestValidator(v => v.RuleFor(x => x.Surname).Length(10,1))

				);
		}

		[Test]
		public void When_the_validator_fails_the_error_message_should_be_set() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Surname).Length(1, 2));
			var result = validator.Validate(new Person { Surname = "Gire and gimble in the wabe" });
			result.Errors.Single().ErrorMessage.ShouldEqual("'Surname' must be between 1 and 2 characters. You entered 27 characters.");
		}

		[Test]
		public void Min_and_max_properties_should_be_set() {
			var validator = new LengthValidator(1, 5);
			validator.Min.ShouldEqual(1);
			validator.Max.ShouldEqual(5);
		}

		[Test]
		public void When_input_is_null_then_the_validator_should_pass() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).Length(5)
			};

			var result = validator.Validate(new Person {Surname = null});
			result.IsValid.ShouldBeTrue();
		}
	}
}