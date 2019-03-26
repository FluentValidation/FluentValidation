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
	using Xunit;
	using TestHelper;
	using Validators;
	using System.Threading.Tasks;

	public class ValidatorTesterTester {
		private TestValidator validator;

		public ValidatorTesterTester() {
			validator = new TestValidator();
			validator.RuleFor(x => x.CreditCard).Must(creditCard => !string.IsNullOrEmpty(creditCard)).WhenAsync((x, cancel) => Task.Run(() => { return x.Age >= 18; }));
			validator.RuleFor(x => x.Forename).NotNull();
			validator.RuleForEach(person => person.NickNames).MinimumLength(5);
		}

		[Fact]
		public void ShouldHaveValidationError_should_not_throw_when_there_are_validation_errors_ruleforeach() {
			validator.ShouldHaveValidationErrorFor(l => l.NickNames, new[] {"magician", "bull"});
		}

		[Fact]
		public void ShouldHaveValidationError_should_throw_when_there_are_not_validation_errors_ruleforeach() {
			ValidationTestException validationTestException = Assert.Throws<ValidationTestException>(() =>
				validator.ShouldHaveValidationErrorFor(l => l.NickNames, new[] {"magician", "awesome"}));

			Assert.Contains(nameof(Person.NickNames), validationTestException.Message);
		}

		[Fact]
		public void ShouldNotHaveValidationError_should_not_throw_when_there_are_not_validation_errors_ruleforeach() {
			validator.ShouldNotHaveValidationErrorFor(l => l.NickNames, new[] {"magician", "awesome"});
		}

		[Fact]
		public void ShouldNotHaveValidationError_should_throw_when_there_are_validation_errors_ruleforeach() {
			ValidationTestException validationTestException = Assert.Throws<ValidationTestException>(() =>
				validator.ShouldNotHaveValidationErrorFor(l => l.NickNames, new[] {"magician", "bull"}));

			Assert.Contains(nameof(Person.NickNames), validationTestException.Message);
		}

		[Fact]
		public void ShouldNotHaveValidationError_should_have_validation_error_details_when_thrown_ruleforeach() {
			ValidationTestException validationTestException = Assert.Throws<ValidationTestException>(() =>
				validator.ShouldNotHaveValidationErrorFor(l => l.NickNames, new[] { "magician", "bull" }));
			Assert.Contains("The length of 'Nick Names' must be at least 5 characters. You entered 4 characters.", validationTestException.Message);
		}

		[Fact]
		public void ShouldHaveValidationError_should_not_throw_when_there_are_validation_errors() {
			validator.ShouldHaveValidationErrorFor(x => x.Forename, (string) null);
		}

		[Fact]
		public void ShouldHaveValidationError_Should_support_nested_properties() {
			validator.RuleFor(x => x.Address.Line1).NotNull();
			validator.ShouldHaveValidationErrorFor(x => x.Address.Line1, new Person {
				Address = new Address {
					Line1 = null,
				},
			});
		}

		[Fact]
		public void ShouldNotHaveValidationError_Should_support_nested_properties() {
			validator.RuleFor(x => x.Address.Line1).NotNull();
			validator.ShouldNotHaveValidationErrorFor(x => x.Address.Line1, new Person {
				Address = new Address {
					Line1 = "anything",
				},
			});
		}

		[Fact]
		public void ShouldHaveValidationError_Should_throw_when_there_are_no_validation_errors() {
			typeof(ValidationTestException).ShouldBeThrownBy(() => validator.ShouldHaveValidationErrorFor(x => x.Forename, "test"));
		}

		[Fact]
		public void ShouldNotHaveValidationError_should_not_throw_when_there_are_no_errors() {
			validator.ShouldNotHaveValidationErrorFor(x => x.Forename, "test");
		}

		[Fact]
		public void ShouldNotHaveValidationError_should_throw_when_there_are_errors() {
			typeof(ValidationTestException).ShouldBeThrownBy(() => validator.ShouldNotHaveValidationErrorFor(x => x.Forename, (string) null));
		}

		[Fact]
		public void ShouldHaveValidationError_should_not_throw_when_there_are_errors_with_preconstructed_object() {
			validator.ShouldHaveValidationErrorFor(x => x.Forename, new Person {Forename = null});
		}

		[Fact]
		public void ShouldHaveValidationError_should_throw_when_there_are_no_validation_errors_with_preconstructed_object() {
			typeof(ValidationTestException).ShouldBeThrownBy(() => validator.ShouldHaveValidationErrorFor(x => x.Forename, new Person {Forename = "test"}));
		}

		[Fact]
		public void ShouldNotHAveValidationError_should_not_throw_When_there_are_no_errors_with_preconstructed_object() {
			validator.ShouldNotHaveValidationErrorFor(x => x.Forename, new Person {Forename = "test"});
		}

		[Fact]
		public void ShouldNotHaveValidationError_should_throw_when_there_are_errors_with_preconstructed_object() {
			typeof(ValidationTestException).ShouldBeThrownBy(() => validator.ShouldNotHaveValidationErrorFor(x => x.Forename, new Person {Forename = null}));
		}


		[Fact]
		public void ShouldHaveChildValidator_throws_when_property_does_not_have_child_validator() {
			var ex = typeof(ValidationTestException).ShouldBeThrownBy(() =>
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
			var ex = typeof(ValidationTestException).ShouldBeThrownBy(() =>
				validator.ShouldHaveChildValidator(x => x.Orders, typeof(OrderValidator))
			);

			ex.Message.ShouldEqual("Expected property 'Orders' to have a child validator of type 'OrderValidator.'. Instead found 'none'");
		}

		[Fact]
		public void ShouldHaveChildValidator_should_throw_when_property_has_a_different_child_validator() {
			validator.RuleFor(x => x.Address).SetValidator(new AddressValidator());
			var ex = typeof(ValidationTestException).ShouldBeThrownBy(() =>
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

			testValidator.ShouldHaveValidationErrorFor(x => x.Forename, new Person(), "Names");
		}

		[Fact]
		public void ShouldHaveValidationErrorFor_takes_account_of_rulesets_fluent_approach() {
			var testValidator = new TestValidator();
			testValidator.RuleSet("Names", () => {
				testValidator.RuleFor(x => x.Surname).NotNull();
				testValidator.RuleFor(x => x.Forename).NotNull();
			});
			testValidator.RuleFor(x => x.Id).NotEqual(0);

			var assertionRoot = testValidator.TestValidate(new Person(), "Names").Which;

			assertionRoot.Property(x => x.Forename).ShouldHaveValidationError()
				.WithErrorCode("NotNullValidator");
			assertionRoot.Property(x => x.Surname).ShouldHaveValidationError().WithErrorCode("NotNullValidator");
			assertionRoot.Property(x => x.Id).ShouldNotHaveValidationError();
		}

		[Fact]
		public void ShouldHaveValidationError_should_correctly_handle_explicitly_providing_object_to_validate() {
			var unitOfMeasure = new UnitOfMeasure {
				Value = 1
			};

			var validator = new UnitOfMeasureValidator();

			validator.ShouldHaveValidationErrorFor(unit => unit.Type, unitOfMeasure);
		}

		[Fact]
		public void ShouldNotHaveValidationError_should_correctly_handle_explicitly_providing_object_to_validate() {
			var unitOfMeasure = new UnitOfMeasure {
				Value = 1,
				Type = 43
			};

			var validator = new UnitOfMeasureValidator();

			validator.ShouldNotHaveValidationErrorFor(unit => unit.Type, unitOfMeasure);
		}

		[Fact]
		public void ShouldNotHaveValidationError_should_correctly_handle_explicitly_providing_object_to_validate_and_other_property_fails_validation() {
			var validator = new Address2Validator();
			validator.RuleFor(x => x.StreetNumber).Equal("foo");

			var address = new Address2 {
				StreetNumber = "a",
				Street = "b"
			};

			validator.ShouldNotHaveValidationErrorFor(a => a.Street, address);
		}

		[Fact]
		public void ShouldHaveValidationError_preconstructed_object_does_not_throw_for_unwritable_property() {
			validator.RuleFor(x => x.ForenameReadOnly).NotNull();
			validator.ShouldHaveValidationErrorFor(x => x.ForenameReadOnly, new Person {Forename = null});
		}

		[Fact]
		public void Expected_message_check() {
			bool exceptionCaught = false;

			try {
				var validator = new InlineValidator<Person> {
					v => v.RuleFor(x => x.Surname).NotNull().WithMessage("bar")
				};
				validator.ShouldHaveValidationErrorFor(x => x.Surname, null as string).WithErrorMessage("foo");
			}
			catch (ValidationTestException e) {
				exceptionCaught = true;

				e.Message.ShouldEqual("Expected an error message of 'foo'. Actual message was 'bar'");
			}

			exceptionCaught.ShouldBeTrue();
		}

		/// <summary>
		/// Full test (for WhenAll)
		/// </summary>
		/// <param name="withoutErrMsg"></param>
		/// <param name="errMessages"></param>
		/// <param name="shouldBe"></param>
		[Theory]
		[InlineData("bar", new string[] { })]
		[InlineData("bar", new string[] { "foo", })]
		[InlineData("bar", new string[] { "foo", "bar" })]
		[InlineData("bar", new string[] { "bar", })]
		public void Unexpected_message_check(string withoutErrMsg, string[] errMessages) {
			bool exceptionCaught = false;

			try {
				var validator = new InlineValidator<Person>();
				foreach(var msg in errMessages) {
					validator.Add(v => v.RuleFor(x => x.Surname).NotNull().WithMessage(msg));
				}
				validator.TestValidate(new Person { }).Result.Errors.WithoutErrorMessage(withoutErrMsg);
			}
			catch (ValidationTestException e) {
				exceptionCaught = true;

				e.Message.ShouldEqual($"Found an unexpected error message of '{withoutErrMsg}'");
			}

			exceptionCaught.ShouldEqual(errMessages.Contains(withoutErrMsg));
		}

		[Fact]
		public void Expected_state_check() {
			bool exceptionCaught = false;

			try {
				var validator = new InlineValidator<Person> {
					v => v.RuleFor(x => x.Surname).NotNull().WithState(x => "bar")
				};
				validator.ShouldHaveValidationErrorFor(x => x.Surname, null as string).WithCustomState("foo");
			}
			catch (ValidationTestException e) {
				exceptionCaught = true;

				e.Message.ShouldEqual("Expected custom state of 'foo'. Actual state was 'bar'");
			}

			exceptionCaught.ShouldBeTrue();
		}

		[Fact]
		public void Unexpected_state_check() {
			bool exceptionCaught = false;

			try {
				var validator = new InlineValidator<Person> {
					v => v.RuleFor(x => x.Surname).NotNull().WithState(x => "bar"),
					v => v.RuleFor(x => x.Surname).NotNull().WithState(x => "foo"),
				};
				validator.ShouldHaveValidationErrorFor(x => x.Surname, null as string).WithoutCustomState("bar");
			}
			catch (ValidationTestException e) {
				exceptionCaught = true;

				e.Message.ShouldEqual("Found an unexpected custom state of 'bar'");
			}

			exceptionCaught.ShouldBeTrue();
		}

		[Fact]
		public void Expected_error_code_check() {
			bool exceptionCaught = false;

			try {
				var validator = new InlineValidator<Person> {
					v => v.RuleFor(x => x.Surname).NotNull().WithErrorCode("bar")
				};
				validator.ShouldHaveValidationErrorFor(x => x.Surname, null as string).WithErrorCode("foo");
			}
			catch (ValidationTestException e) {
				exceptionCaught = true;

				e.Message.ShouldEqual("Expected an error code of 'foo'. Actual error code was 'bar'");
			}

			exceptionCaught.ShouldBeTrue();
		}

		[Fact]
		public void Unexpected_error_code_check() {
			bool exceptionCaught = false;

			try {
				var validator = new InlineValidator<Person> {
					v => v.RuleFor(x => x.Surname).NotNull().WithErrorCode("bar"),
					v => v.RuleFor(x => x.Surname).NotNull().WithErrorCode("foo")
				};
				validator.ShouldHaveValidationErrorFor(x => x.Surname, null as string).WithoutErrorCode("bar");
			}
			catch (ValidationTestException e) {
				exceptionCaught = true;

				e.Message.ShouldEqual("Found an unexpected error code of 'bar'");
			}

			exceptionCaught.ShouldBeTrue();
		}

		[Fact]
		public void Expected_severity_check() {
			bool exceptionCaught = false;

			try {
				var validator = new InlineValidator<Person> {
					v => v.RuleFor(x => x.Surname).NotNull().WithSeverity(Severity.Warning)
				};
				validator.ShouldHaveValidationErrorFor(x => x.Surname, null as string).WithSeverity(Severity.Error);
			}
			catch (ValidationTestException e) {
				exceptionCaught = true;

				e.Message.ShouldEqual($"Expected a severity of '{nameof(Severity.Error)}'. Actual severity was '{nameof(Severity.Warning)}'");
			}

			exceptionCaught.ShouldBeTrue();
		}

		[Fact]
		public void Unexpected_severity_check() {
			bool exceptionCaught = false;

			try {
				var validator = new InlineValidator<Person> {
					v => v.RuleFor(x => x.Surname).NotNull().WithSeverity(Severity.Warning),
					v => v.RuleFor(x => x.Surname).NotNull().WithSeverity(Severity.Error),
				};
				validator.ShouldHaveValidationErrorFor(x => x.Surname, null as string).WithoutSeverity(Severity.Warning);
			}
			catch (ValidationTestException e) {
				exceptionCaught = true;

				e.Message.ShouldEqual($"Found an unexpected severity of '{nameof(Severity.Warning)}'");
			}

			exceptionCaught.ShouldBeTrue();
		}

		[Theory]
		[InlineData(42, null)]
		[InlineData(42, "")]
		public void ShouldHaveValidationError_should_not_throw_when_there_are_validation_errors__WhenAsyn_is_used(int age, string cardNumber) {
			Person testPerson = new Person() {
				CreditCard = cardNumber,
				Age = age
			};

			validator.ShouldHaveValidationErrorFor(x => x.CreditCard, testPerson);
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

			Assert.Throws<ValidationTestException>(() => validator.ShouldHaveValidationErrorFor(x => x.CreditCard, testPerson));
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

			validator.ShouldNotHaveValidationErrorFor(x => x.CreditCard, testPerson);
		}

		[Theory]
		[InlineData(42, null)]
		[InlineData(42, "")]
		public void ShouldNotHaveValidationError_should_throw_when_there_are_validation_errors__WhenAsyn_is_used(int age, string cardNumber) {
			Person testPerson = new Person() {
				CreditCard = cardNumber,
				Age = age
			};

			Assert.Throws<ValidationTestException>(() => validator.ShouldNotHaveValidationErrorFor(x => x.CreditCard, testPerson));
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
    
			result.ShouldHaveError().WithErrorCode("nota");
			result.ShouldHaveError().WithErrorCode("notb");
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

		public class Address2Validator : AbstractValidator<Address2> {
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
}