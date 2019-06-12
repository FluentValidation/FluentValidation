namespace FluentValidation.Tests {
	using System;
	using System.Collections.Generic;
	using System.Net.Http;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;
	using AspNetCore;
	using AspNetCore.Controllers;
	using Attributes;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Abstractions;
	using Microsoft.AspNetCore.Mvc.ModelBinding;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using Newtonsoft.Json;
	using Xunit;
	using Xunit.Abstractions;

	public class RazorPagesTestsWithImplicitValidationDisabled : IClassFixture<WebAppFixture> {
		private readonly ITestOutputHelper _output;
		private readonly HttpClient _client;

		public RazorPagesTestsWithImplicitValidationDisabled(ITestOutputHelper output, WebAppFixture webApp) {
			CultureScope.SetDefaultCulture();

			_output = output;
			_client = webApp.WithImplicitValidationEnabled(false).CreateClient();
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
	}

	public class RazorPagesTestsWithImplicitValidationEnabled : IClassFixture<WebAppFixture> {
		private readonly ITestOutputHelper _output;
		private readonly HttpClient _client;

		public RazorPagesTestsWithImplicitValidationEnabled(ITestOutputHelper output, WebAppFixture webApp) {
			CultureScope.SetDefaultCulture();

			_output = output;
			_client = webApp.WithImplicitValidationEnabled(true).CreateClient();
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
	}

}
