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
// The latest version of this file can be found at https://github.com/jeremyskinner/FluentValidation
#endregion

namespace FluentValidation.Tests {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Xunit;

	
	public class UserStateTester {
		TestValidator validator;

		
		public  UserStateTester() {
			validator = new TestValidator();
		}

		[Fact]
		public void Stores_user_state_against_validation_failure() {
			validator.RuleFor(x => x.Surname).NotNull().WithState(x =>  "foo");
			var result = validator.Validate(new Person());
			result.Errors.Single().CustomState.ShouldEqual("foo");
		}

		[Fact]
		public void Throws_when_provider_is_null() {
			typeof(ArgumentNullException).ShouldBeThrownBy(() => validator.RuleFor(x => x.Surname).NotNull().WithState((Func<Person, object>) null));
		}

		[Fact]
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

		[Fact]
		public void Can_Provide_state_for_item_in_collection() {
			validator.RuleForEach(x => x.Children).NotNull().WithState((person, child) => "test");
			var result = validator.Validate(new Person {Children = new List<Person> {null}});
			result.Errors[0].CustomState.ToString().ShouldEqual("test");
		}
	}
}