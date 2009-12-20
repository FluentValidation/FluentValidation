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
	using NUnit.Framework;
	using TestHelper;

	[TestFixture]
	public class ValidatorTesterTester {
		private TestValidator validator;

		[SetUp]
		public void Setup() {
			validator = new TestValidator();
			validator.RuleFor(x => x.Forename).NotNull();
		}

		[Test]
		public void ShouldHaveValidationError_should_not_throw_when_there_are_validation_errors() {
			validator.ShouldHaveValidationErrorFor(x => x.Forename, (string)null);
		}

		[Test]
		public void ShouldHaveValidationError_Should_throw_when_there_are_no_validation_errors() {
			typeof(ValidationTestException).ShouldBeThrownBy(() => validator.ShouldHaveValidationErrorFor(x => x.Forename, "test"));
		}

		[Test]
		public void ShouldNotHaveValidationError_should_not_throw_when_there_are_no_errors() {
			validator.ShouldNotHaveValidationErrorFor(x => x.Forename, "test");
		}

		[Test]
		public void ShouldNotHaveValidationError_should_throw_when_there_are_errors() {
			typeof(ValidationTestException).ShouldBeThrownBy(() => validator.ShouldNotHaveValidationErrorFor(x => x.Forename, (string)null));
		}

		[Test]
		public void ShouldHaveValidationError_should_not_throw_when_there_are_errors_with_preconstructed_object() {
			validator.ShouldHaveValidationErrorFor(x => x.Forename, new Person { Forename = null });
		}

		[Test]
		public void ShouldHaveValidationError_should_throw_when_there_are_no_validation_errors_with_preconstructed_object() {
			typeof(ValidationTestException).ShouldBeThrownBy(() => validator.ShouldHaveValidationErrorFor(x => x.Forename, new Person { Forename = "test" }));
		}

		[Test]
		public void ShouldNotHAveValidationError_should_not_throw_When_there_are_no_errors_with_preconstructed_object() {
			validator.ShouldNotHaveValidationErrorFor(x => x.Forename, new Person { Forename = "test"});
		}

		[Test]
		public void ShouldNotHaveValidationError_should_throw_when_there_are_errors_with_preconstructed_object() {
			typeof(ValidationTestException).ShouldBeThrownBy(() => validator.ShouldNotHaveValidationErrorFor(x => x.Forename, new Person { Forename = null }));
		}
	}
}