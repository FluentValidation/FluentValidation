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
// The latest version of this file can be found at http://fluentvalidation.codeplex.com
#endregion
namespace FluentValidation.Internal {
	using System;
	using System.Reflection;
	using Attributes;
	using Resources;

	/// <summary>
	/// Keeps all the conditional compilation in one place.
	/// </summary>
	internal static class Compatibility {
		public static PropertyInfo GetPublicStaticProperty(this Type type, string propertyName) {
#if PORTABLE || CoreCLR
            return type.GetRuntimeProperty(propertyName);
#else
			return type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static);
#endif
		}

		public static MethodInfo GetPublicInstanceMethod(this Type type, string name) {
#if PORTABLE || CoreCLR
			return type.GetRuntimeMethod(name, null);
#else
			return type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public);
#endif
		}

		public static PropertyInfo GetPublicInstanceProperty(this Type type, string name) {
#if PORTABLE || CoreCLR
			return type.GetRuntimeProperty(name);
#else
			return type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
#endif
		}

		public static Func<string> CreateGetter(this PropertyInfo property) {
			Func<string> accessor;
#if PORTABLE || CoreCLR
            accessor = (Func<string>)property.GetMethod.CreateDelegate(typeof(Func<string>), property.GetMethod);
#else
			accessor = (Func<string>)Delegate.CreateDelegate(typeof(Func<string>), property.GetGetMethod());
#endif
			return accessor;
		}

		public static bool CanAssignTo(this Type type, Type other) {
#if PORTABLE || CoreCLR
            return other.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
#else
			return other.IsAssignableFrom(type);
#endif 
		}

		public static ValidatorAttribute GetValidatorAttribute(this Type type) {
#if PORTABLE || CoreCLR
            var attribute = (ValidatorAttribute)type.GetTypeInfo().GetCustomAttribute<ValidatorAttribute>(true);
#else
			var attribute = (ValidatorAttribute)Attribute.GetCustomAttribute(type, typeof(ValidatorAttribute));
#endif
			return attribute;
		}

		public static Assembly GetAssembly(this Type type) {
#if PORTABLE || CoreCLR
                    return typeof(Messages).GetTypeInfo().Assembly;
#else
			return typeof(Messages).Assembly;
#endif
		}

		public static MethodInfo GetDeclaredMethod(this Type type, string name) {
#if PORTABLE || CoreCLR
            return type.GetTypeInfo().GetDeclaredMethod(name);
#else
			return type.GetMethod(name, new Type[0]);
#endif
		}

        public static bool IsGenericType(this Type type)
        {
#if PORTABLE || CoreCLR
            return type.GetTypeInfo().IsGenericType;
#else 
            return type.IsGenericType;
#endif
        }
	}
}