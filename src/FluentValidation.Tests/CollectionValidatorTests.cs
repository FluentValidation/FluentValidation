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
	using System.Threading.Tasks;
	using Xunit;

	
	public class CollectionValidatorTests {
		private Person person;
		private object _lock = new object();
		private int _counter;

		public CollectionValidatorTests() {
			_counter = 0;
			
			person = new Person() {
				Orders = new List<Order>() {
					new Order { Amount = 5},
					new Order { ProductName = "Foo"}
				}
			};
		}

		[Fact]
		public void Validates_collection() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull(),
				v => v.RuleFor(x => x.Orders).SetCollectionValidator(new OrderValidator())
			};

			var results = validator.Validate(person);
			results.Errors.Count.ShouldEqual(3);

			results.Errors[1].PropertyName.ShouldEqual("Orders[0].ProductName");
			results.Errors[2].PropertyName.ShouldEqual("Orders[1].Amount");
		}

		[Fact]
		public void Collection_should_be_explicitly_included_with_expression() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull(),
				v => v.RuleFor(x => x.Orders).SetCollectionValidator(new OrderValidator())
			};

			var results = validator.Validate(person, x => x.Orders);
			results.Errors.Count.ShouldEqual(2);
		}

		[Fact]
		public void Collection_should_be_explicitly_included_with_string() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull(),
				v => v.RuleFor(x => x.Orders).SetCollectionValidator(new OrderValidator())
			};

			var results = validator.Validate(person, "Orders");
			results.Errors.Count.ShouldEqual(2);
		}

		[Fact]
		public void Collection_should_be_excluded() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull(),
				v => v.RuleFor(x => x.Orders).SetCollectionValidator(new OrderValidator())
			};

			var results = validator.Validate(person, x => x.Forename);
			results.Errors.Count.ShouldEqual(0);
		}

		[Fact]
		public void Condition_should_work_with_child_collection() {
			var validator = new TestValidator() {
				v => v.RuleFor(x => x.Orders).SetCollectionValidator(new OrderValidator()).When(x => x.Orders.Count == 3 /*there are only 2*/)
			};

			var result = validator.Validate(person);
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
	    public void Async_condition_should_work_with_child_collection() {
	        var validator = new TestValidator() {
	                                                v => v.RuleFor(x => x.Orders).SetCollectionValidator(new OrderValidator()).WhenAsync(async (x,c) => x.Orders.Count == 3 /*there are only 2*/)
	                                            };

	        var result = validator.ValidateAsync(person).Result;
	        result.IsValid.ShouldBeTrue();
	    }

	    [Fact]
		public void Skips_null_items() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull(),
				v => v.RuleFor(x => x.Orders).SetCollectionValidator(new OrderValidator())
			};

			person.Orders[0] = null;
			var results = validator.Validate(person);
			results.Errors.Count.ShouldEqual(2); //2 errors - 1 for person, 1 for 2nd Order.
		}

		[Fact]
		public void Can_validate_collection_using_validator_for_base_type() {
			var validator = new TestValidator() {
				v => v.RuleFor(x => x.Orders).SetCollectionValidator(new OrderInterfaceValidator())
			};

			var result = validator.Validate(person);
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void Can_specifiy_condition_for_individual_collection_elements() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Orders)
					.SetCollectionValidator(new OrderValidator())
					.Where(x => x.ProductName != null)
			};

			var results = validator.Validate(person);
			results.Errors.Count.ShouldEqual(1);

		}

		[Fact]
		public void Should_override_property_name() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Orders).SetCollectionValidator(new OrderValidator())
					.OverridePropertyName("Orders2")
			};

			var results = validator.Validate(person);
			results.Errors[0].PropertyName.ShouldEqual("Orders2[0].ProductName");
		}

		[Fact]
		public void Top_level_collection() {
			var v = new InlineValidator<IEnumerable<Order>>();
			v.RuleFor(x => x).SetCollectionValidator(new OrderValidator());
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
			validator.RuleFor(x => x.Children).SetCollectionValidator(childValidator);

			validator.Validate(new Person() { Children = new List<Person> { new Person() }});
			childValidator.WasCalledAsync.ShouldEqual(false);
		}

		[Fact]
		public void Validates_child_validator_asynchronously() {
			var validator = new ComplexValidationTester.TracksAsyncCallValidator<Person>();
			var childValidator = new ComplexValidationTester.TracksAsyncCallValidator<Person>();
			childValidator.RuleFor(x => x.Forename).NotNull();
			validator.RuleFor(x => x.Children).SetCollectionValidator(childValidator);

			validator.ValidateAsync(new Person() { Children = new List<Person> { new Person() }}).GetAwaiter().GetResult();
			childValidator.WasCalledAsync.ShouldEqual(true);
		}
	
		[Fact]
		public async Task Collection_async_RunsTasksSynchronously() {
			var result = new List<bool>();
			var validator = new InlineValidator<Person>();
			var orderValidator = new InlineValidator<Order>();
		
			orderValidator.RuleFor(x => x.ProductName).MustAsync((x, token) => {
				return ExclusiveDelay(1)
					.ContinueWith(t => result.Add(t.Result))
					.ContinueWith(t => true);
			});

			validator.RuleFor(x => x.Orders).SetCollectionValidator(orderValidator);
			
			await validator.ValidateAsync(person);

			Assert.NotEmpty(result);
			Assert.All(result, Assert.True);
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
	}
}