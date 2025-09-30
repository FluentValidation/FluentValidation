namespace FluentValidation.Tests;

using System;
using System.Linq;
using Xunit;

public class GuidValidatorTests {
	TestValidator validator;

	public GuidValidatorTests() {
		CultureScope.SetDefaultCulture();

		validator = new TestValidator {
			v => v.RuleFor(x => x.ExternalId).IsValidGuid()
		};
	}

	[Fact]
	public void IsValidTests() {
		validator.Validate(new Person { ExternalId = null }).IsValid.ShouldBeTrue(); // Optional values are always valid
		validator.Validate(new Person { ExternalId = "00000000-0000-0000-0000-000000000000" }).IsValid.ShouldBeTrue(); // Valid GUID
		validator.Validate(new Person { ExternalId = "12345678-1234-1234-1234-123456789abc" }).IsValid.ShouldBeTrue(); // Valid GUID
		validator.Validate(new Person { ExternalId = "12345678123412341234123456789abc" }).IsValid.ShouldBeTrue(); // Valid GUID without hyphens
		validator.Validate(new Person { ExternalId = "{12345678-1234-1234-1234-123456789abc}" }).IsValid.ShouldBeTrue(); // Valid GUID with braces
		validator.Validate(new Person { ExternalId = "(12345678-1234-1234-1234-123456789abc)" }).IsValid.ShouldBeTrue(); // Valid GUID with parentheses
		validator.Validate(new Person { ExternalId = "not-a-guid" }).IsValid.ShouldBeFalse(); // Invalid GUID
		validator.Validate(new Person { ExternalId = "12345678-1234-1234-1234-123456789abg" }).IsValid.ShouldBeFalse(); // Invalid character 'g'
		validator.Validate(new Person { ExternalId = "12345678-1234-1234-1234-123456789ab" }).IsValid.ShouldBeFalse(); // Too short
		validator.Validate(new Person { ExternalId = "12345678-1234-1234-1234-123456789abcd" }).IsValid.ShouldBeFalse(); // Too long
		validator.Validate(new Person { ExternalId = "" }).IsValid.ShouldBeFalse(); // Empty string
	}

	[Fact]
	public void When_validation_fails_the_default_error_should_be_set() {
		string invalidGuid = "not-a-guid";
		var result = validator.Validate(new Person { ExternalId = invalidGuid });
		result.Errors.Single().ErrorMessage.ShouldEqual("'Surname' is not a valid GUID.");
	}

	[Fact]
	public void Should_work_with_guid_string_conversion() {
		var guidValidator = new TestValidator {
			v => v.RuleFor(x => x.ExternalId).IsValidGuid()
		};

		var validGuid = Guid.NewGuid().ToString();
		guidValidator.Validate(new Person { ExternalId = validGuid }).IsValid.ShouldBeTrue();

		var invalidGuid = "invalid-guid-string";
		guidValidator.Validate(new Person { ExternalId = invalidGuid }).IsValid.ShouldBeFalse();
	}
}
