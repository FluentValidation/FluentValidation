namespace FluentValidation.Tests;

using System.Linq;
using Xunit;
using System;
using TestHelper;

public class NameResolutionPluggabilityTester : IDisposable {
	[Fact]
	public void Uses_custom_property_name() {
		ValidatorOptions.Global.PropertyNameResolver = (type, prop, expr) => "foo";

		var validator = new TestValidator() {
			v => v.RuleFor(x => x.Surname).NotNull()
		};

		var error = validator.Validate(new Person()).Errors.Single();
		error.PropertyName.ShouldEqual("foo");
	}

	[Fact]
	public void Resolves_nested_properties() {
		var validator = new TestValidator {
			v => v.RuleFor(x => x.Address.Country).NotNull()
		};

		var error = validator.Validate(new Person { Address = new Address() }).Errors.Single();
		error.PropertyName.ShouldEqual("Address.Country");
	}

	[Fact]
	public void ShouldHaveValidationError_Should_support_custom_propertynameresolver() {
		try {
			ValidatorOptions.Global.PropertyNameResolver = (type, prop, expr) => "foo";
			var validator = new TestValidator() {
				v => v.RuleFor(x => x.Surname).NotNull()
			};
			validator.TestValidate(new Person()).ShouldHaveValidationErrorFor(x => x.Surname);
		}
		finally {
			ValidatorOptions.Global.PropertyNameResolver = null;
		}
	}

	[Fact]
	public void ShouldHaveValidationError_Should_support_custom_propertynameresolver_with_include_properties() {
		try {
			ValidatorOptions.Global.PropertyNameResolver = (type, prop, expr) => "foo";
			var validator = new TestValidator() {
				v => v.RuleFor(x => x.Surname).NotNull()
			};
			validator.TestValidate(new Person(), strategy => strategy.IncludeProperties(x => x.Surname)).ShouldHaveValidationErrorFor(x => x.Surname);
		}
		finally {
			ValidatorOptions.Global.PropertyNameResolver = null;
		}
	}

	[Fact]
	public void ShouldHaveValidationError_Should_support_custom_propertynameresolver_with_include_properties_and_nested_properties() {
		try {
			ValidatorOptions.Global.PropertyNameResolver = (type, prop, expr) => "foo";
			var validator = new TestValidator() {
				v => v.RuleFor(x => x.Address.Line1).NotNull()
			};
			validator.TestValidate(new Person {
				Address = new Address()
			}, strategy => strategy.IncludeProperties(x => x.Address.Line1)).ShouldHaveValidationErrorFor(x => x.Address.Line1);
		}
		finally {
			ValidatorOptions.Global.PropertyNameResolver = null;
		}
	}

	public void Dispose() {
		ValidatorOptions.Global.PropertyNameResolver = null;
	}
}
