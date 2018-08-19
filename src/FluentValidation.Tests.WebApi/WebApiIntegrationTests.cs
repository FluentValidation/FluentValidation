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

namespace FluentValidation.Tests.WebApi {
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Xunit;
	using Xunit.Abstractions;

	public class WebApiIntegrationTests : IClassFixture<WebApiFixture<Startup>>, IClassFixture<WebApiFixture<StartupRootValidationOnly>> {

		private readonly ITestOutputHelper _output;
		private readonly WebApiFixture<Startup> _webApp;
		private readonly WebApiFixture<StartupRootValidationOnly> _webAppRootValidationOnly;


		public WebApiIntegrationTests(ITestOutputHelper output, WebApiFixture<Startup> webApp, WebApiFixture<StartupRootValidationOnly> webAppRootValidationOnly) {
			_output = output;
			_webApp = webApp;
			_webAppRootValidationOnly = webAppRootValidationOnly;
		}

		[Fact]
		public async Task Should_add_all_errors_in_one_go_when_NotEmpty_rule_specified_for_non_nullable_value_type() {
			var result = await _webApp.InvokeTest<TestModel5>(@"{
				SomeBool:'false',
				Id:0}");

			result.IsValidField("model.SomeBool").ShouldBeFalse(); //Complex rule
			result.IsValidField("model.Id").ShouldBeFalse(); //NotEmpty for non-nullable value type
		}

		[Fact]
		public async Task Should_add_all_errors_in_one_go() {
			var result = await _webApp.InvokeTest<TestModel4>(@"Email=foo&Surname=foo&Forename=foo&DateOfBirth=&Address1=");

			result.Count.ShouldEqual(4);
			result.IsValidField("model.Email").ShouldBeFalse(); //Email validation failed
			result.IsValidField("model.DateOfBirth").ShouldBeFalse(); //Date of Birth not specified (implicit required error)
			result.IsValidField("model.Surname").ShouldBeFalse(); //cross-property
			result.IsValidField("model.Address1").ShouldBeFalse();
		}

		[Fact]
		public async Task When_a_validation_error_occurs_the_error_should_be_added_to_modelstate() {
			var result = await _webApp.InvokeTest<TestModel>(@"Name=");
			result.GetMessage("model.Name").ShouldEqual("Validation Failed");
		}

		[Fact]
		public async Task Should_not_fail_when_no_validator_can_be_found() {
			var result = await _webApp.InvokeTest<TestModel2>(@"");
			result.IsValid().ShouldBeTrue();
		}

		[Fact]
		public async Task Should_add_default_message_to_modelstate_when_there_is_no_required_validator_explicitly_specified() {
			var result = await _webApp.InvokeTest<TestModel6>(@"Id=");
			result.Count.ShouldEqual(1);
			result.IsValidField("model.Id").ShouldBeFalse();
			result.GetMessage("model.Id").ShouldEqual(@"A value is required.");
		}


		[Fact]
		public async Task Should_add_implicit_required_validator() {
			var result = await _webApp.InvokeTest<TestModel6>(@"Id=");
			result.Count.ShouldEqual(1);
			result.IsValidField("model.Id").ShouldBeFalse();
			result.GetMessage("model.Id").ShouldEqual(@"A value is required.");
		}


		[Fact]
		public async Task Should_validate_less_than() {
			 var result = await _webApp.InvokeTest<TestModel7>(@"AnIntProperty=15");
			result.IsValidField("model.AnIntProperty").ShouldBeFalse();
			result.GetMessage("model.AnIntProperty").ShouldEqual("Less than 10");
		}

		[Fact]
		public async Task Should_validate_custom_after_property_errors() {
			var result = await _webApp.InvokeTest<TestModel7>(@"AnIntProperty=7&CustomProperty=14");

			result.IsValidField("model.CustomProperty").ShouldBeFalse();
			result.GetMessage("model.CustomProperty").ShouldEqual("Cannot be 14");

		}

        [Fact]
	    public async Task Should_still_validate_other_properties_when_error_found_in_collection() {
	        var result = await _webApp.InvokeTest<TestModel9>(@"{Children:[{}]}", "application/json");

            result.IsValidField("model.Name").ShouldBeFalse();
            result.IsValidField("model.Children[0].Name").ShouldBeFalse();
	    }

