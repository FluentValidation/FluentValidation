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

	[TestFixture]
	public class ValidateAndThrowTester {
		[SetUp]
		public void SetUp() {
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
		}

		[Test]
		public void Throws_exception() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull()
			};

			typeof(ValidationException).ShouldBeThrownBy(() => validator.ValidateAndThrow(new Person()));
		}

		[Test]
		public void Does_not_throw_when_valid() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull()
			};

			validator.ValidateAndThrow(new Person {Surname = "foo"});
		}

		[Test]
		public void Populates_errors() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull()
			};

			var ex = (ValidationException)typeof(ValidationException).ShouldBeThrownBy(() => validator.ValidateAndThrow(new Person()));
			ex.Errors.Count().ShouldEqual(1);
		}

		[Test]
		public void ToString_provides_error_details() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull(),
				v => v.RuleFor(x => x.Forename).NotNull()
			};

			var ex = typeof(ValidationException).ShouldBeThrownBy(() => validator.ValidateAndThrow(new Person()));
			ex.ToString().ShouldStartWith("FluentValidation.ValidationException: Validation failed: \r\n -- 'Surname' must not be empty.\r\n -- 'Forename' must not be empty.");
		}
	}
}