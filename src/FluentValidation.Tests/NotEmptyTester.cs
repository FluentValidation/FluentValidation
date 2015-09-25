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
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Threading;
	using Xunit;

	
	public class NotEmptyTester {
		public NotEmptyTester() {
          CultureScope.SetDefaultCulture();
        }

		[Fact]
		public void When_there_is_a_value_then_the_validator_should_pass() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotEmpty()
			};

			var result = validator.Validate(new Person { Surname = "Foo" });
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void When_value_is_null_validator_should_fail() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotEmpty()
			};

			var result = validator.Validate(new Person { Surname = null });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_value_is_empty_string_validator_should_fail() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotEmpty()
			};

			var result = validator.Validate(new Person { Surname = "" });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_value_is_whitespace_validation_should_fail() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotEmpty()
			};

			var result = validator.Validate(new Person { Surname = "         " });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_value_is_Default_for_type_validator_should_fail_datetime() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.DateOfBirth).NotEmpty()
			};

			var result = validator.Validate(new Person { DateOfBirth = default(DateTime) });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_value_is_Default_for_type_validator_should_fail_int() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Id).NotEmpty()
			};

			var result = validator.Validate(new Person { Id = 0 });
			result.IsValid.ShouldBeFalse();

			var result1 = validator.Validate(new Person{Id = 1});
			result1.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Fails_when_collection_empty() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Children).NotEmpty()
			};

			var result = validator.Validate(new Person { Children = new List<Person>() });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_validation_fails_error_should_be_set() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotEmpty()
			};

			var result = validator.Validate(new Person { Surname = null });
			result.Errors.Single().ErrorMessage.ShouldEqual("'Surname' should not be empty.");
		}

	    [Fact]
	    public void Fails_for_ienumerable_that_doesnt_implement_ICollection() {
	        var validator = new InlineValidator<TestModel> {
                v => v.RuleFor(x => x.Strings).NotEmpty()
	        };

	        var result = validator.Validate(new TestModel());
            result.IsValid.ShouldBeFalse();
	    }

        public class TestModel {
            public IEnumerable<string> Strings {
                get { yield break; }
            } 
        }
	}
}