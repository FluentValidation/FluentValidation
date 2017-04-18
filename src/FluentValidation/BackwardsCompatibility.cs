using System;
using System.Collections.Generic;
using System.Text;

namespace FluentValidation
{
    using System.Linq;
    using System.Linq.Expressions;
    using Internal;
    using Resources;
    using Validators;

	public static class BackwardsCompatibilityExtensions {

		/// <summary>
		/// Overrides the name of the property associated with this rule.
		/// NOTE: This is a considered to be an advanced feature. 99% of the time that you use this, you actually meant to use WithName.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="propertyName">The property name to use</param>
		/// <returns></returns>
		[Obsolete("WithPropertyName has been deprecated. If you wish to set the DisplayName within the error message, use 'WithName'. If you actually intended to change which property this rule was declared against, use 'OverridePropertyName' instead.")]
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
		[Obsolete("Use WithName(x => ResourceType.ResourceName) instead")]
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
		[Obsolete("Use WithMessage(x => ResourceType.ResourceName) instead.")]
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
		[Obsolete("Use WithMessage(x => string.Format(ResourceType.ResourceName, args)) instead.")]
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
		[Obsolete("Use WithMessage(x => ResourceType.ResourceName) instead.")]
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
		[Obsolete("Use WithMessage(x => string.Format(ResourceType.ResourceName, args) instead.")]
		public static IRuleBuilderOptions<T, TProperty> WithLocalizedMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Expression<Func<string>> resourceSelector, params Func<T, object>[] formatArgs) {
			// We use the StaticResourceAccessorBuilder here because we don't want calls to WithLocalizedMessage to be overriden by the ResourceProviderType.
			return rule.WithLocalizedMessage(resourceSelector, (IResourceAccessorBuilder)null)
				.Configure(cfg => {
					formatArgs
						.Select(func => new Func<object, object, object>((instance, value) => func((T)instance)))
						.ForEach(cfg.CurrentValidator.CustomMessageFormatArguments.Add);
				});

		}

