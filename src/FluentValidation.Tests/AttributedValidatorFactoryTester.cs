namespace FluentValidation.Tests {
	using Attributes;
	using Xunit;

	
	public class AttributedValidatorFactoryTester {
		IValidatorFactory factory;

		public AttributedValidatorFactoryTester() {
			factory = new AttributedValidatorFactory();
		}

		[Fact]
		public void Should_instantiate_validator() {
			var validator = factory.GetValidator<AttributedPerson>();
			validator.ShouldBe<TestValidator>();
		}

		[Fact]
		public void Should_instantiate_validator_non_generic() {
			var validator = factory.GetValidator(typeof(AttributedPerson));
			validator.ShouldBe<TestValidator>();
		}

		[Fact]
		public void Should_return_null_when_null_is_passed_to_GetValidator() {
			factory.GetValidator(null).ShouldBeNull();
		}

		[Fact]
		public void Should_return_null_when_type_has_no_attribute() {
			factory.GetValidator<NonAttributedPerson>().ShouldBeNull();
		}

		[Fact]
		public void Should_return_null_when_attribute_has_no_type() {
			factory.GetValidator<AttributedPersonWithNoType>().ShouldBeNull();
		}

		[Validator(typeof(TestValidator))]
		private class AttributedPerson {
		}

		private class NonAttributedPerson {
			
		}

		[Validator(null)]
		private class AttributedPersonWithNoType {
			
		}

		private class TestValidator : AbstractValidator<AttributedPerson> {
		}
	}
}