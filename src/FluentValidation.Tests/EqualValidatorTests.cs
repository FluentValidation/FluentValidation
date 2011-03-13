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
			var validator = new TestValidator { v => v.RuleFor(x => x.Forename).Equal("Foo") };
			var result = validator.Validate(new Person { Forename = "Foo"});

			result.IsValid.ShouldBeTrue();
		}

		[Test]
		public void When_the_objects_are_not_equal_validation_should_fail() {
			var validator = new TestValidator { v => v.RuleFor(x => x.Forename).Equal("Foo") };
			var result = validator.Validate(new Person() { Forename = "Bar" });

			result.IsValid.ShouldBeFalse();
		}

		[Test]
		public void When_validation_fails_the_error_should_be_set() {
			var validator = new TestValidator { v => v.RuleFor(x => x.Forename).Equal("Foo") };
			var result = validator.Validate(new Person() {Forename = "Bar"});
			
			result.Errors.Single().ErrorMessage.ShouldEqual("'Forename' should be equal to 'Foo'.");
		}

		[Test]
		public void Should_store_property_to_compare() {
			var validator = new TestValidator { v => v.RuleFor(x => x.Forename).Equal(x => x.Surname) };
			var descriptor = validator.CreateDescriptor();
			var propertyValidator = descriptor.GetValidatorsForMember("Forename").Cast<EqualValidator>().Single();

			propertyValidator.MemberToCompare.ShouldEqual(typeof(Person).GetProperty("Surname"));
		}

		[Test]
		public void Should_store_comparison_type() {
			var validator = new TestValidator { v => v.RuleFor(x => x.Surname).Equal("Foo") };
			var descriptor = validator.CreateDescriptor();
			var propertyValidator = descriptor.GetValidatorsForMember("Surname").Cast<EqualValidator>().Single();

			propertyValidator.Comparison.ShouldEqual(Comparison.Equal);
		}

		[Test]
		public void Validates_against_property() {
			var validator = new TestValidator { v => v.RuleFor(x => x.Surname).Equal(x => x.Forename) };
			var result = validator.Validate(new Person { Surname = "foo", Forename = "foo" });
			result.IsValid.ShouldBeTrue();
		}

		[Test]
		public void Should_succeed_on_case_insensitive_comparison() {
			var validator = new TestValidator { v => v.RuleFor(x => x.Surname).Equal("FOO", StringComparer.OrdinalIgnoreCase) };
			var result = validator.Validate(new Person { Surname = "foo" });

			result.IsValid.ShouldBeTrue();
		}

		[Test]
		public void Should_succeed_on_case_insensitive_comparison_using_expression() {
			var validator = new TestValidator { v => v.RuleFor(x => x.Surname).Equal(x => x.Forename, StringComparer.OrdinalIgnoreCase) };
			var result = validator.Validate(new Person { Surname = "foo", Forename = "FOO"});

			result.IsValid.ShouldBeTrue();
		}
	}
}