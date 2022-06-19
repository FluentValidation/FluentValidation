namespace FluentValidation.Tests;

using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Controllers;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;
using FormData = System.Collections.Generic.Dictionary<string, string>;

public class MvcIntegrationTests : IClassFixture<WebAppFixture> {
	private readonly ITestOutputHelper _output;
	private readonly WebAppFixture _webApp;
	private readonly HttpClient _client;

	public MvcIntegrationTests(ITestOutputHelper output, WebAppFixture webApp) {
		CultureScope.SetDefaultCulture();

		_output = output;
		_webApp = webApp;
		_client = webApp.CreateClientWithServices(services => {
#pragma warning disable CS0618
			services.AddMvc().AddNewtonsoftJson().AddFluentValidation();
#pragma warning restore CS0618
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddScoped<IValidator<TestModel>, TestModelValidator>();
			services.AddScoped<IValidator<TestModel3>, TestModelValidator3>();
			services.AddScoped<IValidator<TestModel4>, TestModel4Validator>();
			services.AddScoped<IValidator<TestModel5>, TestModel5Validator>();
			services.AddScoped<IValidator<TestModel6>, TestModel6Validator>();
			services.AddScoped<IValidator<TestModelWithOverridenMessageValueType>, TestModelWithOverridenMessageValueTypeValidator>();
			services.AddScoped<IValidator<RulesetTestModel>, RulesetTestValidator>();
			services.AddScoped<IValidator<PropertiesTestModel>, PropertiesValidator>();
			services.AddScoped<IValidator<PropertiesTestModel2>, PropertiesValidator2>();
			services.AddScoped<IValidator<MultipleErrorsModel>, MultipleErrorsModelValidator>();
			services.AddScoped<IValidator<MultiValidationModel>, MultiValidationValidator>();
			services.AddScoped<IValidator<MultiValidationModel2>, MultiValidationValidator2>();
			services.AddScoped<IValidator<MultiValidationModel3>, MultiValidationValidator3>();
			services.AddScoped<IValidator<ImplementsIValidatableObjectModel>, ImplementsIValidatableObjectValidator>();
			services.AddScoped<IValidator<ParentModel6>, ParentModel6Validator>();
			services.AddScoped<IValidator<TestModelWithOverridenPropertyNameValueType>, TestModelWithOverridenPropertyNameValidator>();
		});
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
	public async Task Should_add_all_errors_in_one_go_when_NotEmpty_rule_specified_for_non_nullable_value_type() {
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
	public async Task Uses_DataAnnotations_when_no_FV_validatior_defined() {
		var result = await _client.GetErrors("DataAnnotations", new FormData());
		result.Count.ShouldEqual(1);
		result[0].Message.ShouldEqual("The Name field is required.");
	}

	[Fact]
	public async void Can_mix_FV_with_IValidatableObject() {
		var result = await _client.GetErrors("ImplementsIValidatableObject", new FormData());
		_output.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
		result.Count.ShouldEqual(2);
	}

	[Fact]
	public async void Can_validate_using_JSON() {
		var result = await _client.GetErrorsViaJSON("Test5", new TestModel5());
		result.IsValidField("SomeBool").ShouldBeFalse();
		result.Count.ShouldEqual(2);
	}

	[Fact]
	public async Task Skips_validation() {
		var results = await _client.GetErrors("SkipsValidation", new FormData());
		results.Count.ShouldEqual(0);
	}

	[Fact]
	public async void Does_not_implicitly_validate_child_collections_by_default() {
		var result = await _client.GetErrorsViaJSONRaw("ImplicitChildCollection", @"{ Children: [ { Name: null } ] }");
		result.Count.ShouldEqual(0);
	}

	[Fact]
	public async void Does_implicitly_validate_child_collections_by_default_with_DataAnnotations() {
		var result = await _client.GetErrorsViaJSONRaw("ImplicitChildCollectionDataAnnotations", @"{ Children: [ { Name: null } ] }");
		result.Count.ShouldEqual(1);
	}

	[Fact]
	public async void When_skipping_children_does_not_leave_validation_state_unvalidated() {
		string json = @"{ Children: [ { Name: null } ] }";

		var request = new HttpRequestMessage(HttpMethod.Post, $"/Test/CheckUnvalidated");
		request.Content = new StringContent(json, Encoding.UTF8, "application/json");
		var responseMessage = await _client.SendAsync(request);
		responseMessage.EnsureSuccessStatusCode();
		var response = await responseMessage.Content.ReadAsStringAsync();
		response.ShouldEqual("0");
	}

	[Fact]
	public async Task Validation_invoked_with_ApiController() {
		string json = @"{ Name: null }";
		var response = await _client.PostAsync("/ApiTest", new StringContent(json, Encoding.UTF8, "application/json"));
		var responseJson = await response.Content.ReadAsStringAsync();

		string expected = @"{""errors"":{""Name"":[""Validation Failed""]}";
		Assert.Equal(400, (int)response.StatusCode);
		Assert.StartsWith(expected, responseJson);
	}


	[Fact]
	public async Task When_calling_AddFluentValidation_prior_to_AddMvc_doesnot_break() {
		var form = new FormData {
			{"Email", "foo"},
			{"Surname", "foo"},
			{"Forename", "foo"},
			{"DateOfBirth", null},
			{"Address1", null}
		};

		// Usual mechanism calls AddMvc().AddFluentValidation(). Test it the other way around.
		var client = _webApp.CreateClientWithServices(services => {
			services.AddFluentValidationAutoValidation();
			services.AddMvc().AddNewtonsoftJson();
			services.AddScoped<IValidator<TestModel4>, TestModel4Validator>();
		});

		var result = await client.GetErrors("Test4", form);

		result.IsValidField("Email").ShouldBeFalse(); //Email validation failed
		result.IsValidField("DateOfBirth").ShouldBeFalse(); //Date of Birth not specified (implicit required error)
		result.IsValidField("Surname").ShouldBeFalse(); //cross-property
	}

	[Fact]
	public async Task Generates_error_when_async_validator_invoked_synchronously() {
		var client = _webApp.CreateClientWithServices(services => {
			services.AddFluentValidationAutoValidation();
			services.AddMvc().AddNewtonsoftJson();
			services.AddScoped<IValidator<BadAsyncModel>, BadAsyncValidator>();
		});

		var ex = await Assert.ThrowsAsync<AsyncValidatorInvokedSynchronouslyException>(async () => {
			await client.PostResponse("/Test/BadAsyncModel", new FormData());
		});

		// Default exception message should've been swapped out for the asp.net specific version.
		ex.Message.ShouldEqual("Validator \"BadAsyncValidator\" can't be used with ASP.NET automatic validation as it contains asynchronous rules. ASP.NET's validation pipeline is not asynchronous and can't invoke asynchronous rules. Remove the asynchronous rules in order for this validator to run.");
	}
}
