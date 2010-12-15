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
	public class InclusiveBetweenValidatorTests {
		DateTime fromDate;
		DateTime toDate;

		[SetUp]
		public void Setup() {
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
			fromDate = new DateTime(2009, 1, 1);
			toDate = new DateTime(2009, 12, 31);
		}

		[Test]
		public void When_the_value_is_between_the_range_specified_then_the_validator_should_pass() {
			int value = 5;
			var validator = new InclusiveBetweenValidator(1, 10);
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeTrue();
		}

		[Test]
		public void When_the_value_is_smaller_than_the_range_then_the_validator_should_fail() {
			int value = 0;
			var validator = new InclusiveBetweenValidator(1, 10);
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_the_text_is_larger_than_the_range_then_the_validator_should_fail() {
			int value = 11;
			var validator = new InclusiveBetweenValidator(1, 10);
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_the_value_is_exactly_the_size_of_the_upper_bound_then_the_validator_should_pass() {
			int value = 10;
			var validator = new InclusiveBetweenValidator(1, 10);
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeTrue();
		}

		[Test]
		public void When_the_value_is_exactly_the_size_of_the_lower_bound_then_the_validator_should_pass() {
			int value = 1;
			var validator = new InclusiveBetweenValidator(1, 10);
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeTrue();
		}

		[Test]
		public void When_the_to_is_smaller_than_the_from_then_the_validator_should_throw() {
			typeof(ArgumentOutOfRangeException).ShouldBeThrownBy(() => new InclusiveBetweenValidator(10, 1));
		}

		[Test]
		public void When_the_validator_fails_the_error_message_should_be_set() {
			var validator = new InclusiveBetweenValidator(1, 10);
			var result =
				validator.Validate(new PropertyValidatorContext("Value", null, x => 0));
			result.Single().ErrorMessage.ShouldEqual("'Value' must be between 1 and 10. You entered 0.");
		}

		[Test]
		public void To_and_from_properties_should_be_set() {
			var validator = new InclusiveBetweenValidator(1, 10);
			validator.From.ShouldEqual(1);
			validator.To.ShouldEqual(10);
		}

		[Test]
		public void When_the_value_is_between_the_range_specified_then_the_validator_should_pass_for_strings() {
			string value = "bbb";
			var validator = new InclusiveBetweenValidator("aa", "zz");
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeTrue();
		}

		[Test]
		public void When_the_value_is_smaller_than_the_range_then_the_validator_should_fail_for_strings() {
			string value = "aaa";
			var validator = new InclusiveBetweenValidator("bbb", "zz");
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_the_text_is_larger_than_the_range_then_the_validator_should_fail_for_strings() {
			string value = "zzz";
			var validator = new InclusiveBetweenValidator("aaa", "bbb");
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_the_value_is_exactly_the_size_of_the_upper_bound_then_the_validator_should_pass_for_strings() {
			string value = "aa";
			var validator = new InclusiveBetweenValidator("aa", "zz");
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeTrue();
		}

		[Test]
		public void When_the_value_is_exactly_the_size_of_the_lower_bound_then_the_validator_should_pass_for_strings() {
			string value = "zz";
			var validator = new InclusiveBetweenValidator("aa", "zz");
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeTrue();
		}

		[Test]
		public void When_the_to_is_smaller_than_the_from_then_the_validator_should_throw_for_strings() {
			typeof(ArgumentOutOfRangeException).ShouldBeThrownBy(() => new InclusiveBetweenValidator("ccc", "aaa"));
		}

		[Test]
		public void When_the_validator_fails_the_error_message_should_be_set_for_strings() {
			string value = "aaa";
			var validator = new InclusiveBetweenValidator("bbb", "zzz");
			var result = validator.Validate(new PropertyValidatorContext("Value", null, x => value));
			result.Single().ErrorMessage.ShouldEqual("'Value' must be between bbb and zzz. You entered aaa.");
		}

		[Test]
		public void To_and_from_properties_should_be_set_for_strings() {
			var validator = new InclusiveBetweenValidator("a", "c");
			validator.From.ShouldEqual("a");
			validator.To.ShouldEqual("c");
		}

	
		[Test]
		public void To_and_from_properties_should_be_set_for_dates() {
			var validator = new InclusiveBetweenValidator(fromDate, toDate);
			validator.From.ShouldEqual(fromDate);
			validator.To.ShouldEqual(toDate);
		}
	}
}