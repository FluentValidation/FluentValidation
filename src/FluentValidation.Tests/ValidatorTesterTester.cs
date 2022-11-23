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

namespace FluentValidation.Tests;

using System;
using System.Collections.Generic;
using Xunit;
using TestHelper;
using System.Threading.Tasks;

public class ValidatorTesterTester {
	private TestValidator validator;

	public ValidatorTesterTester() {
		validator = new TestValidator();
		validator.RuleFor(x => x.Forename).NotNull();
		validator.RuleForEach(person => person.NickNames).MinimumLength(5);
		CultureScope.SetDefaultCulture();
	}

	[Fact]
	public void ShouldHaveValidationError_should_not_throw_when_there_are_validation_errors_ruleforeach() {
		var person = new Person {NickNames = new[] {"magician", "bull"}};
		validator.TestValidate(person).ShouldHaveValidationErrorFor(l => l.NickNames);
	}

	[Fact]
	public void ShouldHaveValidationError_should_throw_when_there_are_not_validation_errors_ruleforeach() {
		var person = new Person {NickNames = new[] {"magician", "awesome"}};

		ValidationTestException validationTestException = Assert.Throws<ValidationTestException>(() =>
			validator.TestValidate(person).ShouldHaveValidationErrorFor(l => l.NickNames));

		Assert.Contains(nameof(Person.NickNames), validationTestException.Message);
	}

	[Fact]
	public void ShouldNotHaveValidationError_should_not_throw_when_there_are_not_validation_errors_ruleforeach() {
		var person = new Person {NickNames = new[] {"magician", "awesome"}};
		validator.TestValidate(person).ShouldNotHaveValidationErrorFor(l => l.NickNames);
	}

	[Fact]
	public void ShouldNotHaveValidationError_should_throw_when_there_are_validation_errors_ruleforeach() {
		var person = new Person {NickNames = new[] {"magician", "bull"}};

		ValidationTestException validationTestException = Assert.Throws<ValidationTestException>(() =>
			validator.TestValidate(person).ShouldNotHaveValidationErrorFor(l => l.NickNames));

		Assert.Contains(nameof(Person.NickNames), validationTestException.Message);
	}

	[Fact]
	public void ShouldNotHaveValidationError_should_have_validation_error_details_when_thrown_ruleforeach() {
		var person = new Person {NickNames = new[] {"magician", "bull"}};
		ValidationTestException validationTestException = Assert.Throws<ValidationTestException>(() =>
			validator.TestValidate(person).ShouldNotHaveValidationErrorFor(l => l.NickNames));
		Assert.Contains("The length of 'Nick Names' must be at least 5 characters. You entered 4 characters.", validationTestException.Message);
	}

	[Fact]
	public void ShouldHaveValidationError_should_not_throw_when_there_are_validation_errors() {
		var person = new Person();
		validator.TestValidate(person).ShouldHaveValidationErrorFor(x => x.Forename);
	}

	[Fact]
	public void ShouldHaveValidationError_Should_support_nested_properties() {
		validator.RuleFor(x => x.Address.Line1).NotNull();

		var person = new Person {
			Address = new Address {
				Line1 = null,
			},
		};

		validator.TestValidate(person).ShouldHaveValidationErrorFor(x => x.Address.Line1);
	}

	[Fact]
	public void ShouldNotHaveValidationError_Should_support_nested_properties() {
		validator.RuleFor(x => x.Address.Line1).NotNull();

		var person = new Person {
			Address = new Address {
				Line1 = "anything",
			},
		};

		validator.TestValidate(person).ShouldNotHaveValidationErrorFor(x => x.Address.Line1);
	}

	[Fact]
	public void ShouldHaveValidationError_Should_throw_when_there_are_no_validation_errors() {
		var person = new Person {Forename = "test"};
		Assert.Throws<ValidationTestException>(() => validator.TestValidate(person).ShouldHaveValidationErrorFor(x => x.Forename));
	}

	[Fact]
	public void ShouldNotHaveValidationError_should_not_throw_when_there_are_no_errors() {
		var person = new Person {Forename = "test"};
		validator.TestValidate(person).ShouldNotHaveValidationErrorFor(x => x.Forename);
	}

	[Fact]
	public void ShouldNotHaveValidationError_should_throw_when_there_are_errors() {
		var person = new Person();
		Assert.Throws<ValidationTestException>(() => validator.TestValidate(person).ShouldNotHaveValidationErrorFor(x => x.Forename));
	}

	[Fact]
	public void ShouldHaveValidationError_should_not_throw_when_there_are_errors_with_preconstructed_object() {
		validator.TestValidate(new Person()).ShouldHaveValidationErrorFor(x => x.Forename);
	}

	[Fact]
	public void ShouldHaveValidationError_should_throw_when_there_are_no_validation_errors_with_preconstructed_object() {
		Assert.Throws<ValidationTestException>(() => validator.TestValidate(new Person {Forename = "test"}).ShouldHaveValidationErrorFor(x => x.Forename));
	}

	[Fact]
	public void ShouldNotHAveValidationError_should_not_throw_When_there_are_no_errors_with_preconstructed_object() {
		validator.TestValidate(new Person {Forename = "test"}).ShouldNotHaveValidationErrorFor(x => x.Forename);
	}

