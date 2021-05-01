namespace FluentValidation.Tests {
	using System.Collections.Generic;
	using System.Net.Http;
	using AspNetCore;
	using AspNetCore.Controllers;
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
				services.AddMvc().AddNewtonsoftJson().AddFluentValidation();
				services.AddScoped<IValidator<TestModel>, TestModelValidator>();
				services.AddScoped<IValidator<RulesetTestModel>, RulesetTestValidator>();
				services.AddScoped<IValidator<ClientsideRulesetModel>, ClientsideRulesetValidator>();
			});
		}

		[Fact]
		public async void Validates_with_BindProperty_attribute_when_implicit_validation_disabled() {
			var form = new FormData {
				{"Name", null},
			};

			var result = await _client.PostResponse("/TestPage1", form);
			var errors = JsonConvert.DeserializeObject<List<SimpleError>>(result);

			errors.Count.ShouldEqual(1);
		}

		[Fact]
		public async void Validates_with_BindProperty_attribute_when_implicit_validation_disabled_using_prefix() {
			var form = new FormData {
				{"Test.Name", null},
			};

			var result = await _client.PostResponse("/TestPageWithPrefix", form);
			var errors = JsonConvert.DeserializeObject<List<SimpleError>>(result);

			errors.Count.ShouldEqual(1);
		}

#if NETCOREAPP3_1 || NET5_0
		[Fact]
		public async void Should_only_validate_specified_ruleset() {
			var form = new FormData {
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
#endif
	}

	public class RazorPagesTestsWithImplicitValidationEnabled : IClassFixture<WebAppFixture> {
		private readonly ITestOutputHelper _output;
		private readonly HttpClient _client;

		public RazorPagesTestsWithImplicitValidationEnabled(ITestOutputHelper output, WebAppFixture webApp) {
			CultureScope.SetDefaultCulture();

			_output = output;
			_client = _client = webApp.CreateClientWithServices(services => {
				services.AddMvc().AddNewtonsoftJson().AddFluentValidation(fv => {
					fv.ImplicitlyValidateChildProperties = true;
				});
				services.AddScoped<IValidator<TestModel>, TestModelValidator>();
				services.AddScoped<IValidator<RulesetTestModel>, RulesetTestValidator>();
				services.AddScoped<IValidator<ClientsideRulesetModel>, ClientsideRulesetValidator>();
			});
		}

		[Fact]
		public async void Validates_with_BindProperty_attribute_when_implicit_validation_enabled() {
			var form = new FormData {
				{"Name", null},
			};

			var result = await _client.PostResponse("/TestPage1", form);
			var errors = JsonConvert.DeserializeObject<List<SimpleError>>(result);

			errors.Count.ShouldEqual(1);
		}

		[Fact]
		public async void Validates_with_BindProperty_attribute_when_implicit_validation_disabled_using_prefix() {
			var form = new FormData {
				{"Test.Name", null},
			};

			var result = await _client.PostResponse("/TestPageWithPrefix", form);
			var errors = JsonConvert.DeserializeObject<List<SimpleError>>(result);

			errors.Count.ShouldEqual(1);
		}

#if NETCOREAPP3_1 || NET5_0
		[Fact]
		public async void Should_only_validate_specified_ruleset() {
			var form = new FormData {
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
#endif
	}

}
