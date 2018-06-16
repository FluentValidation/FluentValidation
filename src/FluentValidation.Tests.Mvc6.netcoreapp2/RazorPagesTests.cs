namespace FluentValidation.Tests {
	using System;
	using System.Collections.Generic;
	using System.Net.Http;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;
	using AspNetCore;
	using AspNetCore.Controllers;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Abstractions;
	using Microsoft.AspNetCore.Mvc.ModelBinding;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using Newtonsoft.Json;
	using Xunit;
	using Xunit.Abstractions;

	public class RazorPagesTestsWithImplicitValidationDisabled : IClassFixture<WebAppFixture<StartupWithImplicitValidationDisabled>> {
		private readonly ITestOutputHelper _output;
		private readonly WebAppFixture<StartupWithImplicitValidationDisabled> _webApp;

		public RazorPagesTestsWithImplicitValidationDisabled(ITestOutputHelper output, WebAppFixture<StartupWithImplicitValidationDisabled> webApp) {
			CultureScope.SetDefaultCulture();

			this._output = output;
			this._webApp = webApp;
		}

		[Fact]
		public async void Validates_with_BindProperty_attribute_when_implicit_validation_disabled() {
			var form = new FormData {
				{"Name", null},
			};

			var result = await _webApp.PostResponse("/TestPage1", form);
			var errors = JsonConvert.DeserializeObject<List<SimpleError>>(result);

			errors.Count.ShouldEqual(1);
		}
		
		[Fact]
		public async void Validates_with_BindProperty_attribute_when_implicit_validation_disabled_using_prefix() {
			var form = new FormData {
				{"Test.Name", null},
			};

			var result = await _webApp.PostResponse("/TestPageWithPrefix", form);
			var errors = JsonConvert.DeserializeObject<List<SimpleError>>(result);

			errors.Count.ShouldEqual(1);
		}
	}
	
	public class RazorPagesTestsWithImplicitValidationEnabled : IClassFixture<WebAppFixture<Startup>> {
		private readonly ITestOutputHelper _output;
		private readonly WebAppFixture<Startup> _webApp;

		public RazorPagesTestsWithImplicitValidationEnabled(ITestOutputHelper output, WebAppFixture<Startup> webApp) {
			CultureScope.SetDefaultCulture();

			this._output = output;
			this._webApp = webApp;
		}

		[Fact]
		public async void Validates_with_BindProperty_attribute_when_implicit_validation_disabled() {
			var form = new FormData {
				{"Name", null},
			};

			var result = await _webApp.PostResponse("/TestPage1", form);
			var errors = JsonConvert.DeserializeObject<List<SimpleError>>(result);

			errors.Count.ShouldEqual(1);
		}
		
		[Fact]
		public async void Validates_with_BindProperty_attribute_when_implicit_validation_disabled_using_prefix() {
			var form = new FormData {
				{"Test.Name", null},
			};

			var result = await _webApp.PostResponse("/TestPageWithPrefix", form);
			var errors = JsonConvert.DeserializeObject<List<SimpleError>>(result);

			errors.Count.ShouldEqual(1);
		}
	}

}