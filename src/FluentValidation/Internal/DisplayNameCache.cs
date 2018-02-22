namespace FluentValidation.Internal {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

#if !NETSTANDARD1_0
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel;
#endif

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

#if NETSTANDARD1_0
		// Nasty hack to work around not referencing DataAnnotations directly. 
		// At some point investigate the DataAnnotations reference issue in more detail and go back to using the code above. 
		static Func<string> GetDisplayName(MemberInfo member) {
			var attributes = (from attr in member.GetCustomAttributes(true)
				select new { attr, type = attr.GetType() }).ToList();

			Func<string> name = (from attr in attributes
				where attr.type.Name == "DisplayAttribute"
				let method = attr.type.GetRuntimeMethod("GetName", new Type[0])
				where method != null
				select new Func<string>(() => method.Invoke(attr.attr, null) as string)).FirstOrDefault();

			if (name == null) {
				name = (from attr in attributes
					where attr.type.Name == "DisplayNameAttribute"
					let property = attr.type.GetRuntimeProperty("DisplayName")
					where property != null
					select new Func<string>(() => property.GetValue(attr.attr, null) as string)).FirstOrDefault();
			}

			return name;
		}


#else
		static Func<string> GetDisplayName(MemberInfo member) {

			if (member == null) return null;

			var displayAttribute = (DisplayAttribute)Attribute.GetCustomAttribute(member, typeof(DisplayAttribute));

			if (displayAttribute != null) {
				return () => displayAttribute.GetName();
			}

			// Couldn't find a name from a DisplayAttribute. Try DisplayNameAttribute instead.
			var displayNameAttribute = (DisplayNameAttribute)Attribute.GetCustomAttribute(member, typeof(DisplayNameAttribute));
			if (displayNameAttribute != null) {
				return () => displayNameAttribute.DisplayName;
			}
			

			return null;

		}
#endif
	}
}