	[Fact]
	public void ShouldNotHaveValidationError_should_throw_when_there_are_errors_with_preconstructed_object() {
		Assert.Throws<ValidationTestException>(() => validator.TestValidate(new Person {Forename = null}).ShouldNotHaveValidationErrorFor(x => x.Forename));
	}


	[Fact]
	public void ShouldHaveChildValidator_throws_when_property_does_not_have_child_validator() {
		var ex = Assert.Throws<ValidationTestException>(() =>
			validator.ShouldHaveChildValidator(x => x.Address, typeof(AddressValidator))
		);

		ex.Message.ShouldEqual("Expected property 'Address' to have a child validator of type 'AddressValidator.'. Instead found 'none'");
	}


	[Fact]
	public void ShouldHaveChildValidator_should_not_throw_when_property_Does_have_child_validator() {
		validator.RuleFor(x => x.Address).SetValidator(new AddressValidator());
		validator.ShouldHaveChildValidator(x => x.Address, typeof(AddressValidator));
	}

	[Fact]
	public void ShouldHaveChildValidator_should_not_throw_when_property_Does_have_child_validator_and_expecting_a_basetype() {
		validator.RuleFor(x => x.Address).SetValidator(new AddressValidator());
		validator.ShouldHaveChildValidator(x => x.Address, typeof(AbstractValidator<Address>));
	}

	[Fact]
	public void ShouldHaveChildvalidator_throws_when_collection_property_Does_not_have_child_validator() {
		var ex = Assert.Throws<ValidationTestException>(() =>
			validator.ShouldHaveChildValidator(x => x.Orders, typeof(OrderValidator))
		);

		ex.Message.ShouldEqual("Expected property 'Orders' to have a child validator of type 'OrderValidator.'. Instead found 'none'");
	}

	[Fact]
	public void ShouldHaveChildValidator_should_throw_when_property_has_a_different_child_validator() {
		validator.RuleFor(x => x.Address).SetValidator(new AddressValidator());
		var ex = Assert.Throws<ValidationTestException>(() =>
			validator.ShouldHaveChildValidator(x => x.Address, typeof(OrderValidator))
		);

		ex.Message.ShouldEqual("Expected property 'Address' to have a child validator of type 'OrderValidator.'. Instead found 'AddressValidator'");
	}

	[Fact]
	public void ShouldHaveChildValidator_should_not_throw_when_property_has_collection_validators() {
		validator.RuleForEach(x => x.Orders).SetValidator(new OrderValidator());
		validator.ShouldHaveChildValidator(x => x.Orders, typeof(OrderValidator));
	}

	[Fact]
	public void ShouldHaveChildValidator_works_on_model_level_rules() {
		validator.RuleFor(x => x).SetValidator(new InlineValidator<Person>());
		validator.ShouldHaveChildValidator(x => x, typeof(InlineValidator<Person>));
	}

	[Fact]
	public void ShouldHaveValidationErrorFor_takes_account_of_rulesets() {
		var testValidator = new TestValidator();
		testValidator.RuleSet("Names", () => {
			testValidator.RuleFor(x => x.Surname).NotNull();
			testValidator.RuleFor(x => x.Forename).NotNull();
		});
		testValidator.RuleFor(x => x.Id).NotEqual(0);

		testValidator.TestValidate(new Person(), opt => opt.IncludeRuleSets("Names"))
			.ShouldHaveValidationErrorFor(x => x.Forename);
	}

	[Fact]
	public void ShouldHaveValidationErrorFor_takes_account_of_rulesets_fluent_approach() {
		var testValidator = new TestValidator();
		testValidator.RuleSet("Names", () => {
			testValidator.RuleFor(x => x.Surname).NotNull();
			testValidator.RuleFor(x => x.Forename).NotNull();
		});
		testValidator.RuleFor(x => x.Id).NotEqual(0);

		var assertionRoot = testValidator.TestValidate(new Person(), opt => opt.IncludeRuleSets("Names"));

		assertionRoot.ShouldHaveValidationErrorFor(x => x.Forename)
			.WithErrorCode("NotNullValidator");
		assertionRoot.ShouldHaveValidationErrorFor(x => x.Surname).WithErrorCode("NotNullValidator");
		assertionRoot.ShouldNotHaveValidationErrorFor(x => x.Id);
	}

	[Fact]
	public void Tests_nested_property_using_obsolete_method() {
		var validator = new TestValidator();
		validator.RuleFor(x => x.Address.Line1).NotEqual("foo");

		var result = validator.TestValidate(new Person() {
			Address = new Address {Line1 = "bar"}
		});

		var ex = Assert.Throws<ValidationTestException>(() => {
			result.ShouldHaveValidationErrorFor(x => x.Address.Line1);
		});

		ex.Message.ShouldEqual("Expected a validation error for property Address.Line1");
	}

	[Fact]
	public void Tests_nested_property() {
		var validator = new TestValidator();
		validator.RuleFor(x => x.Address.Line1).NotEqual("foo");

		var result = validator.TestValidate(new Person() {
			Address = new Address {Line1 = "bar"}
		});

		var ex = Assert.Throws<ValidationTestException>(() => {
			result.ShouldHaveValidationErrorFor(x => x.Address.Line1);
		});

		ex.Message.ShouldEqual("Expected a validation error for property Address.Line1");
	}

