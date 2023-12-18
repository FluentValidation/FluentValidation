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

namespace FluentValidation.Tests.DependencyInjectionExtensions;

using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;

public class ServiceCollectionExtensionsTests {

	[Fact]
	public void Should_register_validators_as_individual_service_types() {
		var services = new ServiceCollection().AddValidatorsFromAssemblyContaining(GetType(), filter: ValidatorsFromThisFile);

		Assert.Single(services, service => service.ServiceType == typeof(FirstDummyValidator));
		Assert.Single(services, service => service.ServiceType == typeof(SecondDummyValidator));
	}

	[Fact]
	public void Should_register_validator_service_types_only_once() {
		var services = new ServiceCollection().AddValidatorsFromAssemblyContaining(GetType(), filter: ValidatorsFromThisFile)
																					.AddValidatorsFromAssemblyContaining(GetType(), filter: ValidatorsFromThisFile)
																					.AddValidatorsFromAssemblyContaining(GetType(), filter: ValidatorsFromThisFile);

		Assert.Single(services, service => service.ImplementationType == typeof(FirstDummyValidator) && service.ServiceType == typeof(FirstDummyValidator));
	}

	[Fact]
	public void Should_register_validators_as_enumerable_interface_type() {
		var services = new ServiceCollection().AddValidatorsFromAssemblyContaining(GetType(), filter: ValidatorsFromThisFile);

		var validatorServices = services.Where(s => s.ServiceType == typeof(IValidator<DummyModel>));
		Assert.Collection(validatorServices, first => {
			Assert.Equal(typeof(FirstDummyValidator), first.ImplementationType);
		}, second => {
			Assert.Equal(typeof(SecondDummyValidator), second.ImplementationType);
		});
	}

	[Fact]
	public void Should_register_validators_as_enumerable_interface_type_only_once() {
		var services = new ServiceCollection().AddValidatorsFromAssemblyContaining(GetType(), filter: ValidatorsFromThisFile)
																					.AddValidatorsFromAssemblyContaining(GetType(), filter: ValidatorsFromThisFile)
																					.AddValidatorsFromAssemblyContaining(GetType(), filter: ValidatorsFromThisFile);

		var validatorServices = services.Where(s => s.ServiceType == typeof(IValidator<DummyModel>));
		Assert.Collection(validatorServices, first => {
			Assert.Equal(typeof(FirstDummyValidator), first.ImplementationType);
		}, second => {
			Assert.Equal(typeof(SecondDummyValidator), second.ImplementationType);
		});
	}

	private bool ValidatorsFromThisFile(AssemblyScanner.AssemblyScanResult result) {
		return result.ValidatorType == typeof(FirstDummyValidator)
				|| result.ValidatorType == typeof(SecondDummyValidator);
	}
}

public record DummyModel { }

public class FirstDummyValidator : AbstractValidator<DummyModel> { }

public class SecondDummyValidator : AbstractValidator<DummyModel> { }
