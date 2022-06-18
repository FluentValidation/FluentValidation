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

namespace FluentValidation.Tests;

using System.Threading.Tasks;
using Controllers;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

public class DisableAutoValidationTests : IClassFixture<WebAppFixture> {
	private WebAppFixture _webApp;

	public DisableAutoValidationTests(ITestOutputHelper output, WebAppFixture webApp) {
		_webApp = webApp;
	}

	[Fact]
	public async Task Disables_automatic_validation() {
		var client = _webApp.CreateClientWithServices(services => {
			services.AddMvc().AddNewtonsoftJson().AddFluentValidation(fv => {
				fv.RegisterValidatorsFromAssemblyContaining<TestController>();
				fv.AutomaticValidationEnabled = false;
			});
		});

		var result = await client.GetErrors("InjectsExplicitChildValidator");

		// Should be valid as automatic validation is completely disabled..
		result.IsValidField("Child.Name").ShouldBeTrue();
	}

	[Fact]
	public async Task Disables_automatic_validation_for_implicit_validation() {
		var client = _webApp.CreateClientWithServices(services => {
			services.AddMvc().AddNewtonsoftJson().AddFluentValidation(fv => {
				fv.RegisterValidatorsFromAssemblyContaining<TestController>();
#pragma warning disable CS0618
				fv.ImplicitlyValidateChildProperties = true;
#pragma warning restore CS0618
				// Disabling auto validation supersedes enabling implicit validation.
				fv.AutomaticValidationEnabled = false;
			});
		});

		var result = await client.GetErrors("ImplicitChildValidator");
		// Validation is disabled; no errors.
		result.Count.ShouldEqual(0);
	}
}
