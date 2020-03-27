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
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Xunit;
	using Results;


	public class AbstractValidatorTester {
		TestValidator validator;
		TestValidatorWithPreValidate testValidatorWithPreValidate;

		public AbstractValidatorTester() {
			CultureScope.SetDefaultCulture();
			validator = new TestValidator();
			testValidatorWithPreValidate = new TestValidatorWithPreValidate();
		}

		[Fact]
		public void When_the_Validators_pass_then_the_validatorRunner_should_return_true() {
			validator.RuleFor(x => x.Forename).NotNull();
			validator.Validate(new Person {Forename = "Jeremy"}).IsValid.ShouldBeTrue();
		}

		[Fact]
		public void When_the_validators_fail_then_validatorrunner_should_return_false() {
			validator.RuleFor(x => x.Forename).NotNull();
			validator.Validate(new Person()).IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_the_validators_fail_then_the_errors_Should_be_accessible_via_the_errors_property() {
			validator.RuleFor(x => x.Forename).NotNull();
			var result = validator.Validate(new Person());
			result.Errors.Count.ShouldEqual(1);
		}

		[Fact]
		public void Should_validate_public_Field() {
			validator.RuleFor(x => x.NameField).NotNull();
			var result = validator.Validate(new Person());
			result.Errors.Count.ShouldEqual(1);
		}

		[Fact]
		public void WithMessage_should_override_error_message() {
			validator.RuleFor(x => x.Forename).NotNull().WithMessage("Foo");
			var result = validator.Validate(new Person());
			result.Errors[0].ErrorMessage.ShouldEqual("Foo");
		}

		[Fact]
		public void Default_error_code_should_be_class_name() {
			validator.RuleFor(x => x.Forename).NotNull();
			var result = validator.Validate(new Person());
			result.Errors[0].ErrorCode.ShouldEqual("NotNullValidator");
		}

		[Fact]
		public void Can_replace_default_errorcode_resolver() {
			ValidatorOptions.ErrorCodeResolver = x => x.GetType().Name + "_foo";
			validator.RuleFor(x => x.Forename).NotNull();
			var result = validator.Validate(new Person());
			ValidatorOptions.ErrorCodeResolver = null;
			result.Errors[0].ErrorCode.ShouldEqual("NotNullValidator_foo");
		}

		[Fact]
		public void WithErrorCode_should_override_error_code() {
			validator.RuleFor(x => x.Forename).NotNull().WithErrorCode("ErrCode101");
			var result = validator.Validate(new Person());
			result.Errors[0].ErrorCode.ShouldEqual("ErrCode101");
		}

		[Fact]
		public void WithMessage_and_WithErrorCode_should_override_error_message_and_error_code() {
			validator.RuleFor(x => x.Forename).NotNull().WithMessage("Foo").WithErrorCode("ErrCode101");
			var result = validator.Validate(new Person());
			result.Errors[0].ErrorMessage.ShouldEqual("Foo");
			result.Errors[0].ErrorCode.ShouldEqual("ErrCode101");
		}

		[Fact]
		public void WithName_should_override_field_name() {
			validator.RuleFor(x => x.Forename).NotNull().WithName("First Name");
			var result = validator.Validate(new Person());
			result.Errors[0].ErrorMessage.ShouldEqual("'First Name' must not be empty.");
		}

		[Fact]
		public void WithName_should_override_field_name_with_value_from_other_property() {
			validator.RuleFor(x => x.Forename).NotNull().WithName(x => x.Surname);
			var result = validator.Validate(new Person(){Surname = "Foo"});
			result.Errors[0].ErrorMessage.ShouldEqual("'Foo' must not be empty.");
		}

		[Fact]
		public void OverridePropertyName_should_override_property_name() {
			validator.RuleFor(x => x.Surname).NotNull().OverridePropertyName("foo");
			var result = validator.Validate(new Person());
			result.Errors[0].PropertyName.ShouldEqual("foo");
		}

		[Fact]
		public void OverridePropertyName_with_lambda_should_override_property_name() {
			validator.RuleFor(x => x.Surname).NotNull().OverridePropertyName(x => x.Forename);
			var result = validator.Validate(new Person());
			result.Errors[0].PropertyName.ShouldEqual("Forename");
		}

		[Fact]
		public void Should_not_main_state() {
			validator.RuleFor(x => x.Forename).NotNull();
			validator.Validate(new Person());
			var result = validator.Validate(new Person());
			result.Errors.Count.ShouldEqual(1);
		}

		[Fact]
		public void Should_throw_when_rule_is_null() {
			typeof(ArgumentNullException).ShouldBeThrownBy(() => validator.RuleFor<string>(null));
		}

		[Fact]
		public void Should_validate_single_property() {
			validator.RuleFor(x => x.Forename).NotNull();
			validator.RuleFor(x => x.Surname).NotNull();
			var result = validator.Validate(new Person(), x => x.Surname);
			result.Errors.Count.ShouldEqual(1);
		}

		[Fact]
		public void Should_validate_single_Field() {
			validator.RuleFor(x => x.NameField).NotNull();
			var result = validator.Validate(new Person(), x => x.NameField);
			result.Errors.Count.ShouldEqual(1);
		}

		[Fact]
		public void Should_throw_for_non_member_expression_when_validating_single_property() {
			typeof(ArgumentException).ShouldBeThrownBy(() => validator.Validate(new Person(), x => "foo"));
		}

		[Fact]
		public void Should_be_valid_when_there_are_no_failures_for_single_property() {
			validator.RuleFor(x => x.Surname).NotNull();
			var result = validator.Validate(new Person {Surname = "foo"}, x => x.Surname);
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Should_validate_single_property_where_property_as_string() {
			validator.RuleFor(x => x.Forename).NotNull();
			validator.RuleFor(x => x.Surname).NotNull();
			var result = validator.Validate(new Person(), "Surname");
			result.Errors.Count.ShouldEqual(1);
		}

		[Fact]
		public void Should_validate_single_property_where_invalid_property_as_string() {
			validator.RuleFor(x => x.Forename).NotNull();
			validator.RuleFor(x => x.Surname).NotNull();
			var result = validator.Validate(new Person(), "Surname1");
			result.Errors.Count.ShouldEqual(0);
		}

		[Fact]
		public void Validates_single_property_by_path() {
			var addressValidator = new InlineValidator<Address>();
			addressValidator.RuleFor(x => x.Line1).NotNull();
			addressValidator.RuleFor(x => x.Line2).NotNull();

			validator.RuleFor(x => x.Address).SetValidator(addressValidator);
			validator.RuleFor(x => x.Forename).NotNull();

			var result = validator.Validate(new Person { Address = new Address() }, properties: "Address.Line1");
			result.Errors.Count.ShouldEqual(1);
			result.Errors.Single().PropertyName.ShouldEqual("Address.Line1");
		}

		[Fact]
		public void CanValidateInstancesOfType_returns_true_when_comparing_against_same_type() {
			var validator = (IValidator)this.validator;
			validator.CanValidateInstancesOfType(typeof(Person)).ShouldBeTrue();
		}

		[Fact]
		public void CanValidateInstancesOfType_returns_true_when_comparing_against_subclass() {
			var validator = (IValidator)this.validator;
			validator.CanValidateInstancesOfType(typeof(DerivedPerson)).ShouldBeTrue();
		}

		[Fact]
		public void CanValidateInstancesOfType_returns_false_when_comparing_against_some_other_type() {
			var validator = (IValidator)this.validator;
			validator.CanValidateInstancesOfType(typeof(Address)).ShouldBeFalse();
		}

		[Fact]
		public void Uses_named_parameters_to_validate_ruleset() {
			validator.RuleSet("Names", () => {
				validator.RuleFor(x => x.Surname).NotNull();
				validator.RuleFor(x => x.Forename).NotNull();
			});
			validator.RuleFor(x => x.Id).NotEqual(0);

			var result = validator.Validate(new Person(), ruleSet: "Names");
			result.Errors.Count.ShouldEqual(2);
		}

		[Fact]
		public void Validates_type_when_using_non_generic_validate_overload() {
			IValidator nonGenericValidator = validator;

			typeof(InvalidOperationException).ShouldBeThrownBy(() =>
				nonGenericValidator.Validate("foo"));
		}

		[Fact]
		public void RuleForeach_with_null_instances() {
			var model = new Person {
				NickNames = new string[] { null }
			};

			validator.RuleForEach(x => x.NickNames).NotNull();
			var result = validator.Validate(model);
			Console.WriteLine(result.Errors[0].ErrorMessage);
			result.IsValid.ShouldBeFalse();
		}

		private class DerivedPerson : Person { }


		[Theory]
		[MemberData(nameof(PreValidationReturnValueTheoryData))]
		public void WhenPreValidationReturnsFalse_ResultReturnToUserImmediatly_Validate(ValidationResult preValidationResult) {
			testValidatorWithPreValidate.PreValidateMethod = (context, validationResult) => {
				foreach (ValidationFailure validationFailure in preValidationResult.Errors) {
					validationResult.Errors.Add(validationFailure);
				}

				return false;
			};
			testValidatorWithPreValidate.RuleFor(person => person.Age).GreaterThanOrEqualTo(0);

			var result = testValidatorWithPreValidate.Validate(new Person() { Age = -1 });

			Assert.Equal(preValidationResult.Errors.Count, result.Errors.Count);
			Assert.DoesNotContain(nameof(Person.Age), result.Errors.Select(failure => failure.PropertyName));
		}

		[Theory]
		[MemberData(nameof(PreValidationReturnValueTheoryData))]
		public async Task WhenPreValidationReturnsFalse_ResultReturnToUserImmediatly_ValidateAsync(ValidationResult preValidationResult) {
			testValidatorWithPreValidate.PreValidateMethod = (context, validationResult) => {
				foreach (ValidationFailure validationFailure in preValidationResult.Errors) {
					validationResult.Errors.Add(validationFailure);
				}

				return false;
			};
			testValidatorWithPreValidate.RuleFor(person => person.Age).MustAsync((age, token) => Task.FromResult(age >= 0));

			var result = await testValidatorWithPreValidate.ValidateAsync(new Person() { Age = -1 });

			Assert.Equal(preValidationResult.Errors.Count, result.Errors.Count);
			Assert.DoesNotContain(nameof(Person.Age), result.Errors.Select(failure => failure.PropertyName));
		}

		[Fact]
		public void PreValidate_bypasses_nullcheck_on_instance() {
			testValidatorWithPreValidate.RuleFor(x => x.Surname).NotNull();
			testValidatorWithPreValidate.PreValidateMethod = (ctx, r) => false;

			var result = testValidatorWithPreValidate.Validate((Person)null);
			result.IsValid.ShouldBeTrue();

		}

		[Fact]
		public void WhenPreValidationReturnsTrue_ValidatorsGetHit_Validate() {
			const string testProperty = "TestProperty";
			const string testMessage = "Test Message";
			testValidatorWithPreValidate.PreValidateMethod = (context, validationResult) => {
				validationResult.Errors.Add(new ValidationFailure(testProperty, testMessage));
				return true;
			};
			testValidatorWithPreValidate.RuleFor(person => person.Age).GreaterThanOrEqualTo(0);

			var result = testValidatorWithPreValidate.Validate(new Person() { Age = -1 });

			Assert.Contains(nameof(Person.Age), result.Errors.Select(failure => failure.PropertyName));
			Assert.Contains(testProperty, result.Errors.Select(failure => failure.PropertyName));
			Assert.Contains(testMessage, result.Errors.Select(failure => failure.ErrorMessage));
		}

		[Fact]
		public async Task WhenPreValidationReturnsTrue_ValidatorsGetHit_ValidateAsync() {
			const string testProperty = "TestProperty";
			const string testMessage = "Test Message";
			testValidatorWithPreValidate.PreValidateMethod = (context, validationResult) => {
				validationResult.Errors.Add(new ValidationFailure(testProperty, testMessage));
				return true;
			};
			testValidatorWithPreValidate.RuleFor(person => person.Age).MustAsync((age, token) => Task.FromResult(age >= 0));

			var result = await testValidatorWithPreValidate.ValidateAsync(new Person() { Age = -1 });

			Assert.Contains(nameof(Person.Age), result.Errors.Select(failure => failure.PropertyName));
			Assert.Contains(testProperty, result.Errors.Select(failure => failure.PropertyName));
			Assert.Contains(testMessage, result.Errors.Select(failure => failure.ErrorMessage));
		}

		public static TheoryData<ValidationResult> PreValidationReturnValueTheoryData = new TheoryData<ValidationResult> {
			new ValidationResult(),
			new ValidationResult(new List<ValidationFailure> {new ValidationFailure(nameof(Person.AnotherInt), $"{nameof(Person.AnotherInt)} Test Message")})
		};

	}
}
