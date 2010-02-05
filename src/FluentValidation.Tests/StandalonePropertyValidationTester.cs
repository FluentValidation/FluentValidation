namespace FluentValidation.Tests {
	using System.Linq;
	using NUnit.Framework;
	using Validators;

	[TestFixture]
	public class StandalonePropertyValidationTester {
		[Test]
		public void Should_validate_property_value_without_instance() {
			var validator = new NotNullValidator();
			var context = new PropertyValidatorContext("Surname", null, null as object, null);
			validator.Validate(context).Single().ShouldNotBeNull();
		}
	}
}