	    /// <summary>
	    /// Specifies a localized name for the error message. 
	    /// </summary>
	    /// <param name="rule">The current rule</param>
	    /// <param name="resourceType">The type of the generated resource file</param>
	    /// <param name="resourceName">The name of the resource to use</param>
	    [Obsolete("Use WithName(x => ResourceType.ResourceName) instead")]
	    public static IRuleBuilderOptions<T, TProperty> WithLocalizedName<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Type resourceType, string resourceName) {
		    resourceType.Guard("A resource type must be specified.");
		    resourceName.Guard("A resource name must be specified.");

		    return rule.Configure(config => {
			    config.DisplayName = new LocalizedStringSource(resourceType, resourceName);
		    });
	    }


	    /// <summary>
	    /// Specifies a custom error message to use if validation fails.
	    /// </summary>
	    /// <param name="rule">The current rule</param>
	    /// <param name="errorMessage">The error message to use</param>
	    /// <param name="formatArgs">Additional arguments to be specified when formatting the custom error message.</param>
	    /// <returns></returns>
	    [Obsolete("Use WithMessage(x => string.Format(\"Custom message with placeholder\", args) instead")]
	    public static IRuleBuilderOptions<T, TProperty> WithMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, string errorMessage, params object[] formatArgs) {
		    var funcs = DefaultValidatorOptions.ConvertArrayOfObjectsToArrayOfDelegates<T>(formatArgs);
		    return rule.WithMessage(errorMessage, funcs);
	    }

	    /// <summary>
	    /// Specifies a custom error message to use if validation fails.
	    /// </summary>
	    /// <param name="rule">The current rule</param>
	    /// <param name="errorMessage">The error message to use</param>
	    /// <param name="funcs">Additional property values to be included when formatting the custom error message.</param>
	    /// <returns></returns>
	    [Obsolete("Use WithMessage(x => string.Format(\"Custom message with placeholder\", x.Arg1, x.Arg2) instead")]
	    public static IRuleBuilderOptions<T, TProperty> WithMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, string errorMessage, params Func<T, object>[] funcs) {
		    errorMessage.Guard("A message must be specified when calling WithMessage.");

		    return rule.Configure(config => {
			    config.CurrentValidator.ErrorMessageSource = new StaticStringSource(errorMessage);

			    funcs.Select(func => new Func<object, object, object>((instance, value) => func((T)instance)))
				    .ForEach(config.CurrentValidator.CustomMessageFormatArguments.Add);
		    });
	    }

		/// <summary>
		/// Specifies a custom error message to use if validation fails.
		/// </summary>
		/// <param name="rule">The current rule</param>
		/// <param name="errorMessage">The error message to use</param>
		/// <param name="funcs">Additional property values to use when formatting the custom error message.</param>
		/// <returns></returns>
		[Obsolete("Use WithMessage(x => string.Format(\"Custom message with placeholder\", x.Arg1, x.Arg2) instead")]
		public static IRuleBuilderOptions<T, TProperty> WithMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, string errorMessage, params Func<T, TProperty, object>[] funcs) {
		    errorMessage.Guard("A message must be specified when calling WithMessage.");

		    return rule.Configure(config => {
			    config.CurrentValidator.ErrorMessageSource = new StaticStringSource(errorMessage);

			    funcs
				    .Select(func => new Func<object, object, object>((instance, value) => func((T)instance, (TProperty)value)))
				    .ForEach(config.CurrentValidator.CustomMessageFormatArguments.Add);
		    });
	    }

	    /// <summary>
	    /// Specifies a custom error message resource to use when validation fails.
	    /// </summary>
	    /// <param name="rule">The current rule</param>
	    /// <param name="resourceSelector">The resource to use as an expression, eg () => Messages.MyResource</param>
	    /// <param name="resourceName">Name of resource</param>
	    /// <param name="formatArgs">Custom message format args</param>
	    /// <param name="resourceType">Type of resource representing a resx file</param>
	    /// <returns></returns>
	    [Obsolete("Use WithMessage(x => string.Format(ResourceType.ResourceName, args) instead.")]
	    public static IRuleBuilderOptions<T, TProperty> WithLocalizedMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Type resourceType, string resourceName, params object[] formatArgs) {
		    var funcs = DefaultValidatorOptions.ConvertArrayOfObjectsToArrayOfDelegates<T>(formatArgs);
		    return rule.WithLocalizedMessage(resourceType, resourceName, funcs);
	    }

	    /// <summary>
	    /// Specifies a custom error message resource to use when validation fails.
	    /// </summary>
	    /// <param name="rule">The current rule</param>
	    /// <param name="resourceName">Resource name</param>
	    /// <param name="formatArgs">Custom message format args</param>
	    /// <param name="resourceType">Resource type representing a resx file</param>
	    /// <returns></returns>
	    [Obsolete("Use WithMessage(x => string.Format(ResourceType.ResourceName, x.Arg1, x.Arg2)")]
	    public static IRuleBuilderOptions<T, TProperty> WithLocalizedMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Type resourceType, string resourceName, params Func<T, object>[] formatArgs) {
		    // We use the StaticResourceAccessorBuilder here because we don't want calls to WithLocalizedMessage to be overriden by the ResourceProviderType.
		    return rule.WithLocalizedMessage(resourceType, resourceName)
			    .Configure(cfg => {
				    formatArgs
					    .Select(func => new Func<object, object, object>((instance, value) => func((T)instance)))
					    .ForEach(cfg.CurrentValidator.CustomMessageFormatArguments.Add);
			    });
	    }



	    /// <summary>
	    /// Replace the first validator of this type and remove all the others.
	    /// </summary>
	    [Obsolete("Run-time modification of validators is not recommended or supported.", true)]
	    public static void ReplaceRule<T>(this IValidator<T> validators,
		    Expression<Func<T, object>> expression,
		    IPropertyValidator newValidator) {
		    var property = expression.GetMember();
		    if (property == null) throw new ArgumentException("Property could not be identified", "expression");
		    var type = newValidator.GetType();

		    var rules = validators as IEnumerable<IValidationRule>;
		    if (rules == null) {
			    throw new NotSupportedException(string.Format("Validator '{0}' does not support replacing rules.", validators.GetType().Name));
		    }

		    // replace the first validator of this type, then remove the others
		    bool replaced = false;
		    foreach (var rule in rules.OfType<PropertyRule>()) {
			    if (rule.Member == property) {
				    foreach (var original in rule.Validators.Where(v => v.GetType() == type).ToArray()) {
					    if (!replaced) {
						    rule.ReplaceValidator(original, newValidator);
						    replaced = true;
					    } else {
						    rule.RemoveValidator(original);
					    }
				    }
			    }
		    }
	    }

	    /// <summary>
	    /// Remove all validators of the specifed type.
	    /// </summary>
	    [Obsolete("Run-time modification of validators is not recommended or supported.", true)]
	    public static void RemoveRule<T>(this IValidator<T> validators,
		    Expression<Func<T, object>> expression, Type oldValidatorType) {
		    var property = expression.GetMember();
		    if (property == null) throw new ArgumentException("Property could not be identified", "expression");

		    var rules = validators as IEnumerable<IValidationRule>;
		    if (rules == null) {
			    throw new NotSupportedException(string.Format("Validator '{0}' does not support replacing rules.", validators.GetType().Name));
		    }

		    foreach (var rule in rules.OfType<PropertyRule>()) {
			    if (rule.Member == property) {
				    foreach (var original in rule.Validators.Where(v => v.GetType() == oldValidatorType).ToArray()) {
					    rule.RemoveValidator(original);
				    }
			    }
		    }
	    }

	    /// <summary>
	    /// Remove all validators for the given property.
	    /// </summary>
	    [Obsolete("Run-time modification of validators is not recommended or supported.")]
	    public static void ClearRules<T>(this IValidator<T> validators,
		    Expression<Func<T, object>> expression) {
		    var property = expression.GetMember();
		    if (property == null) throw new ArgumentException("Property could not be identified", "expression");

		    var rules = validators as IEnumerable<IValidationRule>;
		    if (rules == null) {
			    throw new NotSupportedException(string.Format("Validator '{0}' does not support replacing rules.", validators.GetType().Name));
		    }

		    foreach (var rule in rules.OfType<PropertyRule>()) {
			    if (rule.Member == property) {
				    rule.ClearValidators();
			    }
		    }
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
