namespace FluentValidation.Internal {
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Reflection;

	/// <summary>
	/// Member accessor cache.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public static class AccessorCache<T> {
		private static readonly Dictionary<Key, Delegate> _cache = new Dictionary<Key, Delegate>();
		private static readonly object _locker = new object();

		/// <summary>
		/// Gets an accessor func based on an expression
		/// </summary>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="member">The member represented by the expression</param>
		/// <param name="expression"></param>
		/// <returns>Accessor func</returns>
		public static Func<T, TProperty> GetCachedAccessor<TProperty>(MemberInfo member, Expression<Func<T, TProperty>> expression) {
			if (member == null || ValidatorOptions.DisableAccessorCache) {
				return expression.Compile();
			}
			
			Delegate result;

			lock (_locker) {
				var key = new Key(member, expression);
				if (_cache.TryGetValue(key, out result)) {
					return (Func<T, TProperty>)result;
				}

				var func = expression.Compile();
				_cache[key] = func;
				return func;
			}
		}

		public static void Clear() {
			lock (_locker) {
				_cache.Clear();
			}
		}

		private class Key {
			private readonly MemberInfo memberInfo;
			private readonly string expressionDebugView;

			public Key(MemberInfo member, Expression expression) {
				memberInfo = member;
				expressionDebugView = expression.ToString();
			}

			protected bool Equals(Key other) {
				return Equals(memberInfo, other.memberInfo) && string.Equals(expressionDebugView, other.expressionDebugView);
			}

			public override bool Equals(object obj) {
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != this.GetType()) return false;
				return Equals((Key) obj);
			}

			public override int GetHashCode() {
				unchecked {
					return ((memberInfo != null ? memberInfo.GetHashCode() : 0)*397) ^ (expressionDebugView != null ? expressionDebugView.GetHashCode() : 0);
				}
			}
		}

	}
}