        [Fact]
        public async Task Should_validate_child_properties()
        {
            var result = await _webApp.InvokeTest<TestModel10>(@"{Child: {}}", "application/json");

            result.IsValidField("model.Child.Name").ShouldBeFalse();
        }


		[Fact]
		public async Task Should_only_validate_specified_ruleset() {
			var form = new Dictionary<string,string> {
				{"Email", "foo"},
				{"Surname", "foo"},
				{"Forename", "foo"},
			};


			var results =  await _webApp.PostForm("/api/Test/RulesetTestModel", form);
			results.IsValidField("model.Forename").ShouldBeFalse();
			results.IsValidField("model.Surname").ShouldBeFalse();
			results.IsValidField("model.Email").ShouldBeTrue();
		}
		[Fact]
		public async Task Should_only_validate_specified_properties() {
			var form = new Dictionary<string,string> {
				{"Email", "foo"},
				{"Surname", "foo"},
				{"Forename", "foo"},
			};

			var result = await _webApp.PostForm("/api/Test/PropertiesTestModel", form);

			result.IsValidField("model.Forename").ShouldBeFalse();
			result.IsValidField("model.Surname").ShouldBeFalse();
			result.IsValidField("model.Email").ShouldBeTrue();
		}

		[Fact]
		public  async Task When_interceptor_specified_Intercepts_validation() {
			var form = new Dictionary<string,string> {
				{"Email", "foo"},
				{"Surname", "foo"},
				{"Forename", "foo"},
			};
			var result = await _webApp.PostForm("/api/Test/InterceptorTest", form);

			result.IsValidField("model.Forename").ShouldBeFalse();
			result.IsValidField("model.Surname").ShouldBeFalse();
			result.IsValidField("model.Email").ShouldBeTrue();
		}

		[Fact]
		public async Task When_interceptor_specified_Intercepts_validation_provides_custom_errors() {
			var form = new Dictionary<string,string> {
				{"Email", "foo"},
				{"Surname", "foo"},
				{"Forename", "foo"},
			};

			var result = await _webApp.PostForm("/api/Test/ClearErrorsInterceptorTest", form);

			result.Count.ShouldEqual(0);
		}

		[Fact]
		public async Task When_validator_implements_IValidatorInterceptor_directly_interceptor_invoked() {
			var form = new Dictionary<string,string> {
				{"Email", "foo"},
				{"Surname", "foo"},
				{"Forename", "foo"},
			};

			var result = await _webApp.PostForm("/api/Test/BuiltInInterceptorTest", form);

			result.Count.ShouldEqual(0);
		}

		/*[Fact]
		public async Task Validator_customizations_should_only_apply_to_single_parameter() {
			var form = new Dictionary<string,string> {
				{"first.Email", "foo"},
				{"first.Surname", "foo"},
				{"first.Forename", "foo"},
				{"second.Email", "foo"},
				{"second.Surname", "foo"},
				{"second.Forename", "foo"}
			};

			var result = await _webApp.PostForm("/api/Test/TwoParameters", form);

			//customizations should only apply to the first validator
			result.IsValidField("first.Forename").ShouldBeFalse();
			result.IsValidField("first.Surname").ShouldBeFalse();
			result.IsValidField("second.Forename").ShouldBeTrue();
			result.IsValidField("second.Surname").ShouldBeTrue();
		}*/

		[Fact]
		public async Task Should_only_call_root_validator() {
			var result = await _webAppRootValidationOnly.InvokeTest<TestModel11>(@"{Child: {}}", "application/json");

			result.IsValidField("model.Name").ShouldBeFalse();
			result.IsValidField("model.Child.Name").ShouldBeTrue();
		}

		[Fact]
		public async Task Should_only_call_children_validators() {
			var result = await _webApp.InvokeTest<TestModel11>(@"{Child: {}}", "application/json");

			result.IsValidField("model.Name").ShouldBeFalse();
			result.IsValidField("model.Child.Name").ShouldBeFalse();
		}
    }
}