namespace FluentValidation.Tests
{
    using System.Linq;
    using Xunit;

    public class EnumValidatorTests {
		TestValidator validator;

		public EnumValidatorTests() {
			CultureScope.SetDefaultCulture();

			validator = new TestValidator {
				v => v.RuleFor(x => x.Gender).IsInEnum()
			};
		}

		[Fact]
		public void IsValidTests() {
			validator.Validate(new Person { Gender = EnumGender.Female }).IsValid.ShouldBeTrue();  // Simplest valid value
			validator.Validate(new Person { Gender = EnumGender.Male }).IsValid.ShouldBeTrue();    // Other valid value
            validator.Validate(new Person { Gender = (EnumGender)1 }).IsValid.ShouldBeTrue();      // Casting with valid value
        }

        [Fact]
        public void When_the_enum_is_not_initialized_with_valid_value_then_the_validator_should_fail() {
            var result = validator.Validate(new Person());                                         // Default value 0 is not defined in Enum
            result.IsValid.ShouldBeFalse();
        }

        [Fact]
        public void When_the_enum_is_initialized_with_invalid_value_then_the_validator_should_fail()
        {
            var result = validator.Validate(new Person { Gender = (EnumGender)3 });                // 3 in not defined in Enum
            result.IsValid.ShouldBeFalse();
        }


        [Fact]
		public void When_validation_fails_the_default_error_should_be_set() {
			var result = validator.Validate(new Person());
			result.Errors.Single().ErrorMessage.ShouldEqual("Property Gender it not a valid enum value.");
		}
	}
}