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
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Internal;
	using Validators;
	using Xunit;


	public class ForEachRuleTests {
		private object _lock = new object();
		private int _counter;
		private Person _person;

		public ForEachRuleTests() {
			_counter = 0;

			_person = new Person() {
				Orders = new List<Order>() {
					new Order { Amount = 5},
					new Order { ProductName = "Foo"}
				}
			};
		}

		[Fact]
		public void Executes_rule_for_each_item_in_collection() {
			var validator = new TestValidator {
				v => v.RuleForEach(x => x.NickNames).NotNull()
			};

			var person = new Person {
				NickNames = new[] {null, "foo", null}
			};

			var result = validator.Validate(person);
			result.Errors.Count.ShouldEqual(2);
		}

		[Fact]
		public void Correctly_gets_collection_indices() {
			var validator = new TestValidator {
				v => v.RuleForEach(x => x.NickNames).NotNull()
			};

			var person = new Person {
				NickNames = new[] {null, "foo", null}
			};

			var result = validator.Validate(person);
			result.Errors[0].PropertyName.ShouldEqual("NickNames[0]");
			result.Errors[1].PropertyName.ShouldEqual("NickNames[2]");
		}

		[Fact]
		public void Overrides_indexer() {
			var validator = new TestValidator {
				v => v.RuleForEach(x => x.NickNames)
					.OverrideIndexer((x, collection, element, index) => {
						return "<" + index + ">";
					})
					.NotNull()
			};

			var person = new Person {
				NickNames = new[] {null, "foo", null}
			};

			var result = validator.Validate(person);
			result.Errors[0].PropertyName.ShouldEqual("NickNames<0>");
			result.Errors[1].PropertyName.ShouldEqual("NickNames<2>");
		}

		[Fact]
		public async Task Overrides_indexer_async() {
			var validator = new TestValidator {
				v => v.RuleForEach(x => x.NickNames)
					.OverrideIndexer((x, collection, element, index) => {
						return "<" + index + ">";
					})
					.MustAsync(async (x, elem, ct) => elem != null)
			};

			var person = new Person {
				NickNames = new[] {null, "foo", null}
			};

			var result = await validator.ValidateAsync(person);
			result.Errors[0].PropertyName.ShouldEqual("NickNames<0>");
			result.Errors[1].PropertyName.ShouldEqual("NickNames<2>");
		}

		[Fact]
		public async Task Executes_rule_for_each_item_in_collection_async() {
			var validator = new TestValidator {
				v => v.RuleForEach(x => x.NickNames).SetValidator(new MyAsyncNotNullValidator())
			};

			var person = new Person {
				NickNames = new[] {null, "foo", null}
			};

			var result = await validator.ValidateAsync(person);
			result.Errors.Count.ShouldEqual(2);
		}

		[Fact]
		public async Task Correctly_gets_collection_indices_async() {
			var validator = new TestValidator {
				v => v.RuleForEach(x => x.NickNames).SetValidator(new MyAsyncNotNullValidator())
			};

			var person = new Person {
				NickNames = new[] {null, "foo", null}
			};

			var result = await validator.ValidateAsync(person);
			result.Errors[0].PropertyName.ShouldEqual("NickNames[0]");
			result.Errors[1].PropertyName.ShouldEqual("NickNames[2]");
		}

		class request {
			public Person person = null;
		}

		private class MyAsyncNotNullValidator : NotNullValidator {
			public override bool ShouldValidateAsynchronously(ValidationContext context) {
				return context.IsAsync();
			}
		}

		[Fact]
		public void Nested_collection_for_null_property_should_not_throw_null_reference() {
			var validator = new InlineValidator<request>();
			validator.When(r => r.person != null, () => { validator.RuleForEach(x => x.person.NickNames).NotNull(); });

			var result = validator.Validate(new request());
			result.Errors.Count.ShouldEqual(0);
		}

		[Fact]
		public void Should_not_scramble_property_name_when_using_collection_validators_several_levels_deep() {
			var v = new ApplicationViewModelValidator();
			var result = v.Validate(new ApplicationViewModel());

			result.Errors.Single().PropertyName.ShouldEqual("TradingExperience[0].Questions[0].SelectedAnswerID");
		}

		[Fact]
		public async Task Should_not_scramble_property_name_when_using_collection_validators_several_levels_deep_with_ValidateAsync() {
			var v = new ApplicationViewModelValidator();
			var result = await v.ValidateAsync(new ApplicationViewModel());

			result.Errors.Single().PropertyName.ShouldEqual("TradingExperience[0].Questions[0].SelectedAnswerID");
		}

		[Fact]
		public void Uses_useful_error_message_when_used_on_non_property() {
			var validator = new InlineValidator<Person>();
			validator.RuleForEach(x => x.NickNames.AsEnumerable()).NotNull();

			bool thrown = false;
			try {
				validator.Validate(new Person {NickNames = new string[] {null, null}});
			}
			catch (System.InvalidOperationException ex) {
				thrown = true;
				ex.Message.ShouldEqual("Could not infer property name for expression: x => x.NickNames.AsEnumerable(). Please explicitly specify a property name by calling OverridePropertyName as part of the rule chain. Eg: RuleForEach(x => x).NotNull().OverridePropertyName(\"MyProperty\")");
			}

			thrown.ShouldBeTrue();
		}

		[Fact]
		public async Task RuleForEach_async_RunsTasksSynchronously() {
			var validator = new InlineValidator<Person>();
			var result = new List<bool>();

			validator.RuleForEach(x => x.Children).MustAsync(async (person, token) =>
				await ExclusiveDelay(1)
					.ContinueWith(t => result.Add(t.Result), token)
					.ContinueWith(t => true, token)
			);

			await validator.ValidateAsync(new Person() {
				Children = new List<Person> {new Person(), new Person() }
			});

			Assert.NotEmpty(result);
			Assert.All(result, Assert.True);
		}

		[Fact]
		public void Can_use_cascade_with_RuleForEach() {
			var validator = new InlineValidator<Person>();
			validator.RuleForEach(x => x.NickNames)
				.Cascade(CascadeMode.StopOnFirstFailure)
				.NotNull()
				.NotEqual("foo");

			var result = validator.Validate(new Person {NickNames = new string[] {null}});
			result.Errors.Count.ShouldEqual(1);
		}

		[Fact]
		public void Nested_conditions_Rule_For() {
			var validator = new InlineValidator<request>();
			validator.When(r => true, () => {
				validator.When(r => r.person?.NickNames?.Any() == true, () => {
					validator.RuleFor(r => r.person.NickNames)
						.Must(nn => true)
						.WithMessage("Failed RuleFor");
				});
			});
			var result = validator.Validate(new request());
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Nested_conditions_Rule_For_Each() {
			var validator = new InlineValidator<request>();

			validator.When(x => true, () => {
				validator.When(r => r.person?.NickNames?.Any() == true, () => {
					validator.RuleForEach(x => x.person.NickNames)
						.Must(nn => true)
						.WithMessage("Failed RuleForEach");
				});
			});

			var result = validator.Validate(new request());
			result.Errors.Count.ShouldEqual(0);
		}

		[Fact]
		public void Regular_rules_can_drop_into_RuleForEach() {
			var validator = new TestValidator();
			validator.RuleFor(x => x.Children)
				.Must(x => x.Count > 2).WithMessage("Foo")
				.ForEach(forEachElement => {
					forEachElement.NotNull().WithMessage("Bar");
				});

			var result = validator.Validate(new Person {Children = new List<Person> {null, null}});
			result.Errors.Count.ShouldEqual(3);
			result.Errors[0].ErrorMessage.ShouldEqual("Foo");
			result.Errors[0].PropertyName.ShouldEqual("Children");

			result.Errors[1].ErrorMessage.ShouldEqual("Bar");
			result.Errors[1].PropertyName.ShouldEqual("Children[0]");

			result.Errors[2].ErrorMessage.ShouldEqual("Bar");
			result.Errors[2].PropertyName.ShouldEqual("Children[1]");
		}

		public class ApplicationViewModel {
			public List<ApplicationGroup> TradingExperience { get; set; } = new List<ApplicationGroup> {new ApplicationGroup()};
		}

		public class ApplicationGroup {
			public List<Question> Questions = new List<Question> {new Question()};
		}

		public class Question {
			public int SelectedAnswerID { get; set; }
		}

		public class ApplicationViewModelValidator : AbstractValidator<ApplicationViewModel> {
			public ApplicationViewModelValidator() {
				RuleForEach(x => x.TradingExperience)
					.SetValidator(new AppropriatenessGroupViewModelValidator());
			}
		}

		public class AppropriatenessGroupViewModelValidator : AbstractValidator<ApplicationGroup> {
			public AppropriatenessGroupViewModelValidator() {
				RuleForEach(m => m.Questions)
					.SetValidator(new AppropriatenessQuestionViewModelValidator());
			}
		}

		public class AppropriatenessQuestionViewModelValidator : AbstractValidator<Question> {
			public AppropriatenessQuestionViewModelValidator() {
				RuleFor(m => m.SelectedAnswerID)
					.SetValidator(new AppropriatenessAnswerViewModelRequiredValidator());
				;
			}
		}

		public class AppropriatenessAnswerViewModelRequiredValidator : PropertyValidator {
			public AppropriatenessAnswerViewModelRequiredValidator()
				: base("Error message here.") {
			}

			protected override bool IsValid(PropertyValidatorContext context) {
				return false;
			}
		}

		private async Task<bool> ExclusiveDelay(int milliseconds) {
			lock (_lock) {
				if (_counter != 0) return false;
				_counter += 1;
			}

			await Task.Delay(milliseconds);

			lock (_lock) {
				_counter -= 1;
			}

			return true;
		}

		[Fact]
		public void Validates_collection() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull(),
				v => v.RuleForEach(x => x.Orders).SetValidator(new OrderValidator())
			};

			var results = validator.Validate(_person);
			results.Errors.Count.ShouldEqual(3);

			results.Errors[1].PropertyName.ShouldEqual("Orders[0].ProductName");
			results.Errors[2].PropertyName.ShouldEqual("Orders[1].Amount");
		}

		[Fact]
		public void Collection_should_be_explicitly_included_with_expression() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull(),
				v => v.RuleForEach(x => x.Orders).SetValidator(new OrderValidator())
			};

			var results = validator.Validate(_person, x => x.Orders);
			results.Errors.Count.ShouldEqual(2);
		}

		[Fact]
		public void Collection_should_be_explicitly_included_with_string() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull(),
				v => v.RuleForEach(x => x.Orders).SetValidator(new OrderValidator())
			};

			var results = validator.Validate(_person, "Orders");
			results.Errors.Count.ShouldEqual(2);
		}

		[Fact]
		public void Collection_should_be_excluded() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull(),
				v => v.RuleForEach(x => x.Orders).SetValidator(new OrderValidator())
			};

			var results = validator.Validate(_person, x => x.Forename);
			results.Errors.Count.ShouldEqual(0);
		}

		[Fact]
		public void Condition_should_work_with_child_collection() {
			var validator = new TestValidator() {
				v => v.RuleForEach(x => x.Orders).SetValidator(new OrderValidator()).When(x => x.Orders.Count == 3 /*there are only 2*/)
			};

			var result = validator.Validate(_person);
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
	    public async Task Async_condition_should_work_with_child_collection() {
	        var validator = new TestValidator() {
	                                                v => v.RuleForEach(x => x.Orders).SetValidator(new OrderValidator()).WhenAsync(async (x,c) => x.Orders.Count == 3 /*there are only 2*/)
	                                            };

	        var result = await validator.ValidateAsync(_person);
	        result.IsValid.ShouldBeTrue();
	    }

	    [Fact]
		public void Skips_null_items() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull(),
				v => v.RuleForEach(x => x.Orders).SetValidator(new OrderValidator())
			};

			_person.Orders[0] = null;
			var results = validator.Validate(_person);
			results.Errors.Count.ShouldEqual(2); //2 errors - 1 for person, 1 for 2nd Order.
		}

		[Fact]
		public void Can_validate_collection_using_validator_for_base_type() {
			var validator = new TestValidator() {
				v => v.RuleForEach(x => x.Orders).SetValidator(new OrderInterfaceValidator())
			};

			var result = validator.Validate(_person);
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void Can_specify_condition_for_individual_collection_elements() {
			var validator = new TestValidator {
				v => v.RuleForEach(x => x.Orders)
					.Where(x => x.ProductName != null)
					.SetValidator(new OrderValidator())
			};

			var results = validator.Validate(_person);
			results.Errors.Count.ShouldEqual(1);

		}

		[Fact]
		public void Should_override_property_name() {
			var validator = new TestValidator {
				v => v.RuleForEach(x => x.Orders).SetValidator(new OrderValidator())
					.OverridePropertyName("Orders2")
			};

			var results = validator.Validate(_person);
			results.Errors[0].PropertyName.ShouldEqual("Orders2[0].ProductName");
		}

		[Fact]
		public void Top_level_collection() {
			var v = new InlineValidator<IEnumerable<Order>>();
			v.RuleForEach(x => x).SetValidator(new OrderValidator());
			var orders = new List<Order> {
				new Order(),
				new Order()
			};

			var result = v.Validate(orders);
			result.Errors.Count.ShouldEqual(4);
			result.Errors[0].PropertyName.ShouldEqual("x[0].ProductName");
		}

		[Fact]
		public void Validates_child_validator_synchronously() {
			var validator = new ComplexValidationTester.TracksAsyncCallValidator<Person>();
			var childValidator = new ComplexValidationTester.TracksAsyncCallValidator<Person>();
			childValidator.RuleFor(x => x.Forename).NotNull();
			validator.RuleForEach(x => x.Children).SetValidator(childValidator);

			validator.Validate(new Person() { Children = new List<Person> { new Person() }});
			childValidator.WasCalledAsync.ShouldEqual(false);
		}

		[Fact]
		public async Task Validates_child_validator_asynchronously() {
			var validator = new ComplexValidationTester.TracksAsyncCallValidator<Person>();
			var childValidator = new ComplexValidationTester.TracksAsyncCallValidator<Person>();
			childValidator.RuleFor(x => x.Forename).NotNull();
			validator.RuleForEach(x => x.Children).SetValidator(childValidator);

			await validator.ValidateAsync(new Person() {Children = new List<Person> {new Person()}});
			childValidator.WasCalledAsync.ShouldEqual(true);
		}

		[Fact]
		public void Can_access_colletion_index() {
			var validator = new InlineValidator<Person>();
			validator.RuleForEach(x => x.Orders).NotNull().WithMessage("{CollectionIndex}");
			var result = validator.Validate(new Person {Orders = new List<Order>() {new Order(), null}});
			result.IsValid.ShouldBeFalse();
			result.Errors[0].ErrorMessage.ShouldEqual("1");
		}

		[Fact]
		public async Task Can_access_colletion_index_async() {
			var validator = new InlineValidator<Person>();
			validator.RuleForEach(x => x.Orders).NotNull().WithMessage("{CollectionIndex}");
			var result = await validator.ValidateAsync(new Person {Orders = new List<Order>() {new Order(), null}});
			result.IsValid.ShouldBeFalse();
			result.Errors[0].ErrorMessage.ShouldEqual("1");
		}

		[Fact]
		public void When_runs_outside_RuleForEach_loop() {
			// Shouldn't throw an exception if the condition is run outside the loop.
			var validator = new InlineValidator<Tuple<Person>>();
			validator.RuleForEach(x => x.Item1.Orders).Must(x => false)
				.When(x => x.Item1 != null);

			var result = validator.Validate(Tuple.Create((Person) null));
			result.IsValid.ShouldBeTrue();

			result = validator.Validate(Tuple.Create(new Person() { Orders = new List<Order> { new Order() }}));
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public async Task When_runs_outside_RuleForEach_loop_async() {
			// Shouldn't throw an exception if the condition is run outside the loop.
			var validator = new InlineValidator<Tuple<Person>>();
			validator.RuleForEach(x => x.Item1.Orders)
				.MustAsync((x,c) => Task.FromResult(false))
				.When(x => x.Item1 != null);

			var result =	await validator.ValidateAsync(Tuple.Create((Person) null));
			result.IsValid.ShouldBeTrue();

			result = await validator.ValidateAsync(Tuple.Create(new Person() { Orders = new List<Order> { new Order() }}));
			result.IsValid.ShouldBeFalse();
		}

		public class OrderValidator : AbstractValidator<Order> {
			public OrderValidator() {
				RuleFor(x => x.ProductName).NotEmpty();
				RuleFor(x => x.Amount).NotEqual(0);
			}
		}

		public class OrderInterfaceValidator : AbstractValidator<IOrder> {
			public OrderInterfaceValidator() {
				RuleFor(x => x.Amount).NotEqual(0);
			}
		}
	}
}
