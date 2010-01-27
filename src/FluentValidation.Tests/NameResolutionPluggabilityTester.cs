namespace FluentValidation.Tests {
	using System.Linq;
	using NUnit.Framework;

	[TestFixture]
	public class NameResolutionPluggabilityTester {
		[Test]
		public void Uses_custom_property_name() {
			ValidatorOptions.PropertyNameResolver = (type, prop) => "foo";

			var validator = new TestValidator() {
				v => v.RuleFor(x => x.Surname).NotNull()
			};

			var error = validator.Validate(new Person()).Errors.Single();
			error.PropertyName.ShouldEqual("foo");
		}

		[TearDown]
		public void Teardown() {
			ValidatorOptions.PropertyNameResolver = null;
		}
	}
}