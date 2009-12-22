#region License
// Copyright 2008-2009 Jeremy Skinner (http://www.jeremyskinner.co.uk)
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
	using System.Threading;
	using NUnit.Framework;
	using Validators;

	[TestFixture]
	public class EqualValidatorTests {
		EqualValidator<Person, string> validator;

		[SetUp]
		public void Setup() {
			validator = new EqualValidator<Person, string>(x => x.Forename);
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
		}

		[Test]
		public void When_the_objects_are_equal_validation_should_succeed() {
			var person = new Person { Forename = "Foo"};
			var result = validator.Validate(new PropertyValidatorContext(null, person, x => "Foo", null, null));
			result.IsValid.ShouldBeTrue();
		}

		[Test]
		public void When_the_objects_are_not_equal_validation_should_fail() {
			var person = new Person() { Forename = "Bar" };
			var result = validator.Validate(new PropertyValidatorContext(null, person, x => "Foo", null, null));
			result.IsValid.ShouldBeFalse();
		}

		[Test]
		public void When_validation_fails_the_error_should_be_set() {
			var person = new Person() {Forename = "Bar"};
			var result = validator.Validate(new PropertyValidatorContext("Forename", person, x => "Foo", null, null));
			result.Error.ShouldEqual("'Forename' should be equal to 'Bar'.");
		}

		[Test]
		public void Should_store_property_to_compare() {
			var validator = new EqualValidator<Person, string>(x => x.Surname);
			validator.MemberToCompare.ShouldEqual(typeof(Person).GetProperty("Surname"));
		}

		[Test]
		public void Should_store_comparison_type() {
			var validator = new EqualValidator<Person, string>(x => x.Surname);
			validator.Comparison.ShouldEqual(Comparison.Equal);
		}

		[Test]
		public void Extracts_property_from_constant() {
			IComparisonValidator validator = new EqualValidator<Person, int>(x => 2);
			validator.ValueToCompare.ShouldEqual(2);
		}

		[Test]
		public void Should_succeed_on_case_insensitive_comparison() {
			var person = new Person { Surname = "foo" };
			var validator = new EqualValidator<Person, string>(x => x.Surname, StringComparer.OrdinalIgnoreCase);
			var context = new PropertyValidatorContext("Surname", person, x => "FOO", null, null);

			var result = validator.Validate(context);
			result.IsValid.ShouldBeTrue();
		}
	}
}