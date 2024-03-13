namespace FluentValidation.Internal;

using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

/// <summary>
/// Member accessor cache.
/// </summary>
/// <typeparam name="T">The type for which to cache member accessors.</typeparam>
public static class AccessorCache<T> {
	private static readonly ConcurrentDictionary<Key, Delegate> _cache = new();

	/// <summary>
	/// Gets an accessor func based on an expression.
	/// </summary>
	/// <typeparam name="TProperty">The type of property.</typeparam>
	/// <param name="member">The member represented by the expression</param>
	/// <param name="expression">The accessor expression.</param>
	/// <param name="bypassCache"><see langword="true"/> to bypass the cache, <see langword="false"/> by default.</param>
	/// <param name="cachePrefix">The cache prefix used to distinguish between collection and non-collection access.</param>
	/// <returns>An accessor func.</returns>
	public static Func<T, TProperty> GetCachedAccessor<TProperty>(MemberInfo member, Expression<Func<T, TProperty>> expression, bool bypassCache = false, string cachePrefix = null) {
		if (bypassCache || ValidatorOptions.Global.DisableAccessorCache) {
			return expression.Compile();
		}

		Key key;

		if (member == null) {
			// If the expression doesn't reference a property we don't support
			// caching it, The only exception is parameter expressions referencing the same object (eg RuleFor(x => x))
			if (expression.IsParameterExpression() && typeof(T) == typeof(TProperty)) {
				key = new Key(null, expression, typeof(T).FullName + ":" + cachePrefix);
			}
			else {
				// Unsupported expression type. Non cacheable.
				return expression.Compile();
			}
		}
		else {
			key = new Key(member, expression, cachePrefix);
		}

		return (Func<T,TProperty>)_cache.GetOrAdd(key, static (_, exp) => exp.Compile(), expression);
	}

	public static void Clear() {
		_cache.Clear();
	}


	/// <summary>
	/// Represents a unique cache key.
	/// </summary>
	private class Key {
		private readonly MemberInfo _memberInfo;
		/// <remarks>
		/// The expression key ensures that the accessor is not shared between collection and non-collection access.
		/// Collection and non-collection access must use different accessors or a runtime exception will occur.
		/// </remarks>
		private readonly string _expressionKey;

		public Key(MemberInfo member, Expression expression, string cachePrefix) {
			_memberInfo = member;
			_expressionKey = cachePrefix != null ? cachePrefix + expression.ToString() : expression.ToString();
		}

		public bool Equals(Key other) {
			return Equals(_memberInfo, other._memberInfo) && string.Equals(_expressionKey, other._expressionKey);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Key) obj);
		}

		public override int GetHashCode() {
			unchecked {
				return ((_memberInfo != null ? _memberInfo.GetHashCode() : 0)*397) ^ (_expressionKey != null ? _expressionKey.GetHashCode() : 0);
			}
		}
	}

}
