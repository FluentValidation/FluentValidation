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
    using System.Linq;
    using Xunit;


    public class NullTester {
		public NullTester() {
            CultureScope.SetDefaultCulture();
        }

		[Fact]
		public void NullValidator_should_fail_if_value_has_value() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Surname).Null());
			var result = validator.Validate(new Person{Surname = "Foo"});
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void NullValidator_should_pass_if_value_is_null() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Surname).Null());
			var result = validator.Validate(new Person { Surname = null });
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void When_the_validator_passes_the_error_message_should_be_set() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Surname).Null());
			var result = validator.Validate(new Person { Surname = "Foo" });
			result.Errors.Single().ErrorMessage.ShouldEqual("'Surname' must be empty.");
		}

		[Fact]
		public void Not_null_validator_should_not_crash_with_non_nullable_value_type() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Id).Null());
			var result = validator.Validate(new Person());
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void Passes_when_nullable_value_type_is_null() {
			var validator = new TestValidator(v => v.RuleFor(x => x.NullableInt).Null());
			var result = validator.Validate(new Person());
			result.IsValid.ShouldBeTrue();
		}
	}
}