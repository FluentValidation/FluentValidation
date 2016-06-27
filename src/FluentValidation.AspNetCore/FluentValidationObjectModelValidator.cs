namespace FluentValidation.AspNetCore {
	using System;
	using System.Collections.Generic;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Internal;
	using Microsoft.AspNetCore.Mvc.ModelBinding;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using System.Linq;
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

		public void Validate(ActionContext actionContext, IModelValidatorProvider validatorProvider,
			ValidationStateDictionary validationState, string prefix, object model) {
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
//				visitor.Validate(metadata, prefix, model);

				return;
			}

			foreach (var value in actionContext.ModelState.Values
				.Where(v => v.ValidationState == ModelValidationState.Unvalidated)) {
				// Set all unvalidated states to valid. If we end up adding an error below then that properties state
				// will become ModelValidationState.Invalid and will set ModelState.IsValid to false

				value.ValidationState = ModelValidationState.Valid;
			}

           

			// validate the model using Fluent Validation rules

			var result = validator.Validate(model);

			// add all our model errors to the modelstate

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
	}
}