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
	using System.Linq;
	using System.Threading.Tasks;
	using Internal;
	using Xunit;

	public class RuleDependencyTests {
		[Fact]
		public void Invokes_dependent_rule_if_parent_rule_passes() {
			var validator = new TestValidator();
			validator.RuleFor(x => x.Surname).NotNull()
				.DependentRules(() => {
					validator.RuleFor(x => x.Forename).NotNull();
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
				.DependentRules(() =>
				{
					validator.RuleFor(x => x.Forename).NotNull();
				});

			var results = validator.Validate(new Person { Surname = null });
			results.Errors.Count.ShouldEqual(1);
			results.Errors.Single().PropertyName.ShouldEqual("Surname");
		}

		[Fact]
		public void Nested_dependent_rules() {
			var validator = new TestValidator();
			validator.RuleFor(x => x.Surname).NotNull()
				.DependentRules(() => {
					validator.RuleFor(x => x.Forename).NotNull().DependentRules(() => {
						validator.RuleFor(x => x.Forename).NotEqual("foo");
					});
				});

			var results = validator.Validate(new Person { Surname = "foo" });
			results.Errors.Count.ShouldEqual(1);
			results.Errors.Single().PropertyName.ShouldEqual("Forename");
			var rule = validator.Single();
			rule.DependentRules.Count().ShouldEqual(1);
			rule.DependentRules.First().DependentRules.Count().ShouldEqual(1);
		}

		[Fact]
		public void Dependent_rules_inside_ruleset() {
			var validator = new TestValidator();
			validator.RuleSet("MyRuleSet", () => {

				validator.RuleFor(x => x.Surname).NotNull()
					.DependentRules(() => {
						validator.RuleFor(x => x.Forename).NotNull();
					});
			});

			var results = validator.Validate(new Person { Surname = "foo" }, v => v.IncludeRuleSets("MyRuleSet") );
			results.Errors.Count.ShouldEqual(1);
			results.Errors.Single().PropertyName.ShouldEqual("Forename");
		}

		[Fact]
		public void Dependent_rules_inside_when() {
			var validator = new TestValidator();
			validator.When(o => o.Forename != null, () =>
			{
				validator.RuleFor(o => o.Age).LessThan(1)
				.DependentRules(() =>
				{
					validator.RuleFor(o => o.Forename).NotNull();
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
				.DependentRules(() => {
					validator.RuleFor(o => o.Address).NotNull();
					validator.RuleFor(o => o.Age).MustAsync(async (p, token) => await Task.FromResult(p > 10));
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
				.DependentRules(() => {
					validator.RuleFor(o => o.Address).NotNull();
					validator.RuleFor(o => o.Age).MustAsync(async (p, token) => await Task.FromResult(p > 10));
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
		public async Task Async_inside_dependent_rules() {
			var validator = new AsyncValidator();
			var result = await validator.ValidateAsync(0);
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public async Task Async_inside_dependent_rules_when_parent_rule_not_async() {
			var validator = new AsyncValidator2();
			var result = await validator.ValidateAsync(0);
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void Treats_root_level_RuleFor_call_as_dependent_rule_if_user_forgets_to_use_DependentRulesBuilder()
		{
			var validator = new TestValidator();
			validator.RuleFor(x => x.Surname).NotNull()
				.DependentRules(() => {
					validator.RuleFor(x => x.Forename).NotNull();  // Shouldn't be invoked
				});

			var results = validator.Validate(new Person { Surname = null });
			results.Errors.Count.ShouldEqual(1); //only the root NotNull should fire
			results.Errors.Single().PropertyName.ShouldEqual("Surname");
		}

		[Fact]
		public void Nested_dependent_rules_inside_ruleset()
		{
			var validator = new TestValidator();
			validator.RuleSet("MyRuleSet", () =>
			{

				validator.RuleFor(x => x.Surname).NotNull()
					.DependentRules(() =>
					{


						validator.RuleFor(x => x.Forename).NotNull()
							.DependentRules(() =>
							{
								validator.RuleFor(x => x.Address).NotNull();
							});
					});
			});

#pragma warning disable 618
			var results = validator.Validate(new Person { Surname = "foo", Forename = "foo" }, v => v.IncludeRuleSets("MyRuleSet"));
#pragma warning restore 618
			results.Errors.Count.ShouldEqual(1);
			results.Errors.Single().PropertyName.ShouldEqual("Address");
		}

		[Fact]
		public void Nested_dependent_rules_inside_ruleset_no_result_when_top_level_fails()
		{
			var validator = new TestValidator();
			validator.RuleSet("MyRuleSet", () =>
			{

				validator.RuleFor(x => x.Surname).NotNull()
					.DependentRules(() =>
					{


						validator.RuleFor(x => x.Forename).NotNull()
							.DependentRules(() =>
							{
								validator.RuleFor(x => x.Address).NotNull();
							});
					});
			});

#pragma warning disable 618
			var results = validator.Validate(new Person { Surname = null, Forename = "foo" }, v => v.IncludeRuleSets("MyRuleSet"));
#pragma warning restore 618
			results.Errors.Count.ShouldEqual(1);
			results.Errors[0].PropertyName.ShouldEqual("Surname");
		}

		[Fact]
		public void Nested_dependent_rules_inside_ruleset_no_result_when_second_level_fails()
		{
			var validator = new TestValidator();
			validator.RuleSet("MyRuleSet", () =>
			{

				validator.RuleFor(x => x.Surname).NotNull()
					.DependentRules(() =>
					{


						validator.RuleFor(x => x.Forename).NotNull()
							.DependentRules(() =>
							{
								validator.RuleFor(x => x.Address).NotNull();
							});
					});
			});

#pragma warning disable 618
			var results = validator.Validate(new Person { Surname = "bar", Forename = null }, v => v.IncludeRuleSets("MyRuleSet"));
#pragma warning restore 618
			results.Errors.Count.ShouldEqual(1);
			results.Errors[0].PropertyName.ShouldEqual("Forename");
		}


		[Fact]
		public void Nested_dependent_rules_inside_ruleset_inside_method()
		{
			var validator = new TestValidator();
			validator.RuleSet("MyRuleSet", () =>
			{

				validator.RuleFor(x => x.Surname).NotNull()
					.DependentRules(() =>
					{
						validator.RuleFor(x => x.Forename).NotNull()
							.DependentRules(() =>
							{
								BaseValidation(validator);
							});
					});
			});

#pragma warning disable 618
			var results = validator.Validate(new Person { Surname = "foo", Forename = "foo" }, v => v.IncludeRuleSets("MyRuleSet"));
#pragma warning restore 618
			results.Errors.Count.ShouldEqual(1);
			results.Errors.Single().PropertyName.ShouldEqual("Address");
		}

		private void BaseValidation(InlineValidator<Person> validator)
		{
			validator.RuleFor(x => x.Address).NotNull();
		}

		class AsyncValidator : AbstractValidator<int> {
			public AsyncValidator() {
				RuleFor(model => model)
					.MustAsync(async (ie, ct) => {
						await Task.Delay(500);
						return true;
					})
					.DependentRules(() => {
						RuleFor(m => m)
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
					.DependentRules(() => {
						RuleFor(m => m)
							.MustAsync(async (ie, ct) => {
								await Task.Delay(1000);
								return false;
							});
					});
			}
		}
	}
}
