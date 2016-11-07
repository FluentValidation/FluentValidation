namespace FluentValidation.Tests.AspNetCore {
	using System;
	using System.Collections.Generic;
	using System.Net.Http;
	using System.Threading.Tasks;
	using Controllers;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.TestHost;
	using Newtonsoft.Json;
	using Xunit;
	using Xunit.Abstractions;

	public class MvcIntegrationTests {
        private readonly TestServer _server;
        private readonly HttpClient _client;
		private readonly ITestOutputHelper _output;

		public MvcIntegrationTests(ITestOutputHelper output)
        {
			this._output = output;
			_server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        private async Task<string> GetResponse(string url,
            string querystring = "")
        {
            if (!String.IsNullOrEmpty(querystring))
            {
                url += "?" + querystring;
            }

            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<string> PostResponse(string url,
            Dictionary<string,string> form) {

            var c = new FormUrlEncodedContent(form);

            var response = await _client.PostAsync(url, c);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private async Task<List<SimpleError>> GetErrors(string action, Dictionary<string,string> form) {

            var response = await PostResponse($"/Test/{action}", form);
            return JsonConvert.DeserializeObject<List<SimpleError>>(response);
        }

        [Fact]
		public async Task Should_add_all_errors_in_one_go() {
			var form = new FormData {
				{ "Email", "foo" },
				{ "Surname", "foo" },
				{ "Forename", "foo" },
				{ "DateOfBirth", null },
				{ "Address1", null }
			};

	        var result = await GetErrors("Test4", form);

			result.IsValidField("Email").ShouldBeFalse(); //Email validation failed
			result.IsValidField("DateOfBirth").ShouldBeFalse(); //Date of Birth not specified (implicit required error)
			result.IsValidField("Surname").ShouldBeFalse(); //cross-property
		}

       
        [Fact]
        public async Task Should_add_all_erorrs_in_one_go_when_NotEmpty_rule_specified_for_non_nullable_value_type()
        {
            var form = new FormData {
                { "SomeBool", "False" },
                { "Id", "0" }
            };

            var result = await GetErrors("Test5", form);
            result.IsValidField("SomeBool").ShouldBeFalse(); //Complex rule
            result.IsValidField("Id").ShouldBeFalse(); //NotEmpty for non-nullable value type
        }

        [Fact]
        public async Task When_a_validation_error_occurs_the_error_should_be_added_to_modelstate()
        {
            var form = new FormData {
                { "test.Name", null }
            };

            var result = await GetErrors("Test1", form);
            
            result.IsValidField("test.Name").ShouldBeFalse();
            result.GetError("test.Name").ShouldEqual("Validation Failed");
        }

        [Fact]
        public async Task When_a_validation_error_occurs_the_error_should_be_added_to_Modelstate_without_prefix()
        {
            var form = new FormData {
                { "Name", null }
            };

            var result = await GetErrors("Test1a", form);
            result.GetError("Name").ShouldEqual("Validation Failed");
        }

        [Fact]
        public async Task Should_not_fail_when_no_validator_can_be_found() {
	        var result = await PostResponse("/Test/Test2", new FormData());
	        result.ShouldEqual("not null");
        }

        [Fact]
        public async Task Should_not_add_default_message_to_modelstate()
        {
            var form = new FormData {
                { "Id", "" }
            };

            var errors = await GetErrors("Test3", form);
	        errors.Count.ShouldEqual(1);
            errors.GetError("Id").ShouldEqual("Validation failed");
            
        }

        [Fact]
        public async Task Should_not_add_default_message_to_modelstate_prefix()
        {
            var form = new FormData {
                { "test.Id", "" }
            };

            var errors = await GetErrors("Test3", form);

	        errors.Count.ShouldEqual(1);
            errors.GetError("test.Id").ShouldEqual("Validation failed");
		}

		[Fact]
        public async Task Should_not_add_default_message_to_modelstate_not_specified()
        {
            var form = new FormData {
            };

            var errors = await GetErrors("Test3", form);

            errors.GetError("Id").ShouldEqual("Validation failed");
        }

        [Fact]
        public async Task Should_add_default_message_to_modelstate_when_there_is_no_required_validator_explicitly_specified()
        {
            var form = new FormData {
                { "Id", "" }
            };

            var result = await GetErrors("Test6", form);
            result.GetError("Id").ShouldEqual("The value '' is invalid.");
        }

        [Fact]
        public async Task Should_add_Default_message_to_modelstate_when_no_validator_specified()
        {
            var form = new FormData {
                { "Id", "" }
            };

            var result = await GetErrors("WithoutValidator", form);
            result.GetError("Id").ShouldEqual("The value '' is invalid.");
        }

        [Fact]
        public async Task Allows_override_of_required_message_for_non_nullable_value_types()
        {
            var form = new FormData {
                { "Id", "" }
            };

            var errors = await GetErrors("TestModelWithOverridenMessageValueType", form);
            errors.GetError("Id").ShouldEqual("Foo");
        }

        [Fact]
        public async Task Allows_override_of_required_property_name_for_non_nullable_value_types()
        {
            var form = new FormData {
                { "Id", "" }
            };
            var errors = await GetErrors("TestModelWithOverridenPropertyNameValueType", form);
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
        public async Task Should_only_validate_specified_ruleset()
        {
            var form = new FormData {
                { "Email", "foo" },
                { "Surname", "foo" },
                { "Forename", "foo" },
            };

	        var results = await GetErrors("RulesetTest", form);
            results.IsValidField("Forename").ShouldBeFalse();
            results.IsValidField("Surname").ShouldBeFalse();
            results.IsValidField("Email").ShouldBeTrue();
        }

        [Fact]
        public async Task Should_only_validate_specified_properties()
        {
            var form = new FormData {
                { "Email", "foo" },
                { "Surname", "foo" },
                { "Forename", "foo" },
            };

	        var result = await GetErrors("PropertyTest", form);

			result.IsValidField("Forename").ShouldBeFalse();
            result.IsValidField("Surname").ShouldBeFalse();
            result.IsValidField("Email").ShouldBeTrue();

        }
        [Fact]
        public async Task When_interceptor_specified_Intercepts_validation()
        {
            var form = new FormData {
                { "Email", "foo" },
                { "Surname", "foo" },
                { "Forename", "foo" },
            };
			var result = await GetErrors("InterceptorTest", form);

            result.IsValidField("Forename").ShouldBeFalse();
            result.IsValidField("Surname").ShouldBeFalse();
            result.IsValidField("Email").ShouldBeTrue();
        }

        [Fact]
        public async Task When_interceptor_specified_Intercepts_validation_provides_custom_errors()
        {
            var form = new FormData {
                { "Email", "foo" },
                { "Surname", "foo" },
                { "Forename", "foo" },
            };

			var result = await GetErrors("ClearErrorsInterceptorTest", form);

			result.Count.ShouldEqual(0);
        }
        [Fact]
        public async Task When_validator_implements_IValidatorInterceptor_directly_interceptor_invoked()
        {
            var form = new FormData {
                { "Email", "foo" },
                { "Surname", "foo" },
                { "Forename", "foo" },
            };

			var result = await GetErrors("BuiltInInterceptorTest", form);

	        result.Count.ShouldEqual(0);
        }
        [Fact]
        public async Task Validator_customizations_should_only_apply_to_single_parameter()
        {
            var form = new FormData {
                { "first.Email", "foo" },
                { "first.Surname", "foo" },
                { "first.Forename", "foo" },
                { "second.Email", "foo" },
                { "second.Surname", "foo" },
                { "second.Forename", "foo" }
            };

	        var result = await GetErrors("TwoParameters", form);

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

			var result = await GetErrors("Collection", form);

			result.Count.ShouldEqual(2);
			result[0].Name.ShouldEqual("model[0].Name");
		}

		[Fact]
		public async Task Validates_collection_without_prefix() {
			var form = new FormData {
				{"[0].Name", "foo"},
				{"[1].Name", "foo"},
			};

			var result = await GetErrors("Collection", form);

			result.Count.ShouldEqual(2);
			result[0].Name.ShouldEqual("[0].Name");
		}


		[Fact]
		public async Task Returns_multiple_errors_for_same_property() {
			var form = new FormData() {
				{"model.Name", "baz"}
			};

			var result = await GetErrors("MultipleErrors",form);

			result.Count.ShouldEqual(2);
		}
	}
}