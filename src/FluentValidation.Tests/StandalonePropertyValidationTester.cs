namespace FluentValidation.Tests {
	using System;
	using System.Linq;
	using Internal;
	using NUnit.Framework;
	using Validators;

	[TestFixture]
	public class StandalonePropertyValidationTester {
		[Test]
		public void Should_validate_property_value_without_instance() {
			var validator = new NotNullValidator();
			var parentContext = new ValidationContext(null);
			var rule = new PropertyRule(null, x => null, null, null, typeof(string), null) {
				PropertyName = "Surname"
			};
			var context = new PropertyValidatorContext(parentContext, rule, null);
			var result = validator.Validate(context);
			result.Single().ShouldNotBeNull();
		}

		[Test]
		public void Should_validate_property_value_without_instance_differnt_type() {
			var validator = new LessThanValidator(10M);
			var parentContext = new ValidationContext(null);
			var rule = new PropertyRule(null, x => 15, null, null, typeof(decimal), null) {
				PropertyName = "Number"
			};
			var context = new PropertyValidatorContext(parentContext, rule, null);
			var result = validator.Validate(context);
			result.Single().ShouldNotBeNull();
		}

		//TODO: Test the other standalone validators.

        [Test]
        public void Should_validate_property_value_without_instance_but_validator_using_custom_value()
        {
            var validator = new InlineValidator<Person> {
                v => v.RuleFor(x => x.Email).EmailAddress(),
                v => v.RuleFor(x => x.Discount).LessThan(50).GreaterThan(25).NotEqual(50)
            };

            validator.ValidateMember("Email", "matthew@jam.com").IsValid.ShouldBeTrue();
            validator.ValidateMember("Email", "my invalid email").IsValid.ShouldBeFalse();

            validator.ValidateMember("Discount", 36).IsValid.ShouldBeTrue();
            validator.ValidateMember("Discount", 50).Errors.Count.ShouldEqual(2);
            validator.ValidateMember("Discount", 25).Errors.Count.ShouldEqual(1);
        }
	}
}