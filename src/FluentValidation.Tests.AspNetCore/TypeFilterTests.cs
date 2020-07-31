#region License
// Copyright (c) .NET Foundation and contributors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// The latest version of this file can be found at https://github.com/FluentValidation/FluentValidation
#endregion

namespace FluentValidation.Tests {
	using System.Net.Http;
	using System.Threading.Tasks;
	using AspNetCore;
	using AspNetCore.Controllers;
	using Microsoft.Extensions.DependencyInjection;
	using Xunit;
	using Xunit.Abstractions;

	public class TypeFilterTests : IClassFixture<WebAppFixture> {
		private WebAppFixture _webApp;

		public TypeFilterTests(ITestOutputHelper output, WebAppFixture webApp) {
			_webApp = webApp;
		}

		[Fact]
		public async Task Finds_and_executes_validator() {
			var client = _webApp.WithFluentValidation(fv => {
				fv.RegisterValidatorsFromAssemblyContaining<TestController>();
			}).CreateClient();

			var result = await client.GetErrors("InjectsExplicitChildValidator", new FormData());

			// Validator was found and executed so field shouldn't be valid.
			result.IsValidField("Child.Name").ShouldBeFalse();

		}

		[Fact]
		public async Task Filters_types() {
			var client = _webApp.WithFluentValidation(fv => {
				fv.RegisterValidatorsFromAssemblyContaining<TestController>(scanResult => {
					return scanResult.ValidatorType != typeof(InjectsExplicitChildValidator);
				});
			}).CreateClient();

			var result = await client.GetErrors("InjectsExplicitChildValidator", new FormData());

			// Should be valid as the validator was skipped.
			result.IsValidField("Child.Name").ShouldBeTrue();
		}

		[Fact]
		public async Task Disables_automatic_validation() {
			var client = _webApp.WithFluentValidation(fv => {
				fv.RegisterValidatorsFromAssemblyContaining<TestController>();
				fv.AutomaticValidationEnabled = false;
			}).CreateClient();

			var result = await client.GetErrors("InjectsExplicitChildValidator", new FormData());

			// Should be valid as automatic validation is completely disabled..
			result.IsValidField("Child.Name").ShouldBeTrue();
		}
	}
}