	[Fact]
	public void Tests_nested_property_reverse() {
		var validator = new TestValidator();
		validator.RuleFor(x => x.Address.Line1).NotEqual("foo");

		var result = validator.TestValidate(new Person() {
			Address = new Address {Line1 = "foo"}
		});

		var ex = Assert.Throws<ValidationTestException>(() => {
			result.ShouldNotHaveValidationErrorFor(x => x.Address.Line1);
		});

		ex.Message.Contains($"Expected no validation errors for property Address.Line1").ShouldBeTrue();
	}

	[Fact]
	public void ShouldHaveValidationError_with_an_unmatched_rule_and_a_single_error_should_throw_an_exception() {
		var validator = new TestValidator();
		validator.RuleFor(x => x.NullableInt).GreaterThanOrEqualTo(3);

		var result = validator.TestValidate(new Person() {
			NullableInt = 1
		});

		var ex = Assert.Throws<ValidationTestException>(() => result.ShouldHaveValidationErrorFor(x => x.NullableInt.Value));
		Assert.Equal("Expected a validation error for property NullableInt.Value\n----\nProperties with Validation Errors:\n[0]: NullableInt\n", ex.Message);
	}

	[Fact]
	public void ShouldHaveValidationError_with_an_unmatched_rule_and_multiple_errors_should_throw_an_exception() {
		var validator = new TestValidator();
		validator.RuleFor(x => x.NullableInt).GreaterThan(1);
		validator.RuleFor(x => x.Age).GreaterThan(1);
		validator.RuleFor(x => x.AnotherInt).GreaterThan(2);

		var result = validator.TestValidate(new Person() {
			NullableInt = 1,
			Age = 1,
			AnotherInt = 1
		});

		var ex = Assert.Throws<ValidationTestException>(() => result.ShouldHaveValidationErrorFor(x => x.NullableInt.Value));
		Assert.Equal("Expected a validation error for property NullableInt.Value\n----\nProperties with Validation Errors:\n[0]: NullableInt\n[1]: Age\n[2]: AnotherInt\n", ex.Message);
	}

	[Fact]
	public void ShouldNotHaveValidationError_should_correctly_handle_explicitly_providing_object_to_validate() {
		var unitOfMeasure = new UnitOfMeasure {
			Value = 1,
			Type = 43
		};

		var validator = new UnitOfMeasureValidator();

		validator.TestValidate(unitOfMeasure).ShouldNotHaveValidationErrorFor(unit => unit.Type);
	}

	[Fact]
	public void ShouldNotHaveValidationError_should_correctly_handle_explicitly_providing_object_to_validate_and_other_property_fails_validation() {
		var validator = new Address2Validator();
		validator.RuleFor(x => x.StreetNumber).Equal("foo");

		var address = new Address2 {
			StreetNumber = "a",
			Street = "b"
		};

		validator.TestValidate(address).ShouldNotHaveValidationErrorFor(a => a.Street);
	}

	[Fact]
	public void ShouldHaveValidationError_preconstructed_object_does_not_throw_for_unwritable_property() {
		validator.RuleFor(x => x.ForenameReadOnly).NotNull();
		validator.TestValidate(new Person {Forename = null}).ShouldHaveValidationErrorFor(x => x.ForenameReadOnly);
	}

	[Fact]
	public void Expected_message_check() {
		var validator = new InlineValidator<Person> {
			v => v.RuleFor(x => x.Surname).NotNull().WithMessage("bar")
		};

		var ex = Assert.Throws<ValidationTestException>(() =>
			validator.TestValidate(new Person())
				.ShouldHaveValidationErrorFor(x => x.Surname)
				.WithErrorMessage("foo"));
		ex.Message.ShouldEqual("Expected an error message of 'foo'. Actual message was 'bar'");
	}

	[Theory]
	[InlineData("bar", new string[] { "foo", "bar" })]
	[InlineData("bar", new string[] { "bar", })]
	public void Unexpected_message_check(string withoutErrMsg, string[] errMessages) {
		var validator = new InlineValidator<Person>();
		foreach (var msg in errMessages) {
			validator.Add(v => v.RuleFor(x => x.Surname).NotNull().WithMessage(msg));
		}

		var ex = Assert.Throws<ValidationTestException>(() =>
			validator.TestValidate(new Person { }).ShouldHaveValidationErrors().WithoutErrorMessage(withoutErrMsg));
		ex.Message.ShouldEqual($"Found an unexpected error message of '{withoutErrMsg}'");
	}

	[Fact]
	public void Expected_message_argument_check() {
		var validator = new InlineValidator<Person> {
			v => v.RuleFor(x => x.Surname)
				.Must((x, y, context) => {
					context.MessageFormatter.AppendArgument("Foo", "bar");
					return false;
				})
				.WithMessage("{Foo}")
		};
		var ex = Assert.Throws<ValidationTestException>(() =>
			validator.TestValidate(new Person())
				.ShouldHaveValidationErrorFor(x => x.Surname)
				.WithMessageArgument("Foo", "foo")
		);
		ex.Message.ShouldEqual("Expected message argument 'Foo' with value 'foo'. Actual value was 'bar'");
	}

