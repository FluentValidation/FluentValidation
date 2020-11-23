namespace FluentValidation.Tests {
	using System;
	using System.Linq;
	using Internal;
	using Xunit;
	using Validators;


	public class StandalonePropertyValidationTester {
		[Fact]
		public void Should_validate_property_value_without_instance() {
			var validator = new NotNullValidator();
			var parentContext = new ValidationContext<string>(null);
			var rule = new PropertyRule<Person>(null, x => null, null, null, typeof(string)) {
				PropertyName = "Surname"
			};
			var context = new PropertyValidatorContext(parentContext, rule, null, null);
			var result = validator.Validate(context);
			result.Single().ShouldNotBeNull();
		}
	}
}
