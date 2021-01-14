#region License

// Copyright (c) .NET Foundation and contributors.
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
// The latest version of this file can be found at https://github.com/FluentValidation/FluentValidation

#endregion

namespace FluentValidation.Tests {
	using System.Collections.Generic;
	using Xunit;

	public class ChildRulesTests {

		[Fact]
		public void Can_define_nested_rules_for_collection() {
			var validator = new InlineValidator<Person>();

			validator.RuleForEach(x => x.Orders).ChildRules(order => {
				order.RuleFor(x => x.ProductName).NotNull();
				order.RuleFor(x => x.Amount).GreaterThan(0);
			});

			var result = validator.Validate(new Person {Orders = new List<Order> {
				new Order { ProductName = null, Amount = 10 },
				new Order { ProductName = "foo", Amount = 0},
				new Order { ProductName = "foo", Amount = 10 }
			}});

			result.Errors.Count.ShouldEqual(2);
			result.Errors[0].PropertyName.ShouldEqual("Orders[0].ProductName");
			result.Errors[1].PropertyName.ShouldEqual("Orders[1].Amount");
		}

		[Fact]
		public void ChildRules_works_with_RuleSet() {
			var validator = new RulesetChildRulesValidator();

			// As Child Rules are implemented as a child validator, the child rules are technically
			// not inside the "testing" ruleset (going by the usual way rulesets cascade).
			// However, child rules should still be executed.
			var result = validator.Validate(new Person {
				Orders = new List<Order> {
					new Order()
				}
			}, options => options.IncludeRuleSets("testing"));

			result.Errors.Count.ShouldEqual(2);
			result.Errors[0].PropertyName.ShouldEqual("Surname");
			result.Errors[1].PropertyName.ShouldEqual("Orders[0].ProductName");

			// They shouldn't be executed if a different ruleset is chosen.
			result = validator.Validate(new Person {
				Orders = new List<Order> {
					new Order()
				}
			}, options => options.IncludeRuleSets("other"));

			result.Errors.Count.ShouldEqual(0);
		}

		private class RulesetChildRulesValidator : AbstractValidator<Person>  {
			public RulesetChildRulesValidator() {
				RuleSet("testing", () => {
					RuleFor(a => a.Surname).NotEmpty();
					RuleForEach(a => a.Orders).ChildRules(child => {
						child.RuleFor(o => o.ProductName).NotEmpty();
					});
				});
			}
		}
	}
}
