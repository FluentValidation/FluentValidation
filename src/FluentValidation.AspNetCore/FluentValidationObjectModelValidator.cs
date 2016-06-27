namespace FluentValidation.AspNetCore {
	using System;
	using System.Collections.Generic;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Internal;
	using Microsoft.AspNetCore.Mvc.ModelBinding;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using System.Linq;
	using System.Reflection;
	using Microsoft.AspNetCore.Mvc.Controllers;
	using FluentValidation;

	//TODO: Need support for CustomizeValidatorAttribute and client-side

	public class FluentValidationObjectModelValidator : IObjectModelValidator {
	    public const string InvalidValuePlaceholder = "__FV_InvalidValue";

		private readonly IValidatorFactory _validatorFactory;
		private IModelMetadataProvider _modelMetadataProvider;
		private readonly ValidatorCache _validatorCache;
		private CompositeModelValidatorProvider _validatorProvider;

		/// <summary>
		///     Initializes a new instance of <see cref="FluentValidationObjectModelValidator" />.
		/// </summary>
		public FluentValidationObjectModelValidator(IModelMetadataProvider modelMetadataProvider, IList<IModelValidatorProvider> validatorProviders,
			IValidatorFactory validatorFactory) {
			if (modelMetadataProvider == null) {
				throw new ArgumentNullException(nameof(modelMetadataProvider));
			}

			_validatorFactory = validatorFactory;
			_modelMetadataProvider = modelMetadataProvider;
			_validatorCache = new ValidatorCache();
			_validatorProvider = new CompositeModelValidatorProvider(validatorProviders);
		}

		public void Validate(ActionContext actionContext, ValidationStateDictionary validationState, string prefix, object model)
		{
			if (actionContext == null) {
				throw new ArgumentNullException(nameof(actionContext));
			}

			IValidator validator = null;

			if (model != null) {
				validator = _validatorFactory.GetValidator(model.GetType());
			}

			if (validator == null) {
				// Use default impl if FV doesn't have a validator for the type.
				var visitor = new ValidationVisitor(
						   actionContext,
						   _validatorProvider,
						   _validatorCache,
						   _modelMetadataProvider,
						   validationState);

				var metadata = model == null ? null : _modelMetadataProvider.GetMetadataForType(model.GetType());
				visitor.Validate(metadata, prefix, model);

				return;
			}

			foreach (var value in actionContext.ModelState.Values
				.Where(v => v.ValidationState == ModelValidationState.Unvalidated)) {
				// Set all unvalidated states to valid. If we end up adding an error below then that properties state
				// will become ModelValidationState.Invalid and will set ModelState.IsValid to false

				value.ValidationState = ModelValidationState.Valid;
			}


			var customizations = GetCustomizations(actionContext, model.GetType(), prefix);

			var selector = customizations.ToValidatorSelector();
			var interceptor = customizations.GetInterceptor() ?? (validator as IValidatorInterceptor);
			var context = new FluentValidation.ValidationContext(model, new FluentValidation.Internal.PropertyChain(), selector);

			if (interceptor != null)
			{
				// Allow the user to provide a customized context
				// However, if they return null then just use the original context.
				context = interceptor.BeforeMvcValidation((ControllerContext)actionContext, context) ?? context;
			}

			var result = validator.Validate(context);

			if (interceptor != null)
			{
				// allow the user to provice a custom collection of failures, which could be empty.
				// However, if they return null then use the original collection of failures. 
				result = interceptor.AfterMvcValidation((ControllerContext)actionContext, context, result) ?? result;
			}

			if (!string.IsNullOrEmpty(prefix)) {
		        prefix = prefix + ".";
		    }

			foreach (var modelError in result.Errors) {
                // See if there's already an item in the ModelState for this key. 
			    if (actionContext.ModelState.ContainsKey(prefix + modelError.PropertyName)) {
			        actionContext.ModelState[prefix + modelError.PropertyName].Errors.Clear();
			    }


				actionContext.ModelState.AddModelError(prefix + modelError.PropertyName, modelError.ErrorMessage);
			}


			// Otherwise:
			/*
				
						 */
		}

		private CustomizeValidatorAttribute GetCustomizations(ActionContext actionContext, Type type, string prefix) {
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