	[Fact]
	public void Expected_state_check() {
		var validator = new InlineValidator<Person> {
			v => v.RuleFor(x => x.Surname).NotNull().WithState(x => "bar")
		};
		var ex = Assert.Throws<ValidationTestException>(() =>
			validator.TestValidate(new Person())
				.ShouldHaveValidationErrorFor(x => x.Surname)
				.WithCustomState("foo"));
		ex.Message.ShouldEqual("Expected custom state of 'foo'. Actual state was 'bar'");
	}

	[Fact]
	public void Unexpected_state_check() {
		var validator = new InlineValidator<Person> {
			v => v.RuleFor(x => x.Surname).NotNull().WithState(x => "bar"),
			v => v.RuleFor(x => x.Surname).NotNull().WithState(x => "foo"),
		};
		var ex = Assert.Throws<ValidationTestException>(() =>
			validator.TestValidate(new Person())
				.ShouldHaveValidationErrorFor(x => x.Surname)
				.WithoutCustomState("bar"));
		ex.Message.ShouldEqual("Found an unexpected custom state of 'bar'");
	}

	[Fact]
	public void Test_custom_state_with_concatenated_string() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => x.Surname).NotNull().WithState(x => "Test" + 123);
		var result = validator.TestValidate(new Person());

		// String concatenated with integer means a different string reference is created:
		/*
			object s1 = "Test" + 123.ToString();
			object s2 = "Test123";
			bool check1 = s1 == s2; // False
		 */
		// Test to ensure that this scenario is handled properly.
		result.ShouldHaveValidationErrorFor(x => x.Surname)
			.WithCustomState("Test123");
	}

	[Fact]
	public void Custom_state_comparer_check() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => x.Surname).NotNull().WithState(x => "Test" + 123);
		var result = validator.TestValidate(new Person());

		// Throws without comparer.
		Assert.Throws<ValidationTestException>(() => {
			result.ShouldHaveValidationErrorFor(x => x.Surname)
				.WithCustomState("test123");
		});

		// Doesn't throw with comparer.
		result.ShouldHaveValidationErrorFor(x => x.Surname)
			.WithCustomState("test123", StringComparer.OrdinalIgnoreCase);
	}

	[Fact]
	public void Expected_error_code_check() {
		var validator = new InlineValidator<Person> {
			v => v.RuleFor(x => x.Surname).NotNull().WithErrorCode("bar")
		};
		var ex = Assert.Throws<ValidationTestException>(() =>
			validator.TestValidate(new Person())
				.ShouldHaveValidationErrorFor(x => x.Surname)
				.WithErrorCode("foo"));
		ex.Message.ShouldEqual("Expected an error code of 'foo'. Actual error code was 'bar'");
	}

	[Fact]
	public void Unexpected_error_code_check() {
		var validator = new InlineValidator<Person> {
			v => v.RuleFor(x => x.Surname).NotNull().WithErrorCode("bar"),
			v => v.RuleFor(x => x.Surname).NotNull().WithErrorCode("foo")
		};
		var ex = Assert.Throws<ValidationTestException>(() =>
			validator.TestValidate(new Person())
				.ShouldHaveValidationErrorFor(x => x.Surname)
				.WithoutErrorCode("bar"));
		ex.Message.ShouldEqual("Found an unexpected error code of 'bar'");
	}

	[Fact]
	public void Expected_without_error_code_check() {
		//#1937
		var validator = new InlineValidator<Person> {
			v => v.RuleFor(x => x.Surname).NotNull(),
			v => v.RuleFor(x => x.Forename).NotNull()
		};

		validator.TestValidate(new Person())
			.ShouldHaveValidationErrorFor(x => x.Surname)
			.WithoutErrorCode("foo")
			.WithoutErrorMessage("bar")
			.WithoutSeverity(Severity.Warning)
			.WithoutCustomState(1);
	}

	[Fact]
	public void Unexpected_with_error_message_check() {
		//#1937
		var validator = new InlineValidator<Person>
		{
			v => v.RuleFor(x => x.Forename).NotEmpty(),
			v => v.RuleFor(x => x.Surname).NotEmpty()
		};

		var ex = Assert.Throws<ValidationTestException>(() =>
			validator.TestValidate(new Person())
				.ShouldHaveValidationErrorFor(x => x.Surname)
				.WithErrorMessage("bar"));
		ex.Message.ShouldEqual("Expected an error message of 'bar'. Actual message was ''Surname' must not be empty.'");
	}

	[Fact]
	public void Expected_with_error_code_check() {
		var validator = new InlineValidator<Person> {
			v => v.RuleFor(x => x.Forename).NotNull(),
			v => v.RuleFor(x => x.Surname).NotNull()
				.WithErrorCode("foo")
				.WithMessage("bar")
				.WithSeverity(Severity.Warning)
				.WithState(_ => 1)
		};

		validator.TestValidate(new Person())
			.ShouldHaveValidationErrorFor(x => x.Surname)
			.WithErrorCode("foo")
			.WithErrorMessage("bar")
			.WithSeverity(Severity.Warning)
			.WithCustomState(1);
	}

	[Fact]
	public void Expected_severity_check() {
		var validator = new InlineValidator<Person> {
			v => v.RuleFor(x => x.Surname).NotNull().WithSeverity(Severity.Warning)
		};

		var ex = Assert.Throws<ValidationTestException>(() =>
			validator.TestValidate(new Person())
				.ShouldHaveValidationErrorFor(x => x.Surname)
				.WithSeverity(Severity.Error));

		ex.Message.ShouldEqual($"Expected a severity of '{nameof(Severity.Error)}'. Actual severity was '{nameof(Severity.Warning)}'");
	}

	[Fact]
	public void Unexpected_severity_check() {
		var validator = new InlineValidator<Person> {
			v => v.RuleFor(x => x.Surname).NotNull().WithSeverity(Severity.Warning),
			v => v.RuleFor(x => x.Surname).NotNull().WithSeverity(Severity.Error),
		};
		var ex = Assert.Throws<ValidationTestException>(() =>
			validator.TestValidate(new Person())
				.ShouldHaveValidationErrorFor(x => x.Surname)
				.WithoutSeverity(Severity.Warning));

		ex.Message.ShouldEqual($"Found an unexpected severity of '{nameof(Severity.Warning)}'");
	}

	[Theory]
	[InlineData(42, null)]
	[InlineData(42, "")]
	public async Task ShouldHaveValidationError_should_not_throw_when_there_are_validation_errors__WhenAsyn_is_used(int age, string cardNumber) {
		Person testPerson = new Person() {
			CreditCard = cardNumber,
			Age = age
		};

		validator.RuleFor(x => x.CreditCard)
			.Must(creditCard => !string.IsNullOrEmpty(creditCard))
			.WhenAsync((x, cancel) => Task.FromResult(x.Age >= 18));

		// Throws when called sync.
		Assert.Throws<AsyncValidatorInvokedSynchronouslyException>(() =>
			validator.TestValidate(testPerson).ShouldHaveValidationErrorFor(x => x.CreditCard));

		(await validator.TestValidateAsync(testPerson)).ShouldHaveValidationErrorFor(x => x.CreditCard);
	}

	[Theory]
	[InlineData(42, "cardNumber")]
	[InlineData(17, null)]
	[InlineData(17, "")]
	[InlineData(17, "cardNumber")]
	public void ShouldHaveValidationError_should_throw_when_there_are_not_validation_errors__WhenAsyn_Is_Used(int age, string cardNumber) {
		Person testPerson = new Person() {
			CreditCard = cardNumber,
			Age = age
		};

		Assert.Throws<ValidationTestException>(() => validator.TestValidate(testPerson).ShouldHaveValidationErrorFor(x => x.CreditCard));
	}

	[Theory]
	[InlineData(42, "cardNumber")]
	[InlineData(17, null)]
	[InlineData(17, "")]
	[InlineData(17, "cardNumber")]
	public void ShouldNotHaveValidationError_should_throw_when_there_are_not_validation_errors__WhenAsyn_is_used(int age, string cardNumber) {
		Person testPerson = new Person() {
			CreditCard = cardNumber,
			Age = age
		};

		validator.TestValidate(testPerson)
			.ShouldNotHaveValidationErrorFor(x => x.CreditCard);
	}

	[Theory]
	[InlineData(42, null)]
	[InlineData(42, "")]
	public async Task ShouldNotHaveValidationError_should_throw_when_there_are_validation_errors__WhenAsyn_is_used(int age, string cardNumber) {
		Person testPerson = new Person() {
			CreditCard = cardNumber,
			Age = age
		};

		validator.RuleFor(x => x.CreditCard)
			.Must(creditCard => !string.IsNullOrEmpty(creditCard))
			.WhenAsync((x, cancel) => Task.FromResult(x.Age >= 18));

		// Throws async exception when invoked synchronously
		Assert.Throws<AsyncValidatorInvokedSynchronouslyException>(() => validator.TestValidate(testPerson).ShouldNotHaveValidationErrorFor(x => x.CreditCard));

		// Executes normally when run async.
		await Assert.ThrowsAsync<ValidationTestException>(async () =>
			(await validator.TestValidateAsync(testPerson)).ShouldNotHaveValidationErrorFor(x => x.CreditCard));
	}

	[Fact]
	public void ShouldHaveChildValidator_should_work_with_DependentRules() {
		var validator = new InlineValidator<Person>();

		validator.RuleFor(x => x.Children)
			.NotNull().When(p => true)
			.DependentRules(() => {
				validator.RuleForEach(p => p.Children).SetValidator(p => new InlineValidator<Person>());
			});

		validator.ShouldHaveChildValidator(x => x.Children, typeof(InlineValidator<Person>));
	}

	[Fact]
	public void Allows_only_one_failure_to_match() {
		var validator = new InlineValidator<Person> {
			v => v.RuleFor(x => x.Surname).Equal("a").WithErrorCode("nota"),
			v => v.RuleFor(x => x.Surname).Equal("b").WithErrorCode("notb")
		};

		var person = new Person() { Surname = "c" };
		var result = validator.TestValidate(person);

		result.ShouldHaveValidationErrors().WithErrorCode("nota");
		result.ShouldHaveValidationErrors().WithErrorCode("notb");
	}

	[Fact]
	public void Matches_any_failure() {
		var validator = new InlineValidator<Person> {
			v => v.RuleFor(x => x.Surname).NotEqual("foo"),
		};

		var resultWithFailure = validator.TestValidate(new Person { Surname = "foo"});
		var resultWithoutFailure = validator.TestValidate(new Person { Surname = ""});

		Assert.Throws<ValidationTestException>(() => resultWithoutFailure.ShouldHaveValidationErrors());
		Assert.Throws<ValidationTestException>(() => resultWithFailure.ShouldNotHaveAnyValidationErrors());

		// Neither should throw.
		resultWithFailure.ShouldHaveValidationErrors();
		resultWithoutFailure.ShouldNotHaveAnyValidationErrors();
	}

	[Fact]
	public void Model_level_check_fails_if_no_model_level_failures() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => x.Surname).Must(x => false);
		var result = validator.TestValidate(new Person());

		Assert.Throws<ValidationTestException>(() => {
			result.ShouldHaveValidationErrorFor(x => x);
		});

		Assert.Throws<ValidationTestException>(() => {
			result.ShouldHaveValidationErrorFor("");
		});
	}

	[Fact]
	public void Matches_model_level_rule() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => x).Must(x => false);
		validator.RuleFor(x => x.Surname).Must(x => false);

		var result = validator.TestValidate(new Person());
		result.ShouldHaveValidationErrorFor(x => x);
		result.ShouldHaveValidationErrorFor("");
	}

	[Fact]
	public void Can_use_indexer_in_string_message() {
		var validator = new InlineValidator<Person>();
		var orderValidator = new InlineValidator<Order>();
		orderValidator.RuleFor(x => x.ProductName).NotNull();
		validator.RuleForEach(x => x.Orders).SetValidator(orderValidator);

		var model = new Person { Orders = new List<Order> { new Order() }};
		var result = validator.TestValidate(model);
		result.ShouldHaveValidationErrorFor("Orders[0].ProductName");
	}

	[Fact]
	public void Can_use_indexer_in_string_message_inverse() {
		var validator = new InlineValidator<Person>();
		var orderValidator = new InlineValidator<Order>();
		orderValidator.RuleFor(x => x.ProductName).Null();
		validator.RuleForEach(x => x.Orders).SetValidator(orderValidator);

		var model = new Person { Orders = new List<Order> { new Order() }};
		var result = validator.TestValidate(model);
		result.ShouldNotHaveValidationErrorFor("Orders[0].ProductName");
	}

	[Fact]
	public async Task TestValidate_runs_async() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => x.Surname).MustAsync((x, ct) => Task.FromResult(false));
		var result = await validator.TestValidateAsync(new Person());
		result.ShouldHaveValidationErrorFor(x => x.Surname);
	}

	[Fact]
	public async Task TestValidate_runs_async_throws() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => x.Surname).MustAsync((x, ct) => Task.FromResult(false));
		var result = await validator.TestValidateAsync(new Person());
		Assert.Throws<ValidationTestException>(() => {
			result.ShouldNotHaveValidationErrorFor(x => x.Surname);
		});
	}

	[Fact]
	public async Task ShouldHaveValidationError_async() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => x.Surname).MustAsync((x, ct) => Task.FromResult(false));
		(await validator.TestValidateAsync(new Person())).ShouldHaveValidationErrorFor(x => x.Surname);
	}

	[Fact]
	public async Task ShouldHaveValidationError_async_throws() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => x.Surname).MustAsync((x, ct) => Task.FromResult(true));
		await Assert.ThrowsAsync<ValidationTestException>(async () => {
			(await validator.TestValidateAsync(new Person())).ShouldHaveValidationErrorFor(x => x.Surname);
		});
	}

	[Fact]
	public async Task ShouldNotHaveValidationError_async() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => x.Surname).MustAsync((x, ct) => Task.FromResult(true));
		(await validator.TestValidateAsync(new Person())).ShouldNotHaveValidationErrorFor(x => x.Surname);
	}

	[Fact]
	public async Task ShouldNotHaveValidationError_async_throws() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => x.Surname).MustAsync((x, ct) => Task.FromResult(false));
		await Assert.ThrowsAsync<ValidationTestException>(async () => {
			(await validator.TestValidateAsync(new Person())).ShouldNotHaveValidationErrorFor(x => x.Surname);
		});
	}

	[Fact]
	public async Task ShouldHaveValidationError_model_async() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => x.Surname).MustAsync((x, ct) => Task.FromResult(false));
		(await validator.TestValidateAsync(new Person())).ShouldHaveValidationErrorFor(x => x.Surname);
	}

	[Fact]
	public async Task ShouldHaveValidationError_model_async_throws() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => x.Surname).MustAsync((x, ct) => Task.FromResult(true));
		await Assert.ThrowsAsync<ValidationTestException>(async () => {
			(await validator.TestValidateAsync(new Person())).ShouldHaveValidationErrorFor(x => x.Surname);
		});
	}

	[Fact]
	public async Task ShouldNotHaveValidationError_model_async() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => x.Surname).MustAsync((x, ct) => Task.FromResult(true));
		(await validator.TestValidateAsync(new Person())).ShouldNotHaveValidationErrorFor(x => x.Surname);
	}

	[Fact]
	public async Task ShouldNotHaveValidationError_async_model_throws() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => x.Surname).MustAsync((x, ct) => Task.FromResult(false));
		await Assert.ThrowsAsync<ValidationTestException>(async () => {
			(await validator.TestValidateAsync(new Person())).ShouldNotHaveValidationErrorFor(x => x.Surname);
		});
	}

	[Fact]
	public void ShouldHaveValidationErrorFor_Only() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => x.Surname).Must((x, ct) => false);
		validator.TestValidate(new Person())
			.ShouldHaveValidationErrorFor(x => x.Surname)
			.Only();
	}

	[Fact]
	public void ShouldHaveValidationErrorFor_Only_throws() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => x.Surname).Must((x, ct) => false);
		validator.RuleFor(x => x.Age).Must((x, ct) => false);
		Assert.Throws<ValidationTestException>(() =>
			validator.TestValidate(new Person())
				.ShouldHaveValidationErrorFor(x => x.Surname)
				.Only()
		).Message.ShouldEqual("Expected to have errors only matching specified conditions\n----\nUnexpected Errors:\n[0]: The specified condition was not met for 'Age'.\n");
	}

	[Fact]
	public async Task ShouldHaveValidationErrorFor_Only_async() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => x.Surname).MustAsync((x, ct) => Task.FromResult(false));
		(await validator.TestValidateAsync(new Person()))
			.ShouldHaveValidationErrorFor(x => x.Surname)
			.Only();
	}

	[Fact]
	public async Task ShouldHaveValidationErrorFor_Only_async_throws() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => x.Surname).MustAsync((x, ct) => Task.FromResult(false));
		validator.RuleFor(x => x.Age).MustAsync((x, ct) => Task.FromResult(false));
		(await Assert.ThrowsAsync<ValidationTestException>(async () => {
			(await validator.TestValidateAsync(new Person()))
				.ShouldHaveValidationErrorFor(x => x.Surname)
				.Only();
		})).Message.ShouldEqual("Expected to have errors only matching specified conditions\n----\nUnexpected Errors:\n[0]: The specified condition was not met for 'Age'.\n");
	}

	[Fact]
	public void ShouldHaveValidationErrorFor_WithMultipleRules_Only() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => x.Surname)
			.Must((x, ct) => false)
			.NotEmpty();
		validator.TestValidate(new Person())
			.ShouldHaveValidationErrorFor(x => x.Surname)
			.Only();
	}

	[Fact]
	public void ShouldHaveValidationErrorFor_WithMessage_Only_throws() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => x.Surname)
			.Must((x, ct) => false)
			.NotEmpty();
		Assert.Throws<ValidationTestException>(() =>
			validator.TestValidate(new Person())
				.ShouldHaveValidationErrorFor(x => x.Surname)
				.WithErrorMessage("The specified condition was not met for 'Surname'.")
				.Only()
		).Message.ShouldEqual("Expected to have errors only matching specified conditions\n----\nUnexpected Errors:\n[0]: 'Surname' must not be empty.\n");
	}

	[Fact]
	public void ShouldHaveValidationErrorFor_WithMessage_Only() {
		var validator = new InlineValidator<Person>();
		var message = "Something's wrong but I won't tell you what";
		validator.RuleFor(x => x.Surname)
			.Must((x, ct) => false).WithMessage(message)
			.NotEmpty().WithMessage(message);
		validator.TestValidate(new Person())
			.ShouldHaveValidationErrorFor(x => x.Surname)
			.WithErrorMessage(message)
			.Only();
	}

	[Fact]
	public void ShouldHaveValidationErrorFor_WithMessage_Only_throws_when_there_is_a_failure_for_a_different_property() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => x.Surname)
			.Must((x, ct) => false);
		validator.RuleFor(x => x.Forename).NotEmpty();

		Assert.Throws<ValidationTestException>(() =>
			validator.TestValidate(new Person())
				.ShouldHaveValidationErrorFor(x => x.Surname)
				.WithErrorMessage("The specified condition was not met for 'Surname'.")
				.Only()
		).Message.ShouldEqual("Expected to have errors only matching specified conditions\n----\nUnexpected Errors:\n[0]: 'Forename' must not be empty.\n");
	}

	[Fact]
	public void ShouldHaveValidationErrorFor_WithSeverity_Only() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => x.Surname)
			.Must((x, ct) => false).WithSeverity(Severity.Warning)
			.NotEmpty().WithSeverity(Severity.Warning);
		validator.TestValidate(new Person())
			.ShouldHaveValidationErrorFor(x => x.Surname)
			.WithSeverity(Severity.Warning)
			.Only();
	}

	[Fact]
	public void ShouldHaveValidationErrorFor_WithSeverity_Only_throws() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => x.Surname)
			.Must((x, ct) => false).WithSeverity(Severity.Warning)
			.NotEmpty();
		Assert.Throws<ValidationTestException>(() =>
			validator.TestValidate(new Person())
				.ShouldHaveValidationErrorFor(x => x.Surname)
				.WithSeverity(Severity.Warning)
				.Only()
		).Message.ShouldEqual("Expected to have errors only matching specified conditions\n----\nUnexpected Errors:\n[0]: 'Surname' must not be empty.\n");
	}

	[Fact]
	public void ShouldHaveValidationErrorFor_WithErrorCode_Only() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => x.Surname)
			.Must((x, ct) => false).WithErrorCode("100")
			.NotEmpty().WithErrorCode("100");
		validator.TestValidate(new Person())
			.ShouldHaveValidationErrorFor(x => x.Surname)
			.WithErrorCode("100")
			.Only();
	}

	[Fact]
	public void ShouldHaveValidationErrorFor_WithErrorCode_Only_throws() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => x.Surname)
			.Must((x, ct) => false).WithErrorCode("100")
			.NotEmpty().WithErrorCode("200");
		Assert.Throws<ValidationTestException>(() =>
			validator.TestValidate(new Person())
				.ShouldHaveValidationErrorFor(x => x.Surname)
				.WithErrorCode("100")
				.Only()
		).Message.ShouldEqual("Expected to have errors only matching specified conditions\n----\nUnexpected Errors:\n[0]: 'Surname' must not be empty.\n");
	}

	[Fact]
	public void ShouldHaveValidationErrorFor_WithPropertyName_Only() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => DateTime.Now)
			.Must((x, ct) => false);
		validator.TestValidate(new Person())
			.ShouldHaveValidationErrorFor("Now")
			.WithErrorMessage("The specified condition was not met for 'Now'.")
			.Only();
	}

	[Fact]
	public void ShouldHaveValidationErrorFor_WithPropertyName_Only_throws() {
		var validator = new InlineValidator<Person>();
		validator.RuleFor(x => DateTime.Now)
			.Must((x, ct) => false)
			.LessThan(new DateTime(1900, 1, 1));
		Assert.Throws<ValidationTestException>(() =>
			validator.TestValidate(new Person())
				.ShouldHaveValidationErrorFor("Now")
				.WithErrorMessage("The specified condition was not met for 'Now'.")
				.Only()
		).Message.ShouldEqual("Expected to have errors only matching specified conditions\n----\nUnexpected Errors:\n[0]: 'Now' must be less than '1/1/1900 12:00:00 AM'.\n");
	}

	[Fact]
	public void ShouldHaveValidationErrorFor_WithMessage_Only_throws_combining_several_conditions() {
		var validator = new InlineValidator<Person>();

		// 2 rules with same property and message, different error code.
		validator.RuleFor(x => x.Surname)
			.Must((x, ct) => false);

		validator.RuleFor(x => x.Surname)
			.Must((x, ct) => false)
			.WithErrorCode("Foo");

		Assert.Throws<ValidationTestException>(() =>
			validator.TestValidate(new Person())
				.ShouldHaveValidationErrorFor(x => x.Surname)
				.WithErrorMessage("The specified condition was not met for 'Surname'.")
				.WithErrorCode("Foo")
				.Only()
		).Message.ShouldEqual("Expected to have errors only matching specified conditions\n----\nUnexpected Errors:\n[0]: The specified condition was not met for 'Surname'.\n");
	}

	[Fact]
	public void ShouldHaveValidationErrorFor_WithMessage_Only_throws_combining_several_conditions_and_another_property() {
		var validator = new InlineValidator<Person>();

		// 2 rules with same property and message, different error code.
		validator.RuleFor(x => x.Surname)
			.Must((x, ct) => false);

		validator.RuleFor(x => x.Surname)
			.Must((x, ct) => false)
			.WithErrorCode("Foo");

		// Another message for a different property.
		validator.RuleFor(x => x.Forename).NotEmpty();

		Assert.Throws<ValidationTestException>(() =>
			validator.TestValidate(new Person())
				.ShouldHaveValidationErrorFor(x => x.Surname)
				.WithErrorMessage("The specified condition was not met for 'Surname'.")
				.WithErrorCode("Foo")
				.Only()
		).Message.ShouldEqual("Expected to have errors only matching specified conditions\n----\nUnexpected Errors:\n[0]: The specified condition was not met for 'Surname'.\n[1]: 'Forename' must not be empty.\n");
	}

	private class AddressValidator : AbstractValidator<Address> {
	}

	private class OrderValidator : AbstractValidator<Order> {
	}

	public class UnitOfMeasure {
		public int Value { get; set; }
		public int? Type { get; set; }
	}


	public class UnitOfMeasureValidator : AbstractValidator<UnitOfMeasure> {
		public UnitOfMeasureValidator() {
			RuleFor(unit => unit.Value).GreaterThanOrEqualTo(0);

			RuleFor(unit => unit.Type).NotNull()
				.When(unit => unit.Value > 0)
				.WithMessage("If a unit of measure's 'Value' is provided, then a 'Type' also needs to be provided.");
		}
	}

	public class Address2 {
		public string StreetNumber { get; set; }
		public string Street { get; set; }
	}

	public class Address2Validator : InlineValidator<Address2> {
		public static string RuleLocationNames = "LocationNames";

		public Address2Validator() {
			// Cannot have a street number/lot and no street name.
			RuleFor(address => address.Street)
				.NotNull()
				.When(address => !string.IsNullOrWhiteSpace(address.StreetNumber))
				.WithMessage("A street name is required when a street number has been provided. Eg. Smith Street.");
		}
	}
}
