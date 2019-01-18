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
// The latest version of this file can be found at https://github.com/JeremySkinner/FluentValidation
#endregion

namespace FluentValidation.Tests {
	using System;
	using System.Linq;
	using Internal;
	using Results;
	using Validators;
	using Xunit;

	
	public class RulesetTests {


		[Fact]
		public void Executes_rules_in_specified_ruleset() {
			var validator = new TestValidator();
			var result = validator.Validate(new ValidationContext<Person>(new Person(), new PropertyChain(), new RulesetValidatorSelector("Names")));

			result.Errors.Count.ShouldEqual(2); // 2 rules in this ruleset
			AssertExecuted(result, "Names");
		}

		[Fact]
		public void Executes_rules_not_specified_in_ruleset() {
			var validator = new TestValidator();
			var result = validator.Validate(new Person());

			result.Errors.Count.ShouldEqual(1); // 1 rule not inside a ruleset
			AssertExecuted(result, "default");
		}

		[Fact]
		public void Ruleset_cascades_to_child_validator() {
			var addressValidator = new InlineValidator<Address>();
			addressValidator.RuleSet("Test", () => {
				addressValidator.RuleFor(x => x.Line1).NotNull();
			});

			var validator = new TestValidator();

			validator.RuleSet("Test", () => {
				validator.RuleFor(x => x.Address).SetValidator(addressValidator);			
			});

			var person = new Person {
			    Address = new Address()
			};

			var result = validator.Validate(new ValidationContext<Person>(person, new PropertyChain(), new RulesetValidatorSelector("Test")));

			result.Errors.Count.ShouldEqual(1);
			AssertExecuted(result, "Test");
		}

		[Fact]
		public void Ruleset_cascades_to_child_collection_validator() {
			var orderValidator = new InlineValidator<Order>();
			orderValidator.RuleSet("Test", () => {
				orderValidator.RuleFor(x => x.ProductName).NotNull();
			});

			var validator = new TestValidator();
			
			validator.RuleSet("Test", () => {
				validator.RuleForEach(x => x.Orders).SetValidator(orderValidator);
			});

			var person = new Person {
				Orders = { new Order(), new Order() }
			};

			var result = validator.Validate(new ValidationContext<Person>(person, new PropertyChain(), new RulesetValidatorSelector("Test")));


			result.Errors.Count.ShouldEqual(2); //one for each order
			AssertExecuted(result, "Test");
		}

		[Fact]
		public void Executes_multiple_rulesets() {
			var validator = new TestValidator();
			validator.RuleSet("Id", () => {
				validator.RuleFor(x => x.Id).NotEqual(0);
			});

			var person = new Person();
			var result = validator.Validate(new ValidationContext<Person>(person, new PropertyChain(), new RulesetValidatorSelector("Names", "Id")));

			result.Errors.Count.ShouldEqual(3);
			AssertExecuted(result, "Names", "Id");
		}

		[Fact]
		public void Executes_all_rules() {
			var validator = new TestValidator();
			var person = new Person();
			var result = validator.Validate(person, ruleSet: "*");
			result.Errors.Count.ShouldEqual(3);
			AssertExecuted(result, "Names", "default");
		}

		[Fact]
		public void Executes_rules_in_default_ruleset_and_specific_ruleset() {
			var validator = new TestValidator();
			validator.RuleSet("foo", () => {
				validator.RuleFor(x => x.Age).NotEqual(0);
			});

			var result = validator.Validate(new Person(), ruleSet : "default,Names");
			result.Errors.Count.ShouldEqual(3);
			AssertExecuted(result, "default", "Names");

		}

		[Fact]
		public void WithMessage_works_inside_rulesets() {
			var validator = new TestValidator2();
			var result = validator.Validate(new Person(), ruleSet: "Names");
			Assert.Equal("foo", result.Errors[0].ErrorMessage);
			AssertExecuted(result, "Names");
		}

		[Fact]
		public void Ruleset_selection_should_not_cascade_downwards_when_set_on_property() {
			var validator = new TestValidator4();
			var result = validator.Validate(new PersonContainer() { Person = new Person() }, ruleSet: "Names");
			result.IsValid.ShouldBeTrue();
			AssertExecuted(result);
		}

