using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FluentValidation
{
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Adds all validators in specified assembly
		/// </summary>
		/// <param name="services">The collection of services</param>
		/// <param name="assembly">The assembly to scan</param>
		/// <param name="lifetime">The lifetime the validator is registered. The default is transient</param>
		/// <returns></returns>
		public static IServiceCollection AddValidatorsFromAssembly(this IServiceCollection services, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Transient)
		{
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
		/// <param name="lifetime">The lifetime the validator is registered. The default is transient</param>
		/// <returns></returns>
		public static IServiceCollection AddValidatorsFromAssemblyContaining(this IServiceCollection services, Type type, ServiceLifetime lifetime = ServiceLifetime.Transient)
			=> services.AddValidatorsFromAssembly(type.Assembly, lifetime);

		/// <summary>
		/// Adds all validators in the assembly of the type specified by the generic parameter
		/// </summary>
		/// <param name="services">The collection of services</param>
		/// <param name="lifetime">The lifetime the validator is registered. The default is transient</param>
		/// <returns></returns>
		public static IServiceCollection AddValidatorsFromAssemblyContaining<T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
			=> services.AddValidatorsFromAssembly(typeof(T).Assembly, lifetime);

		/// <summary>
		/// Helper method to register a validator from an AssemblyScanner result
		/// </summary>
		/// <param name="services">The collection of services</param>
		/// <param name="scanResult">The scan result</param>
		/// <param name="lifetime">The lifetime the validator is registered. The default is transient</param>
		/// <returns></returns>
		private static IServiceCollection AddScanResult(this IServiceCollection services, AssemblyScanner.AssemblyScanResult scanResult, ServiceLifetime lifetime)
		{
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