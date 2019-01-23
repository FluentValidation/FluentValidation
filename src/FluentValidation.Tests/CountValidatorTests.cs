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
	using System.Linq;
	using Validators;
	using Xunit;

	public class CountValidatorTests {
		public CountValidatorTests() {
			CultureScope.SetDefaultCulture();
		}

		[Fact]
		public void When_the_min_is_negative_then_the_validator_should_throw() {
			typeof(ArgumentOutOfRangeException).ShouldBeThrownBy(() =>
				new TestValidator(v => v.RuleFor(x => x.Children).Count(-1, 0)));
		}

		[Fact]
		public void When_the_max_is_smaller_than_the_min_then_the_validator_should_throw() {
			typeof(ArgumentOutOfRangeException).ShouldBeThrownBy(() =>
				new TestValidator(v => v.RuleFor(x => x.Children).Count(2, 1)));
		}

		[Fact]
		public void When_the_count_is_between_the_range_then_the_validator_should_pass() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Children).Count(1, 3));
			var result = validator.Validate(PersonWithChildren(2));
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void When_the_count_is_exactly_the_size_of_the_lower_bound_then_the_validator_should_pass() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Children).Count(1, 3));
			var result = validator.Validate(PersonWithChildren(1));
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void When_the_count_is_exactly_the_size_of_the_upper_bound_then_the_validator_should_pass() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Children).Count(1, 3));
			var result = validator.Validate(PersonWithChildren(3));
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void When_the_count_is_smaller_than_the_range_then_the_validator_should_fail() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Children).Count(1, 3));
			var result = validator.Validate(PersonWithChildren(0));
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_the_count_is_larger_than_the_range_then_the_validator_should_fail() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Children).Count(1, 3));
			var result = validator.Validate(PersonWithChildren(4));
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_the_validator_fails_the_error_message_should_be_set() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Children).Count(1, 3));
			var result = validator.Validate(PersonWithChildren(4));
			result.Errors.Single().ErrorMessage.ShouldEqual("'Children' must have between 1 and 3 elements, but actually contains 4 elements.");
		}

		[Fact]
		public void Min_and_max_properties_should_be_set() {
			var validator = new CountValidator(1, 3);
			validator.Min.ShouldEqual(1);
			validator.Max.ShouldEqual(3);
		}

		[Fact]
		public void When_input_is_null_then_the_validator_should_pass() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Children).Count(1, 3));
			var result = validator.Validate(new Person {Children = null});
			result.IsValid.ShouldBeTrue();
		}

		private static Person PersonWithChildren(int num) =>
			new Person {Children = Enumerable.Repeat(new Person(), num).ToList()};
	}
}