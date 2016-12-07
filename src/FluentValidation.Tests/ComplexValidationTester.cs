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
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Xunit;

	
	public class ComplexValidationTester {
		PersonValidator validator;
		Person person;

		public ComplexValidationTester() {
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

		[Fact]
		public void Validates_complex_property() {
			var results = validator.Validate(person);

			results.Errors.Count.ShouldEqual(3);
			results.Errors[0].PropertyName.ShouldEqual("Forename");
			results.Errors[1].PropertyName.ShouldEqual("Address.Postcode");
			results.Errors[2].PropertyName.ShouldEqual("Address.Country.Name");
		}

		[Fact]
		public void Should_override_propertyName() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Address).SetValidator(new AddressValidator())
					.OverridePropertyName("Address2")
			};

			var results = validator.Validate(person);
			results.Errors[0].PropertyName.ShouldEqual("Address2.Postcode");
		}


		[Fact]
		public void Complex_validator_should_not_be_invoked_on_null_property() {
			var results = validator.Validate(new Person());
			results.Errors.Count.ShouldEqual(1);
		}

		[Fact]
		public void Should_allow_normal_rules_and_complex_property_on_same_property() {
			validator.RuleFor(x => x.Address.Line1).NotNull();
			var result = validator.Validate(person);
			result.Errors.Count.ShouldEqual(4);
		}

		[Fact]
		public void Explicitly_included_properties_should_be_propogated_to_nested_validators() {
			var results = validator.Validate(person, x => x.Address);
			results.Errors.Count.ShouldEqual(2);
			results.Errors.First().PropertyName.ShouldEqual("Address.Postcode");
			results.Errors.Last().PropertyName.ShouldEqual("Address.Country.Name");
		}

		[Fact]
		public void Explicitly_included_properties_should_be_propogated_to_nested_validators_using_strings() {
			var results = validator.Validate(person, "Address");
			results.Errors.Count.ShouldEqual(2);
			results.Errors.First().PropertyName.ShouldEqual("Address.Postcode");
			results.Errors.Last().PropertyName.ShouldEqual("Address.Country.Name");
		}


		[Fact]
		public void Complex_property_should_be_excluded() {
			var results = validator.Validate(person, x => x.Surname);
			results.Errors.Count.ShouldEqual(0);
		}

		[Fact]
		public void Condition_should_work_with_complex_property() {
			var validator = new TestValidator() {
				v => v.RuleFor(x => x.Address).SetValidator(new AddressValidator()).When(x => x.Address.Line1 == "foo")
			};

			var result = validator.Validate(person);
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Async_condition_should_work_with_complex_property() {
			var validator = new TestValidator() {
				v => v.RuleFor(x => x.Address).SetValidator(new AddressValidator()).WhenAsync(async x => x.Address.Line1 == "foo")
			};

			var result = validator.ValidateAsync(person).Result;
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
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

        [Fact]
        public void Can_directly_validate_multiple_fields_of_same_type()
        {
            var sut = new TestObjectValidator();
            var testObject = new TestObject {
                Foo2 = new TestDetailObject() { Surname = "Bar" }
            };
            
            //Should not throw
            sut.Validate(testObject);
        }

        public class TestObject
        {
            public TestDetailObject Foo1 { get; set; }
            public TestDetailObject Foo2 { get; set; }
        }

	    public class TestDetailObject {
	        public string Surname { get; set; }
	    }

        public class TestObjectValidator : AbstractValidator<TestObject>
        {
            public TestObjectValidator()
            {
                RuleFor(x => x.Foo1.Surname).NotEmpty().When(x => x.Foo1 != null);
                RuleFor(x => x.Foo2.Surname).NotEmpty();
            }
        }

        //[Fact]
        //public void Should_not_infinite_loop() {
        //	var val = new InfiniteLoopValidator();
        //	var target = new InfiniteLoop();
        //	target.Property = new InfiniteLoop2 {Property = target};
        //	val.Validate(target);
        //}

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

		public class InfiniteLoop {
			public InfiniteLoop2 Property { get; set; }
		}

		public class InfiniteLoop2 {
			public InfiniteLoop Property { get; set; }
		}

		public class InfiniteLoopValidator : AbstractValidator<InfiniteLoop> {
			public InfiniteLoopValidator() {
				RuleFor(x => x.Property).SetValidator(new InfiniteLoop2Validator());
			} 
		}

		public class InfiniteLoop2Validator : AbstractValidator<InfiniteLoop2> {
			public InfiniteLoop2Validator() {
				RuleFor(x => x.Property).SetValidator(new InfiniteLoopValidator());
			}
		}
	}
}