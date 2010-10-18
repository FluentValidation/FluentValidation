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
	using System.Globalization;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading;
	using Internal;
	using NUnit.Framework;
	using Validators;

	[TestFixture]
	public class GreaterThanValidatorTester {
		private GreaterThanValidator validator;
		private const int value = 1;

		[SetUp]
		public void Setup() {
			validator = new GreaterThanValidator(value);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
		}


		[Test]
		public void Should_fail_when_less_than_input() {
			var result = validator.Validate(new PropertyValidatorContext(null, null, x => 0));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void Should_succeed_when_greater_than_input() {
			var result = validator.Validate(new PropertyValidatorContext(null, null, x => 2));
			result.IsValid().ShouldBeTrue();
		}

		[Test]
		public void Should_fail_when_equal_to_input() {
			var result = validator.Validate(new PropertyValidatorContext(null, null, x => value));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void Should_set_default_error_when_validation_fails() {
			var result = validator.Validate(new PropertyValidatorContext("Discount", null, x => 0));
			result.Single().ErrorMessage.ShouldEqual("'Discount' must be greater than '1'.");
		}

		[Test]
		public void Validates_with_property() {
			validator = CreateValidator(x => x.Id);
			var result = validator.Validate(new PropertyValidatorContext(null, new Person { Id = 2 }, x => 1));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void Comparison_Type() {
			validator.Comparison.ShouldEqual(Comparison.GreaterThan);
		}

		private GreaterThanValidator CreateValidator<T>(Expression<PropertySelector<Person, T>> expression) {
			PropertySelector selector = x => expression.Compile()((Person)x);
			return new GreaterThanValidator(selector, expression.GetMember());
		}
	}
}