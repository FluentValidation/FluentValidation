namespace FluentValidation.Resources {
	using System;
	using System.Reflection;

	/// <summary>
	/// Builds a delegate for retrieving a localised resource from a resource type and property name.
	/// </summary>
	public interface IResourceAccessorBuilder {
		/// <summary>
		/// Gets a function that can be used to retrieve a message from a resource type and resource name.
		/// </summary>
		Func<string> GetResourceAccessor(Type resourceType, string resourceName);
	}


	/// <summary>
	/// Builds a delegate for retrieving a localised resource from a resource type and property name.
	/// </summary>
	public class StaticResourceAccessorBuilder : IResourceAccessorBuilder {

		/// <summary>
		/// Builds a function used to retrieve the resource.
		/// </summary>
		public virtual Func<string> GetResourceAccessor(Type resourceType, string resourceName) {
			var property = GetResourceProperty(ref resourceType, ref resourceName);

			if (property == null) {
				throw new InvalidOperationException(string.Format("Could not find a property named '{0}' on type '{1}'.", resourceName, resourceType));
			}

			if (property.PropertyType != typeof(string)) {
				throw new InvalidOperationException(string.Format("Property '{0}' on type '{1}' does not return a string", resourceName, resourceType));
			}

			var accessor = (Func<string>)Delegate.CreateDelegate(typeof(Func<string>), property.GetGetMethod());
			return accessor;
		}

	
		/// <summary>
		/// Gets the PropertyInfo for a resource.
		/// ResourceType and ResourceName are ref parameters to allow derived types
		/// to replace the type/name of the resource before the delegate is constructed.
		/// </summary>
		protected virtual PropertyInfo GetResourceProperty(ref Type resourceType, ref string resourceName) {
			return resourceType.GetProperty(resourceName, BindingFlags.Public | BindingFlags.Static);
		}
	}

	/// <summary>
	/// Implemenetation of IResourceAccessorBuilder that can fall back to the default resource provider.
	/// </summary>
	public class FallbackAwareResourceAccessorBuilder : StaticResourceAccessorBuilder {
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
				var property = ValidatorOptions.ResourceProviderType.GetProperty(resourceName, BindingFlags.Public | BindingFlags.Static);

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
	}
}