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
// The latest version of this file can be found at http://fluentvalidation.codeplex.com
#endregion
namespace FluentValidation.Tests {
	using NUnit.Framework;

	[TestFixture]
	public class ForEachRuleTests {
		[Test]
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

		[Test]
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
	}
}