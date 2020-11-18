namespace FluentValidation.Tests.AspNetCore {
	using System.Collections.Generic;
	using System.Net.Http;
	using System.Text;
	using System.Threading.Tasks;
	using Attributes;
	using Controllers;
	using FluentValidation.AspNetCore;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.DependencyInjection;
	using Newtonsoft.Json;
	using Xunit;
	using Xunit.Abstractions;

	public class MvcIntegrationTests : IClassFixture<WebAppFixture> {
		private readonly ITestOutputHelper _output;
		private readonly WebAppFixture _webApp;
		private readonly HttpClient _client;

		public MvcIntegrationTests(ITestOutputHelper output, WebAppFixture webApp) {
			CultureScope.SetDefaultCulture();

			_output = output;
			_webApp = webApp;
			_client = webApp
				.WithFluentValidation(fv => {
					fv.ValidatorFactoryType = typeof(AttributedValidatorFactory);
					fv.ImplicitlyValidateChildProperties = true;
				})
				.CreateClient();
		}

		[Fact]
		public async Task Should_add_all_errors_in_one_go() {
			var form = new FormData {
				{"Email", "foo"},
				{"Surname", "foo"},
				{"Forename", "foo"},
				{"DateOfBirth", null},
				{"Address1", null}
			};

			var result = await _client.GetErrors("Test4", form);

			result.IsValidField("Email").ShouldBeFalse(); //Email validation failed
			result.IsValidField("DateOfBirth").ShouldBeFalse(); //Date of Birth not specified (implicit required error)
			result.IsValidField("Surname").ShouldBeFalse(); //cross-property
		}


		[Fact]
		public async Task Should_add_all_erorrs_in_one_go_when_NotEmpty_rule_specified_for_non_nullable_value_type() {
			var form = new FormData {
				{"SomeBool", "False"},
				{"Id", "0"}
			};

			var result = await _client.GetErrors("Test5b", form);
			result.IsValidField("SomeBool").ShouldBeFalse(); //Complex rule
			result.IsValidField("Id").ShouldBeFalse(); //NotEmpty for non-nullable value type
		}

		[Fact]
		public async Task When_a_validation_error_occurs_the_error_should_be_added_to_modelstate() {
			var form = new FormData {
				{"test.Name", null}
			};

			var result = await _client.GetErrors("Test1", form);

			result.IsValidField("test.Name").ShouldBeFalse();
			result.GetError("test.Name").ShouldEqual("Validation Failed");
		}

		[Fact]
		public async Task When_a_validation_error_occurs_the_error_should_be_added_to_modelstate_using_TryUpdateModel() {
			var form = new FormData {
				{"test.Name", null}
			};

			var result = await _client.GetErrors("UpdateModel", form);

			result.IsValidField("Name").ShouldBeFalse();
			result.GetError("Name").ShouldEqual("Validation Failed");
		}


		[Fact]
		public async Task When_a_validation_error_occurs_the_error_should_be_added_to_Modelstate_without_prefix() {
			var form = new FormData {
				{"Name", null}
			};

			var result = await _client.GetErrors("Test1a", form);
			result.GetError("Name").ShouldEqual("Validation Failed");
		}

		[Fact]
		public async Task Should_not_fail_when_no_validator_can_be_found() {
			var result = await _client.PostResponse("/Test/Test2", new FormData());
			result.ShouldEqual("not null");
		}

		[Fact]
		public async Task Should_not_add_default_message_to_modelstate() {
			var form = new FormData {
				{"Id", ""}
			};

			var errors = await _client.GetErrors("Test3", form);
			errors.Count.ShouldEqual(1);
			errors.GetError("Id").ShouldEqual("Validation failed");
		}

		[Fact]
		public async Task Should_not_add_default_message_to_modelstate_prefix() {
			var form = new FormData {
				{"test.Id", ""}
			};

			var errors = await _client.GetErrors("Test3", form);

			errors.Count.ShouldEqual(1);
			errors.GetError("test.Id").ShouldEqual("Validation failed");
		}

		[Fact]
		public async Task Should_not_add_default_message_to_modelstate_not_specified() {
			var form = new FormData {
			};

			var errors = await _client.GetErrors("Test3", form);

			errors.GetError("Id").ShouldEqual("Validation failed");
		}

		[Fact]
		public async Task Should_add_default_message_to_modelstate_when_there_is_no_required_validator_explicitly_specified() {
			var form = new FormData {
				{"Id", ""}
			};

			var result = await _client.GetErrors("Test6", form);
			result.GetError("Id").ShouldEqual("The value '' is invalid.");
		}

		[Fact]
		public async Task Should_add_Default_message_to_modelstate_when_no_validator_specified() {
			var form = new FormData {
				{"Id", ""}
			};

			var result = await _client.GetErrors("WithoutValidator", form);
			result.GetError("Id").ShouldEqual("The value '' is invalid.");
		}

		[Fact]
		public async Task Allows_override_of_required_message_for_non_nullable_value_types() {
			var form = new FormData {
				{"Id", ""}
			};

			var errors = await _client.GetErrors("TestModelWithOverridenMessageValueType", form);
			errors.GetError("Id").ShouldEqual("Foo");
		}

		[Fact]
		public async Task Allows_override_of_required_property_name_for_non_nullable_value_types() {
			var form = new FormData {
				{"Id", ""}
			};
			var errors = await _client.GetErrors("TestModelWithOverridenPropertyNameValueType", form);
			errors.GetError("Id").ShouldEqual("'Foo' must not be empty.");
		}

		[Fact]
		public async Task Should_only_validate_specified_ruleset() {
			var form = new FormData {
				{"Email", "foo"},
				{"Surname", "foo"},
				{"Forename", "foo"},
			};

			var results = await _client.GetErrors("RulesetTest", form);
			results.IsValidField("Forename").ShouldBeFalse();
			results.IsValidField("Surname").ShouldBeFalse();
			results.IsValidField("Email").ShouldBeTrue();
		}

		[Fact]
		public async Task Should_only_validate_specified_properties() {
			var form = new FormData {
				{"Email", "foo"},
				{"Surname", "foo"},
				{"Forename", "foo"},
			};

			var result = await _client.GetErrors("PropertyTest", form);

			result.IsValidField("Forename").ShouldBeFalse();
			result.IsValidField("Surname").ShouldBeFalse();
			result.IsValidField("Email").ShouldBeTrue();
		}

		[Fact]
		public async Task When_interceptor_specified_Intercepts_validation() {
			var form = new FormData {
				{"Email", "foo"},
				{"Surname", "foo"},
				{"Forename", "foo"},
			};
			var result = await _client.GetErrors("InterceptorTest", form);

			result.IsValidField("Forename").ShouldBeFalse();
			result.IsValidField("Surname").ShouldBeFalse();
			result.IsValidField("Email").ShouldBeTrue();
		}

		[Fact]
		public async Task When_action_context_interceptor_specified_Intercepts_validation() {
			var form = new FormData {
				{"Email", "foo"},
				{"Surname", "foo"},
				{"Forename", "foo"},
			};
			var result = await _client.GetErrors("ActionContextInterceptorTest", form);

			result.IsValidField("Forename").ShouldBeFalse();
			result.IsValidField("Surname").ShouldBeFalse();
			result.IsValidField("Email").ShouldBeTrue();
		}

		[Fact]
		public async Task When_global_interceptor_specified_Intercepts_validation_for_razor_pages() {
			var form = new FormData {
				{"Email", "foo"},
				{"Surname", "foo"},
				{"Forename", "foo"},
			};
			var client = _webApp
				.WithFluentValidation(fv => {
					fv.ValidatorFactoryType = typeof(AttributedValidatorFactory);
					fv.ImplicitlyValidateChildProperties = true;
				})
				.WithWebHostBuilder(builder => builder.ConfigureServices(
					services => services.AddSingleton<IValidatorInterceptor, SimplePropertyInterceptor>())
				)
				.CreateClient();

			// IValidatorInterceptor won't be called and shouldn't throw.
			var response = await client.PostResponse($"/RulesetTest", form);
		}

		[Fact]
		public async Task When_global_action_context_interceptor_specified_Intercepts_validation_for_razor_pages() {
			var form = new FormData {
				{"Email", "foo"},
				{"Surname", "foo"},
				{"Forename", "foo"},
			};
			var client = _webApp
				.WithFluentValidation(fv => {
					fv.ValidatorFactoryType = typeof(AttributedValidatorFactory);
					fv.ImplicitlyValidateChildProperties = true;
				})
				.WithWebHostBuilder(builder => builder.ConfigureServices(
#pragma warning disable 618
#pragma warning disable 612
						services => services.AddSingleton<IActionContextValidatorInterceptor, SimpleActionContextPropertyInterceptor>())
#pragma warning restore 612
#pragma warning restore 618
				)
				.CreateClient();
			var response = await client.PostResponse($"/RulesetTest", form);
			var result = JsonConvert.DeserializeObject<List<SimpleError>>(response);

			result.IsValidField("Forename").ShouldBeFalse();
			result.IsValidField("Surname").ShouldBeFalse();
			result.IsValidField("Email").ShouldBeTrue();
		}

		[Fact]
		public async Task When_interceptor_specified_Intercepts_validation_provides_custom_errors() {
			var form = new FormData {
				{"Email", "foo"},
				{"Surname", "foo"},
				{"Forename", "foo"},
			};

			var result = await _client.GetErrors("ClearErrorsInterceptorTest", form);

			result.Count.ShouldEqual(0);
		}

		[Fact]
		public async Task When_validator_implements_IValidatorInterceptor_directly_interceptor_invoked() {
			var form = new FormData {
				{"Email", "foo"},
				{"Surname", "foo"},
				{"Forename", "foo"},
			};

			var result = await _client.GetErrors("BuiltInInterceptorTest", form);

			result.Count.ShouldEqual(0);
		}

		[Fact]
		public async Task Validator_customizations_should_only_apply_to_single_parameter() {
			var form = new FormData {
				{"first.Email", "foo"},
				{"first.Surname", "foo"},
				{"first.Forename", "foo"},
				{"second.Email", "foo"},
				{"second.Surname", "foo"},
				{"second.Forename", "foo"}
			};

			var result = await _client.GetErrors("TwoParameters", form);

			//customizations should only apply to the first validator
			result.IsValidField("first.Forename").ShouldBeFalse();
			result.IsValidField("first.Surname").ShouldBeFalse();
			result.IsValidField("second.Forename").ShouldBeTrue();
			result.IsValidField("second.Surname").ShouldBeTrue();
		}

		[Fact]
		public async Task Validates_collection() {
			var form = new FormData {
				{"model[0].Name", "foo"},
				{"model[1].Name", "foo"},
			};

			var result = await _client.GetErrors("Collection", form);

			result.Count.ShouldEqual(2);
			result[0].Name.ShouldEqual("model[0].Name");
		}

		[Fact]
		public async Task Validates_collection_without_prefix() {
			var form = new FormData {
				{"[0].Name", "foo"},
				{"[1].Name", "foo"},
			};

			var result = await _client.GetErrors("Collection", form);

			result.Count.ShouldEqual(2);
			result[0].Name.ShouldEqual("[0].Name");
		}


		[Fact]
		public async Task Returns_multiple_errors_for_same_property() {
			var form = new FormData() {
				{"model.Name", "baz"}
			};

			var result = await _client.GetErrors("MultipleErrors", form);
			_output.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
			result.Count.ShouldEqual(2);
		}

		[Fact]
		public async Task Uses_both_dataannotations_and_fv_in_same_model() {
			var result = await _client.GetErrors("MultipleValidationStrategies", new FormData());
			_output.WriteLine(JsonConvert.SerializeObject(result));
			result.Count.ShouldEqual(2);
		}

		[Fact]
		public async Task Uses_both_dataannotations_and_fv_on_same_property() {
			var result = await _client.GetErrors("MultipleValidationStrategies2", new FormData());
			result.Count.ShouldEqual(2);
		}

		[Fact]
		public async void Mixes_DataAnnotations_with_FV_on_explicitly_set_child_validator() {
			var result = await _client.GetErrors("MultipleValidationStrategies3", new FormData());
			_output.WriteLine(JsonConvert.SerializeObject(result));
			result.Count.ShouldEqual(3);
		}


		[Fact]
		public async Task Does_not_use_both_dataannotations_and_fv_in_same_model_when_MVC_val_disabled() {
			var client = _webApp.WithDataAnnotationsDisabled().CreateClient();
			var result = await client.GetErrors("MultipleValidationStrategies", new FormData());
			result.Count.ShouldEqual(1);
			result[0].Message.ShouldEqual("'Some Other Property' must not be empty.");
		}

		[Fact]
		public async Task Uses_DataAnnotations_when_no_FV_validatior_defined() {
			var result = await _client.GetErrors("DataAnnotations", new FormData());
			result.Count.ShouldEqual(1);
			result[0].Message.ShouldEqual("The Name field is required.");
		}

		[Fact]
		public async void Does_not_implicitly_run_child_validator_when_disabled() {
			var client = _webApp.WithImplicitValidationEnabled(false).CreateClient();
			var result = await client.GetErrors("ImplicitChildValidator", new FormData());
			result.Count.ShouldEqual(0);
		}

		[Fact]
		public async void Executes_implicit_child_validator_when_enabled() {
			var result = await _client.GetErrors("ImplicitChildValidator", new FormData());
			result.Count.ShouldEqual(1);
			result[0].Name.ShouldEqual("Child.Name");
		}

		[Fact]
		public async void Ignores_null_child() {
			var result = await _client.GetErrors("ImplicitChildValidatorWithNullChild", new FormData());
			result.Count.ShouldEqual(0);
		}

		[Fact]
		public async void Does_not_implicitly_run_root_collection_element_validator_when_disabled() {
			var client = _webApp.WithImplicitCollectionValidationEnabled(false).CreateClient();
			var result = await client.GetErrorsViaJSON(
				nameof(TestController.ImplicitRootCollectionElementValidator),
				new[] { new ChildModel() });

			result.Count.ShouldEqual(0);
		}

		[Fact]
		public async void Does_not_implicitly_run_child_validator_when_root_collection_element_validation_enabled() {
			var client = _webApp.WithImplicitCollectionValidationEnabled(true).CreateClient();
			var result = await client.GetErrorsViaJSON(
				nameof(TestController.ImplicitRootCollectionElementValidationEnabled),
				new ParentModel());

			result.Count.ShouldEqual(0);
		}

		[Fact]
		public async void Executes_implicit_root_collection_element_validator_when_enabled() {
			var client = _webApp.WithImplicitCollectionValidationEnabled(true).CreateClient();
			var result = await client.GetErrorsViaJSON(
				nameof(TestController.ImplicitRootCollectionElementValidator),
				new[] { new ChildModel() });

			result.Count.ShouldEqual(1);
			result[0].Name.ShouldEqual("[0].Name");
		}

		[Fact]
		public async void Can_mix_FV_with_IValidatableObject() {
			var result = await _client.GetErrors("ImplementsIValidatableObject", new FormData());
			_output.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
			result.Count.ShouldEqual(2);
		}


		[Fact]
		public async void Executes_implicit_child_validator_and_mixes_with_DataAnnotations() {
			var result = await _client.GetErrors("ImplicitChildWithDataAnnotations", new FormData());
			result.Count.ShouldEqual(2);
		}

		[Fact]
		public async void Executes_implicit_child_validator_and_mixes_with_IValidatableObject() {
			var result = await _client.GetErrors("ImplicitChildImplementsIValidatableObject", new FormData());
			_output.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));

			result.Count.ShouldEqual(3);
		}


		[Fact]
		public async void Executes_implicit_child_validator_when_enabled_does_not_execute_multiple_times() {
			var result = await _client.GetErrors("ImplicitChildValidator", new FormData());
			result.Count.ShouldEqual(1);

			result = await _client.GetErrors("ImplicitChildValidator", new FormData());
			result.Count.ShouldEqual(1);
		}


		[Fact]
		public async void ImplicitValidation_enabled_but_validator_explicitly_only_includes_error_message_once() {
			var result = await _client.GetErrors("ImplicitAndExplicitChildValidator", new FormData());
			_output.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
			result.Count.ShouldEqual(1);
		}

		[Fact]
		public async void Validates_dictionary_with_prefix() {
			var form = new FormData {
				{"model[0].Key", "0"},
				{"model[0].Value.Name", null},

				{"model[1].Key", "1"},
				{"model[1].Value.Name", null},

				{"model[2].Key", "2"},
				{"model[2].Value.Name", "boop"}
			};
			var result = await _client.GetErrors("DictionaryParameter", form);
			_output.WriteLine(JsonConvert.SerializeObject(result));

			result.Count.ShouldEqual(2);
		}

		[Fact]
		public async void Validates_dictionary_without_prefix() {
			var form = new FormData {
				{"[0].Name", null},
				{"[1].Name", null},
				{"[2].Name", "whoop"},
			};
			var result = await _client.GetErrors("DictionaryParameter", form);
			_output.WriteLine(JsonConvert.SerializeObject(result));

			result.Count.ShouldEqual(2);
		}


		[Fact]
		public async void Can_validate_using_JSON() {
			var result = await _client.GetErrorsViaJSON("Test5", new TestModel5());
			result.IsValidField("SomeBool").ShouldBeFalse();
			result.Count.ShouldEqual(2);
		}

		[Fact]
		public async void Can_validate_enumerable() {
			var list = new List<TestModel5>() {
				new TestModel5() {SomeBool = true, Id = 1},
				new TestModel5(),
				new TestModel5() {SomeBool = true}
			};

			var result = await _client.GetErrorsViaJSON("UsingEnumerable", list);

			result.IsValidField("[1].Id").ShouldBeFalse();
			result.IsValidField("[1].SomeBool").ShouldBeFalse();
			result.IsValidField("[2].Id").ShouldBeFalse();
			result.Count.ShouldEqual(3);
		}

		[Fact]
		public async void Can_validate_dictionary() {
			var dictionary = new Dictionary<int, TestModel5>() {
				{123, new TestModel5() {SomeBool = true, Id = 1}},
				{456, new TestModel5()}
			};
			var result = await _client.GetErrorsViaJSON("UsingDictionaryWithJsonBody", dictionary);
			result.Count.ShouldEqual(2);
			result.IsValidField("[1].Value.Id").ShouldBeFalse();
			result.IsValidField("[1].Value.SomeBool").ShouldBeFalse();
		}

		[Fact]
		public async Task Skips_validation() {
			var results = await _client.GetErrors("SkipsValidation", new FormData());
			results.Count.ShouldEqual(0);
		}

		[Fact]
		public async void Skips_implicit_child_validation() {
			var result = await _client.GetErrors("SkipsImplicitChildValidator", new FormData());
			result.Count.ShouldEqual(0);
		}

		[Fact]
		public async void Does_not_implicitly_validate_child_collections_by_default() {
			var client = _webApp.WithImplicitValidationEnabled(false).CreateClient();
			var result = await client.GetErrorsViaJSONRaw("ImplicitChildCollection", @"{ Children: [ { Name: null } ] }");
			result.Count.ShouldEqual(0);
		}

		[Fact]
		public async void Does_implicitly_validate_child_collections_by_default_with_DataAnnotations() {
			var client = _webApp.WithImplicitValidationEnabled(false).CreateClient();
			var result = await client.GetErrorsViaJSONRaw("ImplicitChildCollectionDataAnnotations", @"{ Children: [ { Name: null } ] }");
			result.Count.ShouldEqual(1);
		}


		[Fact]
		public async void When_skipping_children_does_not_leave_validation_state_unvalidated() {
			var client = _webApp.WithImplicitValidationEnabled(false).CreateClient();
			string json = @"{ Children: [ { Name: null } ] }";

			var request = new HttpRequestMessage(HttpMethod.Post, $"/Test/CheckUnvalidated");
			request.Content = new StringContent(json, Encoding.UTF8, "application/json");
			var responseMessage = await client.SendAsync(request);
			responseMessage.EnsureSuccessStatusCode();
			var response = await responseMessage.Content.ReadAsStringAsync();
			response.ShouldEqual("0");
		}

	}
}
