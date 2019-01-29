namespace FluentValidation.Tests {
	using System.Threading.Tasks;
	using AspNetCore;
	using AspNetCore.Controllers;
	using Xunit;
	using Xunit.Abstractions;

	public class DependencyInjectionTests : IClassFixture<WebAppFixture<StartupForDependencyInjectionTests>> {
		private readonly ITestOutputHelper _output;
		private readonly WebAppFixture<StartupForDependencyInjectionTests> _webApp;

		public DependencyInjectionTests(ITestOutputHelper output, WebAppFixture<StartupForDependencyInjectionTests> webApp) {
			CultureScope.SetDefaultCulture();

			_output = output;
			_webApp = webApp;
		}
		
		[Fact]
		public async Task Resolves_explicit_child_validator() {
			var result = await _webApp.GetErrors("InjectsExplicitChildValidator", new FormData());
			result.IsValidField("Child.Name").ShouldBeFalse();
			result.GetError("Child.Name").ShouldEqual("NotNullInjected");
		}
	}
}