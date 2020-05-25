namespace FluentValidation.Tests {
	using System.Linq;
	using Xunit;
    using System;
	using FluentAssertions;
	using TestHelper;

	public class NameResolutionPluggabilityTester : IDisposable {
		[Fact]
		public void Uses_custom_property_name() {
			ValidatorOptions.PropertyNameResolver = (type, prop, expr) => "foo";

			var validator = new TestValidator() {
				v => v.RuleFor(x => x.Surname).NotNull()
			};

			var error = validator.Validate(new Person()).Errors.Single();
			error.PropertyName.Should().Be("foo");
		}

		[Fact]
		public void Resolves_nested_properties() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Address.Country).NotNull()
			};

			var error = validator.Validate(new Person { Address = new Address() }).Errors.Single();
			error.PropertyName.Should().Be("Address.Country");

		}
		
		[Fact]
		public void ShouldHaveValidationError_Should_support_custom_propertynameresolver() {
			try {
				ValidatorOptions.PropertyNameResolver = (type, prop, expr) => "foo";
				var validator = new TestValidator() {
					v => v.RuleFor(x => x.Surname).NotNull()
				};
				validator.ShouldHaveValidationErrorFor(x => x.Surname, (string) null);
			}
			finally {
				ValidatorOptions.PropertyNameResolver = null;
			}
		}

		public void Dispose() {
			ValidatorOptions.PropertyNameResolver = null;
		}
	}
}
