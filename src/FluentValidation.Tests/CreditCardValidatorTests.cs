namespace FluentValidation.Tests {
	using System.Globalization;
	using System.Linq;
	using System.Threading;
	using NUnit.Framework;

	[TestFixture]
	public class CreditCardValidatorTests {
		TestValidator validator;

		[SetUp]
		public void Setup() {
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");

			validator = new TestValidator {
				v => v.RuleFor(x => x.CreditCard).CreditCard()
			};
		}

		[Test] // copied these tests from the mvc3 unit tests.
		public void IsValidTests() {
			validator.Validate(new Person { CreditCard = null }).IsValid.ShouldBeTrue(); // Optional values are always valid
			validator.Validate(new Person { CreditCard = "0000000000000000" }).IsValid.ShouldBeTrue(); // Simplest valid value
			validator.Validate(new Person { CreditCard = "1234567890123452" }).IsValid.ShouldBeTrue(); // Good checksum
			validator.Validate(new Person { CreditCard = "1234-5678-9012-3452" }).IsValid.ShouldBeTrue(); // Good checksum, with dashes
			validator.Validate(new Person { CreditCard = "0000000000000001" }).IsValid.ShouldBeFalse(); // Bad checksum
		}

		[Test]
		public void When_validation_fails_the_default_error_should_be_set() {
			string creditcard = "foo";
			var result = validator.Validate(new Person { CreditCard = creditcard });
			result.Errors.Single().ErrorMessage.ShouldEqual("'Credit Card' is not a valid credit card number.");
		}

	}
}