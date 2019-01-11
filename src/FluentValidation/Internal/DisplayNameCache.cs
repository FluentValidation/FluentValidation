namespace FluentValidation.Internal {
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.ComponentModel;

	/// <summary>
	/// Display name cache.
	/// </summary>
	internal static class DisplayNameCache {
		private static readonly ConcurrentDictionary<MemberInfo, Func<string>> _cache = new ConcurrentDictionary<MemberInfo, Func<string>>();

		public static string GetCachedDisplayName(MemberInfo member) {
			var result = _cache.GetOrAdd(member, GetDisplayName);
			return result?.Invoke();
		}

		public static void Clear() {
			_cache.Clear();
		}

		static Func<string> GetDisplayName(MemberInfo member) {

			if (member == null) return null;

			var displayAttribute = member.GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>();

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