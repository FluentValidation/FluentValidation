#region License
// Copyright 2008-2009 Jeremy Skinner (http://www.jeremyskinner.co.uk)
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
// The latest version of this file can be found at http://www.codeplex.com/FluentValidation
#endregion
namespace FluentValidation.Tests {
	using System.Globalization;
	using System.Linq;
	using System.Threading;
	using Attributes;
	using Internal;
	using NUnit.Framework;
	using Results;
	using Validators;
#pragma warning disable 612,618

	[TestFixture]
	public class BackwardsCompatibilityTester {
		TestValidator validator;

		[SetUp]
		public void Setup() {
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
			validator = new TestValidator();
		}

		[Test]
		public void NotNullValidator_should_pass_if_value_has_value() {
			validator.RuleFor(x => x.Surname).SetValidator(new ObsoleteNotNullValidator<Person, string>());
			var result = validator.Validate(new Person() { Surname = "Jeremy" });
			result.IsValid.ShouldBeTrue();
		}

		[Test]
		public void NotNullValidator_should_fail_if_value_is_null() {
			validator.RuleFor(x => x.Surname).SetValidator(new ObsoleteNotNullValidator<Person, string>());
			var result = validator.Validate(new Person());
			result.IsValid.ShouldBeFalse();
		}

		[Test]
		public void When_the_validator_fails_the_error_message_should_be_set() {
			validator.RuleFor(x => x.Surname).SetValidator(new ObsoleteNotNullValidator<Person, string>());
			
			var result = validator.Validate(new Person())
				.Errors.Single();

			result.ErrorMessage.ShouldEqual("'Surname' must not be empty.");
		}

		[Test]
		public void WithMessage() {
			validator.RuleFor(x => x.Surname).SetValidator(new ObsoleteNotNullValidator<Person, string>())
				.WithMessage("foo");

			var result = validator.Validate(new Person())
			.Errors.Single();

			result.ErrorMessage.ShouldEqual("foo");
			
		}

		[Test]
		public void WithName() {
			validator.RuleFor(x => x.Surname).SetValidator(new ObsoleteNotNullValidator<Person, string>()).WithName("foo");

			var result = validator.Validate(new Person())
				.Errors.Single();

			result.ErrorMessage.ShouldEqual("'foo' must not be empty.");
		}

		[Test]
		public void Custom_formatting() {
			validator.RuleFor(x => x.Surname).SetValidator(new ObsoleteNotNullValidator<Person, string>())
				.WithMessage("foo {0} {1}", x => x.Id, x => x.Id);

			var result = validator.Validate(new Person())
			.Errors.Single();

			result.ErrorMessage.ShouldEqual("foo 0 0");
		}

		[Test]
		public void Not_null_validator_should_work_ok_with_non_nullable_value_type() {
			validator.RuleFor(x => x.Id).SetValidator(new ObsoleteNotNullValidator<Person, int>());
			var result = validator.Validate(new Person { Id = 3 });
			result.IsValid.ShouldBeTrue();
		}

		[ValidationMessage(Key = "notnull_error")]
		public class ObsoleteNotNullValidator<T, TProperty> : IPropertyValidator<T, TProperty> {
			public PropertyValidatorResult Validate(PropertyValidatorContext<T, TProperty> context) {
				if (context.PropertyValue == null) {
					var formatter = new MessageFormatter().AppendProperyName(context.PropertyDescription);
					string error = context.GetFormattedErrorMessage(typeof(ObsoleteNotNullValidator<T, TProperty>), formatter);

					return PropertyValidatorResult.Failure(error);
				}

				return PropertyValidatorResult.Success();
			}
		}
	}
#pragma warning restore 612,618

}
