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
	using System.Globalization;
	using System.Linq;
	using System.Threading;
	using Xunit;
	using Validators;


	public class ValidatorDescriptorTester {
		TestValidator validator;

		public ValidatorDescriptorTester() {
			CultureScope.SetDefaultCulture();
			validator = new TestValidator();
		}

		[Fact]
		public void Should_retrieve_name_given_to_it_pass_property_as_string() {
			validator.RuleFor(x => x.Forename).NotNull().WithName("First Name");
			var descriptor = validator.CreateDescriptor();
			var name = descriptor.GetName("Forename");
			name.ShouldEqual("First Name");
		}

		[Fact]
		public void Gets_validators_for_property() {
			validator.RuleFor(x => x.Forename).NotNull();
			var descriptor = validator.CreateDescriptor();
			var validators = descriptor.GetValidatorsForMember("Forename");
			Assert.IsType<NotNullValidator<Person, string>>(validators.Single().Validator);
		}

		[Fact]
		public void Returns_empty_collection_for_property_with_no_validators() {
			var descriptor = validator.CreateDescriptor();
			var validators = descriptor.GetValidatorsForMember("NoSuchProperty");
			validators.Count().ShouldEqual(0);
		}

		[Fact]
		public void Does_not_throw_when_rule_declared_without_property() {
			validator.RuleFor(x => x).NotNull();
			var descriptor = validator.CreateDescriptor();
			descriptor.GetValidatorsForMember("Surname");
		}

		[Fact]
		public void GetValidatorsForMember_and_GetRulesForMember_can_both_retrieve_for_model_level_rule() {
			validator.RuleFor(x => x).NotNull();
			var descriptor = validator.CreateDescriptor();

			descriptor.GetValidatorsForMember(null).Count().ShouldEqual(1);
			descriptor.GetRulesForMember(null).Count().ShouldEqual(1);
		}
	}
}
