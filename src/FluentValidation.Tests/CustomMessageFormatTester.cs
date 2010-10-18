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
	using System.Linq;
	using NUnit.Framework;

	[TestFixture]
	public class CustomMessageFormatTester {
		private TestValidator validator;

		[SetUp]
		public void Setup() {
			validator = new TestValidator();
		}

		[Test]
		public void Should_format_custom_message() {
			const string expected = "Surname";
			validator.RuleFor(x => x.Surname).NotNull().WithMessage("{PropertyName}");
			string error = validator.Validate(new Person()).Errors.Single().ErrorMessage;
			error.ShouldEqual(expected);
        }

		[Test]
		public void Should_format_validation_message_with_custom_args() {
			const string expected = "Property Name: Surname Custom: One, Custom: Two";
			validator.RuleFor(x => x.Surname).NotNull().WithMessage("Property Name: {PropertyName} Custom: {0}, Custom: {1}", "One", "Two");
			string error = validator.Validate(new Person()).Errors.Single().ErrorMessage;
			error.ShouldEqual(expected);
		}

		[Test]
		public void Should_format_validation_with_property_values() {
			const string expected = "Property Name: Surname Custom: Foo";
			validator.RuleFor(x => x.Surname).NotNull().WithMessage("Property Name: {PropertyName} Custom: {0}", x => x.Forename);
			var person = new Person { Forename = "Foo" };

			string error = validator.Validate(person).Errors.Single().ErrorMessage;
			error.ShouldEqual(expected);
		}
	}
}