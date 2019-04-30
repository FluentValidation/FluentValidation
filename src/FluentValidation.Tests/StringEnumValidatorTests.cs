namespace FluentValidation.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using Xunit;

	public class StringEnumValidatorTests
	{
		private readonly TestValidator _validator;

		private readonly TestValidator _caseSensitiveValidator;

		public StringEnumValidatorTests()
		{
			CultureScope.SetDefaultCulture();

			_validator = new TestValidator
			{
				v => v.RuleFor(x => x.GenderString).IsEnumName(typeof(EnumGender))
			};

			_caseSensitiveValidator = new TestValidator
			{
				v => v.RuleFor(x => x.GenderString).IsEnumName(typeof(EnumGender), true)
			};
		}

		[Fact]
		public void IsValidTests_CaseInsensitive_CaseCorrect()
		{
			_validator.Validate(new Person { GenderString = "Female" }).IsValid.ShouldBeTrue();
			_validator.Validate(new Person { GenderString = "Male" }).IsValid.ShouldBeTrue();
		}

		[Fact]
		public void IsValidTests_CaseInsensitive_CaseIncorrect()
		{
			_validator.Validate(new Person { GenderString = "femAlE" }).IsValid.ShouldBeTrue();
			_validator.Validate(new Person { GenderString = "maLe" }).IsValid.ShouldBeTrue();
		}

		[Fact]
		public void IsValidTests_CaseSensitive_CaseCorrect()
		{
			_caseSensitiveValidator.Validate(new Person { GenderString = "Female" }).IsValid.ShouldBeTrue();
			_caseSensitiveValidator.Validate(new Person { GenderString = "Male" }).IsValid.ShouldBeTrue();
		}

		[Fact]
		public void IsValidTests_CaseSensitive_CaseIncorrect()
		{
			_caseSensitiveValidator.Validate(new Person { GenderString = "femAlE" }).IsValid.ShouldBeFalse();
			_caseSensitiveValidator.Validate(new Person { GenderString = "maLe" }).IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_the_property_is_initialized_with_invalid_string_then_the_validator_should_fail()
		{
			_validator.Validate(new Person { GenderString = "other" }).IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_the_property_is_initialized_with_empty_string_then_the_validator_should_fail()
		{
			_validator.Validate(new Person { GenderString = string.Empty }).IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_the_property_is_initialized_with_null_then_the_validator_should_be_valid()
		{
			_validator.Validate(new Person { GenderString = null }).IsValid.ShouldBeTrue();
		}

		[Fact]
		public void When_validation_fails_the_default_error_should_be_set()
		{
			var result = _validator.Validate(new Person { GenderString = "invalid" });
			result.Errors.Single().ErrorMessage.ShouldEqual("'Gender String' has names which do not include 'invalid'.");
		}
	}
}
