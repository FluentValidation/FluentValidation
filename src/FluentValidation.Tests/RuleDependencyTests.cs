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
	}
}