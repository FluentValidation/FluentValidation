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

namespace FluentValidation.WebApi {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Http.Controllers;
	using System.Web.Http.Metadata;
	using System.Web.Http.Validation;

	public class FluentValidationBodyModelValidator : DefaultBodyModelValidator, IBodyModelValidator {
		private bool ImplicitlyValidateChildProperties { get; set; }

		public FluentValidationBodyModelValidator(bool implicitlyValidateChildProperties = true) {
			ImplicitlyValidateChildProperties = implicitlyValidateChildProperties;
		}

		bool IBodyModelValidator.Validate(object model, Type type, ModelMetadataProvider metadataProvider, HttpActionContext actionContext, string keyPrefix) {
			var customizations = actionContext.ActionDescriptor.GetParameters().Where(x => x.ParameterName == keyPrefix)
				.Select(x => x.GetCustomAttributes<CustomizeValidatorAttribute>().FirstOrDefault())
				.FirstOrDefault();

			if (customizations != null) {
				actionContext.Request.Properties["_FV_Customizations"] = customizations;
			}

			return base.Validate(model, type, metadataProvider, actionContext, keyPrefix);
		}

		protected override bool ValidateNodeAndChildren(ModelMetadata metadata, BodyModelValidatorContext validationContext, object container, IEnumerable<ModelValidator> validators) {
			CustomizeValidatorAttribute customizations = null;

			if (validationContext.ActionContext.Request.Properties.ContainsKey("_FV_Customizations")) {
				customizations = validationContext.ActionContext.Request.Properties["_FV_Customizations"] as CustomizeValidatorAttribute;
			}

			if (customizations == null) customizations = new CustomizeValidatorAttribute();

			if (validators == null)
				validators = GetValidators(validationContext.ActionContext, metadata, validationContext.ValidatorCache);

			validators = ApplyCustomizationsToValidators(validators, customizations, validationContext.ActionContext);

			bool isValid = !ImplicitlyValidateChildProperties || base.ValidateNodeAndChildren(metadata, validationContext, container, validators);

			var model = GetModel(metadata);

			if ((!isValid || !ImplicitlyValidateChildProperties) && model != null && !HasAlreadyBeenValidated(validationContext, model)) {
				// default impl skips validating root node if any children fail, so we explicitly validate it in this scenario
				var rootModelValidators = validationContext.ActionContext.GetValidators(metadata);
				rootModelValidators = ApplyCustomizationsToValidators(rootModelValidators, customizations, validationContext.ActionContext);
				ShallowValidate(metadata, validationContext, container, rootModelValidators);
				return false;
			}

			return isValid;
		}

		private IEnumerable<ModelValidator> ApplyCustomizationsToValidators(IEnumerable<ModelValidator> validators, CustomizeValidatorAttribute customizations, HttpActionContext ctx) {
			if (validators == null) return null;
			// For the FV-specific model validator, clone it passing the context and customizations to the clone
			// This is done rather than setting them on the original validator so we don't up with stale contexts in WebApi's cache
			var projection = from validator in validators
				let fluentValidator = validator as FluentValidationModelValidator
				let needsCustomiations = fluentValidator != null && fluentValidator.Customizations == null
				let newValidator = needsCustomiations ? (ModelValidator) fluentValidator.CloneWithCustomizations(customizations, ctx) : validator
				select newValidator;

			return projection.ToList();
		}


		private static IEnumerable<ModelValidator> GetValidators(HttpActionContext actionContext, ModelMetadata metadata, IModelValidatorCache validatorCache) {
			if (validatorCache == null) {
				// slow path: there is no validator cache on the configuration
				return metadata.GetValidators(actionContext.GetValidatorProviders());
			}
			else {
				return validatorCache.GetValidators(metadata);
			}
		}

		protected override bool ShallowValidate(ModelMetadata metadata, BodyModelValidatorContext validationContext, object container, IEnumerable<ModelValidator> validators) {
			var valid = base.ShallowValidate(metadata, validationContext, container, validators);

			var model = GetModel(metadata);

			if (model != null && validators.Any(x => x is FluentValidationModelValidator)) {
				HashSet<object> progress = GetProgress(validationContext);
				progress.Add(model);
			}

			return valid;
		}

		private object GetModel(ModelMetadata meta) {
			object model = null;

			try {
				model = meta.Model;
			}
			catch {
			}

			return model;
		}

		private bool HasAlreadyBeenValidated(BodyModelValidatorContext validationContext, object model) {
			return GetProgress(validationContext).Contains(model);
		}

		private HashSet<object> GetProgress(BodyModelValidatorContext context) {
			HashSet<object> progress;

			if (!context.ActionContext.Request.Properties.ContainsKey("_FV_Progress")) {
				context.ActionContext.Request.Properties["_FV_Progress"] = progress = new HashSet<object>();
			}
			else {
				progress = (HashSet<object>) context.ActionContext.Request.Properties["_FV_Progress"];
			}

			return progress;
		}
	}
}