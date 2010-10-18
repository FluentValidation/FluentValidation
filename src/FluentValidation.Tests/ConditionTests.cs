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
	using NUnit.Framework;

	[TestFixture]
	public class ConditionTests {
		[Test]
		public void Validation_should_succeed_when_condition_does_not_match() {
			var validator = new TestConditionValidator();
			var result = validator.Validate(new Person {Id = 1});
			Assert.That(result.IsValid);
		}

		[Test]
		public void Validation_should_fail_when_condition_matches() {
			var validator = new TestConditionValidator();
			var result = validator.Validate(new Person());
			result.IsValid.ShouldBeFalse();
		}

		[Test]
		public void Validation_should_succeed_when_condition_matches() {
			var validator = new InverseConditionValidator();
			var result = validator.Validate(new Person());
			result.IsValid.ShouldBeTrue();
		}

		[Test]
		public void Validation_should_fail_when_condition_does_not_match() {
			var validator = new InverseConditionValidator();
			var result = validator.Validate(new Person {Id = 1});
			result.IsValid.ShouldBeFalse();
		}

		[Test]
		public void Condition_is_applied_to_all_validators_in_the_chain() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull().NotEqual("foo").When(x => x.Id > 0)
			};

			var result = validator.Validate(new Person());
			result.Errors.Count.ShouldEqual(0);
		}

		[Test]
		public void Condition_is_applied_to_single_validator_in_the_chain_when_ApplyConditionTo_set_to_CurrentValidator() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull().NotEqual("foo").When(x => x.Id > 0, ApplyConditionTo.CurrentValidator)
			};

			var result = validator.Validate(new Person());
			result.Errors.Count.ShouldEqual(1);
		}

		private class TestConditionValidator : AbstractValidator<Person> {
			public TestConditionValidator() {
				RuleFor(x => x.Forename).NotNull().When(x => x.Id == 0);
			}
		}

		private class InverseConditionValidator : AbstractValidator<Person> {
			public InverseConditionValidator() {
				RuleFor(x => x.Forename).NotNull().Unless(x => x.Id == 0);
			}
		}
	}
}