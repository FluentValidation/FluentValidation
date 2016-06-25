namespace FluentValidation.Tests.Mvc6 {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net.Http;
    using System.Reflection;
    using System.Threading.Tasks;
    using Controllers;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.TestHost;
    using Newtonsoft.Json;
    using FluentValidation.Attributes;
    using Xunit;

    public class MvcIntegrationTests {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public MvcIntegrationTests()
        {
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


       




        /*[Fact, Ignore("MVC5 changed validation behaviour sutbley causing this to fail. Investigate for a future release. ")]
		public async Task Should_add_all_errors_in_one_go() {
			var form = new FormData {
				{ "Email", "foo" },
				{ "Surname", "foo" },
				{ "Forename", "foo" },
				{ "DateOfBirth", null },
				{ "Address1", null }
			};

			var context = new ModelBindingContext {
				ModelName = "test",
				ModelMetadata = CreateMetaData(typeof(TestModel4)),
				ModelState = new ModelStateDictionary(),
				FallbackToEmptyPrefix = true,
				ValueProvider = form.ToValueProvider(),
			};

			binder.BindModel(controllerContext, context);

			context.ModelState.IsValidField("Email").ShouldBeFalse(); //Email validation failed
			context.ModelState.IsValidField("DateOfBirth").ShouldBeFalse(); //Date of Birth not specified (implicit required error)
			context.ModelState.IsValidField("Surname").ShouldBeFalse(); //cross-property
		}*/

       
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

        /*[Fact]
        public async Task Should_not_fail_when_no_validator_can_be_found()
        {
            var bindingContext = new ModelBindingContext
            {
                ModelName = "test",
                ModelMetadata = CreateMetaData(typeof(TestModel2)),

                ModelState = new ModelStateDictionary(),
                FallbackToEmptyPrefix = true,
                ValueProvider = new FormData().ToValueProvider()
            };

            binder.BindModel(controllerContext, bindingContext).ShouldNotBeNull();
        }*/

        [Fact]
        public async Task Should_not_add_default_message_to_modelstate()
        {
            var form = new FormData {
                { "Id", "" }
            };

            var errors = await GetErrors("Test3", form);

            errors.GetError("Id").ShouldEqual("Validation failed");
        }

        [Fact]
        public async Task Should_not_add_default_message_to_modelstate_when_there_is_no_required_validator_explicitly_specified()
        {
            var form = new FormData {
                { "Id", "" }
            };

            var result = await GetErrors("Test6", form);
            result.GetError("Id").ShouldEqual("'Id' must not be empty.");
        }

        [Fact]
        public async Task Should_add_Default_message_to_modelstate_when_no_validator_specified()
        {
            var form = new FormData {
                { "Id", "" }
            };

            var result = await GetErrors("TestModelWithoutValidator", form);
            result.GetError("Id").ShouldEqual("A value is required.");
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
            errors.GetError("Id").ShouldEqual("'Foo' must not be empty.");
        }

        /*[Fact]
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
        }*/

       /* [Fact]
        public async Task Should_only_validate_specified_ruleset()
        {
            var form = new FormData {
                { "Email", "foo" },
                { "Surname", "foo" },
                { "Forename", "foo" },
            };

            var context = new ModelBindingContext
            {
                ModelName = "test",
                ModelMetadata = CreateMetaData(typeof(RulesetTestModel)),
                ModelState = new ModelStateDictionary(),
                FallbackToEmptyPrefix = true,
                ValueProvider = form.ToValueProvider(),
            };

            var binder = new CustomizeValidatorAttribute { RuleSet = "Names" };
            binder.BindModel(controllerContext, context);

            context.ModelState.IsValidField("Forename").ShouldBeFalse();
            context.ModelState.IsValidField("Surname").ShouldBeFalse();
            context.ModelState.IsValidField("Email").ShouldBeTrue();
        }*/

       /* [Fact]
        public async Task Should_only_validate_specified_properties()
        {
            var form = new FormData {
                { "Email", "foo" },
                { "Surname", "foo" },
                { "Forename", "foo" },
            };

            var context = new ModelBindingContext
            {
                ModelName = "test",
                ModelMetadata = CreateMetaData(typeof(PropertiesTestModel)),
                ModelState = new ModelStateDictionary(),
                FallbackToEmptyPrefix = true,
                ValueProvider = form.ToValueProvider(),
            };

            var binder = new CustomizeValidatorAttribute { Properties = "Surname,Forename" };
            binder.BindModel(controllerContext, context);

            context.ModelState.IsValidField("Forename").ShouldBeFalse();
            context.ModelState.IsValidField("Surname").ShouldBeFalse();
            context.ModelState.IsValidField("Email").ShouldBeTrue();

        }
*/
     /*   [Fact]
        public async Task When_interceptor_specified_Intercepts_validation()
        {
            var form = new FormData {
                { "Email", "foo" },
                { "Surname", "foo" },
                { "Forename", "foo" },
            };

            var context = new ModelBindingContext
            {
                ModelName = "test",
                ModelMetadata = CreateMetaData(typeof(PropertiesTestModel)),
                ModelState = new ModelStateDictionary(),
                FallbackToEmptyPrefix = true,
                ValueProvider = form.ToValueProvider(),
            };

            var binder = new CustomizeValidatorAttribute { Interceptor = typeof(SimplePropertyInterceptor) };
            binder.BindModel(controllerContext, context);

            context.ModelState.IsValidField("Forename").ShouldBeFalse();
            context.ModelState.IsValidField("Surname").ShouldBeFalse();
            context.ModelState.IsValidField("Email").ShouldBeTrue();

        }
*/
  /*      [Fact]
        public async Task When_interceptor_specified_Intercepts_validation_provides_custom_errors()
        {
            var form = new FormData {
                { "Email", "foo" },
                { "Surname", "foo" },
                { "Forename", "foo" },
            };

            var context = new ModelBindingContext
            {
                ModelName = "test",
                ModelMetadata = CreateMetaData(typeof(PropertiesTestModel)),
                ModelState = new ModelStateDictionary(),
                FallbackToEmptyPrefix = true,
                ValueProvider = form.ToValueProvider(),
            };
            var binder = new CustomizeValidatorAttribute { Interceptor = typeof(ClearErrorsInterceptor) };
            binder.BindModel(controllerContext, context);

            context.ModelState.IsValid.ShouldBeTrue();
        }
*/
       /* [Fact]
        public async Task When_validator_implements_IValidatorInterceptor_directly_interceptor_invoked()
        {
            var form = new FormData {
                { "Email", "foo" },
                { "Surname", "foo" },
                { "Forename", "foo" },
            };

            var context = new ModelBindingContext
            {
                ModelName = "test",
                ModelMetadata = CreateMetaData(typeof(PropertiesTestModel2)),
                ModelState = new ModelStateDictionary(),
                FallbackToEmptyPrefix = true,
                ValueProvider = form.ToValueProvider(),
            };

            binder.BindModel(controllerContext, context);

            context.ModelState.IsValid.ShouldBeTrue();
        }
*/
  /*      [Fact]
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

            var modelstate = new ModelStateDictionary();

            var firstContext = new ModelBindingContext
            {
                ModelName = "first",
                ModelMetadata = CreateMetaData(typeof(RulesetTestModel)),
                ModelState = modelstate,
                FallbackToEmptyPrefix = true,
                ValueProvider = form.ToValueProvider(),
            };

            var secondContext = new ModelBindingContext
            {
                ModelName = "second",
                ModelMetadata = CreateMetaData(typeof(RulesetTestModel)),
                ModelState = modelstate,
                FallbackToEmptyPrefix = true,
                ValueProvider = form.ToValueProvider()
            };

            // Use the customizations for the first 
            var binder = new CustomizeValidatorAttribute { RuleSet = "Names" };
            binder.BindModel(controllerContext, firstContext);

            // ...but not for the second.
            this.binder.BindModel(controllerContext, secondContext);

            //customizations should only apply to the first validator 
            modelstate.IsValidField("first.Forename").ShouldBeFalse();
            modelstate.IsValidField("first.Surname").ShouldBeFalse();
            modelstate.IsValidField("second.Forename").ShouldBeTrue();
            modelstate.IsValidField("second.Surname").ShouldBeTrue();
        }*/



    }
}