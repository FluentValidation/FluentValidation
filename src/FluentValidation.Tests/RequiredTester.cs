#region License
// Copyright (c) .NET Foundation and contributors.
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
// The latest version of this file can be found at https://github.com/FluentValidation/FluentValidation
#endregion

namespace FluentValidation.Tests {
	using System.Linq;
	using Xunit;

	public class RequiredTester {
		public RequiredTester() {
			CultureScope.SetDefaultCulture();
		}

		[Fact]
		public void When_there_is_a_value_then_the_validator_should_pass() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).Required()
			};

			var result = validator.Validate(new Person { Surname = "Foo" });
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void When_value_is_null_validator_should_fail() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).Required()
			};

			var result = validator.Validate(new Person { Surname = null });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_value_is_empty_string_validator_should_fail() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).Required()
			};

			var result = validator.Validate(new Person { Surname = "" });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_value_is_empty_string_validator_should_succeed_if_empty_strings_allowed() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).Required(allowEmptyStrings:true)
			};

			var result = validator.Validate(new Person { Surname = "" });
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void When_value_is_whitespace_validation_should_fail() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).Required()
			};

			var result = validator.Validate(new Person { Surname = "         " });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_value_is_whitespace_validation_should_succeed_if_empty_strings_allowed() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).Required(allowEmptyStrings:true)
			};

			var result = validator.Validate(new Person { Surname = "         " });
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void When_validation_fails_error_should_be_set() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).Required()
			};

			var result = validator.Validate(new Person { Surname = null });
			result.Errors.Single().ErrorMessage.ShouldEqual("'Surname' is required.");
		}
	}

}
