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

#endregion License

namespace FluentValidation.Internal
{
	using Attributes;
	using System;
	using System.Reflection;

	/// <summary>
	/// Keeps all the conditional compilation in one place.
	/// </summary>
	internal static class Compatibility
	{
#if PORTABLE40

		public static PropertyInfo GetRuntimeProperty(this Type type, string propertyName)
		{
			return type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
		}

		public static MethodInfo GetRuntimeMethod(this Type type, string name, Type[] parameters)
		{
			return type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public);
		}

#endif

#if PORTABLE || CoreCLR

		public static bool IsAssignableFrom(this Type type, Type otherType)
		{
			return type.GetTypeInfo().IsAssignableFrom(otherType.GetTypeInfo());
		}

#endif

		public static Func<string> CreateGetter(this PropertyInfo property)
		{
#if PORTABLE || CoreCLR
			return (Func<string>)property.GetMethod.CreateDelegate(typeof(Func<string>));
#else
			return (Func<string>)Delegate.CreateDelegate(typeof(Func<string>), property.GetGetMethod());
#endif
		}

		public static ValidatorAttribute GetValidatorAttribute(this Type type)
		{
#if PORTABLE || CoreCLR
			return type.GetTypeInfo().GetCustomAttribute<ValidatorAttribute>(true);
#else
			return (ValidatorAttribute)Attribute.GetCustomAttribute(type, typeof(ValidatorAttribute));
#endif
		}

		public static ValidatorAttribute GetValidatorAttribute(this ParameterInfo parameterInfo)
		{
#if PORTABLE || CoreCLR
			return parameterInfo.GetCustomAttribute<ValidatorAttribute>(true);
#else
			return (ValidatorAttribute)Attribute.GetCustomAttribute(parameterInfo, typeof(ValidatorAttribute));
#endif
		}

		public static Assembly GetAssembly(this Type type)
		{
#if PORTABLE || CoreCLR
			return type.GetTypeInfo().Assembly;
#else
			return type.Assembly;
#endif
		}

		public static MethodInfo GetDeclaredMethod(this Type type, string name)
		{
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