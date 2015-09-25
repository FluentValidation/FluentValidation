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
    using Xunit;


    public class CascadingFailuresTester : IDisposable {
		TestValidator validator;

		public CascadingFailuresTester() {
			ValidatorOptions.CascadeMode = CascadeMode.Continue;
			validator = new TestValidator();
		}

		public void Dispose() {
			ValidatorOptions.CascadeMode = CascadeMode.Continue;
		}

		[Fact]
		public void Validation_continues_on_failure() {
			validator.RuleFor(x => x.Surname).NotNull().Equal("Foo");
			var results = validator.Validate(new Person());
			results.Errors.Count.ShouldEqual(2);
		}

		[Fact]
		public void Validation_stops_on_first_failure() {
			ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

			validator.RuleFor(x => x.Surname).NotNull().Equal("Foo");
			var results = validator.Validate(new Person());
			results.Errors.Count.ShouldEqual(1);
		}

		[Fact]
		public void Validation_continues_on_failure_when_set_to_Stop_globally_and_overriden_at_rule_level() {
			ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

			validator.RuleFor(x => x.Surname).Cascade(CascadeMode.Continue).NotNull().Equal("Foo");
			var results = validator.Validate(new Person());
			results.Errors.Count.ShouldEqual(2);
		}

		[Fact]
		public void Validation_stops_on_first_Failure_when_set_to_Continue_globally_and_overriden_at_rule_level() {
			ValidatorOptions.CascadeMode = CascadeMode.Continue;
			validator.RuleFor(x => x.Surname).Cascade(CascadeMode.StopOnFirstFailure).NotNull().Equal("Foo");
			var results = validator.Validate(new Person());
			results.Errors.Count.ShouldEqual(1);
		}

		[Fact]
		public void Validation_continues_to_second_validator_when_first_validator_succeeds_and_cascade_set_to_stop() {
			ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
			validator.RuleFor(x => x.Surname).NotNull().Length(2, 10);
			var result = validator.Validate(new Person() {Surname = "x"});
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void Validation_stops_on_first_failure_when_set_to_StopOnFirstFailure_at_validator_level() {
			validator.CascadeMode = CascadeMode.StopOnFirstFailure;

			validator.RuleFor(x => x.Surname).NotNull().Equal("Foo");
			var results = validator.Validate(new Person());
			results.Errors.Count.ShouldEqual(1);
		}

		[Fact]
		public void Validation_continues_when_set_to_Continue_at_validator_level() {
			validator.CascadeMode = CascadeMode.Continue;

			validator.RuleFor(x => x.Surname).NotNull().Equal("Foo");
			var results = validator.Validate(new Person());
			results.Errors.Count.ShouldEqual(2);
		}

		[Fact]
		public void Validation_continues_on_failure_when_set_to_StopOnFirstFailure_at_validator_level_and_overriden_at_rule_level() {
			validator.CascadeMode = CascadeMode.StopOnFirstFailure;

			validator.RuleFor(x => x.Surname).Cascade(CascadeMode.Continue).NotNull().Equal("Foo");
			var results = validator.Validate(new Person());
			results.Errors.Count.ShouldEqual(2);
		}

		[Fact]
		public void Validation_stops_on_failure_when_set_to_Continue_and_overriden_at_rule_level() {
			validator.CascadeMode = CascadeMode.Continue;

			validator.RuleFor(x => x.Surname).Cascade(CascadeMode.StopOnFirstFailure).NotNull().Equal("Foo");
			var results = validator.Validate(new Person());
			results.Errors.Count.ShouldEqual(1);
		}

		[Fact]
		public void Cascade_mode_can_be_set_after_validator_instantiated() {
			validator.RuleFor(x => x.Surname).NotNull().Equal("Foo");
			validator.CascadeMode = CascadeMode.StopOnFirstFailure;
			var results = validator.Validate(new Person());
			results.Errors.Count.ShouldEqual(1);
		}

	}
}