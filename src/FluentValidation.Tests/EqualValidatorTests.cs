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
	using System.Collections;
	using System.Globalization;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading;
	using Internal;
	using NUnit.Framework;
	using Validators;

	[TestFixture]
	public class EqualValidatorTests {

		[SetUp]
		public void Setup() {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
		}

		[Test]
		public void When_the_objects_are_equal_validation_should_succeed() {
			var person = new Person { Forename = "Foo"};
			var validator = CreateValidator(x => x.Forename);
			var result = validator.Validate(new PropertyValidatorContext(null, person, x => "Foo"));
			result.IsValid().ShouldBeTrue();
		}

		[Test]
		public void When_the_objects_are_not_equal_validation_should_fail() {
			var person = new Person() { Forename = "Bar" };
			var validator = CreateValidator(x => x.Forename);
			var result = validator.Validate(new PropertyValidatorContext(null, person, x => "Foo"));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_validation_fails_the_error_should_be_set() {
			var person = new Person() {Forename = "Bar"};
			var validator = CreateValidator(x => x.Forename);
			var result = validator.Validate(new PropertyValidatorContext("Forename", person, x => "Foo"));
			result.Single().ErrorMessage.ShouldEqual("'Forename' should be equal to 'Bar'.");
		}

		[Test]
		public void Should_store_property_to_compare() {
			var validator = CreateValidator(x => x.Surname);
			validator.MemberToCompare.ShouldEqual(typeof(Person).GetProperty("Surname"));
		}

		[Test]
		public void Should_store_comparison_type() {
			var validator = CreateValidator(x => x.Surname);
			validator.Comparison.ShouldEqual(Comparison.Equal);
		}

		[Test]
		public void Validates_against_constant() {
			var validator = new EqualValidator("foo");
			var result = validator.Validate(new PropertyValidatorContext(null, new Person(), x => "bar"));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void Should_succeed_on_case_insensitive_comparison() {
			var person = new Person { Surname = "foo" };
			var validator = CreateValidator(x => x.Surname, StringComparer.OrdinalIgnoreCase);
			var context = new PropertyValidatorContext("Surname", person, x => "FOO");

			var result = validator.Validate(context);
			result.IsValid().ShouldBeTrue();
		}

		private EqualValidator CreateValidator<T>(Expression<PropertySelector<Person, T>> expression) {
			var func = expression.Compile();
			PropertySelector selector = x => func((Person)x);
			return new EqualValidator(selector, expression.GetMember());
		}

		private EqualValidator CreateValidator<T>(Expression<PropertySelector<Person, T>> expression, IEqualityComparer equalityComparer) {
			var func = expression.Compile();
			PropertySelector selector = x => func((Person)x);
			return new EqualValidator(selector, expression.GetMember(), equalityComparer);
		}
	}
}