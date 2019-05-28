#region License
// Copyright (c) Jeremy Skinner (http://www.jeremyskinner.co.uk)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// The latest version of this file can be found at https://github.com/jeremyskinner/FluentValidation
#endregion

namespace FluentValidation.Tests {
	using System.Linq;
	using Xunit;
	using Validators;

	public class EmailValidatorTests {

		public EmailValidatorTests() {
			CultureScope.SetDefaultCulture();
		}
		[Theory]
		[InlineData("")]
		[InlineData("testperso")]
		[InlineData("first.last@test..co.uk")]
		[InlineData("thisisaverylongstringcodeplex.com")]
		public void Invalid_email_addressex_regex(string email) {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Email).EmailAddress(EmailValidationMode.Net4xRegex)
			};
			var result = validator.Validate(new Person {Email = email});
			result.IsValid.ShouldBeFalse();
			result.Errors.Single().ErrorMessage.ShouldEqual("'Email' is not a valid email address.");
		}

		[Theory]
		[InlineData((string)null)]
		[InlineData("testperson@gmail.com")]
		[InlineData("TestPerson@gmail.com")]
		[InlineData("testperson+label@gmail.com")]
		[InlineData("\"Abc\\@def\"@example.com")]
		[InlineData("\"Fred Bloggs\"@example.com")]
		[InlineData("\"Joe\\Blow\"@example.com")]
		[InlineData("\"Abc@def\"@example.com")]
		[InlineData("customer/department=shipping@example.com")]
		[InlineData("$A12345@example.com")]
		[InlineData("!def!xyz%abc@example.com")]
		[InlineData("__somename@example.com")]
		[InlineData("first.last@test.co.uk")]
		public void Valid_email_addresses_regex(string email) {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Email).EmailAddress(EmailValidationMode.Net4xRegex)
			};
			var result = validator.Validate(new Person {Email = email});
			result.IsValid.ShouldBeTrue(string.Format("The email address {0} should be valid", email));
		}

		[Theory]
		[InlineData((string)null)]
		[InlineData("1234@someDomain.com")]
		[InlineData("firstName.lastName@someDomain.com")]
		[InlineData("\u00A0@someDomain.com")]
		[InlineData("!#$%&'*+-/=?^_`|~@someDomain.com")]
		[InlineData("\"firstName.lastName\"@someDomain.com")]
		[InlineData("someName@someDomain.com")]
		[InlineData("someName@some~domain.com")]
		[InlineData("someName@some_domain.com")]
		[InlineData("someName@1234.com")]
		[InlineData("someName@someDomain\uFFEF.com")]
		public void Valid_email_addresses_aspnetcore_compatible(string email) {
			var validator = new InlineValidator<Person>();
			validator.RuleFor(x => x.Email).EmailAddress(EmailValidationMode.AspNetCoreCompatible);
			validator.Validate(new Person { Email = email}).IsValid.ShouldBeTrue();
		}

		[Theory]
		[InlineData(0)]
		[InlineData("")]
		[InlineData(" \r \t \n" )]
		[InlineData("@someDomain.com")]
		[InlineData("@someDomain@abc.com")]
		[InlineData("someName")]
		[InlineData("someName@")]
		[InlineData("someName@a@b.com")]
		public void Fails_email_validation_aspnetcore_compatible(string email) {
			var validator = new InlineValidator<Person>();
			validator.RuleFor(x => x.Email).EmailAddress(EmailValidationMode.AspNetCoreCompatible);
			validator.Validate(new Person { Email = email}).IsValid.ShouldBeFalse();
		}
	}
}
