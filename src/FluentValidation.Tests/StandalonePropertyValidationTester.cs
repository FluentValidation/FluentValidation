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
		public void Should_validate_property_value_without_instance_different_types() {
			var validator = new EqualValidator(100M); // decimal
			var parentContext = new ValidationContext(null);
			var rule = new PropertyRule(null, x => 100D /* double */, null, null, typeof(string), null) {
				PropertyName = "Surname"
			};
			var context = new PropertyValidatorContext(parentContext, rule, null);
			var result = validator.Validate(context); // would fail saying that decimal is not double
			result.Count().ShouldEqual(0);
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

			validator.ValidateMember("Discount", 36M).IsValid.ShouldBeTrue();
			validator.ValidateMember("Discount", 50M).Errors.Count.ShouldEqual(2);
			validator.ValidateMember("Discount", 25M).Errors.Count.ShouldEqual(1);
		}

		[Test]
		public void Should_validate_property_value_with_instance_but_validator_using_custom_value()
		{
			var validator = new InlineValidator<Person> {
					v => v.RuleFor(x => x.Surname)
						  .Must(x => !string.IsNullOrWhiteSpace(x))
						  .When(x => x.Age >= 18)
						  .WithMessage("A person needs to have a surname if he is older than 18")
			};

			var person = new Person { Age = 19 };

			try {
				validator.ValidateMember(x => x.Surname, string.Empty);
				Assert.Fail();
			} catch (NullReferenceException) {
				// an exception is thrown because the validation needs an instance.
			}

			validator.ValidateMember(person, x => x.Surname, string.Empty).Errors.Count.ShouldEqual(1);

			// ignores the instance value
			person.Surname = "Leibowitz";
			validator.ValidateMember(person, x => x.Surname, string.Empty).Errors.Count.ShouldEqual(1);

			validator.ValidateMember(person, x => x.Surname, "Leibowitz").Errors.Count.ShouldEqual(0);
		}
	}
}