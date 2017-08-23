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
#endregion

namespace FluentValidation.Resources {
	using System;
	using System.Linq.Expressions;
	using System.Reflection;
	using Internal;

	/// <summary>
	/// Represents a localized string.
	/// </summary>
	public class LocalizedStringSource : IStringSource {
		readonly Func<string> accessor;
		readonly Type resourceType;
		readonly string resourceName;

		/// <summary>
		/// Creates a new instance of the LocalizedErrorMessageSource class using the specified resource name and resource type.
		/// </summary>
		/// <param name="resourceType">The resource type</param>
		/// <param name="resourceName">The resource name</param>
		public LocalizedStringSource(Type resourceType, string resourceName) {
			var resourceAccessor = BuildResourceAccessor(resourceType, resourceName);

			this.resourceType = resourceAccessor.ResourceType;
			this.resourceName = resourceAccessor.ResourceName;
			this.accessor = resourceAccessor.Accessor;
		}

		/// <summary>
		/// Creates a new instance of the LocalizedErrorMessageSource class using the specified resource name and resource type.
		/// </summary>
		/// <param name="resourceType">The resource type</param>
		/// <param name="resourceName">The resource name</param>
		/// <param name="resourceAccessorBuilder">Strategy used to construct the resource accessor</param>
		[Obsolete]
		public LocalizedStringSource(Type resourceType, string resourceName, IResourceAccessorBuilder resourceAccessorBuilder) {
		    var resourceAccessor = resourceAccessorBuilder.GetResourceAccessor(resourceType, resourceName);

            this.resourceType = resourceAccessor.ResourceType;
			this.resourceName = resourceAccessor.ResourceName;
			this.accessor = resourceAccessor.Accessor;
		}

		/// <summary>
		/// Creates a LocalizedStringSource from an expression: () => MyResources.SomeResourceName
		/// </summary>
		/// <param name="expression">The expression </param>
		/// <param name="resourceProviderSelectionStrategy">Strategy used to construct the resource accessor</param>
		/// <returns>Error message source</returns>
		[Obsolete]
		public static IStringSource CreateFromExpression(Expression<Func<string>> expression, IResourceAccessorBuilder resourceProviderSelectionStrategy) {
			var constant = expression.Body as ConstantExpression;

			if (constant != null) {
				return new StaticStringSource((string)constant.Value);
			}

			var member = expression.GetMember();

			if (member == null) {
				throw new InvalidOperationException("Only MemberExpressions an be passed to BuildResourceAccessor, eg () => Messages.MyResource");
			}

			var resourceType = member.DeclaringType;
			var resourceName = member.Name;

			if(resourceProviderSelectionStrategy != null) 
				return new LocalizedStringSource(resourceType, resourceName, resourceProviderSelectionStrategy);

			return new LocalizedStringSource(resourceType, resourceName);
		}

		/// <summary>
		/// Construct the error message template
		/// </summary>
		/// <returns>Error message template</returns>
		public string GetString(object context) {
			return accessor();
		}

		/// <summary>
		/// The name of the resource if localized.
		/// </summary>
		public string ResourceName {
			get { return resourceName; }
		}

		/// <summary>
		/// The type of the resource provider if localized.
		/// </summary>
		public Type ResourceType {
			get { return resourceType; }
		}

	    protected virtual ResourceAccessor BuildResourceAccessor(Type resourceType, string resourceName) {
			var property = GetResourceProperty(ref resourceType, ref resourceName);

			if (property == null) {
				throw new InvalidOperationException(string.Format("Could not find a property named '{0}' on type '{1}'.", resourceName, resourceType));
			}

			if (property.PropertyType != typeof(string)) {
				throw new InvalidOperationException(string.Format("Property '{0}' on type '{1}' does not return a string", resourceName, resourceType));
			}

			var accessor = (Func<string>)property.GetMethod.CreateDelegate(typeof(Func<string>));

			return new ResourceAccessor {
				Accessor = accessor,
				ResourceName = resourceName,
				ResourceType = resourceType
			};
		}

		/// <summary>
		/// Gets the PropertyInfo for a resource.
		/// ResourceType and ResourceName are ref parameters to allow derived types
		/// to replace the type/name of the resource before the delegate is constructed.
		/// </summary>
		protected virtual PropertyInfo GetResourceProperty(ref Type resourceType, ref string resourceName) {
			return resourceType.GetRuntimeProperty(resourceName);
		}
	}

	/// <summary>
	/// Version of LocalizedStringSource that allows the specified ResourceType to be replaced by the default resource type specified in ValidatorOptions.ResourceProviderType.
	/// This is typically only used by the default validator types built into FV, or if you're building a library of validators. Don't use it in rule definitions. 
	/// </summary>
	[Obsolete]
    public class OverridableLocalizedStringSource : LocalizedStringSource {
        public OverridableLocalizedStringSource(Type resourceType, string resourceName) : base(resourceType, resourceName) {
        }

		/// <summary>
		/// Gets the PropertyInfo for a resource.
		/// ResourceType and ResourceName are ref parameters to allow derived types
		/// to replace the type/name of the resource before the delegate is constructed.
		/// </summary>
		protected override PropertyInfo GetResourceProperty(ref Type resourceType, ref string resourceName) {
			// Rather than just using the specified resource type to find the resource accessor property
			// we first look on the ResourceProviderType which gives our end user the ability  
			// to redirect error messages away from the default Messages class.

			if (ValidatorOptions.ResourceProviderType != null) {
				var property = ValidatorOptions.ResourceProviderType.GetRuntimeProperty(resourceName);

				if (property != null) {
					// We found a matching property on the Resource Provider.
					// In this case, as well as returning the PropertyInfo from this resource provider,
					// we also replace the resource type with the resource provider (remember this is a ref parameter)
					resourceType = ValidatorOptions.ResourceProviderType;
					return property;
				}
			}

			return base.GetResourceProperty(ref resourceType, ref resourceName);
		}

		/// <summary>
		/// Creates a OverridableLocalizedStringSource from an expression: () => MyResources.SomeResourceName
		/// </summary>
		/// <param name="expression">The expression </param>
		/// <returns>Error message source</returns>
		public static IStringSource CreateFromExpression(Expression<Func<string>> expression) {
			var constant = expression.Body as ConstantExpression;

			if (constant != null) {
				return new StaticStringSource((string)constant.Value);
			}

			var member = expression.GetMember();

			if (member == null) {
				throw new InvalidOperationException("Only MemberExpressions an be passed to BuildResourceAccessor, eg () => Messages.MyResource");
			}

			var resourceType = member.DeclaringType;
			var resourceName = member.Name;
			return new OverridableLocalizedStringSource(resourceType, resourceName);
		}
	}


}