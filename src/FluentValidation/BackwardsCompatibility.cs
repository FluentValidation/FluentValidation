using System;
using System.Collections.Generic;
using System.Text;

namespace FluentValidation
{
    using System.Linq;
    using System.Linq.Expressions;
    using Internal;
    using Resources;

    public static class BackwardsCompatibilityExtensions {

		/// <summary>
		/// Overrides the name of the property associated with this rule.
		/// NOTE: This is a considered to be an advanced feature. 99% of the time that you use this, you actually meant to use WithName.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="propertyName">The property name to use</param>
		/// <returns></returns>
		[Obsolete("WithPropertyName has been deprecated. If you wish to set the name of the property within the error message, use 'WithName'. If you actually intended to change which property this rule was declared against, use 'OverridePropertyName' instead.")]
		public static IRuleBuilderOptions<T, TProperty> WithPropertyName<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, string propertyName)
		{
			return rule.OverridePropertyName(propertyName);
		}

		/// <summary>
		/// Specifies a localized name for the error message. 
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="resourceSelector">The resource to use as an expression, eg () => Messages.MyResource</param>
		/// <param name="resourceAccessorBuilder">Resource accessor builder to use</param>
		[Obsolete("Use WithLocalizedName(Type resourceType, string resourceName) instead")]
		public static IRuleBuilderOptions<T, TProperty> WithLocalizedName<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Expression<Func<string>> resourceSelector, IResourceAccessorBuilder resourceAccessorBuilder = null) {
			resourceSelector.Guard("A resource selector must be specified.");
			// default to the static resource accessor builder - explicit resources configured with WithLocalizedName should take precedence over ResourceProviderType.

			return rule.Configure(config => {
				config.DisplayName = LocalizedStringSource.CreateFromExpression(resourceSelector, resourceAccessorBuilder);
			});
		}

		/// <summary>
		/// Specifies a custom error message resource to use when validation fails.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="resourceSelector">The resource to use as an expression, eg () => Messages.MyResource</param>
		/// <returns></returns>
		[Obsolete("Use WithLocalizedMessage(Type resourceType, string resourceName) instead.")]
		public static IRuleBuilderOptions<T, TProperty> WithLocalizedMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Expression<Func<string>> resourceSelector) {
			// We use the StaticResourceAccessorBuilder here because we don't want calls to WithLocalizedMessage to be overriden by the ResourceProviderType.
			return rule.WithLocalizedMessage(resourceSelector, (IResourceAccessorBuilder)null);
		}

		/// <summary>
		/// Specifies a custom error message resource to use when validation fails.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="resourceSelector">The resource to use as an expression, eg () => Messages.MyResource</param>
		/// <param name="formatArgs">Custom message format args</param>
		/// <returns></returns>
		[Obsolete("Use WithLocalizedMessage(Type resourceType, string resourceName, params object[] formatArgs) instead.")]
		public static IRuleBuilderOptions<T, TProperty> WithLocalizedMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Expression<Func<string>> resourceSelector, params object[] formatArgs) {
			var funcs = DefaultValidatorOptions.ConvertArrayOfObjectsToArrayOfDelegates<T>(formatArgs);
			return rule.WithLocalizedMessage(resourceSelector, funcs);
		}

		/// <summary>
		/// Specifies a custom error message resource to use when validation fails.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="resourceSelector">The resource to use as an expression, eg () => Messages.MyResource</param>
		/// <param name="resourceAccessorBuilder">The resource accessor builder to use. </param>
		/// <returns></returns>
		[Obsolete("Use WithLocalizedMessage(Type resourceType, string resourceName) instead.")]
		public static IRuleBuilderOptions<T, TProperty> WithLocalizedMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Expression<Func<string>> resourceSelector, IResourceAccessorBuilder resourceAccessorBuilder) {
			resourceSelector.Guard("An expression must be specified when calling WithLocalizedMessage, eg .WithLocalizedMessage(() => Messages.MyResource)");

			return rule.Configure(config => {
				config.CurrentValidator.ErrorMessageSource = LocalizedStringSource.CreateFromExpression(resourceSelector, resourceAccessorBuilder);
			});
		}

		/// <summary>
		/// Specifies a custom error message resource to use when validation fails.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="resourceSelector">The resource to use as an expression, eg () => Messages.MyResource</param>
		/// <param name="formatArgs">Custom message format args</param>
		/// <returns></returns>
		[Obsolete("Use WithLocalizedMessage(Type resourceType, string resourceName, params Func<T, object>[] formatArgs) instead.")]
		public static IRuleBuilderOptions<T, TProperty> WithLocalizedMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Expression<Func<string>> resourceSelector, params Func<T, object>[] formatArgs) {
			// We use the StaticResourceAccessorBuilder here because we don't want calls to WithLocalizedMessage to be overriden by the ResourceProviderType.
			return rule.WithLocalizedMessage(resourceSelector, (IResourceAccessorBuilder)null)
				.Configure(cfg => {
					formatArgs
						.Select(func => new Func<object, object, object>((instance, value) => func((T)instance)))
						.ForEach(cfg.CurrentValidator.CustomMessageFormatArguments.Add);
				});

		}


	}
}

namespace FluentValidation.Resources {
	/// <summary>
	/// Builds a delegate for retrieving a localised resource from a resource type and property name.
	/// </summary>
	[Obsolete("This functionality has been merged into LocalizedStringSource")]
	public interface IResourceAccessorBuilder {
		/// <summary>
		/// Gets a function that can be used to retrieve a message from a resource type and resource name.
		/// </summary>
		ResourceAccessor GetResourceAccessor(Type resourceType, string resourceName);
	}
}
