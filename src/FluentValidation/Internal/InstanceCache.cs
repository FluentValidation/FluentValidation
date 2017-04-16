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
// The latest version of this file can be found at https://github.com/JeremySkinner/FluentValidation
#endregion

namespace FluentValidation.Internal {
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Reflection;

	/// <summary>
	/// Instancace cache.
	/// </summary>
	public class InstanceCache {
		readonly Dictionary<Type, object> cache = new Dictionary<Type, object>();
		readonly object locker = new object();

		/// <summary>
		/// Gets or creates an instance using Activator.CreateInstance
		/// </summary>
		/// <param name="type">The type to instantiate</param>
		/// <returns>The instantiated object</returns>
		public object GetOrCreateInstance(Type type) {
			return GetOrCreateInstance(type, Activator.CreateInstance);
		}

		/// <summary>
		/// Gets or creates an instance using a custom factory
		/// </summary>
		/// <param name="type">The type to instantiate</param>
		/// <param name="factory">The custom factory</param>
		/// <returns>The instantiated object</returns>
		public object GetOrCreateInstance(Type type, Func<Type, object> factory) {
			object existingInstance;

			lock(locker) {
				if (cache.TryGetValue(type, out existingInstance)) {
					return existingInstance;
				}

				var newInstance = factory(type);
				cache[type] = newInstance;
				return newInstance;
			}
		}
	}

	/// <summary>
	/// Member accessor cache.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public static class AccessorCache<T>
	{
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
			if (member == null)
			{
				return null;
			}
 			Delegate func;

			lock (_locker) {
                var key = new Key(member, expression);
				if (_cache.TryGetValue(key, out func))
				{
					return (Func<T, TProperty>)func;
				}

				func = expression.Compile();
				_cache[key] = func;
				return (Func<T, TProperty>)func;
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