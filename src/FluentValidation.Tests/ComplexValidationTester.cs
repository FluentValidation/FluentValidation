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
// The latest version of this file can be found at http://www.codeplex.com/FluentValidation
#endregion

namespace FluentValidation.Tests {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using NUnit.Framework;

	[TestFixture]
	public class ComplexValidationTester {
		PersonValidator validator;
		Person person;

		[SetUp]
		public void Setup() {
			validator = new PersonValidator();
			person = new Person {
				Address = new Address {
					Country = new Country()
				},
				Orders = new List<Order> {
					new Order() { Amount = 5 },
                    new Order() { ProductName = "Foo" }    	
                }
			};
		}

		[Test]
		public void Validates_complex_property() {
			var results = validator.Validate(person);

			results.Errors.Count.ShouldEqual(3);
			results.Errors[0].PropertyName.ShouldEqual("Forename");
			results.Errors[1].PropertyName.ShouldEqual("Address.Postcode");
			results.Errors[2].PropertyName.ShouldEqual("Address.Country.Name");
		}

		[Test]
		public void Should_override_propertyName() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Address).SetValidator(new AddressValidator())
					.OverridePropertyName("Address2")
			};

			var results = validator.Validate(person);
			results.Errors[0].PropertyName.ShouldEqual("Address2.Postcode");
		}


		[Test]
		public void Complex_validator_should_not_be_invoked_on_null_property() {
			var results = validator.Validate(new Person());
			results.Errors.Count.ShouldEqual(1);
		}

		[Test]
		public void Should_allow_normal_rules_and_complex_property_on_same_property() {
			validator.RuleFor(x => x.Address.Line1).NotNull();
			var result = validator.Validate(person);
			result.Errors.Count.ShouldEqual(4);
		}

		[Test]
		public void Explicitly_included_properties_should_be_propogated_to_nested_validators() {
			var results = validator.Validate(person, x => x.Address);
			results.Errors.Count.ShouldEqual(2);
			results.Errors.First().PropertyName.ShouldEqual("Address.Postcode");
			results.Errors.Last().PropertyName.ShouldEqual("Address.Country.Name");
		}

		[Test]
		public void Explicitly_included_properties_should_be_propogated_to_nested_validators_using_strings() {
			var results = validator.Validate(person, "Address");
			results.Errors.Count.ShouldEqual(2);
			results.Errors.First().PropertyName.ShouldEqual("Address.Postcode");
			results.Errors.Last().PropertyName.ShouldEqual("Address.Country.Name");
		}


		[Test]
		public void Complex_property_should_be_excluded() {
			var results = validator.Validate(person, x => x.Surname);
			results.Errors.Count.ShouldEqual(0);
		}

		[Test]
		public void Should_throw_when_not_a_member_expression() {
			validator.RuleFor(x => PointlessMethod()).SetValidator(new PointlessStringValidator());

			var exception = typeof(InvalidOperationException).ShouldBeThrownBy(() => validator.Validate(person));
		}

		[Test]
		public void Condition_should_work_with_complex_property() {
			var validator = new TestValidator() {
				v => v.RuleFor(x => x.Address).SetValidator(new AddressValidator()).When(x => x.Address.Line1 == "foo")
			};

			var result = validator.Validate(person);
			result.IsValid.ShouldBeTrue();
		}

		[Test]
		public void Can_validate_using_validator_for_base_type() {
			var addressValidator = new InlineValidator<IAddress>() {
				v => v.RuleFor(x => x.Line1).NotNull()
			};

			var validator = new TestValidator {
				v => v.RuleFor(x => x.Address).SetValidator(addressValidator)	
			};

			var result = validator.Validate(new Person { Address = new Address() });
			result.IsValid.ShouldBeFalse();
		}

		private static string PointlessMethod() { return null; }

		public class PersonValidator : AbstractValidator<Person> {
			public PersonValidator() {
				RuleFor(x => x.Forename).NotNull();
				RuleFor(x => x.Address).SetValidator(new AddressValidator());
			}
		}

		public class AddressValidator : AbstractValidator<Address> {
			public AddressValidator() {
				RuleFor(x => x.Postcode).NotNull();
				RuleFor(x => x.Country).SetValidator(new CountryValidator());
			}
		}

		public class CountryValidator : AbstractValidator<Country> {
			public CountryValidator() {
				RuleFor(x => x.Name).NotNull();
			}
		}

		public class PointlessStringValidator : AbstractValidator<string> {

		}
	}
}