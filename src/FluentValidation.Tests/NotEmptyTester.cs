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
	using System.Linq;
	using System.Threading;
	using NUnit.Framework;
	using Validators;

	[TestFixture]
	public class NotEmptyTester {
		[SetUp]
		public void Setup() {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
		}

		[Test]
		public void When_there_is_a_value_then_the_validator_should_pass() {
			var validator = new NotEmptyValidator(default(string));
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => "Farf"));
			result.IsValid().ShouldBeTrue();
		}

		[Test]
		public void When_value_is_null_validator_should_fail() {
			var validator = new NotEmptyValidator(default(string));
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => null));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_value_is_empty_string_validator_should_fail() {
			var validator = new NotEmptyValidator(default(string));
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => ""));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_value_is_whitespace_validation_should_fail() {
			var validator = new NotEmptyValidator(default(string));
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => "           "));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_value_is_Default_for_type_validator_should_fail_datetime() {
			var defaultValue = default(DateTime);
			var validator = new NotEmptyValidator(defaultValue);
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => defaultValue));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_value_is_Default_for_type_validator_should_fail_int() {
			var validator = new NotEmptyValidator(default(int));
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => 0));
			result.IsValid().ShouldBeFalse();

			var result1 = validator.Validate(new PropertyValidatorContext(null, new object(), x => 1));
			result1.IsValid().ShouldBeTrue();
		}

		[Test]
		public void When_validation_fails_error_should_be_set() {
			var validator = new NotEmptyValidator(default(string));
			var result = validator.Validate(new PropertyValidatorContext("name", null, x => null));
			result.Single().ErrorMessage.ShouldEqual("'name' should not be empty.");
		}
	}
}