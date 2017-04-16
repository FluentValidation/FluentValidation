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
	using Validators;
	using Xunit;

	
	public class ForEachRuleTests {
		[Fact]
		public void Executes_rule_for_each_item_in_collection() {
			var validator = new TestValidator {
				v => v.RuleForEach(x => x.NickNames).NotNull()
			};

			var person = new Person {
				NickNames =  new[] { null, "foo", null }
			};

			var result = validator.Validate(person);
			result.Errors.Count.ShouldEqual(2);
		}

		[Fact]
		public void Correctly_gets_collection_indicies() {
			var validator = new TestValidator {
				v => v.RuleForEach(x => x.NickNames).NotNull()
			};

			var person = new Person {
				NickNames = new[] { null, "foo", null }
			};

			var result = validator.Validate(person);
			result.Errors[0].PropertyName.ShouldEqual("NickNames[0]");
			result.Errors[1].PropertyName.ShouldEqual("NickNames[2]");
		}

		[Fact]
		public void Executes_rule_for_each_item_in_collection_async()
		{
			var validator = new TestValidator {
				v => v.RuleForEach(x => x.NickNames).SetValidator(new MyAsyncNotNullValidator())
			};

			var person = new Person
			{
				NickNames = new[] { null, "foo", null }
			};

			var result = validator.ValidateAsync(person).Result;
			result.Errors.Count.ShouldEqual(2);
		}

		[Fact]
		public void Correctly_gets_collection_indicies_async()
		{
			var validator = new TestValidator {
				v => v.RuleForEach(x => x.NickNames).SetValidator(new MyAsyncNotNullValidator())
			};

			var person = new Person
			{
				NickNames = new[] { null, "foo", null }
			};

			var result = validator.ValidateAsync(person).Result;
			result.Errors[0].PropertyName.ShouldEqual("NickNames[0]");
			result.Errors[1].PropertyName.ShouldEqual("NickNames[2]");
		}

		class request {
			public Person person = null;
		}

		private class MyAsyncNotNullValidator : NotNullValidator {
			public override bool IsAsync { get { return true; } }
		}

		[Fact]
		public void Nested_collection_for_null_property_should_not_throw_null_reference() {
			var validator = new InlineValidator<request>();
			validator.When(r => r.person != null, () => {
				validator.RuleForEach(x => x.person.NickNames).NotNull();
			});

			var result = validator.Validate(new request());
			result.Errors.Count.ShouldEqual(0);
		}
	}
}