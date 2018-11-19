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

namespace FluentValidation.Tests
{
	using Xunit;


	public class BooleanValidatorTests {
		public BooleanValidatorTests() {
			CultureScope.SetDefaultCulture();
		}

		[Fact]
		public void When_the_value_is_true_the_validator_should_pass() {
			var validator = new TestValidator(v => v.RuleFor(x => x.IsValid).True());
			var result = validator.Validate(new Person { IsValid = true});
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void When_the_value_is_not_true_the_validator_not_should_pass() {
			var validator = new TestValidator(v => v.RuleFor(x => x.IsValid).True());
			var result = validator.Validate(new Person { IsValid = false });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_the_value_is_false_the_validator_should_pass() {
			var validator = new TestValidator(v => v.RuleFor(x => x.IsValid).False());
			var result = validator.Validate(new Person { IsValid = false });
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void When_the_value_is_true_the_validator_not_should_pass() {
			var validator = new TestValidator(v => v.RuleFor(x => x.IsValid).False());
			var result = validator.Validate(new Person { IsValid = true });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_the_nullable_value_is_true_the_validator_should_pass()	{
			var validator = new TestValidator(v => v.RuleFor(x => x.NullableIsValid).True());
			var result = validator.Validate(new Person { NullableIsValid = true });
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void When_the_nullable_value_is_not_true_the_validator_not_should_pass()	{
			var validator = new TestValidator(v => v.RuleFor(x => x.NullableIsValid).True());
			var result = validator.Validate(new Person { NullableIsValid = false });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_the_nullable_value_is_false_the_validator_should_pass() {
			var validator = new TestValidator(v => v.RuleFor(x => x.NullableIsValid).False());
			var result = validator.Validate(new Person { NullableIsValid = false });
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void When_the_nullable_value_is_true_the_validator_not_should_pass()	{
			var validator = new TestValidator(v => v.RuleFor(x => x.NullableIsValid).False());
			var result = validator.Validate(new Person { NullableIsValid = true });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_the__true_boolean_nullable_value_validator_not_should_pass() {
			var validator = new TestValidator(v => v.RuleFor(x => x.NullableIsValid).True());
			var result = validator.Validate(new Person { NullableIsValid = null });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_the_false_boolean_nullable_value_validator_not_should_pass() {
			var validator = new TestValidator(v => v.RuleFor(x => x.NullableIsValid).False());
			var result = validator.Validate(new Person { NullableIsValid = null });
			result.IsValid.ShouldBeFalse();
		}
	}
}