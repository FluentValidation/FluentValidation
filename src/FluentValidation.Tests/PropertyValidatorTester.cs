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
	using Xunit;
	using Validators;
	using System.Linq;

	
	public class PropertyValidatorTester {
		[Fact]
		public void When_passing_string_to_localizable_lambda_should_convert_to_string_accessor() {
			var validator = new TestValidator() {
				v => v.RuleFor(x => x.Surname).SetValidator(new FooValidator())
			};

			var result = validator.Validate(new Person());
			result.Errors.Single().ErrorMessage.ShouldEqual("foo");
		}

		private class FooValidator:PropertyValidator {
#pragma warning disable 612
			public FooValidator() : base(() => "foo") {
#pragma warning restore 612
				
			}

			protected override bool IsValid(PropertyValidatorContext context) {
				return false;
			}
		}
	}
}