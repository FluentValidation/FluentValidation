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
// The latest version of this file can be found at http://www.codeplex.com/FluentValidation
#endregion

namespace FluentValidation {
	using System;
#if !WINDOWS_PHONE
	using System.ComponentModel;
	//using System.ComponentModel.DataAnnotations;
#endif
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using Internal;

	public static class ValidatorOptions {
		public static CascadeMode CascadeMode = CascadeMode.Continue;
		public static Type ResourceProviderType;

		private static Func<Type, MemberInfo, LambdaExpression, string> propertyNameResolver = DefaultPropertyNameResolver;
		private static Func<Type, MemberInfo, LambdaExpression, string> displayNameResolver = DefaultDisplayNameResolver;

		public static Func<Type, MemberInfo, LambdaExpression, string> PropertyNameResolver {
			get { return propertyNameResolver; }
			set { propertyNameResolver = value ?? DefaultPropertyNameResolver; }
		}

		public static Func<Type, MemberInfo, LambdaExpression, string> DisplayNameResolver {
			get { return displayNameResolver; }
			set { displayNameResolver = value ?? DefaultDisplayNameResolver; }
		}

		static string DefaultPropertyNameResolver(Type type, MemberInfo memberInfo, LambdaExpression expression) {
			if (expression != null) {
				var chain = PropertyChain.FromExpression(expression);
				if (chain.Count > 0) return chain.ToString();
			}

			if (memberInfo != null) {
				return memberInfo.Name;
			}

			return null;
		}	
		
		static string DefaultDisplayNameResolver(Type type, MemberInfo memberInfo, LambdaExpression expression) {
			if (memberInfo == null) return null;
		    return GetDisplayName(memberInfo);
		    /*string name = null;
#if !WINDOWS_PHONE
			var displayAttribute = (DisplayAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DisplayAttribute));

			if(displayAttribute != null) {
				name = displayAttribute.GetName();
			}
#endif

#if !SILVERLIGHT
			// Silverlight doesn't have DisplayAttribute.
			if(string.IsNullOrEmpty(name)) {
				// Couldn't find a name from a DisplayAttribute. Try DisplayNameAttribute instead.
				var displayNameAttribute = (DisplayNameAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(DisplayNameAttribute));
				if(displayNameAttribute != null) {
					name = displayNameAttribute.DisplayName;
				}
			}
#endif
			return name;*/
		}

		// Nasty hack to work around not referencing DataAnnotations directly. 
		// At some point investigate the DataAnnotations reference issue in more detail and go back to using the code above. 
		static string GetDisplayName(MemberInfo member) {
			var attributes = (from attr in member.GetCustomAttributes(true)
			                  select new {attr, type = attr.GetType()}).ToList();

			string name = null;

#if !WINDOWS_PHONE
			name = (from attr in attributes
			        where attr.type.Name == "DisplayAttribute"
			        let method = attr.type.GetMethod("GetName", BindingFlags.Instance | BindingFlags.Public)
			        where method != null
			        select method.Invoke(attr.attr, null) as string).FirstOrDefault();
#endif

#if !SILVERLIGHT
			if (string.IsNullOrEmpty(name)) {
				name = (from attr in attributes
				        where attr.type.Name == "DisplayNameAttribute"
				        let property = attr.type.GetProperty("DisplayName", BindingFlags.Instance | BindingFlags.Public)
				        where property != null
				        select property.GetValue(attr.attr, null) as string).FirstOrDefault();
			}
#endif

			return name;
		}
	}
}