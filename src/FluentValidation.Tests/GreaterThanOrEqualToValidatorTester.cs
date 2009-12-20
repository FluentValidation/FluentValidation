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
	using System.Globalization;
	using System.Threading;
	using NUnit.Framework;
	using Validators;

	[TestFixture]
	public class GreaterThanOrEqualToValidatorTester {
		private GreaterThanOrEqualValidator<Person, int> validator;
		private const int value = 1;

		[SetUp]
		public void Setup() {
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
			validator = new GreaterThanOrEqualValidator<Person, int>(x => value);
		}

		[Test]
		public void Should_fail_when_less_than_input() {
			var result = validator.Validate(new PropertyValidatorContext<Person, int>(null, null, x => 0));
			result.IsValid.ShouldBeFalse();
		}

		[Test]
		public void Should_succeed_when_greater_than_input() {
			var result = validator.Validate(new PropertyValidatorContext<Person, int>(null, null, x => 2));
			result.IsValid.ShouldBeTrue();
		}

		[Test]
		public void Should_succeed_when_equal_to_input() {
			var result = validator.Validate(new PropertyValidatorContext<Person, int>(null, null, x => value));
			result.IsValid.ShouldBeTrue();
		}

		[Test]
		public void Should_set_default_error_when_validation_fails() {
			var result = validator.Validate(new PropertyValidatorContext<Person, int>("Discount", null, x => 0));
			result.Error.ShouldEqual("'Discount' must be greater than or equal to '1'.");
		}

		[Test]
		public void Comparison_type() {
			validator.Comparison.ShouldEqual(Comparison.GreaterThanOrEqual);
		}
	}
}