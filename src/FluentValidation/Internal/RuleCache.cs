#region License

// Copyright (c) Jeremy Skinner (http://www.jeremyskinner.co.uk)
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
// The latest version of this file can be found at https://github.com/jeremyskinner/FluentValidation

#endregion
namespace FluentValidation.Internal {
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// Cache for validation rules.
	/// </summary>
	public static class RuleCache {
		private static ConcurrentDictionary<Type, List<IValidationRule>> _rules = new ConcurrentDictionary<Type, List<IValidationRule>>();

		/// <summary>
		/// Get all rules for a validator type in the cache.
		/// If the cache has not yet been initialized for this validator type,
		/// the specified factory is run to create them.
		/// </summary>
		/// <param name="type">Validator type</param>
		/// <param name="ruleFactory">Rule factory</param>
		/// <returns></returns>
		public static List<IValidationRule> GetRules(Type type, Func<List<IValidationRule>> ruleFactory) {
			type.Guard("Validator type must be specified.", nameof(type));
			ruleFactory.Guard("ruleFactory must be specified", nameof(ruleFactory));
			return _rules.GetOrAdd(type, t => ruleFactory());
		}

		/// <summary>
		/// Tries to get the rules for a specified validator type.
		/// </summary>
		/// <param name="type">The validator type.</param>
		/// <param name="rules">Output parameter containing the cached rules. This will be null if the cache has not yet been initialized for this validator.</param>
		/// <returns>A boolean indicating whether or not there was a cached entry for this validator type.</returns>
		public static bool TryGetRules(Type type, out List<IValidationRule> rules) {
			type.Guard("Validator type must be specified.", nameof(type));
			var result = _rules.TryGetValue(type, out var r);
			rules = r;
			return result;
		}

		/// <summary>
		/// Clears the cache.
		/// </summary>
		public static void Clear() {
			_rules.Clear();
		}
	}
}