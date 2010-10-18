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
	using System.Reflection;

	public static class ValidatorOptions {
		public static CascadeMode CascadeMode = CascadeMode.Continue;
		public static Type ResourceProviderType;

		private static Func<Type, MemberInfo, string> propertyNameResolver = DefaultPropertyNameResolver;

		public static Func<Type, MemberInfo, string> PropertyNameResolver {
			get { return propertyNameResolver; }
			set { propertyNameResolver = value ?? DefaultPropertyNameResolver; }
		}

		static string DefaultPropertyNameResolver(Type type, MemberInfo memberInfo) {
			if(memberInfo != null) {
				return memberInfo.Name;
			}

			return null;
		}
	}
}