		[Fact]
		public void Ruleset_selection_should_cascade_downwards_with_when_setting_child_validator_using_include_statement() {
			var validator = new TestValidator3();
			var result = validator.Validate(new Person(), ruleSet:"Names");
			result.IsValid.ShouldBeFalse();
			AssertExecuted(result, "Names");
		}

		[Fact]
		public void Ruleset_selection_should_cascade_downwards_with_when_setting_child_validator_using_include_statement_with_lambda() {
			var validator = new InlineValidator<Person>();
			validator.Include(x => new TestValidator2());
			var result = validator.Validate(new Person(), ruleSet:"Names");
			result.IsValid.ShouldBeFalse();
		}

		
		[Fact]
		public void Trims_spaces() {
			var validator = new InlineValidator<Person>();
			validator.RuleSet("First", () => {
				validator.RuleFor(x => x.Forename).NotNull();
			});
			validator.RuleSet("Second", () => {
				validator.RuleFor(x => x.Surname).NotNull();
			});

			var result = validator.Validate(new Person(), ruleSet: "First, Second");
			result.Errors.Count.ShouldEqual(2);
			AssertExecuted(result, "First", "Second");
		}

		[Fact]
		public void Applies_multiple_rulesets_to_rule() {
			var validator = new InlineValidator<Person>();
			validator.RuleSet("First, Second", () => {
				validator.RuleFor(x => x.Forename).NotNull();
			});

			var result = validator.Validate(new Person(), ruleSet: "First");
			result.Errors.Count.ShouldEqual(1);
			AssertExecuted(result, "First");

			result = validator.Validate(new Person(), ruleSet: "Second");
			result.Errors.Count.ShouldEqual(1);
			AssertExecuted(result, "Second");

			result = validator.Validate(new Person(), ruleSet: "Third");
			result.Errors.Count.ShouldEqual(0);
			AssertExecuted(result);
			
			result = validator.Validate(new Person());
			result.Errors.Count.ShouldEqual(0);
			AssertExecuted(result, "default");
		}

		[Fact]
		public void Executes_in_rule_in_ruleset_and_default() {
			var validator = new InlineValidator<Person>();
			validator.RuleSet("First, Default", () => {
				validator.RuleFor(x => x.Forename).NotNull();
			});

			var result = validator.Validate(new Person(), ruleSet: "First");
			result.Errors.Count.ShouldEqual(1);
			AssertExecuted(result, "First");

			result = validator.Validate(new Person(), ruleSet: "Second");
			result.Errors.Count.ShouldEqual(0);
			AssertExecuted(result);

			result = validator.Validate(new Person());
			result.Errors.Count.ShouldEqual(1);
			AssertExecuted(result, "default");
		}

		[Fact]
		public void Executes_in_rule_in_default_and_none() {
			var validator = new InlineValidator<Person>();
			validator.RuleSet("First, Default", () => {
				validator.RuleFor(x => x.Forename).NotNull();
			});
			validator.RuleFor(x => x.Forename).NotNull();

			var result = validator.Validate(new Person(), ruleSet: "default");
			result.Errors.Count.ShouldEqual(2);
			AssertExecuted(result, "default");
		}
		
		private void AssertExecuted(ValidationResult result, params string[] names) {
			result.RuleSetsExecuted.Length.ShouldEqual(names.Length);
			result.RuleSetsExecuted.Intersect(names).Count().ShouldEqual(names.Length);
		}

		private class TestValidator : AbstractValidator<Person> {
			public TestValidator() {
				RuleSet("Names", () => {
					RuleFor(x => x.Surname).NotNull();
					RuleFor(x => x.Forename).NotNull();
				});

				RuleFor(x => x.Id).NotEmpty();
			}
		}

		private class TestValidator2 : AbstractValidator<Person>
		{
			public TestValidator2()
			{
				RuleSet("Names", () => {
					RuleFor(x => x.Surname).NotNull().WithMessage("foo");
				});

			}

		}


		public class TestValidator3 : AbstractValidator<Person> {
			public TestValidator3() {
				Include(new TestValidator2());
			}
		}


		public class PersonContainer {
			public Person Person { get; set; }
		}

		public class TestValidator4 : AbstractValidator<PersonContainer>
		{
			public TestValidator4()
			{
				RuleFor(x => x.Person).SetValidator(new TestValidator2());
			}
		}
	}
}