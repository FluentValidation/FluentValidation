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
	using System.Threading;
	using NUnit.Framework;
	using Validators;

	[TestFixture]
	public class PredicateValidatorTester {
		private PredicateValidator validator;

		[SetUp]
		public void Setup() {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
			validator = new PredicateValidator((person, forename) => forename == "Jeremy");
		}

		[Test]
		public void Should_fail_when_predicate_returns_false() {
			var result = validator.Validate(new PropertyValidatorContext(null, null, x => "Foo"));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void Should_succeed_when_predicate_returns_true() {
			var result = validator.Validate(new PropertyValidatorContext(null, null, x => "Jeremy"));
			result.IsValid().ShouldBeTrue();
		}

		[Test]
		public void Should_throw_when_predicate_is_null() {
			typeof(ArgumentNullException).ShouldBeThrownBy(() => new PredicateValidator(null));
		}

		[Test]
		public void When_validation_fails_the_default_error_should_be_set() {
			var result = validator.Validate(new PropertyValidatorContext("Name", null, x => "Foo"));
			result.Single().ErrorMessage.ShouldEqual("The specified condition was not met for 'Name'.");
		}
	}
}