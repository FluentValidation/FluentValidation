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
// The latest version of this file can be found at https://github.com/JeremySkinner/FluentValidation
#endregion
namespace FluentValidation.Tests {
	using System.Linq;
	using Validators;
	using Xunit;

	public class ModelLevelValidatorTests {

		public ModelLevelValidatorTests() {
			CultureScope.SetDefaultCulture();
		}

		[Fact]
		public void Validates_at_model_level() {
			var v = new TestValidator();
			v.RuleFor(x => x).Must(x => false);

			var result = v.Validate(new Person());
			result.Errors.Count.ShouldEqual(1);
			result.Errors.Single().PropertyName.ShouldEqual("");
		}

		[Fact]
		public void Can_use_child_validator_at_model_level() {
			var v = new TestValidator();
			v.RuleFor(x => x).SetValidator(new ChildValidator());

			var result = v.Validate(new Person());
			result.Errors.Count.ShouldEqual(1);
			result.Errors.Single().PropertyName.ShouldEqual("Forename");

		}

		private class ChildValidator : AbstractValidator<Person> {
			public ChildValidator() {
				RuleFor(x => x.Forename).NotNull();
			}
		}
	}
}