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
using AspNetCore;
using Controllers;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS0618

public class TypeFilterTests : IClassFixture<WebAppFixture> {
	private WebAppFixture _webApp;

	public TypeFilterTests(ITestOutputHelper output, WebAppFixture webApp) {
		_webApp = webApp;
	}

	[Fact]
	public async Task Finds_and_executes_validator() {
		var client = _webApp.CreateClientWithServices(services => {
			services.AddMvc().AddNewtonsoftJson().AddFluentValidation(fv => {
				fv.RegisterValidatorsFromAssemblyContaining<TestController>();
			});
		});
		var result = await client.GetErrors("InjectsExplicitChildValidator");

		// Validator was found and executed so field shouldn't be valid.
		result.IsValidField("Child.Name").ShouldBeFalse();

	}

	[Fact]
	public async Task Filters_types() {
		var client = _webApp.CreateClientWithServices(services => {
			services.AddMvc().AddNewtonsoftJson().AddFluentValidation(fv => {
				fv.RegisterValidatorsFromAssemblyContaining<TestController>(scanResult => {
					return scanResult.ValidatorType != typeof(InjectsExplicitChildValidator);
				});
			});
		});

		var result = await client.GetErrors("InjectsExplicitChildValidator");

		// Should be valid as the validator was skipped.
		result.IsValidField("Child.Name").ShouldBeTrue();
	}
}
