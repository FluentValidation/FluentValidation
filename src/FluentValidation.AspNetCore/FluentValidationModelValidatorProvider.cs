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

			if (context.ModelMetadata.MetadataKind == ModelMetadataKind.Type || (context.ModelMetadata.MetadataKind == ModelMetadataKind.Property && _implicitValidationEnabled)) {
					context.Results.Add(new ValidatorItem {
						IsReusable = false,
						Validator = new FluentValidationModelValidator()
					});
			}
		}
	}

	internal class FluentValidationModelValidator : IModelValidator {
		public IEnumerable<ModelValidationResult> Validate(ModelValidationContext mvContext) {

			// Skip validation if model is null.
			if (mvContext.Model == null) return Enumerable.Empty<ModelValidationResult>();

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

		private CustomizeValidatorAttribute GetCustomizations(ActionContext ctx, object model) {
			if (ctx.HttpContext.Items["_FV_Customizations"] is Tuple<object, CustomizeValidatorAttribute> customizations 
				&& ReferenceEquals(model, customizations.Item1)) {
				return customizations.Item2; // the attribute
			}
			return new CustomizeValidatorAttribute();

		}
	}

	
}