using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Extensions;
using Xunit;

namespace FluentValidation.Tests
{
	public class PhoneValidatorTests
	{
		TestValidator validator;

		public PhoneValidatorTests()
		{
			CultureScope.SetDefaultCulture();

			validator = new TestValidator {
				v => v.RuleFor(x => x.PhoneNumber).Phone()
			};
		}

		[Fact]
		public void When_the_text_is_empty_then_the_validator_should_fail()
		{
			string phone = String.Empty;
			var result = validator.Validate(new Person { PhoneNumber = phone });
			result.IsValid.ShouldBeFalse();
		}

		[Theory]
		[InlineData((string)null)]
		[InlineData("920-555-1234")]
		[InlineData("(999)555-7777")]
		public void Valid_phone_numbers(string phone)
		{
			var result = validator.Validate(new Person { PhoneNumber = phone });
			result.IsValid.ShouldBeTrue(string.Format("The phone number {0} should be valid", phone));
		}

	}
}
