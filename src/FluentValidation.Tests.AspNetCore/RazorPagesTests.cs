namespace FluentValidation.Tests;

using System.Collections.Generic;
using System.Net.Http;
using Controllers;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;



public class RazorPagesTestsWithImplicitValidationDisabled : IClassFixture<WebAppFixture> {
	private readonly ITestOutputHelper _output;
	private readonly HttpClient _client;

	public RazorPagesTestsWithImplicitValidationDisabled(ITestOutputHelper output, WebAppFixture webApp) {
		CultureScope.SetDefaultCulture();

		_output = output;
		_client = webApp.CreateClientWithServices(services => {
#pragma warning disable CS0618
			services.AddMvc().AddNewtonsoftJson().AddFluentValidation();
#pragma warning restore CS0618
			services.AddScoped<IValidator<TestModel>, TestModelValidator>();
			services.AddScoped<IValidator<RulesetTestModel>, RulesetTestValidator>();
			services.AddScoped<IValidator<ClientsideRulesetModel>, ClientsideRulesetValidator>();
		});
	}

	[Fact]
	public async void Validates_with_BindProperty_attribute_when_implicit_validation_disabled() {
		var form = new Dictionary<string, string> {
			{"Name", null},
		};

		var result = await _client.PostResponse("/TestPage1", form);
		var errors = JsonConvert.DeserializeObject<List<SimpleError>>(result);

		errors.Count.ShouldEqual(1);
	}

	[Fact]
	public async void Validates_with_BindProperty_attribute_when_implicit_validation_disabled_using_prefix() {
		var form = new Dictionary<string, string> {
			{"Test.Name", null},
		};

		var result = await _client.PostResponse("/TestPageWithPrefix", form);
		var errors = JsonConvert.DeserializeObject<List<SimpleError>>(result);

		errors.Count.ShouldEqual(1);
	}

	[Fact]
	public async void Should_only_validate_specified_ruleset() {
		var form = new Dictionary<string, string> {
			{"Email", "foo"},
			{"Surname", "foo"},
			{"Forename", "foo"},
		};

		var result = await _client.PostResponse("/RuleSetTest", form);
		var errors = JsonConvert.DeserializeObject<List<SimpleError>>(result);

		errors.IsValidField("Forename").ShouldBeFalse();
		errors.IsValidField("Surname").ShouldBeFalse();
		errors.IsValidField("Email").ShouldBeTrue();
	}
}

public class RazorPagesTestsWithImplicitValidationEnabled : IClassFixture<WebAppFixture> {
	private readonly ITestOutputHelper _output;
	private readonly HttpClient _client;

	public RazorPagesTestsWithImplicitValidationEnabled(ITestOutputHelper output, WebAppFixture webApp) {
		CultureScope.SetDefaultCulture();

		_output = output;
		_client = _client = webApp.CreateClientWithServices(services => {
#pragma warning disable CS0618
			services.AddMvc().AddNewtonsoftJson().AddFluentValidation(fv => {
				fv.ImplicitlyValidateChildProperties = true;
			});
#pragma warning restore CS0618
			services.AddScoped<IValidator<TestModel>, TestModelValidator>();
			services.AddScoped<IValidator<RulesetTestModel>, RulesetTestValidator>();
			services.AddScoped<IValidator<ClientsideRulesetModel>, ClientsideRulesetValidator>();
		});
	}

	[Fact]
	public async void Validates_with_BindProperty_attribute_when_implicit_validation_enabled() {
		var form = new Dictionary<string, string> {
			{"Name", null},
		};

		var result = await _client.PostResponse("/TestPage1", form);
		var errors = JsonConvert.DeserializeObject<List<SimpleError>>(result);

		errors.Count.ShouldEqual(1);
	}

	[Fact]
	public async void Validates_with_BindProperty_attribute_when_implicit_validation_disabled_using_prefix() {
		var form = new Dictionary<string, string> {
			{"Test.Name", null},
		};

		var result = await _client.PostResponse("/TestPageWithPrefix", form);
		var errors = JsonConvert.DeserializeObject<List<SimpleError>>(result);

		errors.Count.ShouldEqual(1);
	}

	[Fact]
	public async void Should_only_validate_specified_ruleset() {
		var form = new Dictionary<string, string> {
			{"Email", "foo"},
			{"Surname", "foo"},
			{"Forename", "foo"},
		};

		var result = await _client.PostResponse("/RuleSetTest", form);
		var errors = JsonConvert.DeserializeObject<List<SimpleError>>(result);

		errors.IsValidField("Forename").ShouldBeFalse();
		errors.IsValidField("Surname").ShouldBeFalse();
		errors.IsValidField("Email").ShouldBeTrue();
	}
}
