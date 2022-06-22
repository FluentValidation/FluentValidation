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
	/// Auto-validation configuration.
	/// </summary>
	public class FluentValidationAutoValidationConfiguration {

		/// <summary>
		/// Whether or not child properties should be implicitly validated if a matching validator can be found. By default this is false, and you should wire up child validators using SetValidator.
		/// </summary>
		[Obsolete("Implicit validation of child properties deprecated and will be removed in a future release. Please use SetValidator instead. For details see https://github.com/FluentValidation/FluentValidation/issues/1960")]
		public bool ImplicitlyValidateChildProperties { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the elements of a root model should be implicitly validated when
		/// the root model is a collection type and a matching validator can be found for the element type.
		/// By default this is <see langword="false"/>, and you will need to create a validator for the collection type
		/// (unless <see cref="ImplicitlyValidateChildProperties"/> is <see langword="true"/>.
		/// </summary>
		[Obsolete("Implicit validation of root collection elements is deprecated and will be removed in a future release. Please use an explicit collection validator instead. For details see https://github.com/FluentValidation/FluentValidation/issues/1960")]
		public bool ImplicitlyValidateRootCollectionElements { get; set; }

		/// <summary>
		/// The type of validator factory to use. Uses the ServiceProviderValidatorFactory by default.
		/// </summary>
		[Obsolete("IValidatorFactory and its implementors are deprecated. Please use the Service Provider directly. For details see https://github.com/FluentValidation/FluentValidation/issues/1961")]
		public Type ValidatorFactoryType { get; set; }

		/// <summary>
		/// The validator factory to use. Uses the ServiceProviderValidatorFactory by default.
		/// </summary>
		[Obsolete("IValidatorFactory and its implementors are deprecated. Please use the Service Provider directly. For details see https://github.com/FluentValidation/FluentValidation/issues/1961")]
		public IValidatorFactory ValidatorFactory { get; set; }

		/// <summary>
		/// By default Data Annotations validation will also run as well as FluentValidation.
		/// Setting this to true will disable DataAnnotations and only run FluentValidation.
		/// </summary>
		public bool DisableDataAnnotationsValidation { get; set; }
	}

	/// <summary>
	/// FluentValidation asp.net core configuration
	/// </summary>
	public class FluentValidationMvcConfiguration : FluentValidationAutoValidationConfiguration {
		private readonly IServiceCollection _services;

		[Obsolete]
		public FluentValidationMvcConfiguration(ValidatorConfiguration validatorOptions) {
#pragma warning disable CS0618
			ValidatorOptions = validatorOptions;
#pragma warning restore CS0618
		}

		internal FluentValidationMvcConfiguration(ValidatorConfiguration validatorOptions, IServiceCollection services) {
			_services = services;
#pragma warning disable CS0618
			ValidatorOptions = validatorOptions;
#pragma warning restore CS0618
		}

		/// <summary>
		/// Options that are used to configure all validators.
		/// </summary>
		[Obsolete("Global options should be set using the static ValidatorOptions.Global instead.")]
		public ValidatorConfiguration ValidatorOptions { get; private set; }

		/// <summary>
		/// Enables or disables localization support within FluentValidation
		/// </summary>
		[Obsolete("Set the static ValidatorOptions.Global.LanguageManager.Enabled property instead.")]
		public bool LocalizationEnabled {
			get => ValidatorOptions.LanguageManager.Enabled;
			set => ValidatorOptions.LanguageManager.Enabled = value;
		}

		internal bool ClientsideEnabled = true;
		internal Action<FluentValidationClientModelValidatorProvider> ClientsideConfig = x => {};

		/// <summary>
		/// Whether automatic server-side validation should be enabled (default true).
		/// </summary>
		public bool AutomaticValidationEnabled { get; set; } = true;

		/// <summary>
		/// Registers all validators derived from AbstractValidator within the assembly containing the specified type
		/// </summary>
		/// <param name="filter">Optional filter that allows certain types to be skipped from registration.</param>
		/// <param name="lifetime">The service lifetime that should be used for the validator registration. Defaults to Scoped</param>
		/// <param name="includeInternalTypes">Include internal validators. The default is false.</param>
		// [Obsolete("RegisterValidatorsFromAssemblyContaining is deprecated. Call services.AddValidatorsFromAssemblyContaining<T> instead, which has the same effect. See https://github.com/FluentValidation/FluentValidation/issues/1963")]
		public FluentValidationMvcConfiguration RegisterValidatorsFromAssemblyContaining<T>(Func<AssemblyScanner.AssemblyScanResult, bool> filter = null, ServiceLifetime lifetime = ServiceLifetime.Scoped, bool includeInternalTypes = false) {
			return RegisterValidatorsFromAssemblyContaining(typeof(T), filter, lifetime, includeInternalTypes);
		}

		/// <summary>
		/// Registers all validators derived from AbstractValidator within the assembly containing the specified type
		/// </summary>
		/// <param name="type">The type that indicates which assembly that should be scanned</param>
		/// <param name="filter">Optional filter that allows certain types to be skipped from registration.</param>
		/// <param name="lifetime">The service lifetime that should be used for the validator registration. Defaults to Scoped</param>
		/// <param name="includeInternalTypes">Include internal validators. The default is false.</param>
		// [Obsolete("RegisterValidatorsFromAssemblyContaining is deprecated. Call services.AddValidatorsFromAssemblyContaining instead, which has the same effect. See https://github.com/FluentValidation/FluentValidation/issues/1963")]
		public FluentValidationMvcConfiguration RegisterValidatorsFromAssemblyContaining(Type type, Func<AssemblyScanner.AssemblyScanResult, bool> filter = null, ServiceLifetime lifetime = ServiceLifetime.Scoped, bool includeInternalTypes = false) {
			return RegisterValidatorsFromAssembly(type.Assembly, filter, lifetime, includeInternalTypes);
		}

		/// <summary>
		/// Registers all validators derived from AbstractValidator within the specified assembly
		/// </summary>
		/// <param name="assembly">The assembly to scan</param>
		/// <param name="filter">Optional filter that allows certain types to be skipped from registration.</param>
		/// <param name="lifetime">The service lifetime that should be used for the validator registration. Defaults to Scoped</param>
		/// <param name="includeInternalTypes">Include internal validators. The default is false.</param>
		// [Obsolete("RegisterValidatorsFromAssembly is deprecated. Call services.AddValidatorsFromAssembly instead, which has the same effect. See https://github.com/FluentValidation/FluentValidation/issues/1963")]
		public FluentValidationMvcConfiguration RegisterValidatorsFromAssembly(Assembly assembly, Func<AssemblyScanner.AssemblyScanResult, bool> filter = null, ServiceLifetime lifetime = ServiceLifetime.Scoped, bool includeInternalTypes = false) {
			_services.AddValidatorsFromAssembly(assembly, lifetime, filter, includeInternalTypes);

#pragma warning disable CS0618
			ValidatorFactoryType = typeof(ServiceProviderValidatorFactory);
#pragma warning restore CS0618
			return this;
		}

		/// <summary>
		/// Registers all validators derived from AbstractValidator within the specified assemblies
		/// </summary>
		/// <param name="assemblies">The assemblies to scan</param>
		/// <param name="filter">Optional filter that allows certain types to be skipped from registration.</param>
		/// <param name="lifetime">The service lifetime that should be used for the validator registration. Defaults to Scoped</param>
		/// <param name="includeInternalTypes">Include internal validators. The default is false.</param>
		// [Obsolete("RegisterValidatorsFromAssemblies is deprecated. Call services.AddValidatorsFromAssemblies instead, which has the same effect. See https://github.com/FluentValidation/FluentValidation/issues/1963")]
		public FluentValidationMvcConfiguration RegisterValidatorsFromAssemblies(IEnumerable<Assembly> assemblies, Func<AssemblyScanner.AssemblyScanResult, bool> filter = null, ServiceLifetime lifetime = ServiceLifetime.Scoped, bool includeInternalTypes = false) {
			_services.AddValidatorsFromAssemblies(assemblies, lifetime, filter, includeInternalTypes);

#pragma warning disable CS0618
			ValidatorFactoryType = typeof(ServiceProviderValidatorFactory);
#pragma warning restore CS0618
			return this;
		}

		/// <summary>
		/// Configures clientside validation support
		/// </summary>
		/// <param name="clientsideConfig"></param>
		/// <param name="enabled">Whether clientside validation integration is enabled</param>
		/// <returns></returns>
		[Obsolete("ConfigureClientsideValidation is deprecated and will be removed in a future release. To configure client-side validation call services.AddFluentValidationClientsideAdapters(config => ...) instead. For details see https://github.com/FluentValidation/FluentValidation/issues/1965")]
		public FluentValidationMvcConfiguration ConfigureClientsideValidation(Action<FluentValidationClientModelValidatorProvider> clientsideConfig=null, bool enabled=true) {
			if (clientsideConfig != null) {
				ClientsideConfig = clientsideConfig;
			}
			ClientsideEnabled = enabled;
			return this;
		}
	}
}
