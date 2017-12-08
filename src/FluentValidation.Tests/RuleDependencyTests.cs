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
	using System.Linq;
	using System.Threading.Tasks;
	using Xunit;

	public class RuleDependencyTests {
		[Fact]
		public void Invokes_dependent_rule_if_parent_rule_passes() {
			var validator = new TestValidator();
			validator.RuleFor(x => x.Surname).NotNull()
				.DependentRules(d => {
					d.RuleFor(x => x.Forename).NotNull();
				});

			var results = validator.Validate(new Person {Surname = "foo"});
			results.Errors.Count.ShouldEqual(1);
			results.Errors.Single().PropertyName.ShouldEqual("Forename");
		}

		[Fact]
		public void Does_not_invoke_dependent_rule_if_parent_rule_does_not_pass()
		{
			var validator = new TestValidator();
			validator.RuleFor(x => x.Surname).NotNull()
				.DependentRules(d =>
				{
					d.RuleFor(x => x.Forename).NotNull();
				});

			var results = validator.Validate(new Person { Surname = null });
			results.Errors.Count.ShouldEqual(1);
			results.Errors.Single().PropertyName.ShouldEqual("Surname");
		}

		[Fact]
		public void Dependent_rules_inside_ruleset() {
			var validator = new TestValidator();
			validator.RuleSet("MyRuleSet", () => {

				validator.RuleFor(x => x.Surname).NotNull()
					.DependentRules(d => {
						d.RuleFor(x => x.Forename).NotNull();
					});
			});

			var results = validator.Validate(new Person { Surname = "foo" }, ruleSet: "MyRuleSet");
			results.Errors.Count.ShouldEqual(1);
			results.Errors.Single().PropertyName.ShouldEqual("Forename");
		}

		[Fact]
		public void Dependent_rules_inside_when() {
			var validator = new TestValidator();
			validator.When(o => o.Forename != null, () =>
			{
				validator.RuleFor(o => o.Age).LessThan(1)
				.DependentRules(d =>
				{
					d.RuleFor(o => o.Forename).NotNull();
				});
			}); ;

			var result = validator.Validate(new Person());
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public async Task TestAsyncWithDependentRules_SyncEntry() {
			var validator = new TestValidator();
			validator.RuleFor(o => o.Forename)
				.NotNull()
				.DependentRules(d => {
					d.RuleFor(o => o.Address).NotNull();
					d.RuleFor(o => o.Age).MustAsync(async (p, token) => await Task.FromResult(p > 10));
				});

			var result = await validator.ValidateAsync(new Person());
			Assert.Equal(1, result.Errors.Count);
			Assert.True(result.Errors.Any(x => x.PropertyName == "Forename"));

			result = await validator.ValidateAsync(new Person() { Forename = "Foo" });
			Assert.Equal(2, result.Errors.Count);
			Assert.True(result.Errors.Count(x => x.PropertyName == "Address") == 1, "Address");
			Assert.True(result.Errors.Count(x => x.PropertyName == "Age") == 1, "Age");
		}

		[Fact]
		public async Task TestAsyncWithDependentRules_AsyncEntry() {
			var validator = new TestValidator();
			validator.RuleFor(o => o)
				.MustAsync(async (p, ct) => await Task.FromResult(p.Forename != null))
				.DependentRules(d => {
					d.RuleFor(o => o.Address).NotNull();
					d.RuleFor(o => o.Age).MustAsync(async (p, token) => await Task.FromResult(p > 10));
				});

			var result = await validator.ValidateAsync(new Person());
			Assert.Equal(1, result.Errors.Count);
			Assert.True(result.Errors.Any(x => x.PropertyName == ""));

			result = await validator.ValidateAsync(new Person() { Forename = "Foo" });
			Assert.Equal(2, result.Errors.Count);
			Assert.True(result.Errors.Count(x => x.PropertyName == "Address") == 1, "Address");
			Assert.True(result.Errors.Count(x => x.PropertyName == "Age") == 1, "Age");
		}

		[Fact]
		public void Async_inside_dependent_rules() {
			var validator = new AsyncValidator();
			var result = validator.ValidateAsync(0).Result;
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void Async_inside_dependent_rules_when_parent_rule_not_async() {
			var validator = new AsyncValidator2();
			var result = validator.ValidateAsync(0).Result;
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void Treats_root_level_RuleFor_call_as_dependent_rule_if_user_forgets_to_use_DependentRulesBuilder()
		{
			var validator = new TestValidator();
			validator.RuleFor(x => x.Surname).NotNull()
				.DependentRules(d => {
					validator.RuleFor(x => x.Forename).NotNull();  // Shouldn't be invoked
				});

			var results = validator.Validate(new Person { Surname = null });
			results.Errors.Count.ShouldEqual(1); //only the root NotNull should fire
			results.Errors.Single().PropertyName.ShouldEqual("Surname");
		}



		class AsyncValidator : AbstractValidator<int> {
			public AsyncValidator() {
				RuleFor(model => model)
					.MustAsync(async (ie, ct) => {
						await Task.Delay(500);
						return true;
					})
					.DependentRules(dependentRules => {
						dependentRules.RuleFor(m => m)
							.MustAsync(async (ie, ct) => {
								await Task.Delay(1000);
								return false;
							});
					});
			}
		}

		class AsyncValidator2 : AbstractValidator<int> {
			public AsyncValidator2() {
				RuleFor(model => model)
					.Must((ie) => true)
					.DependentRules(dependentRules => {
						dependentRules.RuleFor(m => m)
							.MustAsync(async (ie, ct) => {
								await Task.Delay(1000);
								return false;
							});
					});
			}
		}
	}
}