namespace FluentValidation.Tests {
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
			var rule = new PropertyRule(null, x => null, null, () => ValidatorOptions.CascadeMode, typeof(string), typeof(Person)) {
				PropertyName = "Surname"
			};
			var context = new PropertyValidatorContext(parentContext, rule, "Surname");
			validator.Validate(context).Single().ShouldNotBeNull();
		}

		//TODO: Test the other standalone validators.
	}
}