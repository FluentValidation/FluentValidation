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

namespace FluentValidation.Tests;

using System;
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

		var result = validator.Validate(new Person {
			Orders = new List<Order> {
				new Order { ProductName = null, Amount = 10 },
				new Order { ProductName = "foo", Amount = 0},
				new Order { ProductName = "foo", Amount = 10 }
			}
		});

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

	[Fact]
	public void ChildRules_works_with_SetValidator_and_RuleSet() {
		var validator = new RulesetChildValidatorRulesValidator();

		// If the validator inside a child rule specifies a rule set "b",
		// the rules inside the rule set "b" should not be used for the validation
		// if the validation context specified the ruleset "a"
		var result = validator.Validate(new Person {
			Orders = new List<Order> {
				new Order()
			}
		}, options => options.IncludeRuleSets("a"));

		result.Errors.Count.ShouldEqual(1);
		result.Errors[0].PropertyName.ShouldEqual("Surname");
	}

	[Fact]
	public void Multiple_levels_of_nested_child_rules_in_ruleset() {
		var validator = new InlineValidator<RulesetChildValidatorRulesValidator.Baz>();
		validator.RuleSet("Set1", () => {
			validator.RuleForEach(baz => baz.Bars)
				.ChildRules(barRule => barRule.RuleForEach(bar => bar.Foos)
					.ChildRules(fooRule => fooRule.RuleForEach(foo => foo.Names)
						.ChildRules(name => name.RuleFor(n => n)
							.NotEmpty()
							.WithMessage("Name is required"))));
		});

		var foos = new List<RulesetChildValidatorRulesValidator.Foo> {
			new() { Names = { "Bob" }},
			new() { Names = { string.Empty }},
		};

		var bars = new List<RulesetChildValidatorRulesValidator.Bar> {
			new(),
			new() { Foos = foos }
		};

		var baz = new RulesetChildValidatorRulesValidator.Baz {
			Bars = bars
		};

		var result = validator.Validate(baz, options => options.IncludeRuleSets("Set1"));
		result.IsValid.ShouldBeFalse();
	}

	[Fact]
	public void Doesnt_throw_InvalidCastException() {
		// See https://github.com/FluentValidation/FluentValidation/issues/2165
		var validator = new RootValidator();
		var result = validator.Validate(new Root { Data = new() { Value = -1 }});
		result.Errors.Count.ShouldEqual(1);
	}

	private class RulesetChildRulesValidator : AbstractValidator<Person> {
		public RulesetChildRulesValidator() {
			RuleSet("testing", () => {
				RuleFor(a => a.Surname).NotEmpty();
				RuleForEach(a => a.Orders).ChildRules(child => {
					child.RuleFor(o => o.ProductName).NotEmpty();
				});
			});
		}
	}

	private class RulesetChildValidatorRulesValidator : AbstractValidator<Person> {
		public RulesetChildValidatorRulesValidator() {
			RuleSet("a, b", () => {
				RuleFor(x => x.Surname).NotEmpty();
				RuleFor(x => x).ChildRules(child => {
					child.RuleForEach(o => o.Orders).SetValidator(new RulesetOrderValidator());
				});
			});
		}

		private class RulesetOrderValidator : AbstractValidator<Order> {
			public RulesetOrderValidator() {
				RuleSet("b", () => {
					RuleFor(o => o.ProductName).NotEmpty();
				});
			}
		}

		public class Foo {
			public List<string> Names { get; set; } = new();
		}

		public class Bar {
			public List<Foo> Foos { get; set; } = new();
		}

		public class Baz {
			public List<Bar> Bars { get; set; } = new();
		}
	}

	public class Root {
		public Bar Data {get; set;} = null;
	}

	public class Base {
		public int Value {get; set;}
	}

	public class Bar : Base  {
		public int BarValue {get; set;}
	}

	public class RootValidator : AbstractValidator<Root> {
		public RootValidator() {
			RuleFor(x => x).ChildRules(RootRules());
		}

		public static Action<InlineValidator<Base>> BaseRules() {
			return rules => {
				rules.RuleFor(x => x.Value).NotEqual(-1);
			};
		}

		public static Action<InlineValidator<Root>> RootRules() {
			return rules => {
				rules.RuleFor(x => x.Data).ChildRules(BaseRules());
			};
		}
	}

}
