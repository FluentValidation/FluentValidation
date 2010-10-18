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
	using System.Globalization;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading;
	using Internal;
	using NUnit.Framework;
	using Validators;

	[TestFixture]
	public class LessThanValidatorTester {
		int value = 1;

		[SetUp]
		public void Setup() {
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
		}

		[Test]
		public void Should_fail_when_greater_than_input() {
			var validator = new LessThanValidator(value);
			var result = validator.Validate(new PropertyValidatorContext(null, null, x => 2));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void Should_succeed_when_less_than_input() {
			var validator = new LessThanValidator(value);
			var result = validator.Validate(new PropertyValidatorContext(null, null, x => 0));
			result.IsValid().ShouldBeTrue();
		}

		[Test]
		public void Should_fail_when_equal_to_input() {
			var validator = new LessThanValidator(value);
			var result = validator.Validate(new PropertyValidatorContext(null, null, x => value));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void Should_set_default_validation_message_when_validation_fails() {
			var validator = new LessThanValidator(value);
			var result = validator.Validate(new PropertyValidatorContext("Discount", new Person(), x => 2));
			result.Single().ErrorMessage.ShouldEqual("'Discount' must be less than '1'.");
		}

		[Test]
		public void Should_throw_when_value_to_compare_is_null() {
			var validator = new LessThanValidator(value);
			typeof(ArgumentNullException).ShouldBeThrownBy(() => new LessThanValidator(null));
		}

		[Test]
		public void Extracts_property_from_expression() {
			IComparisonValidator validator = CreateValidator(x => x.Id);
			validator.MemberToCompare.ShouldEqual(typeof(Person).GetProperty("Id"));
		}

		[Test]
		public void Extracts_property_from_constant_using_expression() {
			IComparisonValidator validator = new LessThanValidator(2);
			validator.ValueToCompare.ShouldEqual(2);
		}

		[Test]
		public void Validates_using_property() {
			var validator = CreateValidator(x => x.Id);;
			var result = validator.Validate(new PropertyValidatorContext(null, new Person() { Id = 1 }, x => 1));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void Comparison_type() {
			var validator = new LessThanValidator(1);
			validator.Comparison.ShouldEqual(Comparison.LessThan);
		}

		private LessThanValidator CreateValidator<T>(Expression<PropertySelector<Person, T>> expression) {
			PropertySelector selector = x => expression.Compile()((Person)x);
			return new LessThanValidator(selector, expression.GetMember());
		}
	}
}