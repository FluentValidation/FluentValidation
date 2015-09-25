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
	using Xunit;

	
	public class ChainedValidationTester {
		PersonValidator validator;
		Person person;

		public ChainedValidationTester() {
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
		public void Validates_chained_property() {
			var results = validator.Validate(person);

			results.Errors.Count.ShouldEqual(3);
			results.Errors[0].PropertyName.ShouldEqual("Forename");
			results.Errors[1].PropertyName.ShouldEqual("Address.Postcode");
			results.Errors[2].PropertyName.ShouldEqual("Address.Country.Name");
		}

		[Fact]
		public void Chained_validator_should_not_be_invoked_on_null_property() {
			var results = validator.Validate(new Person());
			results.Errors.Count.ShouldEqual(1);
		}

		[Fact]
		public void Should_allow_normal_rules_and_chained_property_on_same_property() {
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
		public void Chained_property_should_be_excluded() {
			var results = validator.Validate(person, x => x.Surname);
			results.Errors.Count.ShouldEqual(0);
		}

		[Fact]
		public void Condition_should_work_with_chained_property() {
			var person = new Person {
				Address = new Address {
					Line2 = "foo"
				}
			};
			var result = validator.Validate(person);
			result.Errors.Count.ShouldEqual(3);
			result.Errors.Last().PropertyName.ShouldEqual("Address.Line1");
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
		public void Separate_validation_on_chained_property() {
			var validator = new DepartmentValidator();
			var result = validator.Validate(new Department
			{
				Manager = new Person(),
				Assistant = new Person()
			});
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Separate_validation_on_chained_property_valid() {
			var validator = new DepartmentValidator();
			var result = validator.Validate(new Department {
				Manager = new Person {
					Surname = "foo"
				}

			});
			result.Errors.IsValid().ShouldBeTrue();
		}

		[Fact]
		public void Separate_validation_on_chained_property_conditional() {
			var validator = new DepartmentValidator();
			var result = validator.Validate(new Department {
				Manager = new Person {
					Surname = "foo"
				},
				Assistant = new Person {
					Surname = "foo"
				}
			});
			result.Errors.Count.ShouldEqual(1);
			result.Errors.First().PropertyName.ShouldEqual("Assistant.Surname");
		}

		[Fact]
		public void Chained_validator_descriptor() {
			var descriptor = validator.CreateDescriptor();

			var members = descriptor.GetMembersWithValidators().ToList();
			members.Count.ShouldEqual(4);
			members[0].Key.ShouldEqual("Forename");
			members[1].Key.ShouldEqual("Address.Postcode");
			members[2].Key.ShouldEqual("Address.Country.Name");
			members[3].Key.ShouldEqual("Address.Line1");
		}

		public class DepartmentValidator : AbstractValidator<Department> {
			public DepartmentValidator() {
				CascadeMode = CascadeMode.StopOnFirstFailure; ;
				RuleFor(x => x.Manager).NotNull();
				RuleFor(x => x.Assistant.Surname).NotEqual(x => x.Manager.Surname).When(x => x.Assistant != null && x.Manager.Surname != null);
			}
		}

		public class PersonValidator : AbstractValidator<Person> {
			public PersonValidator() {
				RuleFor(x => x.Forename).NotNull();
				When(x => x.Address != null, () => {
					RuleFor(x => x.Address.Postcode).NotNull();
					RuleFor(x => x.Address.Country.Name).NotNull().When(x => x.Address.Country != null);
					RuleFor(x => x.Address.Line1).NotNull().When(x => x.Address.Line2 != null);
				});
			}
		}

		public class Department {
			public Person Manager { get; set; }
			public Person Assistant { get; set; }
			public IList<Person> Employees { get; set; }
		}
	
	}
}