#region License
// Copyright (c) .NET Foundation and contributors.
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
// The latest version of this file can be found at https://github.com/FluentValidation/FluentValidation
#endregion

namespace FluentValidation.Tests {
	using System.Linq;
	using System.Threading.Tasks;
	using Validators;
	using Xunit;

#pragma warning disable 618

	public class LegacyPropertyValidatorTests {
		[Fact]
		public void Invokes_custom_validator() {
			var validator = new InlineValidator<Person>();
			validator.RuleFor(x => x.Forename).SetValidator(new LegacyNotNullValidator());
			var result = validator.Validate(new Person());
			result.Errors.Single().ErrorMessage.ShouldEqual("A value is required for Forename.");
		}

		[Fact]
		public async Task Invokes_custom_validator_async() {
			var validator = new InlineValidator<Person>();
			validator.RuleFor(x => x.Forename).SetValidator(new LegacyNotNullValidator());
			var result = await validator.ValidateAsync(new Person());
			result.Errors.Single().ErrorMessage.ShouldEqual("A value is required for Forename.");
		}

		[Fact]
		public void Gets_validator_from_metadata() {
			var validator = new InlineValidator<Person>();
			var legacyPropertyValidator = new LegacyNotNullValidator();
			validator.RuleFor(x => x.Forename).SetValidator(legacyPropertyValidator);
			validator.Single().Components.Single().Validator.ShouldBeTheSameAs(legacyPropertyValidator);
		}

		[Fact]
		public void Gets_validator_from_metadata_nullable_struct() {
			var validator = new InlineValidator<Person>();
			var legacyPropertyValidator = new LegacyNotNullValidator();
			validator.RuleFor(x => x.NullableInt).SetValidator(legacyPropertyValidator);
			validator.Single().Components.Single().Validator.ShouldBeTheSameAs(legacyPropertyValidator);
		}


		public class LegacyNotNullValidator : PropertyValidator {

			protected override bool IsValid(PropertyValidatorContext context) {
				return context.PropertyValue != null;
			}

			protected override string GetDefaultMessageTemplate() => "A value is required for {PropertyName}.";
		}
	}
}
