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
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Internal;

	/// <summary>
	/// Class that can be used to find all the validators from a collection of types.
	/// </summary>
	public class AssemblyScanner : IEnumerable<AssemblyScanner.AssemblyScanResult> {
		readonly IEnumerable<Type> _types;

		/// <summary>
		/// Creates a scanner that works on a sequence of types.
		/// </summary>
		public AssemblyScanner(IEnumerable<Type> types) {
			_types = types;
		}

		/// <summary>
		/// Finds all the validators in the specified assembly.
		/// </summary>
		/// <param name="assembly">The assembly to scan</param>
		/// <param name="includeInternalTypes">Whether to include internal validators in the search as well as public validators. The default is false.</param>
		public static AssemblyScanner FindValidatorsInAssembly(Assembly assembly, bool includeInternalTypes = false) {
			return new AssemblyScanner(includeInternalTypes ? assembly.GetTypes() : assembly.GetExportedTypes());
		}

		//TODO: Backwards compat. overload. Remove in 11.0.
		/// <summary>
		/// Finds all public validators in the specified assembly.
		/// </summary>
		public static AssemblyScanner FindValidatorsInAssembly(Assembly assembly) {
			return new AssemblyScanner(assembly.GetExportedTypes());
		}

		/// <summary>
		/// Finds all the validators in the specified assemblies.
		/// </summary>
		/// <param name="assemblies">The assemblies to scan</param>
		/// <param name="includeInternalTypes">Whether to include internal validators as well as public validators. The default is false.</param>
		public static AssemblyScanner FindValidatorsInAssemblies(IEnumerable<Assembly> assemblies, bool includeInternalTypes = false) {
			var types = assemblies.SelectMany(x => includeInternalTypes ? x.GetTypes() : x.GetExportedTypes()).Distinct();
			return new AssemblyScanner(types);
		}

		//TODO: Backwards compat. overload. Remove in 11.0.
		/// <summary>
		/// Finds all public validators in the specified assemblies
		/// </summary>
		/// <param name="assemblies">The assemblies to scan</param>
		public static AssemblyScanner FindValidatorsInAssemblies(IEnumerable<Assembly> assemblies) {
			var types = assemblies.SelectMany(x => x.GetExportedTypes()).Distinct();
			return new AssemblyScanner(types);
		}

		/// <summary>
		/// Finds all the validators in the assembly containing the specified type.
		/// </summary>
		public static AssemblyScanner FindValidatorsInAssemblyContaining<T>() {
			return FindValidatorsInAssembly(typeof(T).Assembly);
		}

		/// <summary>
		/// Finds all the validators in the assembly containing the specified type.
		/// </summary>
		public static AssemblyScanner FindValidatorsInAssemblyContaining(Type type) {
			return FindValidatorsInAssembly(type.Assembly);
		}

		private IEnumerable<AssemblyScanResult> Execute() {
			var openGenericType = typeof(IValidator<>);

			var query = from type in _types
									where !type.IsAbstract && !type.IsGenericTypeDefinition
									let interfaces = type.GetInterfaces()
									let genericInterfaces = interfaces.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGenericType)
									let matchingInterface = genericInterfaces.FirstOrDefault()
									where matchingInterface != null
									select new AssemblyScanResult(matchingInterface, type);

			return query;
		}

		/// <summary>
		/// Performs the specified action to all of the assembly scan results.
		/// </summary>
		public void ForEach(Action<AssemblyScanResult> action) {
			foreach (var result in this) {
				action(result);
			}
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public IEnumerator<AssemblyScanResult> GetEnumerator() {
			return Execute().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		/// <summary>
		/// Result of performing a scan.
		/// </summary>
		public class AssemblyScanResult {
			/// <summary>
			/// Creates an instance of an AssemblyScanResult.
			/// </summary>
			public AssemblyScanResult(Type interfaceType, Type validatorType) {
				InterfaceType = interfaceType;
				ValidatorType = validatorType;
			}

			/// <summary>
			/// Validator interface type, eg IValidator&lt;Foo&gt;
			/// </summary>
			public Type InterfaceType { get; private set; }

			/// <summary>
			/// Concrete type that implements the InterfaceType, eg FooValidator.
			/// </summary>
			public Type ValidatorType { get; private set; }
		}
	}
}
