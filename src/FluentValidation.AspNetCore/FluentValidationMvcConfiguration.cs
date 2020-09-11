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

namespace FluentValidation.AspNetCore {
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Microsoft.Extensions.DependencyInjection;

	/// <summary>
	/// FluentValidation asp.net core configuration
	/// </summary>
	public class FluentValidationMvcConfiguration {

		public FluentValidationMvcConfiguration(ValidatorConfiguration validatorOptions) {
			ValidatorOptions = validatorOptions;
		}

		/// <summary>
		/// Options that are used to configure all validators.
		/// </summary>
		public ValidatorConfiguration ValidatorOptions { get; private set; }

		/// <summary>
		/// The type of validator factory to use. Uses the ServiceProviderValidatorFactory by default.
		/// </summary>
		public Type ValidatorFactoryType { get; set; }

		/// <summary>
		/// The validator factory to use. Uses the ServiceProviderValidatorFactory by default.
		/// </summary>
		public IValidatorFactory ValidatorFactory { get; set; }

		/// <summary>
		/// Whether to run MVC's default validation process (including DataAnnotations) after FluentValidation is executed. True by default.
		/// </summary>
		public bool RunDefaultMvcValidationAfterFluentValidationExecutes { get; set; } = true;

		/// <summary>
		/// Enables or disables localization support within FluentValidation
		/// </summary>
		public bool LocalizationEnabled {
			get => ValidatorOptions.LanguageManager.Enabled;
			set => ValidatorOptions.LanguageManager.Enabled = value;
		}

		/// <summary>
		/// Whether or not child properties should be implicitly validated if a matching validator can be found. By default this is false, and you should wire up child validators using SetValidator.
		/// </summary>
		public bool ImplicitlyValidateChildProperties { get; set; }


		internal bool ClientsideEnabled = true;
		internal Action<FluentValidationClientModelValidatorProvider> ClientsideConfig = x => {};
		internal List<Assembly> AssembliesToRegister { get; } = new List<Assembly>();
		internal Func<AssemblyScanner.AssemblyScanResult, bool> TypeFilter { get; set; }
		internal ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Transient;

		/// <summary>
		/// Whether automatic server-side validation should be enabled (default true).
		/// </summary>
		public bool AutomaticValidationEnabled { get; set; } = true;

		/// <summary>
		/// Registers all validators derived from AbstractValidator within the assembly containing the specified type
		/// </summary>
		/// <param name="filter">Optional filter that allows certain types to be skipped from registration.</param>
		/// <param name="lifetime">The service lifetime that should be used for the validator registration. Defaults to Transient</param>
		public FluentValidationMvcConfiguration RegisterValidatorsFromAssemblyContaining<T>(Func<AssemblyScanner.AssemblyScanResult, bool> filter = null, ServiceLifetime lifetime = ServiceLifetime.Transient) {
			return RegisterValidatorsFromAssemblyContaining(typeof(T), filter, lifetime);
		}

		/// <summary>
		/// Registers all validators derived from AbstractValidator within the assembly containing the specified type
		/// </summary>
		/// <param name="type">The type that indicates which assembly that should be scanned</param>
		/// <param name="filter">Optional filter that allows certain types to be skipped from registration.</param>
		/// <param name="lifetime">The service lifetime that should be used for the validator registration. Defaults to Transient</param>
		public FluentValidationMvcConfiguration RegisterValidatorsFromAssemblyContaining(Type type, Func<AssemblyScanner.AssemblyScanResult, bool> filter = null, ServiceLifetime lifetime = ServiceLifetime.Transient) {
			return RegisterValidatorsFromAssembly(type.Assembly, filter, lifetime);
		}

		/// <summary>
		/// Registers all validators derived from AbstractValidator within the specified assembly
		/// </summary>
		/// <param name="assembly">The assembly to scan</param>
		/// <param name="filter">Optional filter that allows certain types to be skipped from registration.</param>
		/// <param name="lifetime">The service lifetime that should be used for the validator registration. Defaults to Transient</param>
		public FluentValidationMvcConfiguration RegisterValidatorsFromAssembly(Assembly assembly, Func<AssemblyScanner.AssemblyScanResult, bool> filter = null, ServiceLifetime lifetime = ServiceLifetime.Transient) {
			ValidatorFactoryType = typeof(ServiceProviderValidatorFactory);
			AssembliesToRegister.Add(assembly);
			TypeFilter = filter;
			ServiceLifetime = lifetime;
			return this;
		}

		/// <summary>
		/// Registers all validators derived from AbstractValidator within the specified assemblies
		/// </summary>
		/// <param name="assemblies">The assemblies to scan</param>
		/// <param name="filter">Optional filter that allows certain types to be skipped from registration.</param>
		/// <param name="lifetime">The service lifetime that should be used for the validator registration. Defaults to Transient</param>
		public FluentValidationMvcConfiguration RegisterValidatorsFromAssemblies(IEnumerable<Assembly> assemblies, Func<AssemblyScanner.AssemblyScanResult, bool> filter = null, ServiceLifetime lifetime = ServiceLifetime.Transient) {
			ValidatorFactoryType = typeof(ServiceProviderValidatorFactory);
			AssembliesToRegister.AddRange(assemblies);
			TypeFilter = filter;
			ServiceLifetime = lifetime;
			return this;
		}

		/// <summary>
		/// Configures clientside validation support
		/// </summary>
		/// <param name="clientsideConfig"></param>
		/// <param name="enabled">Whether clientisde validation integration is enabled</param>
		/// <returns></returns>
		public FluentValidationMvcConfiguration ConfigureClientsideValidation(Action<FluentValidationClientModelValidatorProvider> clientsideConfig=null, bool enabled=true) {
			if (clientsideConfig != null) {
				ClientsideConfig = clientsideConfig;
			}
			ClientsideEnabled = enabled;
			return this;
		}

	}
}
