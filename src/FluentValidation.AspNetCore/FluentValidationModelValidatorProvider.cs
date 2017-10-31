namespace FluentValidation.AspNetCore {
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;
	using System.Reflection;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Controllers;
	using Microsoft.AspNetCore.Mvc.DataAnnotations.Internal;
	using Microsoft.AspNetCore.Mvc.Internal;
	using Microsoft.AspNetCore.Mvc.ModelBinding;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;


	internal class FVBindingMetadataProvider : IBindingMetadataProvider, IValidationMetadataProvider {
		public void CreateBindingMetadata(BindingMetadataProviderContext context) {
			if (context.Key.MetadataKind == ModelMetadataKind.Property) {
				var original = context.BindingMetadata.ModelBindingMessageProvider.ValueMustNotBeNullAccessor;
				context.BindingMetadata.ModelBindingMessageProvider.ValueMustNotBeNullAccessor = s => "_FV_REQUIRED|" + original(s);
			}
		}

		public void CreateValidationMetadata(ValidationMetadataProviderContext context) {
		}
	}

	internal class FVObjectModelValidator2 : IObjectModelValidator {
		private readonly IModelMetadataProvider _modelMetadataProvider;
		private readonly bool _runMvcValidation;
		private readonly ValidatorCache _validatorCache;
		private readonly IModelValidatorProvider _compositeProvider;
		private readonly FluentValidationModelValidatorProvider _fvProvider;

		public FVObjectModelValidator2(
			IModelMetadataProvider modelMetadataProvider,
			IList<IModelValidatorProvider> validatorProviders, bool runMvcValidation) {

			if (modelMetadataProvider == null) {
				throw new ArgumentNullException(nameof(modelMetadataProvider));
			}

			if (validatorProviders == null) {
				throw new ArgumentNullException(nameof(validatorProviders));
			}

			_modelMetadataProvider = modelMetadataProvider;
			_runMvcValidation = runMvcValidation;
			_validatorCache = new ValidatorCache();
			_fvProvider = validatorProviders.SingleOrDefault(x => x is FluentValidationModelValidatorProvider) as FluentValidationModelValidatorProvider;
			_compositeProvider = new CompositeModelValidatorProvider(validatorProviders.Except(new IModelValidatorProvider[]{ _fvProvider }).ToList());
		}

		public void Validate(ActionContext actionContext, ValidationStateDictionary validationState, string prefix, object model) {

			// This is all to work around the default "Required" messages.

			var entries = new List<KeyValuePair<ModelStateEntry, ModelError>>();
			
			foreach (KeyValuePair<string, ModelStateEntry> entry in actionContext.ModelState) {

				List<ModelError> errorsToModify = new List<ModelError>();

				if (entry.Value.ValidationState == ModelValidationState.Invalid) {
					foreach (var err in entry.Value.Errors) {
						if (err.ErrorMessage.StartsWith("_FV_REQUIRED|")) {
							errorsToModify.Add(err);
						}
					}

					foreach (ModelError err in errorsToModify) {
						entry.Value.Errors.Clear();
						entry.Value.ValidationState = ModelValidationState.Unvalidated;
						entries.Add(new KeyValuePair<ModelStateEntry, ModelError>(entry.Value, new ModelError(err.ErrorMessage.Replace("_FV_REQUIRED|", string.Empty)))); ;
					}
				}
				
			}

			// Run default validation

			//_inner.Validate(actionContext, validationState, prefix, model);

			var metadata = model == null ? null : _modelMetadataProvider.GetMetadataForType(model.GetType());

			// First run validation using only the FV validator. Once that's done, allow everything else to run. 
			var visitor = new ValidationVisitor(
				actionContext,
				_fvProvider,
				_validatorCache,
				_modelMetadataProvider,
				validationState);

			visitor.Validate(metadata, prefix, model);

			// and everything else 

			if (_runMvcValidation) {
				visitor = new ValidationVisitor(
					actionContext,
					_compositeProvider, // all except the FV provider
					_validatorCache,
					_modelMetadataProvider,
					validationState);

				visitor.Validate(metadata, prefix, model);
			}

			// Re-add errors that we took out if FV didn't add a key. 
			foreach (var pair in entries) {
				if (pair.Key.ValidationState != ModelValidationState.Invalid) {
					pair.Key.Errors.Add(pair.Value);
					pair.Key.ValidationState = ModelValidationState.Invalid;
				}
			}
		}
	}

	/// <summary>
	/// ModelValidatorProvider implementation only used for child properties.
	/// </summary>
	internal class FluentValidationModelValidatorProvider : IModelValidatorProvider {
		private readonly IList<IModelValidatorProvider> _otherValidators;

		private IModelValidatorProvider _inner;
		private readonly bool _shouldExecute;
		private IValidatorFactory _validatorFactory;
		private CustomizeValidatorAttribute _customizations;

		public FluentValidationModelValidatorProvider(IValidatorFactory validatorFactory, CustomizeValidatorAttribute customizations, IModelValidatorProvider inner) {
			_validatorFactory = validatorFactory;
			_customizations = customizations;
			_inner = inner;
		}

		public FluentValidationModelValidatorProvider(IList<IModelValidatorProvider> otherValidators) {
			_otherValidators = otherValidators;
		}

		public void CreateValidators(ModelValidatorProviderContext context) {

			if (context.ModelMetadata.MetadataKind == ModelMetadataKind.Type) {

			//	var validator = _validatorFactory.GetValidator(context.ModelMetadata.ModelType);

//				if (_otherValidators != null) {
//					_otherValidators.SelectMany(x => x.CreateValidators(context))
//				}

//				if (validator != null) {
					context.Results.Add(new ValidatorItem {
						IsReusable = true,
						Validator = new FluentValidationModelValidator(/*validator, _customizations*/)
					});
//				}
			}

			if (typeof(IValidatableObject).IsAssignableFrom(context.ModelMetadata.ModelType)) {
				context.Results.Add(new ValidatorItem {
					Validator = new ValidatableObjectAdapter(),
					IsReusable = true
				});
			}




			//_inner.CreateValidators(context);
		}
	}

	internal class FluentValidationModelValidator : IModelValidator {
//		private IValidator _validator;
//		private CustomizeValidatorAttribute _customizations;
//
//		public FluentValidationModelValidator(IValidator validator, CustomizeValidatorAttribute customizations) {
//			_validator = validator;
//			_customizations = customizations;
//		}

		public FluentValidationModelValidator() {
			
		}

		public IEnumerable<ModelValidationResult> Validate(ModelValidationContext mvContext) {

			var factory = mvContext.ActionContext.HttpContext.RequestServices.GetService(typeof(IValidatorFactory)) as IValidatorFactory;

			if (factory != null) {
				var validator = factory.GetValidator(mvContext.ModelMetadata.ModelType);

				if (validator != null) {
					var customizations = GetCustomizations(mvContext.ActionContext, mvContext.ModelMetadata.ModelType, "");
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



		private static CustomizeValidatorAttribute GetCustomizations(ActionContext actionContext, Type type, string prefix) {

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