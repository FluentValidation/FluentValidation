namespace FluentValidation.Tests {
	using System;
	using System.Collections.Generic;
	using System.Net.Http;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;
	using AspNetCore;
	using AspNetCore.Controllers;
	using Newtonsoft.Json;
	using Xunit;
	using Xunit.Abstractions;

	public class RazorPagesTests : IClassFixture<WebAppFixture<Startup>> {
		private readonly ITestOutputHelper _output;
		private readonly WebAppFixture<Startup> _webApp;

		public RazorPagesTests(ITestOutputHelper output, WebAppFixture<Startup> webApp) {
			CultureScope.SetDefaultCulture();

			this._output = output;
			this._webApp = webApp;
		}

		[Fact]
		public async void Validates_with_BindProperty_attribute() {
			var form = new FormData {
				{"Name", null},
			};

			var result = await _webApp.PostResponse("/TestPage1", form);
			var errors = JsonConvert.DeserializeObject<List<SimpleError>>(result);

			errors.Count.ShouldEqual(1);
		}
		
		[Fact]
		public async void Validates_with_BindProperty_attribute_and_prefix() {
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
}