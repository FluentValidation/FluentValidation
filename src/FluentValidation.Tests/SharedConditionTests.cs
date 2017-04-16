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
	using System.Threading.Tasks;
	using Xunit;
	using Results;

	
	public class SharedConditionTests {
		class SharedConditionValidator : AbstractValidator<Person> {
			public SharedConditionValidator() {
				// Start with a predicate to group rules together.
				// 
				// The AbstractValidator appends this predicate
				// to each inner RuleFor so you only need write,
				// maintain, and think about it in one place.
				//
				// You can finish with an Unless clause that will
				// void the validation for the entire set when it's 
				// predicate is true.
				// 
				When(x => x.Id > 0, () => {
					RuleFor(x => x.Forename).NotEmpty();
					RuleFor(x => x.Surname).NotEmpty().Equal("Smith");
				});
			}
		}

		class SharedAsyncConditionValidator : AbstractValidator<Person> {
			public SharedAsyncConditionValidator() {
				// Start with a predicate to group rules together.
				// 
				// The AbstractValidator appends this predicate
				// to each inner RuleFor so you only need write,
				// maintain, and think about it in one place.
				//
				// You can finish with an Unless clause that will
				// void the validation for the entire set when it's 
				// predicate is true.
				// 
				WhenAsync(async x => x.Id > 0,
					() => {
						RuleFor(x => x.Forename).NotEmpty();
						RuleFor(x => x.Surname).NotEmpty().Equal("Smith");
					}
				);
			}
		}

		class SharedConditionWithScopedUnlessValidator : AbstractValidator<Person> {
			public SharedConditionWithScopedUnlessValidator() {
				// inner RuleFor() calls can contain their own,
				// locally scoped When and Unless calls that
				// act only on that individual RuleFor() yet the
				// RuleFor() respects the grouped When() and 
				// Unless() predicates.
				// 
				When(x => x.Id > 0 && x.Age <= 65, () => { RuleFor(x => x.Orders.Count).Equal(0).Unless(x => String.IsNullOrWhiteSpace(x.CreditCard) == false); });
				//.Unless(x => x.Age > 65);
			}
		}

		class SharedAsyncConditionWithScopedUnlessValidator : AbstractValidator<Person> {
			public SharedAsyncConditionWithScopedUnlessValidator() {
				// inner RuleFor() calls can contain their own,
				// locally scoped When and Unless calls that
				// act only on that individual RuleFor() yet the
				// RuleFor() respects the grouped When() and 
				// Unless() predicates.
				// 
				WhenAsync(async x => x.Id > 0 && x.Age <= 65,
					() => {
						RuleFor(x => x.Orders.Count).Equal(0).UnlessAsync(async x => String.IsNullOrWhiteSpace(x.CreditCard) == false);
					}
				);
			}
		}

		class SharedConditionInverseValidator : AbstractValidator<Person> {
			public SharedConditionInverseValidator() {
				Unless(x => x.Id == 0, () => { RuleFor(x => x.Forename).NotNull(); });
			}
		}

		class SharedAsyncConditionInverseValidator : AbstractValidator<Person>
		{
			public SharedAsyncConditionInverseValidator()
			{
				UnlessAsync(async x => x.Id == 0, () => { RuleFor(x => x.Forename).NotNull(); });
			}
		}

		[Fact]
		public void Shared_When_is_not_applied_to_grouped_rules_when_initial_predicate_is_false() {
			var validator = new SharedConditionValidator();
			var person = new Person(); // fails the shared When predicate

			var result = validator.Validate(person);
			result.Errors.Count.ShouldEqual(0);
		}

		[Fact]
		public void Shared_async_When_is_not_applied_to_grouped_rules_when_initial_predicate_is_false() {
			var validator = new SharedAsyncConditionValidator();
			var person = new Person(); // fails the shared When predicate

			var result = validator.ValidateAsync(person).Result;
			result.Errors.Count.ShouldEqual(0);
		}

		[Fact]
		public void Shared_When_is_applied_to_grouped_rules_when_initial_predicate_is_true() {
			var validator = new SharedConditionValidator();
			var person = new Person() {
				Id = 4 // triggers the shared When predicate
			};

			var result = validator.Validate(person);
			result.Errors.Count.ShouldEqual(3);
		}

		[Fact]
		public void Shared_async_When_is_applied_to_grouped_rules_when_initial_predicate_is_true() {
			var validator = new SharedAsyncConditionValidator();
			var person = new Person() {
				Id = 4 // triggers the shared When predicate
			};

			var result = validator.ValidateAsync(person).Result;
			result.Errors.Count.ShouldEqual(3);
		}

		[Fact]
		public void Shared_When_is_applied_to_groupd_rules_when_initial_predicate_is_true_and_all_individual_rules_are_satisfied() {
			var validator = new SharedConditionValidator();
			var person = new Person() {
			                          	Id = 4, // triggers the shared When predicate
			                          	Forename = "Kevin", // satisfies RuleFor( x => x.Forename ).NotEmpty()
			                          	Surname = "Smith", // satisfies RuleFor( x => x.Surname ).NotEmpty().Equal( "Smith" )
			                          };

			var result = validator.Validate(person);
			result.Errors.Count.ShouldEqual(0);
		}

		[Fact]
		public void Shared_async_When_is_applied_to_groupd_rules_when_initial_predicate_is_true_and_all_individual_rules_are_satisfied() {
			var validator = new SharedAsyncConditionValidator();
			var person = new Person() {
			                          	Id = 4, // triggers the shared When predicate
			                          	Forename = "Kevin", // satisfies RuleFor( x => x.Forename ).NotEmpty()
			                          	Surname = "Smith", // satisfies RuleFor( x => x.Surname ).NotEmpty().Equal( "Smith" )
			                          };

			var result = validator.ValidateAsync(person).Result;
			result.Errors.Count.ShouldEqual(0);
		}

		[Fact]
		public void Shared_When_respects_the_smaller_scope_of_an_inner_Unless_when_the_inner_Unless_predicate_is_satisfied() {
			var validator = new SharedConditionWithScopedUnlessValidator();
			var person = new Person() {
			                          	Id = 4 // triggers the shared When predicate
			                          };

			person.CreditCard = "1234123412341234"; // satisfies the inner Unless predicate
			person.Orders.Add(new Order());

			var result = validator.Validate(person);
			result.Errors.Count.ShouldEqual(0);
		}

		[Fact]
		public void Shared_async_When_respects_the_smaller_scope_of_an_inner_Unless_when_the_inner_Unless_predicate_is_satisfied() {
			var validator = new SharedAsyncConditionWithScopedUnlessValidator();
			var person = new Person() {
			                          	Id = 4 // triggers the shared When predicate
			                          };

			person.CreditCard = "1234123412341234"; // satisfies the inner Unless predicate
			person.Orders.Add(new Order());

			var result = validator.ValidateAsync(person).Result;
			result.Errors.Count.ShouldEqual(0);
		}

		[Fact]
		public void Shared_When_respects_the_smaller_scope_of_a_inner_Unless_when_the_inner_Unless_predicate_fails() {
			var validator = new SharedConditionWithScopedUnlessValidator();
			var person = new Person() {
			                          	Id = 4 // triggers the shared When predicate
			                          };

			person.Orders.Add(new Order()); // fails the inner Unless predicate

			var result = validator.Validate(person);
			result.Errors.Count.ShouldEqual(1);
		}

		[Fact]
		public void Shared_async_When_respects_the_smaller_scope_of_a_inner_Unless_when_the_inner_Unless_predicate_fails() {
			var validator = new SharedAsyncConditionWithScopedUnlessValidator();
			var person = new Person() {
			                          	Id = 4 // triggers the shared When predicate
			                          };

			person.Orders.Add(new Order()); // fails the inner Unless predicate

			var result = validator.ValidateAsync(person).Result;
			result.Errors.Count.ShouldEqual(1);
		}

		[Fact]
		public void Outer_Unless_clause_will_trump_an_inner_Unless_clause_when_inner_fails_but_the_outer_is_satisfied() {
			var validator = new SharedConditionWithScopedUnlessValidator();
			var person = new Person() {
			                          	Id = 4, // triggers the shared When predicate
			                          	Age = 70 // satisfies the outer Unless predicate
			                          };

			person.Orders.Add(new Order()); // fails the inner Unless predicate

			var result = validator.Validate(person);
			result.Errors.Count.ShouldEqual(0);
		}

		[Fact]
		public void Outer_async_Unless_clause_will_trump_an_inner_Unless_clause_when_inner_fails_but_the_outer_is_satisfied() {
			var validator = new SharedAsyncConditionWithScopedUnlessValidator();
			var person = new Person() {
			                          	Id = 4, // triggers the shared When predicate
			                          	Age = 70 // satisfies the outer Unless predicate
			                          };

			person.Orders.Add(new Order()); // fails the inner Unless predicate

			var result = validator.ValidateAsync(person).Result;
			result.Errors.Count.ShouldEqual(0);
		}

		[Fact]
		public void Condition_can_be_used_inside_ruleset() {
			var validator = new TestValidator();
			validator.RuleSet("foo", () => { validator.When(x => x.Id > 0, () => { validator.RuleFor(x => x.Forename).NotNull(); }); });
			validator.RuleFor(x => x.Surname).NotNull();

			var result = validator.Validate(new Person {Id = 5}, ruleSet : "foo");
			result.Errors.Count.ShouldEqual(1);
			result.Errors.Single().PropertyName.ShouldEqual("Forename");
		}

		[Fact]
		public void Async_condition_can_be_used_inside_ruleset() {
			var validator = new TestValidator();
			validator.RuleSet("foo", () => { validator.WhenAsync(async x => (x.Id > 0), () => { validator.RuleFor(x => x.Forename).NotNull(); }); });
			validator.RuleFor(x => x.Surname).NotNull();

			var result = validator.ValidateAsync(new Person {Id = 5}, ruleSet: "foo").Result;
			result.Errors.Count.ShouldEqual(1);
			result.Errors.Single().PropertyName.ShouldEqual("Forename");
		}

		[Fact]
		public void RuleSet_can_be_used_inside_condition() {
			var validator = new TestValidator();

			validator.When(x => x.Id > 0, () => { validator.RuleSet("foo", () => { validator.RuleFor(x => x.Forename).NotNull(); }); });

			validator.RuleFor(x => x.Surname).NotNull();

			var result = validator.Validate(new Person {Id = 5}, ruleSet : "foo");
			result.Errors.Count.ShouldEqual(1);
			result.Errors.Single().PropertyName.ShouldEqual("Forename");
		}

		[Fact]
		public void RuleSet_can_be_used_inside_async_condition() {
			var validator = new TestValidator();

			validator.WhenAsync(async x => (x.Id > 0), () => { validator.RuleSet("foo", () => { validator.RuleFor(x => x.Forename).NotNull(); }); });

			validator.RuleFor(x => x.Surname).NotNull();

			var result = validator.ValidateAsync(new Person {Id = 5}, ruleSet: "foo").Result;
			result.Errors.Count.ShouldEqual(1);
			result.Errors.Single().PropertyName.ShouldEqual("Forename");
		}

		[Fact]
		public void Rules_invoke_when_inverse_shared_condition_matches() {
			var validator = new SharedConditionInverseValidator();
			var result = validator.Validate(new Person {Id = 1});
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void Rules_invoke_when_inverse_shared_async_condition_matches() {
			var validator = new SharedAsyncConditionInverseValidator();
			var result = validator.ValidateAsync(new Person {Id = 1}).Result;
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void Rules_not_invoked_when_inverse_shared_condition_does_not_match() {
			var validator = new SharedConditionInverseValidator();
			var result = validator.Validate(new Person());
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Rules_not_invoked_when_inverse_shared_async_condition_does_not_match() {
			var validator = new SharedAsyncConditionInverseValidator();
			var result = validator.ValidateAsync(new Person()).Result;
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Does_not_execute_custom_Rule_when_condition_false() {
			var validator = new TestValidator();
			validator.When(x => false, () => { validator.Custom(x => new ValidationFailure("foo", "bar")); });

			var result = validator.Validate(new Person());
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Does_not_execute_custom_Rule_when_async_condition_false() {
			var validator = new TestValidator();
			validator.WhenAsync(async x => (false), () => { validator.Custom(x => new ValidationFailure("foo", "bar")); });

			var result = validator.ValidateAsync(new Person()).Result;
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Does_not_execute_customasync_Rule_when_condition_false()
		{
			var validator = new TestValidator();
			validator.When(x => false, () => validator.CustomAsync(async x => (new ValidationFailure("foo", "bar"))));

			var result = validator.Validate(new Person());
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Does_not_execute_customasync_Rule_when_async_condition_false() {
			var validator = new TestValidator();
			validator.WhenAsync(async x => (false), () => validator.CustomAsync(async x => (new ValidationFailure("foo", "bar"))));

			var result = validator.ValidateAsync(new Person()).Result;
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Executes_custom_rule_when_condition_true() {
			var validator = new TestValidator();
			validator.When(x => true, () => { validator.Custom(x => new ValidationFailure("foo", "bar")); });

			var result = validator.Validate(new Person());
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void Executes_custom_rule_when_async_condition_true() {
			var validator = new TestValidator();
			validator.WhenAsync(async x => (true), () => { validator.Custom(x => new ValidationFailure("foo", "bar")); });

			var result = validator.Validate(new Person());
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void Executes_customasync_rule_when_condition_true() {
			var validator = new TestValidator();
			validator.When(x => true, () => validator.CustomAsync(async x => (new ValidationFailure("foo", "bar"))));

			var result = validator.ValidateAsync(new Person()).Result;
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void Executes_customasync_rule_when_async_condition_true()
		{
			var validator = new TestValidator();
			validator.WhenAsync(async x => (true), () => validator.CustomAsync(async x => (new ValidationFailure("foo", "bar"))));

			var result = validator.ValidateAsync(new Person()).Result;
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void Nested_conditions_with_Custom_rule() {
			var validator = new TestValidator();
			validator.When(x => true, () => {
				validator.When(x => false, () => {
					validator.Custom(x => new ValidationFailure("Custom", "The validation failed"));
					
				});
			});
			var result = validator.Validate(new Person());
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Nested_async_conditions_with_Custom_rule() {
			var validator = new TestValidator();
			validator.When(x => true, () => {
				validator.WhenAsync(async x => (false), () => {
					validator.Custom(x => new ValidationFailure("Custom", "The validation failed"));
				});
			});
			var result = validator.Validate(new Person());
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Nested_conditions_with_CustomAsync_rule() {
			var validator = new TestValidator();
			validator.When(x => true, () => {
				validator.When(x => false, () => {
					validator.CustomAsync(async x => (new ValidationFailure("Custom", "The validation failed")));
				});
			});
			var result = validator.ValidateAsync(new Person()).Result;
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Nested_async_conditions_with_CustomAsync_rule() {
			var validator = new TestValidator();
			validator.When(x => true, () => {
				validator.WhenAsync(async x => (false), () => {
					validator.CustomAsync(async x => (new ValidationFailure("Custom", "The validation failed")));
				});
			});
			var result = validator.ValidateAsync(new Person()).Result;
			result.IsValid.ShouldBeTrue();
		}
	}
}

