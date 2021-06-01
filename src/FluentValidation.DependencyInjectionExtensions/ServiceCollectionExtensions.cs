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

namespace FluentValidation {
	using Microsoft.Extensions.DependencyInjection;
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	public static class ServiceCollectionExtensions	{
		/// <summary>
		/// Adds all validators in specified assemblies
		/// </summary>
		/// <param name="services">The collection of services</param>
		/// <param name="assemblies">The assemblies to scan</param>
		/// <param name="lifetime">The lifetime of the validators. The default is scoped (per-request in web applications)</param>
		/// <param name="filter">Optional filter that allows certain types to be skipped from registration.</param>
		/// <param name="includeInternalTypes">Include internal validators. The default is false.</param>
		/// <returns></returns>
		public static IServiceCollection AddValidatorsFromAssemblies(this IServiceCollection services, IEnumerable<Assembly> assemblies, ServiceLifetime lifetime = ServiceLifetime.Scoped, Func<AssemblyScanner.AssemblyScanResult, bool> filter = null, bool includeInternalTypes = false) {
			foreach (var assembly in assemblies)
				services.AddValidatorsFromAssembly(assembly, lifetime, filter, includeInternalTypes);

			return services;
		}

		/// <summary>
		/// Adds all validators in specified assembly
		/// </summary>
		/// <param name="services">The collection of services</param>
		/// <param name="assembly">The assembly to scan</param>
		/// <param name="lifetime">The lifetime of the validators. The default is scoped (per-request in web application)</param>
		/// <param name="filter">Optional filter that allows certain types to be skipped from registration.</param>
		/// <param name="includeInternalTypes">Include internal validators. The default is false.</param>
		/// <returns></returns>
		public static IServiceCollection AddValidatorsFromAssembly(this IServiceCollection services, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Scoped, Func<AssemblyScanner.AssemblyScanResult, bool> filter = null, bool includeInternalTypes = false) {
			AssemblyScanner
				.FindValidatorsInAssembly(assembly, includeInternalTypes)
				.ForEach(scanResult => services.AddScanResult(scanResult, lifetime, filter));

			return services;
		}

		/// <summary>
		/// Adds all validators in the assembly of the specified type
		/// </summary>
		/// <param name="services">The collection of services</param>
		/// <param name="type">The type whose assembly to scan</param>
		/// <param name="lifetime">The lifetime of the validators. The default is scoped (per-request in web applications)</param>
		/// <param name="filter">Optional filter that allows certain types to be skipped from registration.</param>
		/// <param name="includeInternalTypes">Include internal validators. The default is false.</param>
		/// <returns></returns>
		public static IServiceCollection AddValidatorsFromAssemblyContaining(this IServiceCollection services, Type type, ServiceLifetime lifetime = ServiceLifetime.Scoped, Func<AssemblyScanner.AssemblyScanResult, bool> filter = null, bool includeInternalTypes = false)
			=> services.AddValidatorsFromAssembly(type.Assembly, lifetime, filter, includeInternalTypes);

		/// <summary>
		/// Adds all validators in the assembly of the type specified by the generic parameter
		/// </summary>
		/// <param name="services">The collection of services</param>
		/// <param name="lifetime">The lifetime of the validators. The default is scoped (per-request in web applications)</param>
		/// <param name="filter">Optional filter that allows certain types to be skipped from registration.</param>
		/// <param name="includeInternalTypes">Include internal validators. The default is false.</param>
		/// <returns></returns>
		public static IServiceCollection AddValidatorsFromAssemblyContaining<T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped, Func<AssemblyScanner.AssemblyScanResult, bool> filter = null, bool includeInternalTypes = false)
			=> services.AddValidatorsFromAssembly(typeof(T).Assembly, lifetime, filter, includeInternalTypes);

