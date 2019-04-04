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

namespace FluentValidation.AspNetCore {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using FluentValidation.Internal;
	using Microsoft.Extensions.DependencyInjection;
	using static MvcValidationHelper;

	/// <summary>
	/// ModelValidatorProvider implementation only used for child properties.
	/// </summary>
	internal class FluentValidationModelValidatorProvider : IModelValidatorProvider {
		private readonly bool _implicitValidationEnabled;

		public FluentValidationModelValidatorProvider(bool implicitValidationEnabled) {
			_implicitValidationEnabled = implicitValidationEnabled;
		}

		public void CreateValidators(ModelValidatorProviderContext context) {
			context.Results.Add(new ValidatorItem {
				IsReusable = false,
				Validator = new FluentValidationModelValidator(_implicitValidationEnabled)
			});
		}
	}

	internal class FluentValidationModelValidator : IModelValidator {
		private readonly bool _implicitValidationEnabled;

		public FluentValidationModelValidator(bool implicitValidationEnabled) {
			_implicitValidationEnabled = implicitValidationEnabled;
		}

		public IEnumerable<ModelValidationResult> Validate(ModelValidationContext mvContext) {
			if (ShouldSkip(mvContext)) {
				return Enumerable.Empty<ModelValidationResult>();
			}

			var factory = mvContext.ActionContext.HttpContext.RequestServices.GetService(typeof(IValidatorFactory)) as IValidatorFactory;
			var validator = factory?.GetValidator(mvContext.ModelMetadata.ModelType);

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
				var interceptor = customizations.GetInterceptor() 
				                  ?? validator as IValidatorInterceptor 
				                  ?? mvContext.ActionContext.HttpContext.RequestServices.GetService<IValidatorInterceptor>();
				var context = new ValidationContext(mvContext.Model, new PropertyChain(), selector);
				context.RootContextData["InvokedByMvc"] = true;
				context.SetServiceProvider(mvContext.ActionContext.HttpContext.RequestServices);

				if (interceptor != null) {
					// Allow the user to provide a customized context
					// However, if they return null then just use the original context.
					context = interceptor.BeforeMvcValidation((ControllerContext)mvContext.ActionContext, context) ?? context;
				}

				var result = validator.Validate(context);

				if (interceptor != null) {
					// allow the user to provide a custom collection of failures, which could be empty.
					// However, if they return null then use the original collection of failures. 
					result = interceptor.AfterMvcValidation((ControllerContext)mvContext.ActionContext, context, result) ?? result;
				}

				return result.Errors.Select(x => new ModelValidationResult(x.PropertyName, x.ErrorMessage));
			}

			return Enumerable.Empty<ModelValidationResult>();
		}

		private bool ShouldSkip(ModelValidationContext mvContext) {
			// Skip if there's nothing to process.
			if (mvContext.Model == null) {
				return true;
			}

			// If implicit validation is disabled, then we want to only validate the root object.
			if (! _implicitValidationEnabled) {

				var rootMetadata = GetRootMetadata(mvContext);
				
				// We should always have root metadata, so this should never happen...
				if (rootMetadata == null) return true;
				
				// Careful when handling properties.
				// If we're processing a property of our root object,
				// then we always skip if implicit validation is disabled
				// However if our root object *is* a property (because of [BindProperty])
				// then this is OK to proceed.
				if (mvContext.ModelMetadata.MetadataKind == ModelMetadataKind.Property) {
					if (! ReferenceEquals(rootMetadata, mvContext.ModelMetadata)) {
						// The metadata for the current property is not the same as the root metadata
						// This means we're validating a property on a model, so we want to skip.
						return true;
					}
				}
				
				// If we're handling a type, we need to make sure we're handling the root type.
				// When MVC encounters child properties, it will set the MetadataKind to Type,
				// so we can't use the MetadataKind to differentiate the root from the child property.
				// Instead check if our cached root metadata is the same.
				// If they're not, then it means we're handling a child property, so we should skip
				// validation if implicit validation is disabled
				else if (mvContext.ModelMetadata.MetadataKind == ModelMetadataKind.Type) {
					if (! ReferenceEquals(rootMetadata, mvContext.ModelMetadata)) {
						// The metadata for the current type is not the same as the root metadata
						// This means we're validating a child element of a collection or sub property.
						// Skip it as implicit validation is disabled.
						return true;
					}
				}
			}
			
			return false;
		}
	}
}