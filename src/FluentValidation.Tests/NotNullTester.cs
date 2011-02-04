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
	using System.Threading;
	using NUnit.Framework;
	using Validators;

	[TestFixture]
	public class NotNullTester {
		[SetUp]
		public void Setup() {
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
		}

		[Test]
		public void NotNullValidator_should_pass_if_value_has_value() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Surname).NotNull());
			var result = validator.Validate(new Person{Surname = "Foo"});
			result.IsValid.ShouldBeTrue();
		}

		[Test]
		public void NotNullValidator_should_fail_if_value_is_null() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Surname).NotNull());
			var result = validator.Validate(new Person { Surname = null });
			result.IsValid.ShouldBeFalse();
		}

		[Test]
		public void When_the_validator_fails_the_error_message_should_be_set() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Surname).NotNull());
			var result = validator.Validate(new Person { Surname = null });
			result.Errors.Single().ErrorMessage.ShouldEqual("'Surname' must not be empty.");
		}

		[Test]
		public void Not_null_validator_should_not_crash_with_non_nullable_value_type() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Id).NotNull());
			var result = validator.Validate(new Person());
			result.IsValid.ShouldBeTrue();
		}

		[Test]
		public void Fails_when_nullable_value_type_is_null() {
			var validator = new TestValidator(v => v.RuleFor(x => x.NullableInt).NotNull());
			var result = validator.Validate(new Person());
			result.IsValid.ShouldBeFalse();
		}
	}
}