		/// <summary>
		/// Helper method to register a validator from an AssemblyScanner result
		/// </summary>
		/// <param name="services">The collection of services</param>
		/// <param name="scanResult">The scan result</param>
		/// <param name="lifetime">The lifetime of the validators. The default is scoped (per-request in web applications)</param>
		/// <param name="filter">Optional filter that allows certain types to be skipped from registration.</param>
		/// <returns></returns>
		private static IServiceCollection AddScanResult(this IServiceCollection services, AssemblyScanner.AssemblyScanResult scanResult, ServiceLifetime lifetime, Func<AssemblyScanner.AssemblyScanResult, bool> filter) {
			bool shouldRegister = filter?.Invoke(scanResult) ?? true;
			if (shouldRegister) {
				//Register as interface
				services.Add(
					new ServiceDescriptor(
						serviceType: scanResult.InterfaceType,
						implementationType: scanResult.ValidatorType,
						lifetime: lifetime));

				//Register as self
				services.Add(
					new ServiceDescriptor(
						serviceType: scanResult.ValidatorType,
						implementationType: scanResult.ValidatorType,
						lifetime: lifetime));
			}

			return services;
		}

		#region Backwards compatibility overloads
		//TODO: Remove in FV11
		/// <summary>
		/// Adds all validators in specified assemblies
		/// </summary>
		/// <param name="services">The collection of services</param>
		/// <param name="assemblies">The assemblies to scan</param>
		/// <param name="lifetime">The lifetime of the validators. The default is scoped (per-request in web applications)</param>
		/// <param name="filter">Optional filter that allows certain types to be skipped from registration.</param>
		/// <returns></returns>
		public static IServiceCollection AddValidatorsFromAssemblies(this IServiceCollection services, IEnumerable<Assembly> assemblies, ServiceLifetime lifetime, Func<AssemblyScanner.AssemblyScanResult, bool> filter)
			=> AddValidatorsFromAssemblies(services, assemblies, lifetime, filter, false);

		/// <summary>
		/// Adds all validators in specified assembly
		/// </summary>
		/// <param name="services">The collection of services</param>
		/// <param name="assembly">The assembly to scan</param>
		/// <param name="lifetime">The lifetime of the validators. The default is scoped (per-request in web application)</param>
		/// <param name="filter">Optional filter that allows certain types to be skipped from registration.</param>
		/// <returns></returns>
		public static IServiceCollection AddValidatorsFromAssembly(this IServiceCollection services, Assembly assembly, ServiceLifetime lifetime, Func<AssemblyScanner.AssemblyScanResult, bool> filter)
			=> AddValidatorsFromAssembly(services, assembly, lifetime, filter, false);


		/// <summary>
		/// Adds all validators in the assembly of the specified type
		/// </summary>
		/// <param name="services">The collection of services</param>
		/// <param name="type">The type whose assembly to scan</param>
		/// <param name="lifetime">The lifetime of the validators. The default is scoped (per-request in web applications)</param>
		/// <param name="filter">Optional filter that allows certain types to be skipped from registration.</param>
		/// <returns></returns>
		public static IServiceCollection AddValidatorsFromAssemblyContaining(this IServiceCollection services, Type type, ServiceLifetime lifetime, Func<AssemblyScanner.AssemblyScanResult, bool> filter)
			=> AddValidatorsFromAssemblyContaining(services, type, lifetime, filter, false);


		/// <summary>
		/// Adds all validators in the assembly of the type specified by the generic parameter
		/// </summary>
		/// <param name="services">The collection of services</param>
		/// <param name="lifetime">The lifetime of the validators. The default is scoped (per-request in web applications)</param>
		/// <param name="filter">Optional filter that allows certain types to be skipped from registration.</param>
		/// <returns></returns>
		public static IServiceCollection AddValidatorsFromAssemblyContaining<T>(this IServiceCollection services, ServiceLifetime lifetime, Func<AssemblyScanner.AssemblyScanResult, bool> filter)
			=> AddValidatorsFromAssemblyContaining<T>(services, lifetime, filter, false);

		#endregion
	}
}
