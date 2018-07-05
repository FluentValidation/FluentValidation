namespace FluentValidation.Internal {
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;

	public static class RuleCache {
		private static ConcurrentDictionary<Type, List<IValidationRule>> _rules = new ConcurrentDictionary<Type, List<IValidationRule>>();

		public static List<IValidationRule> GetRules(Type type, Func<List<IValidationRule>> ruleFactory) {
			return _rules.GetOrAdd(type, t => ruleFactory());
		}

		public static bool TryGetRules(Type type, out IEnumerable<IValidationRule> rules) {
			var result = _rules.TryGetValue(type, out var r);
			rules = r;
			return result;
		}

		public static void Clear() {
			_rules.Clear();
		}
	}
}