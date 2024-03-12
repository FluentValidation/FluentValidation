namespace FluentValidation.Internal;

using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

/// <summary>
/// Member accessor cache.
/// </summary>
/// <typeparam name="T"></typeparam>
public static class AccessorCache<T> {
	private static readonly ConcurrentDictionary<Key, Delegate> _cache = new();

	/// <summary>
	/// Gets an accessor func based on an expression
	/// </summary>
	/// <typeparam name="TProperty"></typeparam>
	/// <param name="member">The member represented by the expression</param>
	/// <param name="expression"></param>
	/// <param name="bypassCache"></param>
	/// <param name="cachePrefix">Cache prefix</param>
	/// <returns>Accessor func</returns>
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
#if NET5_0_OR_GREATER
		return (Func<T,TProperty>)_cache.GetOrAdd(key, static (_, exp) => exp.Compile(), expression);
#else
		return (Func<T,TProperty>)_cache.GetOrAdd(key, k => expression.Compile());
#endif
	}

	public static void Clear() {
		_cache.Clear();
	}

	private class Key {
		private readonly MemberInfo _memberInfo;
		private readonly string _expressionString;
#if DEBUG
		private readonly string _expressionDebugView;
#endif

		public Key(MemberInfo member, Expression expression, string cachePrefix) {
			_memberInfo = member;
			_expressionString = expression.ToString();
#if DEBUG
			_expressionDebugView = cachePrefix != null ? cachePrefix + _expressionString : _expressionString;
#endif
		}

		private bool Equals(Key other) {
			return Equals(_memberInfo, other._memberInfo) && string.Equals(_expressionString, other._expressionString);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Key) obj);
		}

		public override int GetHashCode() {
			unchecked {
				unchecked
				{
					var hashCode = (_memberInfo != null ? _memberInfo.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (_expressionString != null ? _expressionString.GetHashCode() : 0);
					return hashCode;
				}
			}
		}
	}

}
