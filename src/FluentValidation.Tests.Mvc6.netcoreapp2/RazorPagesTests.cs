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

	public class RazorPagesTests : IClassFixture<WebAppFixture<StartupWithImplicitValidationDisabled>> {
		private readonly ITestOutputHelper _output;
		private readonly WebAppFixture<Startup> _webApp;

		public RazorPagesTests(ITestOutputHelper output, WebAppFixture<Startup> webApp) {
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

		private static string ExtractAntiForgeryToken(string htmlResponseText) {
			if (htmlResponseText == null) throw new ArgumentNullException(nameof(htmlResponseText));

			var match = Regex.Match(htmlResponseText, @"\<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)"" \/\>");
			return match.Success ? match.Groups[1].Captures[0].Value : null;
		}
	}

	public class ParameterBinder2 : ParameterBinder {
		public ParameterBinder2(IModelMetadataProvider modelMetadataProvider, IModelBinderFactory modelBinderFactory, IObjectModelValidator validator) : base(modelMetadataProvider, modelBinderFactory, validator) {
		}

		public ParameterBinder2(IModelMetadataProvider modelMetadataProvider, IModelBinderFactory modelBinderFactory, IObjectModelValidator validator, IOptions<MvcOptions> mvcOptions, ILoggerFactory loggerFactory) : base(modelMetadataProvider, modelBinderFactory, validator, mvcOptions, loggerFactory) {
		}

		public override Task<ModelBindingResult> BindModelAsync(ActionContext actionContext, IValueProvider valueProvider, ParameterDescriptor parameter, object value) {
			return base.BindModelAsync(actionContext, valueProvider, parameter, value);
		}

		public override Task<ModelBindingResult> BindModelAsync(ActionContext actionContext, IModelBinder modelBinder, IValueProvider valueProvider, ParameterDescriptor parameter, ModelMetadata metadata, object value) {
			return base.BindModelAsync(actionContext, modelBinder, valueProvider, parameter, metadata, value);
		}
	}
}