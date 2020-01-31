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
			var parentContext = new ValidationContext(null);
			var rule = new PropertyRule(null, x => null, null, null, typeof(string), null) {
				PropertyName = "Surname"
			};
			var context = new PropertyValidatorContext(parentContext, rule, null);
			var result = validator.Validate(context);
			result.Single().ShouldNotBeNull();
		}
	}
}
