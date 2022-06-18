namespace FluentValidation.Tests;

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Controllers;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;
using FormData = System.Collections.Generic.Dictionary<string, string>;

#pragma warning disable CS0618

public class ImplicitValidationTests : IClassFixture<WebAppFixture> {
	private WebAppFixture _app;
	private ITestOutputHelper _output;

	public ImplicitValidationTests(WebAppFixture app, ITestOutputHelper output) {
		_app = app;
		_output = output;
	}

	private HttpClient CreateClient(bool implicitValidationEnabled) {
		return _app.CreateClientWithServices(services => {
			services.AddMvc().AddNewtonsoftJson().AddFluentValidation(fv => {
				fv.ImplicitlyValidateChildProperties = implicitValidationEnabled;
			});
			services.AddScoped<IValidator<ParentModel>, ParentModelValidator>();
			services.AddScoped<IValidator<ParentModel2>, ParentModel2Validator>();
			services.AddScoped<IValidator<ParentModel3>, ParentModelValidator3>();
			services.AddScoped<IValidator<ParentModel4>, ParentModel4Validator>();
			services.AddScoped<IValidator<ParentModel5>, ParentModel5Validator>();
			services.AddScoped<IValidator<ChildModel>, ChildModelValidator>();
			services.AddScoped<IValidator<ChildModel2>, ChildModel2Validator>();
			services.AddScoped<IValidator<ChildModel3>, ChildModelValidator3>();
			services.AddScoped<IValidator<ChildModel4>, ChildModel4Validator>();
			services.AddScoped<IValidator<CollectionTestModel>, CollectionTestModelValidator>();
			services.AddScoped<IValidator<TestModel>, TestModelValidator>();
			services.AddScoped<IValidator<TestModel5>, TestModel5Validator>();
		});
	}

	[Fact]
	public async void Does_not_implicitly_run_child_validator() {
		var client = CreateClient(false);
		var result = await client.GetErrors("ImplicitChildValidator", new FormData());
		result.Count.ShouldEqual(0);
	}

	[Fact]
	public async void Implicitly_run_child_validator() {
		var client = CreateClient(true);
		var result = await client.GetErrors("ImplicitChildValidator", new FormData());
		result.Count.ShouldEqual(1);
		result[0].Name.ShouldEqual("Child.Name");
	}

	[Fact]
	public async void Ignores_null_child() {
		var client = CreateClient(true);
		var result = await client.GetErrors("ImplicitChildValidatorWithNullChild", new FormData());
		result.Count.ShouldEqual(0);
	}

	[Fact]
	public async void Executes_implicit_child_validator_and_mixes_with_IValidatableObject() {
		var client = CreateClient(true);
		var result = await client.GetErrors("ImplicitChildImplementsIValidatableObject", new FormData());
		result.Count.ShouldEqual(3);
	}

	[Fact]
	public async void Executes_implicit_child_validator_when_enabled_does_not_execute_multiple_times() {
		var client = CreateClient(true);
		var result = await client.GetErrors("ImplicitChildValidator", new FormData());
		result.Count.ShouldEqual(1);

		result = await client.GetErrors("ImplicitChildValidator", new FormData());
		result.Count.ShouldEqual(1);
	}

	[Fact]
	public async void ImplicitValidation_enabled_but_validator_explicitly_only_includes_error_message_once() {
		var client = CreateClient(true);
		var result = await client.GetErrors("ImplicitAndExplicitChildValidator", new FormData());
		result.Count.ShouldEqual(1);
	}

	[Fact]
	public async void Executes_implicit_child_validator_and_mixes_with_DataAnnotations() {
		var client = CreateClient(true);
		var result = await client.GetErrors("ImplicitChildWithDataAnnotations", new FormData());
		_output.WriteLine(JsonConvert.SerializeObject(result));
		result.Count.ShouldEqual(2);
	}

	[Fact]
	public async void Can_validate_dictionary() {
		var client = CreateClient(true);
		var dictionary = new Dictionary<int, TestModel5>() {
			{123, new TestModel5() {SomeBool = true, Id = 1}},
			{456, new TestModel5()}
		};
		var result = await client.GetErrorsViaJSON("UsingDictionaryWithJsonBody", dictionary);
		result.Count.ShouldEqual(2);
		result.IsValidField("[1].Value.Id").ShouldBeFalse();
		result.IsValidField("[1].Value.SomeBool").ShouldBeFalse();
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
		var client = CreateClient(true);
		var result = await client.GetErrors("DictionaryParameter", form);
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
		var client = CreateClient(true);
		var result = await client.GetErrors("DictionaryParameter", form);
		_output.WriteLine(JsonConvert.SerializeObject(result));

		result.Count.ShouldEqual(2);
	}

	[Fact]
	public async void Can_validate_enumerable() {
		var list = new List<TestModel5>() {
			new TestModel5() {SomeBool = true, Id = 1},
			new TestModel5(),
			new TestModel5() {SomeBool = true}
		};

		var client = CreateClient(true);
		var result = await client.GetErrorsViaJSON("UsingEnumerable", list);

		result.IsValidField("[1].Id").ShouldBeFalse();
		result.IsValidField("[1].SomeBool").ShouldBeFalse();
		result.IsValidField("[2].Id").ShouldBeFalse();
		result.Count.ShouldEqual(3);
	}

	[Fact]
	public async Task Validates_collection() {
		var form = new FormData {
			{"model[0].Name", "foo"},
			{"model[1].Name", "foo"},
		};

		var client = CreateClient(true);
		var result = await client.GetErrors("Collection", form);

		result.Count.ShouldEqual(2);
		result[0].Name.ShouldEqual("model[0].Name");
	}

	[Fact]
	public async Task Validates_collection_without_prefix() {
		var form = new FormData {
			{"[0].Name", "foo"},
			{"[1].Name", "foo"},
		};

		var client = CreateClient(true);
		var result = await client.GetErrors("Collection", form);

		result.Count.ShouldEqual(2);
		result[0].Name.ShouldEqual("[0].Name");
	}

	[Fact]
	public async void Skips_implicit_child_validation() {
		var result = await CreateClient(true).GetErrors("SkipsImplicitChildValidator", new FormData());
		result.Count.ShouldEqual(0);
	}

}
