namespace FluentValidation.Tests {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Internal;
	using Results;
	using Xunit;
	using Validators;


	public class StandalonePropertyValidationTester {
		[Fact]
		public void Should_validate_property_value_without_instance() {
			var validator = new NotNullValidator();
			var parentContext = new ValidationContext<string>(null) {
				Failures = new List<ValidationFailure>(),
			};
			var rule = new PropertyRule<Person>(null, x => null, null, null, typeof(string)) {
				PropertyName = "Surname"
			};
			var context = new PropertyValidatorContext(parentContext, parentContext.Failures, rule, null, null);
			validator.Validate(context);
			parentContext.Failures.Single().ShouldNotBeNull();
		}
	}
}
