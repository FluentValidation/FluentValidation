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
	public class NotEqualValidatorTests {
		[SetUp]
		public void Setup() {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
		}

		[Test]
		public void When_the_objects_are_equal_then_the_validator_should_fail() {
			var validator = CreateValidator(person => person.Forename);
			var result = validator.Validate(new PropertyValidatorContext(null, new Person {Forename = "Foo"}, x => "Foo"));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_the_objects_are_not_equal_then_the_validator_should_pass() {
			var validator = CreateValidator(person => person.Forename);
			var result = validator.Validate(new PropertyValidatorContext(null, new Person {Forename = "Foo"}, x => "Bar"));
			result.IsValid().ShouldBeTrue();
		}

		[Test]
		public void When_the_validator_fails_the_error_message_should_be_set() {
			var validator = CreateValidator(person => person.Forename);
			var result = validator.Validate(new PropertyValidatorContext("Forename", new Person {Forename = "Foo"}, x => "Foo"));
			result.Single().ErrorMessage.ShouldEqual("'Forename' should not be equal to 'Foo'.");
		}

		[Test]
		public void Should_store_property_to_compare() {
			var validator = CreateValidator(x => x.Surname);
			validator.MemberToCompare.ShouldEqual(typeof(Person).GetProperty("Surname"));
		}

		[Test]
		public void Should_store_comparison_type() {
			var validator = CreateValidator(x => x.Surname);
			validator.Comparison.ShouldEqual(Comparison.NotEqual);
		}

		[Test]
		public void Should_not_be_valid_for_case_insensitve_comparison() {
			var validator = CreateValidator(x => x.Surname, StringComparer.OrdinalIgnoreCase);
			var person = new Person { Surname = "foo" };
			var context = new PropertyValidatorContext("Surname", person, x => "FOO");

			var result = validator.Validate(context);
			result.IsValid().ShouldBeFalse();
		}


		[Test]
		public void Validates_against_constant() {
			var validator = new NotEqualValidator("foo");
			var result = validator.Validate(new PropertyValidatorContext(null, new Person(), x => "foo"));
			result.IsValid().ShouldBeFalse();
		}


		private NotEqualValidator CreateValidator<T>(Expression<PropertySelector<Person, T>> expression) {
			var func = expression.Compile();
			PropertySelector selector = x => func((Person)x);
			return new NotEqualValidator(selector, expression.GetMember());
		}

		private NotEqualValidator CreateValidator<T>(Expression<PropertySelector<Person, T>> expression, IEqualityComparer equalityComparer) {
			var func = expression.Compile();
			PropertySelector selector = x => func((Person)x);
			return new NotEqualValidator(selector, expression.GetMember(), equalityComparer);
		}
	}
}