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
	using Xunit;

	
	public class CustomFailureActionTester {
		TestValidator validator;
		public CustomFailureActionTester() {
			validator = new TestValidator();
		}

		[Fact]
		public void Invokes_custom_action_on_failure() {
			bool invoked = false;
			validator.RuleFor(x => x.Surname).NotNull().OnAnyFailure(x => {
				invoked = true;
			});

			validator.Validate(new Person());

			invoked.ShouldBeTrue();
		}

		[Fact]
		public void Passes_object_being_validated_to_action() {
			var person = new Person();
			Person validatedPerson = null;

			validator.RuleFor(x => x.Surname).NotNull().OnAnyFailure(x => {
				validatedPerson = x;
			});

			validator.Validate(person);

			person.ShouldBeTheSameAs(validatedPerson);
		}

		[Fact]
		public void Does_not_invoke_action_if_validation_success() {
			bool invoked = false;
			validator.RuleFor(x => x.Surname).NotNull().OnAnyFailure(x => {
				invoked=true;
			});
			validator.Validate(new Person() { Surname = "foo" });
			invoked.ShouldBeFalse();
		}
	}
}