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
	public class ExclusiveBetweenValidatorTests {
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
			var validator = new ExclusiveBetweenValidator(1, 10);
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeTrue();
		}

		[Test]
		public void When_the_value_is_smaller_than_the_range_then_the_validator_should_fail() {
			int value = 0;
			var validator = new ExclusiveBetweenValidator(1, 10);
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_the_text_is_larger_than_the_range_then_the_validator_should_fail() {
			int value = 11;
			var validator = new ExclusiveBetweenValidator(1, 10);
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_the_value_is_exactly_the_size_of_the_upper_bound_then_the_validator_should_fail() {
			int value = 10;
			var validator = new ExclusiveBetweenValidator(1, 10);
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_the_value_is_exactly_the_size_of_the_lower_bound_then_the_validator_should_fail() {
			int value = 1;
			var validator = new ExclusiveBetweenValidator(1, 10);
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_the_to_is_smaller_than_the_from_then_the_validator_should_throw() {
			typeof(ArgumentOutOfRangeException).ShouldBeThrownBy(() => new ExclusiveBetweenValidator(10, 1));
		}

		[Test]
		public void When_the_validator_fails_the_error_message_should_be_set() {
			var validator = new ExclusiveBetweenValidator(1, 10);
			var result =
				validator.Validate(new PropertyValidatorContext("Value", null, x => 0));
			result.Single().ErrorMessage.ShouldEqual("'Value' must be between 1 and 10 (exclusive). You entered 0.");
		}

		[Test]
		public void To_and_from_properties_should_be_set() {
			var validator = new ExclusiveBetweenValidator(1, 10);
			validator.From.ShouldEqual(1);
			validator.To.ShouldEqual(10);
		}

		[Test]
		public void When_the_value_is_between_the_range_specified_then_the_validator_should_pass_for_strings() {
			string value = "bbb";
			var validator = new ExclusiveBetweenValidator("aa", "zz");
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeTrue();
		}

		[Test]
		public void When_the_value_is_smaller_than_the_range_then_the_validator_should_fail_for_strings() {
			string value = "aaa";
			var validator = new ExclusiveBetweenValidator("bbb", "zz");
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_the_text_is_larger_than_the_range_then_the_validator_should_fail_for_strings() {
			string value = "zzz";
			var validator = new ExclusiveBetweenValidator("aaa", "bbb");
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_the_value_is_exactly_the_size_of_the_upper_bound_then_the_validator_should_fail_for_strings() {
			string value = "aa";
			var validator = new ExclusiveBetweenValidator("aa", "zz");
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_the_value_is_exactly_the_size_of_the_lower_bound_then_the_validator_should_fail_for_strings() {
			string value = "zz";
			var validator = new ExclusiveBetweenValidator("aa", "zz");
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_the_to_is_smaller_than_the_from_then_the_validator_should_throw_for_strings() {
			typeof(ArgumentOutOfRangeException).ShouldBeThrownBy(() => new ExclusiveBetweenValidator("ccc", "aaa"));
		}

		[Test]
		public void When_the_validator_fails_the_error_message_should_be_set_for_strings() {
			string value = "aaa";
			var validator = new ExclusiveBetweenValidator("bbb", "zzz");
			var result = validator.Validate(new PropertyValidatorContext("Value", null, x => value));
			result.Single().ErrorMessage.ShouldEqual("'Value' must be between bbb and zzz (exclusive). You entered aaa.");
		}

		[Test]
		public void To_and_from_properties_should_be_set_for_strings() {
			var validator = new ExclusiveBetweenValidator("a", "c");
			validator.From.ShouldEqual("a");
			validator.To.ShouldEqual("c");
		}

		[Test]
		public void When_the_value_is_between_the_range_specified_then_the_validator_should_pass_for_doubles() {
			double value = 5.0;
			var validator = new ExclusiveBetweenValidator(1.0, 10.0);
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeTrue();
		}

		[Test]
		public void When_the_value_is_smaller_than_the_range_then_the_validator_should_fail_for_doubles() {
			double value = 0.9;
			var validator = new ExclusiveBetweenValidator(1.0, 10.0);
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_the_text_is_larger_than_the_range_then_the_validator_should_fail_for_doubles() {
			double value = 10.1;
			var validator = new ExclusiveBetweenValidator(1.0, 10.0);
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_the_value_is_exactly_the_size_of_the_upper_bound_then_the_validator_should_fail_for_doubles() {
			double value = 10.0;
			var validator = new ExclusiveBetweenValidator(1.0, 10.0);
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_the_value_is_exactly_the_size_of_the_lower_bound_then_the_validator_should_fail_for_doubles() {
			double value = 1.0;
			var validator = new ExclusiveBetweenValidator(1.0, 10.0);
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_the_to_is_smaller_than_the_from_then_the_validator_should_throw_for_doubles() {
			typeof(ArgumentOutOfRangeException).ShouldBeThrownBy(() => new ExclusiveBetweenValidator(10, 1));
		}

		[Test]
		public void When_the_validator_fails_the_error_message_should_be_set_for_doubles() {
			var validator = new ExclusiveBetweenValidator(1.2, 10.9);
			var result =
				validator.Validate(new PropertyValidatorContext("Value", null, x => 0.0));
			result.Single().ErrorMessage.ShouldEqual("'Value' must be between 1.2 and 10.9 (exclusive). You entered 0.");
		}

		[Test]
		public void To_and_from_properties_should_be_set_for_doubles() {
			var validator = new ExclusiveBetweenValidator(1.1, 10.1);
			validator.From.ShouldEqual(1.1);
			validator.To.ShouldEqual(10.1);
		}

		[Test]
		public void When_the_value_is_between_the_range_specified_then_the_validator_should_pass_for_dates() {
			DateTime value = new DateTime(2009, 9, 9);
			var validator = new ExclusiveBetweenValidator(fromDate, toDate);
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeTrue();
		}

		[Test]
		public void When_the_value_is_smaller_than_the_range_then_the_validator_should_fail_for_dates() {
			DateTime value = new DateTime(2008, 1, 1);
			var validator = new ExclusiveBetweenValidator(fromDate, toDate);
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_the_text_is_larger_than_the_range_then_the_validator_should_fail_for_dates() {
			DateTime value = new DateTime(2010, 1, 1);
			var validator = new ExclusiveBetweenValidator(fromDate, toDate);
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_the_value_is_exactly_the_size_of_the_upper_bound_then_the_validator_should_fail_for_dates() {
			DateTime value = new DateTime(2009, 12, 31);
			var validator = new ExclusiveBetweenValidator(fromDate, toDate);
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_the_value_is_exactly_the_size_of_the_lower_bound_then_the_validator_should_fail_for_dates() {
			DateTime value = new DateTime(2009, 1, 1);
			var validator = new ExclusiveBetweenValidator(fromDate, toDate);
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => value));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_the_to_is_smaller_than_the_from_then_the_validator_should_throw_for_dates() {
			typeof(ArgumentOutOfRangeException).ShouldBeThrownBy(() => new ExclusiveBetweenValidator(toDate, fromDate));
		}

		[Test]
		public void When_the_validator_fails_the_error_message_should_be_set_for_dates() {
			DateTime value = new DateTime(2008, 1, 1);
			var validator = new ExclusiveBetweenValidator(fromDate, toDate);
			var result =
				validator.Validate(new PropertyValidatorContext("Value", null, x => value));
			result.Single().ErrorMessage.ShouldEqual("'Value' must be between 1/1/2009 12:00:00 AM and 12/31/2009 12:00:00 AM (exclusive). You entered 1/1/2008 12:00:00 AM.");
		}

		[Test]
		public void To_and_from_properties_should_be_set_for_dates() {
			var validator = new ExclusiveBetweenValidator(fromDate, toDate);
			validator.From.ShouldEqual(fromDate);
			validator.To.ShouldEqual(toDate);
		}
	}
}