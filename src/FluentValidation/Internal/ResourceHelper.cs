#region License
// Copyright 2008-2009 Jeremy Skinner (http://www.jeremyskinner.co.uk)
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

namespace FluentValidation.Internal {
	using System;
	using System.Linq.Expressions;
	using System.Reflection;
	using Resources;

	internal class ResourceHelper {
		private static readonly Type defaultResourceType = typeof(Messages);

		public static Func<string> BuildResourceAccessor(string resourceName, Type resourceType) {
			PropertyInfo property = null;

			if (resourceType == defaultResourceType && ValidatorOptions.ResourceProviderType != null) {
				property = ValidatorOptions.ResourceProviderType.GetProperty(resourceName, BindingFlags.Public | BindingFlags.Static);
				
				if(property != null) {
					resourceType = ValidatorOptions.ResourceProviderType;	
				}
			}

			if(property == null) {
				property = resourceType.GetProperty(resourceName, BindingFlags.Public | BindingFlags.Static);
			}

			if (property == null) {
				throw new InvalidOperationException(string.Format("Could not find a property named '{0}' on type '{1}'.", resourceName, resourceType));
			}

			if (property.PropertyType != typeof(string)) {
				throw new InvalidOperationException(string.Format("Property '{0}' on type '{1}' does not return a string", resourceName, resourceType));
			}

			return () => (string)property.GetValue(null, null);
		}

		public static Func<string> BuildResourceAccessor(Expression<Func<string>> expression) {
			var member = expression.GetMember();

			if(member == null) {
				throw new InvalidOperationException("Only MemberExpressions an be passed to BuildResourceAccessor.");
			}

			var resourceType = member.DeclaringType;
			var resourceName = member.Name;

			return BuildResourceAccessor(resourceName, resourceType);
		}

	}
}