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
	using System.Linq;
	using NUnit.Framework;

	[TestFixture]
	public class UserStateTester {
		TestValidator validator;

		[SetUp]
		public void Setup() {
			validator = new TestValidator();
		}

		[Test]
		public void Stores_user_state_against_validation_failure() {
			validator.RuleFor(x => x.Surname).NotNull().WithState(x =>  "foo");
			var result = validator.Validate(new Person());
			result.Errors.Single().CustomState.ShouldEqual("foo");
		}

		[Test]
		public void Throws_when_provider_is_null() {
			typeof(ArgumentNullException).ShouldBeThrownBy(() => validator.RuleFor(x => x.Surname).NotNull().WithState(null));
		}

		[Test]
		public void Correctly_provides_object_being_validated() {
			Person resultPerson = null;

			validator.RuleFor(x => x.Surname).NotNull().WithState(x => {
				resultPerson = x;
				return new object();
			});

			var person = new Person();
			validator.Validate(person);

			resultPerson.ShouldBeTheSameAs(person);
		}
	}
}