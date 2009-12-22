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
	public class ExactLengthValidatorTester {

		[SetUp]
		public void Setup() {
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
		}

		[Test]
		public void When_the_text_is_an_exact_length_the_validator_should_pass() {
			string text = "test";
			var validator = new ExactLengthValidator<object>(4);
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => text));
			result.IsValid.ShouldBeTrue();
		}

		[Test]
		public void When_the_text_length_is_smaller_the_validator_should_fail() {
			string text = "test";
			var validator = new ExactLengthValidator<object>(10);
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => text));
			result.IsValid.ShouldBeFalse();
		}

		[Test]
		public void When_the_text_length_is_larger_the_validator_should_fail() {
			string text = "test";
			var validator = new ExactLengthValidator<object>(1);
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => text));
			result.IsValid.ShouldBeFalse();
		}

		[Test]
		public void When_the_validator_fails_the_error_message_should_be_set() {
			var validator = new ExactLengthValidator<object>(2);
			var result =
				validator.Validate(new PropertyValidatorContext("Forename", null, x => "Gire and gimble in the wabe"));
			result.Error.ShouldEqual("'Forename' must be 2 characters in length. You entered 27 characters.");
		}

		[Test]
		public void Min_and_max_properties_should_be_set() {
			var validator = new ExactLengthValidator<object>(5);
			validator.Min.ShouldEqual(5);
			validator.Max.ShouldEqual(5);
		}
	}
}