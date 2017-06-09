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
	using TestHelper;
	using Validators;


	public class ValidatorTesterTester {
		private TestValidator validator;

		public ValidatorTesterTester() {
			validator = new TestValidator();
			validator.RuleFor(x => x.Forename).NotNull();
		}

		[Fact]
		public void ShouldHaveValidationError_should_not_throw_when_there_are_validation_errors() {
			validator.ShouldHaveValidationErrorFor(x => x.Forename, (string)null);
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
        public void ShouldNotHaveValidationError_Should_support_nested_properties()
        {
            validator.RuleFor(x => x.Address.Line1).NotNull();
            validator.ShouldNotHaveValidationErrorFor(x => x.Address.Line1, new Person
            {
                Address = new Address
                {
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
			typeof(ValidationTestException).ShouldBeThrownBy(() => validator.ShouldNotHaveValidationErrorFor(x => x.Forename, (string)null));
		}

		[Fact]
		public void ShouldHaveValidationError_should_not_throw_when_there_are_errors_with_preconstructed_object() {
			validator.ShouldHaveValidationErrorFor(x => x.Forename, new Person { Forename = null });
		}

		[Fact]
		public void ShouldHaveValidationError_should_throw_when_there_are_no_validation_errors_with_preconstructed_object() {
			typeof(ValidationTestException).ShouldBeThrownBy(() => validator.ShouldHaveValidationErrorFor(x => x.Forename, new Person { Forename = "test" }));
		}

		[Fact]
		public void ShouldNotHAveValidationError_should_not_throw_When_there_are_no_errors_with_preconstructed_object() {
			validator.ShouldNotHaveValidationErrorFor(x => x.Forename, new Person { Forename = "test" });
		}

		[Fact]
		public void ShouldNotHaveValidationError_should_throw_when_there_are_errors_with_preconstructed_object() {
			typeof(ValidationTestException).ShouldBeThrownBy(() => validator.ShouldNotHaveValidationErrorFor(x => x.Forename, new Person { Forename = null }));
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
		public void ShouldHaveChildvalidator_throws_when_collection_property_Does_not_have_child_validator() {
			var ex = typeof(ValidationTestException).ShouldBeThrownBy(() =>
				validator.ShouldHaveChildValidator(x => x.Orders, typeof(OrderValidator))
			);

			ex.Message.ShouldEqual("Expected property 'Orders' to have a child validator of type 'OrderValidator.'. Instead found 'none'");
		}

		[Fact]
		public void ShouldHaveChildValidator_should_throw_when_property_has_a_different_child_validator()
		{
			validator.RuleFor(x => x.Address).SetValidator(new AddressValidator());
			var ex = typeof(ValidationTestException).ShouldBeThrownBy(() =>
				validator.ShouldHaveChildValidator(x => x.Address, typeof(OrderValidator))
			);

			ex.Message.ShouldEqual("Expected property 'Address' to have a child validator of type 'OrderValidator.'. Instead found 'AddressValidator'");
		}

		[Fact]
		public void ShouldHaveChildValidator_should_not_throw_when_property_does_not_have_child_validator() {
			validator.RuleFor(x => x.Orders).SetCollectionValidator(new OrderValidator());
			validator.ShouldHaveChildValidator(x => x.Orders, typeof(OrderValidator));
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
		public void ShouldHaveValidationError_should_correctly_handle_explicitly_providing_object_to_validate()
		{
			var unitOfMeasure = new UnitOfMeasure
			{
				Value = 1
			};

			var validator = new UnitOfMeasureValidator();

			validator.ShouldHaveValidationErrorFor(unit => unit.Type, unitOfMeasure);
		}

		[Fact]
		public void ShouldNotHaveValidationError_should_correctly_handle_explicitly_providing_object_to_validate()
		{
			var unitOfMeasure = new UnitOfMeasure
			{
				Value = 1,
				Type = 43
			};

			var validator = new UnitOfMeasureValidator();

			validator.ShouldNotHaveValidationErrorFor(unit => unit.Type, unitOfMeasure);
		}

		[Fact]
		public void ShouldNotHaveValidationError_should_correctly_handle_explicitly_providing_object_to_validate_and_other_property_fails_validation()
		{
			var validator = new Address2Validator();
			validator.RuleFor(x => x.StreetNumber).Equal("foo");

			var address = new Address2
			{
				StreetNumber = "a",
				Street = "b"
			};

			validator.ShouldNotHaveValidationErrorFor(a => a.Street, address);
		}

		private class AddressValidator : AbstractValidator<Address> {

		}

		private class OrderValidator : AbstractValidator<Order> {

		}

		public class UnitOfMeasure
		{
			public int Value { get; set; }
			public int? Type { get; set; }
		}

		
        public class UnitOfMeasureValidator : AbstractValidator<UnitOfMeasure>
        {
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

		public class Address2Validator : AbstractValidator<Address2>
		{
			public static string RuleLocationNames = "LocationNames";

			public Address2Validator()
			{
				// Cannot have a street number/lot and no street name.
				RuleFor(address => address.Street)
					.NotNull()
					.When(address => !string.IsNullOrWhiteSpace(address.StreetNumber))
					.WithMessage("A street name is required when a street number has been provided. Eg. Smith Street.");
			}
		}
	}
}