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

namespace FluentValidation.Internal {
	using System;
	using System.Linq.Expressions;
	using System.Reflection;
	using Resources;

	internal class ResourceHelper {
		private static readonly Type defaultResourceType = typeof(Messages);

		public static ResourceMetaData BuildResourceAccessor(string resourceName, Type resourceType) {
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

			Func<string> accessor = () => (string)property.GetValue(null, null);

			return new ResourceMetaData(resourceName, resourceType, accessor);
		}

		public static ResourceMetaData BuildResourceAccessor(Expression<Func<string>> expression) {
			var constant = expression.Body as ConstantExpression;

			if(constant != null) {
				return new ResourceMetaData(null,null, () => (string)constant.Value);
			}

			var member = expression.GetMember();

			if(member == null) {
				throw new InvalidOperationException("Only MemberExpressions an be passed to BuildResourceAccessor, eg () => Messages.MyResource");
			}

			var resourceType = member.DeclaringType;
			var resourceName = member.Name;

			return BuildResourceAccessor(resourceName, resourceType);
		}

	}

	internal class ResourceMetaData {
		public ResourceMetaData(string resourceName, Type resourceType, Func<string> accessor) {
			ResourceName = resourceName;
			ResourceType = resourceType;
			Accessor = accessor;
		}

		public string ResourceName { get; private set; }
		public Type ResourceType { get; private set; }
		public Func<string> Accessor { get; private set; }
	}
}