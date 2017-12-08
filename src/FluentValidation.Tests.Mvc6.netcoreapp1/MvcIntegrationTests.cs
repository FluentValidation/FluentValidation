namespace FluentValidation.Tests.AspNetCore {
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Controllers;
	using Newtonsoft.Json;
	using Xunit;
	using Xunit.Abstractions;

	public class MvcIntegrationTests : IClassFixture<WebAppFixture<Startup>> {
		private readonly ITestOutputHelper _output;
		private readonly WebAppFixture<Startup> _webApp;

		public MvcIntegrationTests(ITestOutputHelper output, WebAppFixture<Startup> webApp) {
			CultureScope.SetDefaultCulture();

			this._output = output;
			this._webApp = webApp;
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

			var result = await _webApp.GetErrors("Test4", form);

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

			var result = await _webApp.GetErrors("Test5", form);
			result.IsValidField("SomeBool").ShouldBeFalse(); //Complex rule
			result.IsValidField("Id").ShouldBeFalse(); //NotEmpty for non-nullable value type
		}

		[Fact]
		public async Task When_a_validation_error_occurs_the_error_should_be_added_to_modelstate() {
			var form = new FormData {
				{"test.Name", null}
			};

			var result = await _webApp.GetErrors("Test1", form);

			result.IsValidField("test.Name").ShouldBeFalse();
			result.GetError("test.Name").ShouldEqual("Validation Failed");
		}

		[Fact]
		public async Task When_a_validation_error_occurs_the_error_should_be_added_to_Modelstate_without_prefix() {
			var form = new FormData {
				{"Name", null}
			};

			var result = await _webApp.GetErrors("Test1a", form);
			result.GetError("Name").ShouldEqual("Validation Failed");
		}

		[Fact]
		public async Task Should_not_fail_when_no_validator_can_be_found() {
			var result = await _webApp.PostResponse("/Test/Test2", new FormData());
			result.ShouldEqual("not null");
		}

		[Fact]
		public async Task Should_not_add_default_message_to_modelstate() {
			var form = new FormData {
				{"Id", ""}
			};

			var errors = await _webApp.GetErrors("Test3", form);
			errors.Count.ShouldEqual(1);
			errors.GetError("Id").ShouldEqual("Validation failed");
		}

		[Fact]
		public async Task Should_not_add_default_message_to_modelstate_prefix() {
			var form = new FormData {
				{"test.Id", ""}
			};

			var errors = await _webApp.GetErrors("Test3", form);

			errors.Count.ShouldEqual(1);
			errors.GetError("test.Id").ShouldEqual("Validation failed");
		}

		[Fact]
		public async Task Should_not_add_default_message_to_modelstate_not_specified() {
			var form = new FormData {
			};

			var errors = await _webApp.GetErrors("Test3", form);

			errors.GetError("Id").ShouldEqual("Validation failed");
		}

		[Fact]
		public async Task Should_add_default_message_to_modelstate_when_there_is_no_required_validator_explicitly_specified() {
			var form = new FormData {
				{"Id", ""}
			};

			var result = await _webApp.GetErrors("Test6", form);
			result.GetError("Id").ShouldEqual("The value '' is invalid.");
		}

		[Fact]
		public async Task Should_add_Default_message_to_modelstate_when_no_validator_specified() {
			var form = new FormData {
				{"Id", ""}
			};

			var result = await _webApp.GetErrors("WithoutValidator", form);
			result.GetError("Id").ShouldEqual("The value '' is invalid.");
		}

		[Fact]
		public async Task Allows_override_of_required_message_for_non_nullable_value_types() {
			var form = new FormData {
				{"Id", ""}
			};

			var errors = await _webApp.GetErrors("TestModelWithOverridenMessageValueType", form);
			errors.GetError("Id").ShouldEqual("Foo");
		}

		[Fact]
		public async Task Allows_override_of_required_property_name_for_non_nullable_value_types() {
			var form = new FormData {
				{"Id", ""}
			};
			var errors = await _webApp.GetErrors("TestModelWithOverridenPropertyNameValueType", form);
			errors.GetError("Id").ShouldEqual("'Foo' should not be empty.");
		}
//
//	    [Fact]
//	    public void Falls_back_to_default_behaviou() {
//		    
//	    }

		/*  [Fact]
		  public async Task Should_add_default_message_to_modelstate_when_both_fv_and_DataAnnotations_have_implicit_required_validation_disabled()
		  {
		      DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
		      provider.AddImplicitRequiredValidator = false;
  
		      var form = new FormData {
		          { "Id", "" }
		      };
  
		      var bindingContext = new ModelBindingContext
		      {
		          ModelName = "test",
		          ModelMetadata = CreateMetaData(typeof(TestModelWithoutValidator)),
		          ModelState = new ModelStateDictionary(),
		          FallbackToEmptyPrefix = true,
		          ValueProvider = form.ToValueProvider()
		      };
  
		      binder.BindModel(controllerContext, bindingContext);
  
		      bindingContext.ModelState["Id"].Errors.Single().ErrorMessage.ShouldEqual("A value is required.");
  
  
		      provider.AddImplicitRequiredValidator = true;
		      DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = true;
		  }
  */
		[Fact]
		public async Task Should_only_validate_specified_ruleset() {
			var form = new FormData {
				{"Email", "foo"},
				{"Surname", "foo"},
				{"Forename", "foo"},
			};

			var results = await _webApp.GetErrors("RulesetTest", form);
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

			var result = await _webApp.GetErrors("PropertyTest", form);

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
			var result = await _webApp.GetErrors("InterceptorTest", form);

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

			var result = await _webApp.GetErrors("ClearErrorsInterceptorTest", form);

			result.Count.ShouldEqual(0);
		}

		[Fact]
		public async Task When_validator_implements_IValidatorInterceptor_directly_interceptor_invoked() {
			var form = new FormData {
				{"Email", "foo"},
				{"Surname", "foo"},
				{"Forename", "foo"},
			};

			var result = await _webApp.GetErrors("BuiltInInterceptorTest", form);

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

			var result = await _webApp.GetErrors("TwoParameters", form);

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

			var result = await _webApp.GetErrors("Collection", form);

			result.Count.ShouldEqual(2);
			result[0].Name.ShouldEqual("model[0].Name");
		}

		[Fact]
		public async Task Validates_collection_without_prefix() {
			var form = new FormData {
				{"[0].Name", "foo"},
				{"[1].Name", "foo"},
			};

			var result = await _webApp.GetErrors("Collection", form);

			result.Count.ShouldEqual(2);
			result[0].Name.ShouldEqual("[0].Name");
		}


		[Fact]
		public async Task Returns_multiple_errors_for_same_property() {
			var form = new FormData() {
				{"model.Name", "baz"}
			};

			var result = await _webApp.GetErrors("MultipleErrors", form);
			_output.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
			result.Count.ShouldEqual(2);
		}

		[Fact]
		public async Task Uses_both_dataannotations_and_fv_in_same_model() {
			var result = await _webApp.GetErrors("MultipleValidationStrategies", new FormData());
			_output.WriteLine(JsonConvert.SerializeObject(result));
			result.Count.ShouldEqual(2);
		}

		[Fact]
		public async Task Uses_both_dataannotations_and_fv_on_same_property() {
			var result = await _webApp.GetErrors("MultipleValidationStrategies2", new FormData());
			result.Count.ShouldEqual(2);
		}

		[Fact]
		public async void Mixes_DataAnnotations_with_FV_on_explicitly_set_child_validator() {
			var result = await _webApp.GetErrors("MultipleValidationStrategies3", new FormData());
			_output.WriteLine(JsonConvert.SerializeObject(result));
			result.Count.ShouldEqual(3);
		}


		[Fact]
		public async Task Does_not_use_both_dataannotations_and_fv_in_same_model_when_MVC_val_disabled() {
			var app = new WebAppFixture<StartupWithMvcValidationDisabled>();

			var result = await app.GetErrors("MultipleValidationStrategies", new FormData());
			result.Count.ShouldEqual(1);
			result[0].Message.ShouldEqual("'Some Other Property' must not be empty.");
		}

		[Fact]
		public async Task Uses_DataAnnotations_when_no_FV_validatior_defined() {
			var result = await _webApp.GetErrors("DataAnnotations", new FormData());
			result.Count.ShouldEqual(1);
			result[0].Message.ShouldEqual("The Name field is required.");
		}

		[Fact]
		public async void Does_not_implicitly_run_child_validator_when_disabled() {
			var webApp = new WebAppFixture<StartupWithImplicitValidationDisabled>();
			var result = await webApp.GetErrors("ImplicitChildValidator", new FormData());
			result.Count.ShouldEqual(0);
		}

		[Fact]
		public async void Executes_implicit_child_validator_when_enabled() {
			var result = await _webApp.GetErrors("ImplicitChildValidator", new FormData());
			result.Count.ShouldEqual(1);
			result[0].Name.ShouldEqual("Child.Name");
		}

		[Fact]
		public async void Ignores_null_child() {
			var result = await _webApp.GetErrors("ImplicitChildValidatorWithNullChild", new FormData());
			result.Count.ShouldEqual(0);
		}

		[Fact]
		public async void Can_mix_FV_with_IValidatableObject() {
			var result = await _webApp.GetErrors("ImplementsIValidatableObject", new FormData());
			_output.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
			result.Count.ShouldEqual(2);
		}


		[Fact]
		public async void Executes_implicit_child_validator_and_mixes_with_DataAnnotations() {
//			var app = new WebAppFixture<StartupWithImplicitValidationEnabled>();

			var result = await _webApp.GetErrors("ImplicitChildWithDataAnnotations", new FormData());
			result.Count.ShouldEqual(2);
		}

		[Fact]
		public async void Executes_implicit_child_validator_and_mixes_with_IValidatableObject() {
//			var app = new WebAppFixture<StartupWithImplicitValidationEnabled>();

			var result = await _webApp.GetErrors("ImplicitChildImplementsIValidatableObject", new FormData());
			_output.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));

			result.Count.ShouldEqual(3);
		}


		[Fact]
		public async void Executes_implicit_child_validator_when_enabled_does_not_execute_multiple_times() {
//			var app = new WebAppFixture<StartupWithImplicitValidationEnabled>();

			var result = await _webApp.GetErrors("ImplicitChildValidator", new FormData());
			result.Count.ShouldEqual(1);

			result = await _webApp.GetErrors("ImplicitChildValidator", new FormData());
			result.Count.ShouldEqual(1);
		}


		[Fact]
		public async void ImplicitValidation_enabled_but_validator_explicitly_only_includes_error_message_once() {
//			var app = new WebAppFixture<StartupWithImplicitValidationEnabled>();

			var result = await _webApp.GetErrors("ImplicitAndExplicitChildValidator", new FormData());
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
			var result = await _webApp.GetErrors("DictionaryParameter", form);
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
			var result = await _webApp.GetErrors("DictionaryParameter", form);
			_output.WriteLine(JsonConvert.SerializeObject(result));

			result.Count.ShouldEqual(2);
		}


		[Fact]
		public async void Can_validate_using_JSON() {
			var result = await _webApp.GetErrorsViaJSON("Test5", new TestModel5());
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

			var result = await _webApp.GetErrorsViaJSON("UsingEnumerable", list);

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
			var result = await _webApp.GetErrorsViaJSON("UsingDictionaryWithJsonBody", dictionary);
			result.Count.ShouldEqual(2);
			result.IsValidField("[1].Value.Id").ShouldBeFalse();
			result.IsValidField("[1].Value.SomeBool").ShouldBeFalse();
		}

		[Fact]
		public async Task Skips_validation() {
			var results = await _webApp.GetErrors("SkipsValidation", new FormData());
			results.Count.ShouldEqual(0);
		}

		[Fact]
		public async void Skips_implicit_child_validation() {
			var result = await _webApp.GetErrors("SkipsImplicitChildValidator", new FormData());
			result.Count.ShouldEqual(0);
		}


	}
}