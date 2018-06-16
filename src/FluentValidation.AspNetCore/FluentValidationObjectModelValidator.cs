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
	using Microsoft.AspNetCore.Mvc.Controllers;
	using Microsoft.AspNetCore.Mvc.Internal;
	using Microsoft.AspNetCore.Mvc.ModelBinding;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

	internal class FluentValidationObjectModelValidator : ObjectModelValidator {
		private readonly bool _runMvcValidation;
		private readonly bool _implicitValidationEnabled;
		private readonly ValidatorCache _validatorCache;
		private readonly FluentValidationModelValidatorProvider _fvProvider;

		public FluentValidationObjectModelValidator(
			IModelMetadataProvider modelMetadataProvider,
			IList<IModelValidatorProvider> validatorProviders, bool runMvcValidation, bool implicitValidationEnabled)
		: base(modelMetadataProvider, validatorProviders) {
			_runMvcValidation = runMvcValidation;
			_implicitValidationEnabled = implicitValidationEnabled;
			_validatorCache = new ValidatorCache();
			_fvProvider = validatorProviders.SingleOrDefault(x => x is FluentValidationModelValidatorProvider) as FluentValidationModelValidatorProvider;
		}

		public override void Validate(ActionContext actionContext, ValidationStateDictionary validationState, string prefix, object model, ModelMetadata metadata) {
			ValidateInternal(actionContext, prefix, model, () => {
				base.Validate(actionContext, validationState, prefix, model, metadata);
			});
		}

		public override void Validate(ActionContext actionContext, ValidationStateDictionary validationState, string prefix, object model) {
			ValidateInternal(actionContext, prefix, model, () => {
				base.Validate(actionContext, validationState, prefix, model);
			});
		}

		private void ValidateInternal(ActionContext actionContext, string prefix, object model, Action action) {
			// Cache the root object. Used by the provider to determine whether we're doing a top-level validation.
			actionContext.HttpContext.Items["_FV_ROOT"] = model;
			
			// Store and remove any implicit required messages.
			// Later we'll re-add those that are still relevant.
			var requiredErrorsNotHandledByFv = RemoveImplicitRequiredErrors(actionContext);

			// Apply any customizations made with the CustomizeValidatorAttribute 
			if (model != null) {
				var customizations = GetCustomizations(actionContext, model.GetType(), prefix);
				actionContext.HttpContext.Items["_FV_Customizations"] = Tuple.Create(model, customizations);
			}

			// Run the base Validate implementaiton.
			// This could be either overload.
			action();

			// Re-add errors that we took out if FV didn't add a key. 
			ReApplyImplicitRequiredErrorsNotHandledByFV(requiredErrorsNotHandledByFv);

			// Remove duplicates. This can happen if someone has implicit child validation turned on and also adds an explicit child validator.
			RemoveDuplicateModelstateEntries(actionContext);
		}
		
		public override ValidationVisitor GetValidationVisitor(ActionContext actionContext, IModelValidatorProvider validatorProvider, ValidatorCache validatorCache, IModelMetadataProvider metadataProvider, ValidationStateDictionary validationState) {
			// Setting as to whether we should run only FV or FV + the other validator providers
			var validatorProviderToUse = _runMvcValidation ? validatorProvider : _fvProvider;

			var visitor = new FluentValidationVisitor(
				actionContext,
				validatorProviderToUse,
				validatorCache,
				metadataProvider,
				validationState)
			{
				ValidateChildren = _implicitValidationEnabled
			};

			return visitor;
		}
		
		internal static void RemoveDuplicateModelstateEntries(ActionContext actionContext) {
			foreach (var entry in actionContext.ModelState) {
				if (entry.Value.ValidationState == ModelValidationState.Invalid) {
					var existing = new HashSet<string>();

					foreach (var err in entry.Value.Errors.ToList()) {
						//ToList to create a copy so we can remove from the original
						if (existing.Contains(err.ErrorMessage)) {
							entry.Value.Errors.Remove(err);
						}
						else {
							existing.Add(err.ErrorMessage);
						}
					}
				}
			}
		}

		internal static void ReApplyImplicitRequiredErrorsNotHandledByFV(List<KeyValuePair<ModelStateEntry, ModelError>> requiredErrorsNotHandledByFv) {
			foreach (var pair in requiredErrorsNotHandledByFv) {
				if (pair.Key.ValidationState != ModelValidationState.Invalid) {
					pair.Key.Errors.Add(pair.Value);
					pair.Key.ValidationState = ModelValidationState.Invalid;
				}
			}
		}

		internal static List<KeyValuePair<ModelStateEntry, ModelError>> RemoveImplicitRequiredErrors(ActionContext actionContext) {
			// This is all to work around the default "Required" messages.
			var requiredErrorsNotHandledByFv = new List<KeyValuePair<ModelStateEntry, ModelError>>();

			foreach (KeyValuePair<string, ModelStateEntry> entry in actionContext.ModelState) {
				List<ModelError> errorsToModify = new List<ModelError>();

				if (entry.Value.ValidationState == ModelValidationState.Invalid) {
					foreach (var err in entry.Value.Errors) {
						if (err.ErrorMessage.StartsWith(FluentValidationBindingMetadataProvider.Prefix)) {
							errorsToModify.Add(err);
						}
					}

					foreach (ModelError err in errorsToModify) {
						entry.Value.Errors.Clear();
						entry.Value.ValidationState = ModelValidationState.Unvalidated;
						requiredErrorsNotHandledByFv.Add(new KeyValuePair<ModelStateEntry, ModelError>(entry.Value, new ModelError(err.ErrorMessage.Replace(FluentValidationBindingMetadataProvider.Prefix, string.Empty))));
						;
					}
				}
			}
			return requiredErrorsNotHandledByFv;
		}

		internal static CustomizeValidatorAttribute GetCustomizations(ActionContext actionContext, Type type, string prefix) {

			if (actionContext?.ActionDescriptor?.Parameters == null) {
				return new CustomizeValidatorAttribute();
			}

			var descriptors = actionContext.ActionDescriptor.Parameters
				.Where(x => x.ParameterType == type)
				.Where(x => (x.BindingInfo != null && x.BindingInfo.BinderModelName != null && x.BindingInfo.BinderModelName == prefix) || x.Name == prefix || (prefix == string.Empty && x.BindingInfo?.BinderModelName == null))
				.OfType<ControllerParameterDescriptor>()
				.ToList();

			CustomizeValidatorAttribute attribute = null;

			if (descriptors.Count == 1) {
				attribute = descriptors[0].ParameterInfo.GetCustomAttributes(typeof(CustomizeValidatorAttribute), true).FirstOrDefault() as CustomizeValidatorAttribute;
			}
			if (descriptors.Count > 1) {
				// We found more than 1 matching with same prefix and name. 
			}

			return attribute ?? new CustomizeValidatorAttribute();
		}

	}

}