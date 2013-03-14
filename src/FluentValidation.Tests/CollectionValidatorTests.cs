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
	using NUnit.Framework;

	[TestFixture]
	public class CollectionValidatorTests {
		Person person;

		[SetUp]
		public void Setup() {
			person = new Person() {
				Orders = new List<Order>() {
					new Order { Amount = 5},
					new Order { ProductName = "Foo"}
				}
			};
		}

		[Test]
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

		[Test]
		public void Collection_should_be_explicitly_included_with_expression() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull(),
				v => v.RuleFor(x => x.Orders).SetCollectionValidator(new OrderValidator())
			};

			var results = validator.Validate(person, x => x.Orders);
			results.Errors.Count.ShouldEqual(2);
		}

		[Test]
		public void Collection_should_be_explicitly_included_with_string() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull(),
				v => v.RuleFor(x => x.Orders).SetCollectionValidator(new OrderValidator())
			};

			var results = validator.Validate(person, "Orders");
			results.Errors.Count.ShouldEqual(2);
		}

		[Test]
		public void Collection_should_be_excluded() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull(),
				v => v.RuleFor(x => x.Orders).SetCollectionValidator(new OrderValidator())
			};

			var results = validator.Validate(person, x => x.Forename);
			results.Errors.Count.ShouldEqual(0);
		}

		[Test]
		public void Condition_should_work_with_child_collection() {
			var validator = new TestValidator() {
				v => v.RuleFor(x => x.Orders).SetCollectionValidator(new OrderValidator()).When(x => x.Orders.Count == 3 /*there are only 2*/)
			};

			var result = validator.Validate(person);
			result.IsValid.ShouldBeTrue();
		}

		[Test]
		public void Skips_null_items() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull(),
				v => v.RuleFor(x => x.Orders).SetCollectionValidator(new OrderValidator())
			};

			person.Orders[0] = null;
			var results = validator.Validate(person);
			results.Errors.Count.ShouldEqual(2); //2 errors - 1 for person, 1 for 2nd Order.
		}

		[Test]
		public void Can_validate_collection_using_validator_for_base_type() {
			var validator = new TestValidator() {
				v => v.RuleFor(x => x.Orders).SetCollectionValidator(new OrderInterfaceValidator())
			};

			var result = validator.Validate(person);
			result.IsValid.ShouldBeFalse();
		}

		[Test]
		public void Can_specifiy_condition_for_individual_collection_elements() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Orders)
					.SetCollectionValidator(new OrderValidator())
					.Where(x => x.ProductName != null)
			};

			var results = validator.Validate(person);
			results.Errors.Count.ShouldEqual(1);

		}

		[Test]
		public void Should_override_property_name() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Orders).SetCollectionValidator(new OrderValidator())
					.OverridePropertyName("Orders2")
			};

			var results = validator.Validate(person);
			results.Errors[0].PropertyName.ShouldEqual("Orders2[0].ProductName");
		}
		
		#region old style - for compatibility with v2 and earlier before we had the explicit SetCollectionValidator
		#pragma warning disable 612,618
		[Test]
		public void Validates_collection_old_style() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull(),
				v => v.RuleFor(x => x.Orders).SetValidator(new OrderValidator())
			};

			var results = validator.Validate(person);
			results.Errors.Count.ShouldEqual(3);

			results.Errors[1].PropertyName.ShouldEqual("Orders[0].ProductName");
			results.Errors[2].PropertyName.ShouldEqual("Orders[1].Amount");
		}

		[Test]
		public void Collection_should_be_explicitly_included_with_expression_old_style() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull(),
				v => v.RuleFor(x => x.Orders).SetValidator(new OrderValidator())
			};
			var results = validator.Validate(person, x => x.Orders);
			results.Errors.Count.ShouldEqual(2);
		}

		[Test]
		public void Collection_should_be_explicitly_included_with_string_old_style() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull(),
				v => v.RuleFor(x => x.Orders).SetValidator(new OrderValidator())
			};
			var results = validator.Validate(person, "Orders");
			results.Errors.Count.ShouldEqual(2);
		}

		[Test]
		public void Collection_should_be_excluded_old_style() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull(),
				v => v.RuleFor(x => x.Orders).SetValidator(new OrderValidator())
			};
			var results = validator.Validate(person, x => x.Forename);
			results.Errors.Count.ShouldEqual(0);
		}

		[Test]
		public void Condition_should_work_with_child_collection_old_style() {
			var validator = new TestValidator() {
				v => v.RuleFor(x => x.Orders).SetValidator(new OrderValidator()).When(x => x.Orders.Count == 3 /*there are only 2*/)
			};

			var result = validator.Validate(person);
			result.IsValid.ShouldBeTrue();
		}

		[Test]
		public void Skips_null_items_old_style() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull(),
				v => v.RuleFor(x => x.Orders).SetValidator(new OrderValidator())
			};
			person.Orders[0] = null;
			var results = validator.Validate(person);
			results.Errors.Count.ShouldEqual(2); //2 errors - 1 for person, 1 for 2nd Order.
		}
		#pragma warning restore 612,618
		#endregion

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