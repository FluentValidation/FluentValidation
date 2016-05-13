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
// The latest version of this file can be found at https://github.com/jeremyskinner/FluentValidation
# endregion
namespace System.Reflection {
	using System;

	public static class Net40Compatibility {
		public static TypeInfo GetTypeInfo(this Type type) {
			return new TypeInfo(type);
		}

		public static PropertyInfo GetRuntimeProperty(this Type type, string propertyName)
		{
			return type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
		}

		public static MethodInfo GetRuntimeMethod(this Type type, string name, Type[] parameters)
		{
			return type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public);
		}

		public static TAttribute GetCustomAttribute<TAttribute>(this ParameterInfo parameterInfo) where TAttribute : Attribute {
			return (TAttribute)Attribute.GetCustomAttribute(parameterInfo, typeof(TAttribute));
		}
	}

	public class TypeInfo {
		private Type _type;

		public TypeInfo(Type type) {
			_type = type;
		}

		public bool IsEnum => _type.IsEnum;
		public bool IsGenericType => _type.IsGenericType;

		public MethodInfo GetDeclaredMethod(string name) {
			return _type.GetMethod(name, new Type[0]);
		}

		public Assembly Assembly => _type.Assembly;


		public TAttribute GetCustomAttribute<TAttribute>() where TAttribute : Attribute {
			return (TAttribute)Attribute.GetCustomAttribute(_type, typeof(TAttribute));
		}

		public bool IsAssignableFrom(TypeInfo other) {
			return _type.IsAssignableFrom(other._type);
		}
	}
}