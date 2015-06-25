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
	using Xunit;
	using TestHelper;

	
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

			ex.Message.ShouldEqual("Expected property 'Address' to have a child validator of type 'AddressValidator.'");
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

			ex.Message.ShouldEqual("Expected property 'Orders' to have a child validator of type 'OrderValidator.'");
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
        public void ShouldHaveValidationErrorFor_takes_account_of_rulesets_fluent_approach(){
            //arrange
            var testValidator = new TestValidator();
            testValidator.RuleSet("Names", () =>
            {
                testValidator.RuleFor(x => x.Surname).NotNull();
                testValidator.RuleFor(x => x.Forename).NotNull();
            });
            testValidator.RuleFor(x => x.Id).NotEqual(0);

            //act
            var assertionRoot = testValidator.TestValidate(new Person(), "Names").Which;

            //assert
            assertionRoot.Property(x => x.Forename).ShouldHaveError().When(x => x.ErrorCode == "notnull_error");
            assertionRoot.Property(x => x.Surname).ShouldHaveError().When(x => x.ErrorCode == "notnull_error");
            assertionRoot.Property(x => x.Id).ShouldNotHaveError();
        }

		private class AddressValidator : AbstractValidator<Address> {

		}

		private class OrderValidator : AbstractValidator<Order> {

		}
	}
}