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

		//TODO: Test the other standalone validators.
	}
}