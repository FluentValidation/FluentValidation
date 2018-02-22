namespace FluentValidation.Internal {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel;

	/// <summary>
	/// Display name cache.
	/// </summary>
	internal static class DisplayNameCache {
		private static readonly Dictionary<MemberInfo, Func<string>> _cache = new Dictionary<MemberInfo, Func<string>>();
		private static readonly object _locker = new object();

		public static string GetCachedDisplayName(MemberInfo member) {
			Func<string> result;

			lock (_locker) {
				if (_cache.TryGetValue(member, out result)) {
					return result?.Invoke();
				}

				Func<string> displayNameFunc = GetDisplayName(member);
				_cache[member] = displayNameFunc;
				return displayNameFunc?.Invoke();
			}
		}

		public static void Clear() {
			lock (_locker) {
				_cache.Clear();
			}
		}

		static Func<string> GetDisplayName(MemberInfo member) {

			if (member == null) return null;

			var displayAttribute = member.GetCustomAttribute<DisplayAttribute>();

			if (displayAttribute != null) {
				return () => displayAttribute.GetName();
			}

			// Couldn't find a name from a DisplayAttribute. Try DisplayNameAttribute instead.
			var displayNameAttribute = member.GetCustomAttribute<DisplayNameAttribute>();

			if (displayNameAttribute != null) {
				return () => displayNameAttribute.DisplayName;
			}
			
			return null;

		}
	}
}