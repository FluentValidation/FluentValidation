namespace FluentValidation.Tests {
	using System.Linq;
	using NUnit.Framework;

	[TestFixture]
	public class NameResolutionPluggabilityTester {
		[Test]
		public void Uses_custom_property_name() {
			ValidatorOptions.PropertyNameResolver = (type, prop, expr) => "foo";

			var validator = new TestValidator() {
				v => v.RuleFor(x => x.Surname).NotNull()
			};

			var error = validator.Validate(new Person()).Errors.Single();
			error.PropertyName.ShouldEqual("foo");
		}

		[Test]
		public void Resolves_nested_properties() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Address.Country).NotNull()
			};

			var error = validator.Validate(new Person { Address = new Address() }).Errors.Single();
			error.PropertyName.ShouldEqual("Address.Country");

		}

		[TearDown]
		public void Teardown() {
			ValidatorOptions.PropertyNameResolver = null;
		}
	}
}