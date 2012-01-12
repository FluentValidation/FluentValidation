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
// The latest version of this file can be found at http://fluentvalidation.codeplex.com
#endregion

namespace FluentValidation.Tests {
	using System;
	using Internal;
	using NUnit.Framework;

	[TestFixture]
	public class RulesetTests {


		[Test]
		public void Executes_rules_in_specified_ruleset() {
			var validator = new TestValidator();
			var result = validator.Validate(new ValidationContext<Person>(new Person(), new PropertyChain(), new RulesetValidatorSelector("Names")));

			result.Errors.Count.ShouldEqual(2); // 2 rules in this ruleset
		}

		[Test]
		public void Executes_rules_not_specified_in_ruleset() {
			var validator = new TestValidator();
			var result = validator.Validate(new Person());

			result.Errors.Count.ShouldEqual(1); // 1 rule not inside a ruleset
		}

		[Test]
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
		
		}

		[Test]
		public void Ruleset_cascades_to_child_collection_validator() {
			var orderValidator = new InlineValidator<Order>();
			orderValidator.RuleSet("Test", () => {
				orderValidator.RuleFor(x => x.ProductName).NotNull();
			});

			var validator = new TestValidator();
			
			validator.RuleSet("Test", () => {
				validator.RuleFor(x => x.Orders).SetCollectionValidator(orderValidator);
			});

			var person = new Person {
				Orders = { new Order(), new Order() }
			};

			var result = validator.Validate(new ValidationContext<Person>(person, new PropertyChain(), new RulesetValidatorSelector("Test")));


			result.Errors.Count.ShouldEqual(2); //one for each order
		}

		[Test]
		public void Executes_multiple_rulesets() {
			var validator = new TestValidator();
			validator.RuleSet("Id", () => {
				validator.RuleFor(x => x.Id).NotEqual(0);
			});

			var person = new Person();
			var result = validator.Validate(new ValidationContext<Person>(person, new PropertyChain(), new RulesetValidatorSelector("Names", "Id")));

			result.Errors.Count.ShouldEqual(3);
		}

		[Test]
		public void Executes_all_rules() {
			var validator = new TestValidator();
			var person = new Person();
			var result = validator.Validate(person, ruleSet: "*");
			result.Errors.Count.ShouldEqual(3);
		}

		[Test]
		public void Executes_rules_in_default_ruleset_and_specific_ruleset() {
			var validator = new TestValidator();
			validator.RuleSet("foo", () => {
				validator.RuleFor(x => x.Age).NotEqual(0);
			});

			var result = validator.Validate(new Person(), ruleSet : "default,Names");
			result.Errors.Count.ShouldEqual(3);

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

	}
}