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

	
	public class ExclusiveBetweenValidatorTests {
		DateTime fromDate;
        DateTime toDate;

        public ExclusiveBetweenValidatorTests() {
			CultureScope.SetDefaultCulture();
			fromDate = new DateTime(2009, 1, 1);
			toDate = new DateTime(2009, 12, 31);
		}

		[Fact]
		public void When_the_value_is_between_the_range_specified_then_the_validator_should_pass() {
			var validator = new TestValidator { v => v.RuleFor(x => x.Id).ExclusiveBetween(1, 10) };
			var result = validator.Validate(new Person { Id = 5 });
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void When_the_value_is_smaller_than_the_range_then_the_validator_should_fail() {
			var validator = new TestValidator { v => v.RuleFor(x => x.Id).ExclusiveBetween(1, 10) };
			var result = validator.Validate(new Person { Id = 0 });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_the_text_is_larger_than_the_range_then_the_validator_should_fail() {
			var validator = new TestValidator { v => v.RuleFor(x => x.Id).ExclusiveBetween(1, 10) };
			var result = validator.Validate(new Person { Id = 11 });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_the_value_is_exactly_the_size_of_the_upper_bound_then_the_validator_should_fail() {
			var validator = new TestValidator { v => v.RuleFor(x => x.Id).ExclusiveBetween(1, 10) };
			var result = validator.Validate(new Person { Id = 10 });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_the_value_is_exactly_the_size_of_the_lower_bound_then_the_validator_should_fail() {
			var validator = new TestValidator { v => v.RuleFor(x => x.Id).ExclusiveBetween(1, 10) };
			var result = validator.Validate(new Person { Id = 1 });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_the_to_is_smaller_than_the_from_then_the_validator_should_throw() {
			typeof(ArgumentOutOfRangeException).ShouldBeThrownBy(() => new TestValidator{v => v.RuleFor(x => x.Id).ExclusiveBetween(10, 1)});
		}

		[Fact]
		public void When_the_validator_fails_the_error_message_should_be_set() {
			var validator = new TestValidator { v => v.RuleFor(x => x.Id).ExclusiveBetween(1, 10) };
			var result = validator.Validate(new Person { Id = 0 });
			result.Errors.Single().ErrorMessage.ShouldEqual("'Id' must be between 1 and 10 (exclusive). You entered 0.");
		}

		[Fact]
		public void To_and_from_properties_should_be_set() {
			var propertyValidator = new ExclusiveBetweenValidator(1, 10);
			propertyValidator.From.ShouldEqual(1);
			propertyValidator.To.ShouldEqual(10);
		}

		[Fact]
		public void When_the_value_is_between_the_range_specified_then_the_validator_should_pass_for_strings() {
			var validator = new TestValidator { v => v.RuleFor(x => x.Surname).ExclusiveBetween("aa", "zz") };
			var result = validator.Validate(new Person { Surname = "bbb" });
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void When_the_value_is_smaller_than_the_range_then_the_validator_should_fail_for_strings() {
			var validator = new TestValidator { v => v.RuleFor(x => x.Surname).ExclusiveBetween("bbb", "zz") };
			var result = validator.Validate(new Person { Surname = "aaa" });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_the_text_is_larger_than_the_range_then_the_validator_should_fail_for_strings() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Surname).ExclusiveBetween("aaa", "bbb"));
			var result = validator.Validate(new Person {Surname = "zzz"});
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_the_value_is_exactly_the_size_of_the_upper_bound_then_the_validator_should_fail_for_strings() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Surname).ExclusiveBetween("aa", "zz"));
			var result = validator.Validate(new Person { Surname = "aa" });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_the_value_is_exactly_the_size_of_the_lower_bound_then_the_validator_should_fail_for_strings() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Surname).ExclusiveBetween("aa", "zz"));
			var result = validator.Validate(new Person { Surname = "zz" });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_the_to_is_smaller_than_the_from_then_the_validator_should_throw_for_strings() {
			typeof(ArgumentOutOfRangeException).ShouldBeThrownBy(() => new ExclusiveBetweenValidator("ccc", "aaa"));
		}

		[Fact]
		public void When_the_validator_fails_the_error_message_should_be_set_for_strings() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Surname).ExclusiveBetween("bbb", "zzz"));
			var result = validator.Validate(new Person { Surname = "aaa" });
			result.Errors.Single().ErrorMessage.ShouldEqual("'Surname' must be between bbb and zzz (exclusive). You entered aaa.");
		}

		[Fact]
		public void To_and_from_properties_should_be_set_for_strings() {
			var validator = new ExclusiveBetweenValidator("a", "c");
			validator.From.ShouldEqual("a");
			validator.To.ShouldEqual("c");
		}

		[Fact]
		public void To_and_from_properties_should_be_set_for_dates() {
			var validator = new ExclusiveBetweenValidator(fromDate, toDate);
			validator.From.ShouldEqual(fromDate);
			validator.To.ShouldEqual(toDate);
		}

		[Fact]
		public void Validates_with_nullable_when_property_is_null() {
			var validator = new TestValidator(v => v.RuleFor(x => x.NullableInt).ExclusiveBetween(1, 5));
			var result = validator.Validate(new Person());
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Validates_with_nullable_when_property_not_null() {
			var validator = new TestValidator(v => v.RuleFor(x => x.NullableInt).ExclusiveBetween(1, 5));
			var result = validator.Validate(new Person { NullableInt = 10 });
			result.IsValid.ShouldBeFalse();
		}
	}
}