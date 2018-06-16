namespace FluentValidation.AspNetCore {
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;
	using System.Reflection;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Controllers;
	using Microsoft.AspNetCore.Mvc.DataAnnotations.Internal;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

	/// <summary>
	/// ModelValidatorProvider implementation only used for child properties.
	/// </summary>
	internal class FluentValidationModelValidatorProvider : IModelValidatorProvider {
		private bool _implicitValidationEnabled;

		public FluentValidationModelValidatorProvider(bool implicitValidationEnabled) {
			_implicitValidationEnabled = implicitValidationEnabled;
		}

		public void CreateValidators(ModelValidatorProviderContext context) {

			// Note that this check does NOT catch complex types that are collection elements (as the ModelMetadataKind will still be 'Type') 
			if (context.ModelMetadata.MetadataKind == ModelMetadataKind.Type || context.ModelMetadata.MetadataKind == ModelMetadataKind.Parameter || (context.ModelMetadata.MetadataKind == ModelMetadataKind.Property)) {
					context.Results.Add(new ValidatorItem {
						IsReusable = false,
						Validator = new FluentValidationModelValidator(_implicitValidationEnabled)
					});
			}
		}
	}

	internal class FluentValidationModelValidator : IModelValidator {
		private readonly bool _implicitValidationEnabled;

		public FluentValidationModelValidator(bool implicitValidationEnabled) {
			_implicitValidationEnabled = implicitValidationEnabled;
		}

		public IEnumerable<ModelValidationResult> Validate(ModelValidationContext mvContext) {

			// Skip validation if model is null or the model has been marked for skipping.
			if (mvContext.Model == null || ShouldSkip(mvContext)) return Enumerable.Empty<ModelValidationResult>();

			var factory = mvContext.ActionContext.HttpContext.RequestServices.GetService(typeof(IValidatorFactory)) as IValidatorFactory;

			if (factory != null) {
				var validator = factory.GetValidator(mvContext.ModelMetadata.ModelType);

				if (validator != null) {

					var customizations = GetCustomizations(mvContext.ActionContext, mvContext.Model);

					if (customizations.Skip) {
						return Enumerable.Empty<ModelValidationResult>();
					}

					if (mvContext.Container != null) {
						var containerCustomizations = GetCustomizations(mvContext.ActionContext, mvContext.Container);
						if (containerCustomizations.Skip) {
							return Enumerable.Empty<ModelValidationResult>();
						}
					}

					var selector = customizations.ToValidatorSelector();
					var interceptor = customizations.GetInterceptor() ?? (validator as IValidatorInterceptor);
					var context = new FluentValidation.ValidationContext(mvContext.Model, new FluentValidation.Internal.PropertyChain(), selector);
					context.RootContextData["InvokedByMvc"] = true;

					if (interceptor != null) {
						// Allow the user to provide a customized context
						// However, if they return null then just use the original context.
						context = interceptor.BeforeMvcValidation((ControllerContext)mvContext.ActionContext, context) ?? context;
					}

					var result = validator.Validate(context);

					if (interceptor != null) {
						// allow the user to provice a custom collection of failures, which could be empty.
						// However, if they return null then use the original collection of failures. 
						result = interceptor.AfterMvcValidation((ControllerContext)mvContext.ActionContext, context, result) ?? result;
					}

					return result.Errors.Select(x => new ModelValidationResult(x.PropertyName, x.ErrorMessage));
				}
			}

			return Enumerable.Empty<ModelValidationResult>();
		}

		private bool ShouldSkip(ModelValidationContext mvContext) {
			if (mvContext.ActionContext.HttpContext.Items.ContainsKey("_FV_SKIP") && mvContext.ActionContext.HttpContext.Items["_FV_SKIP"] is HashSet<object> skip) {
				if (skip.Contains(mvContext.Model)) return true;
				if (mvContext.Container != null && skip.Contains(mvContext.Container)) return true;
			}

			// If implicit validation of children is disabled and we're not validating the root, then skip.
			if (!_implicitValidationEnabled && mvContext.ActionContext.HttpContext.Items.TryGetValue("_FV_ROOT", out var root)) {
				if (! Equals(root, mvContext.Model)) {
				//	return true;
				}
			}

			return false;
		}

		private CustomizeValidatorAttribute GetCustomizations(ActionContext ctx, object model) {
			if (ctx.HttpContext.Items["_FV_Customizations"] is Tuple<object, CustomizeValidatorAttribute> customizations 
				&& ReferenceEquals(model, customizations.Item1)) {
				return customizations.Item2; // the attribute
			}
			return new CustomizeValidatorAttribute();
		}
	}
}