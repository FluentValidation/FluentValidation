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
	using System.Threading;
	using NUnit.Framework;
	using Results;

	[TestFixture]
	public class AbstractValidatorTester {
		TestValidator validator;

		[SetUp]
		public void Setup() {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
			validator = new TestValidator();
		}

		[Test]
		public void When_the_Validators_pass_then_the_validatorRunner_should_return_true() {
			validator.RuleFor(x => x.Forename).NotNull();
			validator.Validate(new Person {Forename = "Jeremy"}).IsValid.ShouldBeTrue();
		}

		[Test]
		public void When_the_validators_fail_then_validatorrunner_should_return_false() {
			validator.RuleFor(x => x.Forename).NotNull();
			validator.Validate(new Person()).IsValid.ShouldBeFalse();
		}

		[Test]
		public void When_the_validators_fail_then_the_errors_Should_be_accessible_via_the_errors_property() {
			validator.RuleFor(x => x.Forename).NotNull();
			var result = validator.Validate(new Person());
			result.Errors.Count.ShouldEqual(1);
		}

		[Test]
		public void Should_validate_public_Field() {
			validator.RuleFor(x => x.NameField).NotNull();
			var result = validator.Validate(new Person());
			result.Errors.Count.ShouldEqual(1);
		}

		[Test]
		public void WithMessage_should_override_error_message() {
			validator.RuleFor(x => x.Forename).NotNull().WithMessage("Foo");
			var result = validator.Validate(new Person());
			result.Errors[0].ErrorMessage.ShouldEqual("Foo");
		}

		[Test]
		public void WithName_should_override_field_name() {
			validator.RuleFor(x => x.Forename).NotNull().WithName("First Name");
			var result = validator.Validate(new Person());
			result.Errors[0].ErrorMessage.ShouldEqual("'First Name' must not be empty.");
		}

		[Test]
		public void WithPropertyName_should_override_property_name() {
			validator.RuleFor(x => x.Surname).NotNull().WithPropertyName("foo");
			var result = validator.Validate(new Person());
			result.Errors[0].PropertyName.ShouldEqual("foo");
		}

		[Test]
		public void Should_not_main_state() {
			validator.RuleFor(x => x.Forename).NotNull();
			validator.Validate(new Person());
			var result = validator.Validate(new Person());
			result.Errors.Count.ShouldEqual(1);
		}

		[Test]
		public void Should_throw_when_rule_is_null() {
			typeof(ArgumentNullException).ShouldBeThrownBy(() => validator.RuleFor<string>(null));
		}

		[Test]
		public void Should_throw_when_custom_rule_is_null() {
			typeof(ArgumentNullException).ShouldBeThrownBy(() => validator.Custom((Func<Person, ValidationFailure>)null));
		}

		[Test]
		public void Should_validate_single_property() {
			validator.RuleFor(x => x.Forename).NotNull();
			validator.RuleFor(x => x.Surname).NotNull();
			var result = validator.Validate(new Person(), x => x.Surname);
			result.Errors.Count.ShouldEqual(1);
		}

		[Test]
		public void Should_validate_single_Field() {
			validator.RuleFor(x => x.NameField).NotNull();
			var result = validator.Validate(new Person(), x => x.NameField);
			result.Errors.Count.ShouldEqual(1);
		}

		[Test]
		public void Should_throw_for_non_member_expression_when_validating_single_property() {
			typeof(ArgumentException).ShouldBeThrownBy(() => validator.Validate(new Person(), x => "foo"));
		}

		[Test]
		public void Should_be_valid_when_there_are_no_failures_for_single_property() {
			validator.RuleFor(x => x.Surname).NotNull();
			var result = validator.Validate(new Person {Surname = "foo"}, x => x.Surname);
			result.IsValid.ShouldBeTrue();
		}

		[Test]
		public void Should_validate_single_property_where_property_as_string() {
			validator.RuleFor(x => x.Forename).NotNull();
			validator.RuleFor(x => x.Surname).NotNull();
			var result = validator.Validate(new Person(), "Surname");
			result.Errors.Count.ShouldEqual(1);
		}

		[Test]
		public void Should_validate_single_property_where_invalid_property_as_string() {
			validator.RuleFor(x => x.Forename).NotNull();
			validator.RuleFor(x => x.Surname).NotNull();
			var result = validator.Validate(new Person(), "Surname1");
			result.Errors.Count.ShouldEqual(0);
		}

		[Test]
		public void CanValidateInstancesOfType_returns_true_when_comparing_against_same_type() {
			var validator = (IValidator)this.validator;
			validator.CanValidateInstancesOfType(typeof(Person)).ShouldBeTrue();
		}

		[Test]
		public void CanValidateInstancesOfType_returns_true_when_comparing_against_subclass() {
			var validator = (IValidator)this.validator;
			validator.CanValidateInstancesOfType(typeof(DerivedPerson)).ShouldBeTrue();
		}

		[Test]
		public void CanValidateInstancesOfType_returns_false_when_comparing_against_some_other_type() {
			var validator = (IValidator)this.validator;
			validator.CanValidateInstancesOfType(typeof(Address)).ShouldBeFalse();
		}

		private class DerivedPerson : Person { }

	}
}