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
		/// <param name="assembly">The assemblies to scan</param>
		/// <param name="lifetime">The lifetime of the validators. The default is transient</param>
		/// <returns></returns>
		public static IServiceCollection AddValidatorsFromAssemblies(this IServiceCollection services, IEnumerable<Assembly> assemblies, ServiceLifetime lifetime = ServiceLifetime.Transient) {
			foreach (var assembly in assemblies)
				services.AddValidatorsFromAssembly(assembly, lifetime);

			return services;
		}

		/// <summary>
		/// Adds all validators in specified assembly
		/// </summary>
		/// <param name="services">The collection of services</param>
		/// <param name="assembly">The assembly to scan</param>
		/// <param name="lifetime">The lifetime of the validators. The default is transient</param>
		/// <returns></returns>
		public static IServiceCollection AddValidatorsFromAssembly(this IServiceCollection services, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Transient) {
			AssemblyScanner
				.FindValidatorsInAssembly(assembly)
				.ForEach(scanResult => services.AddScanResult(scanResult, lifetime));

			return services;
		}

		/// <summary>
		/// Adds all validators in the assembly of the specified type
		/// </summary>
		/// <param name="services">The collection of services</param>
		/// <param name="type">The type whose assembly to scan</param>
		/// <param name="lifetime">The lifetime of the validators. The default is transient</param>
		/// <returns></returns>
		public static IServiceCollection AddValidatorsFromAssemblyContaining(this IServiceCollection services, Type type, ServiceLifetime lifetime = ServiceLifetime.Transient)
			=> services.AddValidatorsFromAssembly(type.Assembly, lifetime);

		/// <summary>
		/// Adds all validators in the assembly of the type specified by the generic parameter
		/// </summary>
		/// <param name="services">The collection of services</param>
		/// <param name="lifetime">The lifetime of the validators. The default is transient</param>
		/// <returns></returns>
		public static IServiceCollection AddValidatorsFromAssemblyContaining<T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
			=> services.AddValidatorsFromAssembly(typeof(T).Assembly, lifetime);

		/// <summary>
		/// Helper method to register a validator from an AssemblyScanner result
		/// </summary>
		/// <param name="services">The collection of services</param>
		/// <param name="scanResult">The scan result</param>
		/// <param name="lifetime">The lifetime of the validators. The default is transient</param>
		/// <returns></returns>
		private static IServiceCollection AddScanResult(this IServiceCollection services, AssemblyScanner.AssemblyScanResult scanResult, ServiceLifetime lifetime) {
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

			return services;
		}
	}
}
