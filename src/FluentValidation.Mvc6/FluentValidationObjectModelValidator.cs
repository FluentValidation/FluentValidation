namespace FluentValidation.Mvc {
	using System;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.ModelBinding;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

	//TODO: Need support for CustomizeValidatorAttribute and client-side

	public class FluentValidationObjectModelValidator : IObjectModelValidator {
		private readonly IValidatorFactory _validatorFactory;

		/// <summary>
		///     Initializes a new instance of <see cref="FluentValidationObjectModelValidator" />.
		/// </summary>
		public FluentValidationObjectModelValidator(IModelMetadataProvider modelMetadataProvider,
			IValidatorFactory validatorFactory) {
			if (modelMetadataProvider == null) {
				throw new ArgumentNullException(nameof(modelMetadataProvider));
			}

			_validatorFactory = validatorFactory;
		}

		public void Validate(ActionContext actionContext, IModelValidatorProvider validatorProvider,
			ValidationStateDictionary validationState, string prefix, object model) {
			if (actionContext == null) {
				throw new ArgumentNullException(nameof(actionContext));
			}

			// would model ever be null ??

			if (model == null) {
				return;
			}

			// get our IValidator

			var validator = _validatorFactory.GetValidator(model.GetType());

			if (validator == null) {
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

			foreach (var modelError in result.Errors) {
				actionContext.ModelState.AddModelError(modelError.PropertyName, modelError.ErrorMessage);
			}
		}
	}
}