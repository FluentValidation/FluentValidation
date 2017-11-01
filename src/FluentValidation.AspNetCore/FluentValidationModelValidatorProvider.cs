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
	using Microsoft.AspNetCore.Mvc.ModelBinding.Internal;
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
			_compositeProvider = new CompositeModelValidatorProvider(validatorProviders); //.Except(new IModelValidatorProvider[]{ _fvProvider }).ToList());
		}

		public void Validate(ActionContext actionContext, ValidationStateDictionary validationState, string prefix, object model) {

			// This is all to work around the default "Required" messages.

			var requiredErrorsNotHandledByFv = new List<KeyValuePair<ModelStateEntry, ModelError>>();
			
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
						requiredErrorsNotHandledByFv.Add(new KeyValuePair<ModelStateEntry, ModelError>(entry.Value, new ModelError(err.ErrorMessage.Replace("_FV_REQUIRED|", string.Empty)))); ;
					}
				}
				
			}

			// Run default validation

			//_inner.Validate(actionContext, validationState, prefix, model);

			var metadata = model == null ? null : _modelMetadataProvider.GetMetadataForType(model.GetType());

			var validatorProvider = _runMvcValidation ? _compositeProvider : _fvProvider;

			var visitor = new ValidationVisitor2(
				actionContext,
				validatorProvider,
				_validatorCache,
				_modelMetadataProvider,
				validationState);

			visitor.Validate(metadata, prefix, model);


			// Re-add errors that we took out if FV didn't add a key. 
			foreach (var pair in requiredErrorsNotHandledByFv) {
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
		private bool _implicitValidationEnabled;

		public FluentValidationModelValidatorProvider(IValidatorFactory validatorFactory, CustomizeValidatorAttribute customizations, IModelValidatorProvider inner, bool implicitValidationEnabled) {
			_validatorFactory = validatorFactory;
			_customizations = customizations;
			_inner = inner;
			_implicitValidationEnabled = implicitValidationEnabled;
		}

		public FluentValidationModelValidatorProvider(IList<IModelValidatorProvider> otherValidators, bool implicitValidationEnabled) {
			_otherValidators = otherValidators;
			_implicitValidationEnabled = implicitValidationEnabled;
		}

		public void CreateValidators(ModelValidatorProviderContext context) {

			if (context.ModelMetadata.MetadataKind == ModelMetadataKind.Type || (context.ModelMetadata.MetadataKind == ModelMetadataKind.Property && _implicitValidationEnabled)) {

			//	var validator = _validatorFactory.GetValidator(context.ModelMetadata.ModelType);

//				if (_otherValidators != null) {
//					_otherValidators.SelectMany(x => x.CreateValidators(context))
//				}

//				if (validator != null) {
					context.Results.Add(new ValidatorItem {
						IsReusable = false,
						Validator = new FluentValidationModelValidator(/*validator, _customizations*/)
					});
//				}
			}

//			if (typeof(IValidatableObject).IsAssignableFrom(context.ModelMetadata.ModelType)) {
//				context.Results.Add(new ValidatorItem {
//					Validator = new ValidatableObjectAdapter(),
//					IsReusable = false
//				});
//			}




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

	/// <summary>
	/// A visitor implementation that interprets <see cref="ValidationStateDictionary"/> to traverse
	/// a model object graph and perform validation.
	/// </summary>
	public class ValidationVisitor2 {
		private readonly IModelValidatorProvider _validatorProvider;
		private readonly IModelMetadataProvider _metadataProvider;
		private readonly ValidatorCache _validatorCache;
		private readonly ActionContext _actionContext;
		private readonly ModelStateDictionary _modelState;
		private readonly ValidationStateDictionary _validationState;
		private readonly ValidationStack _currentPath;

		private object _container;
		private string _key;
		private object _model;
		private ModelMetadata _metadata;
		private IValidationStrategy _strategy;

		/// <summary>
		/// Creates a new <see cref="ValidationVisitor"/>.
		/// </summary>
		/// <param name="actionContext">The <see cref="ActionContext"/> associated with the current request.</param>
		/// <param name="validatorProvider">The <see cref="IModelValidatorProvider"/>.</param>
		/// <param name="validatorCache">The <see cref="ValidatorCache"/> that provides a list of <see cref="IModelValidator"/>s.</param>
		/// <param name="metadataProvider">The provider used for reading metadata for the model type.</param>
		/// <param name="validationState">The <see cref="ValidationStateDictionary"/>.</param>
		public ValidationVisitor2(
			ActionContext actionContext,
			IModelValidatorProvider validatorProvider,
			ValidatorCache validatorCache,
			IModelMetadataProvider metadataProvider,
			ValidationStateDictionary validationState) {
			if (actionContext == null) {
				throw new ArgumentNullException(nameof(actionContext));
			}

			if (validatorProvider == null) {
				throw new ArgumentNullException(nameof(validatorProvider));
			}

			if (validatorCache == null) {
				throw new ArgumentNullException(nameof(validatorCache));
			}

			_actionContext = actionContext;
			_validatorProvider = validatorProvider;
			_validatorCache = validatorCache;

			_metadataProvider = metadataProvider;
			_validationState = validationState;

			_modelState = actionContext.ModelState;
			_currentPath = new ValidationStack();
		}

		/// <summary>
		/// Validates a object.
		/// </summary>
		/// <param name="metadata">The <see cref="ModelMetadata"/> associated with the model.</param>
		/// <param name="key">The model prefix key.</param>
		/// <param name="model">The model object.</param>
		/// <returns><c>true</c> if the object is valid, otherwise <c>false</c>.</returns>
		public bool Validate(ModelMetadata metadata, string key, object model) {
			return Validate(metadata, key, model, alwaysValidateAtTopLevel: false);
		}

		/// <summary>
		/// Validates a object.
		/// </summary>
		/// <param name="metadata">The <see cref="ModelMetadata"/> associated with the model.</param>
		/// <param name="key">The model prefix key.</param>
		/// <param name="model">The model object.</param>
		/// <param name="alwaysValidateAtTopLevel">If <c>true</c>, applies validation rules even if the top-level value is <c>null</c>.</param>
		/// <returns><c>true</c> if the object is valid, otherwise <c>false</c>.</returns>
		public bool Validate(ModelMetadata metadata, string key, object model, bool alwaysValidateAtTopLevel) {
			if (model == null && key != null && !alwaysValidateAtTopLevel) {
				var entry = _modelState[key];
				if (entry != null && entry.ValidationState != ModelValidationState.Valid) {
					entry.ValidationState = ModelValidationState.Valid;
				}

				return true;
			}

			return Visit(metadata, key, model);
		}

		/// <summary>
		/// Validates a single node in a model object graph.
		/// </summary>
		/// <returns><c>true</c> if the node is valid, otherwise <c>false</c>.</returns>
		protected virtual bool ValidateNode() {
			var state = _modelState.GetValidationState(_key);

			// Rationale: we might see the same model state key used for two different objects.
			// We want to run validation unless it's already known that this key is invalid.
			if (state != ModelValidationState.Invalid) {
				var validators = _validatorCache.GetValidators(_metadata, _validatorProvider);

				var count = validators.Count;
				if (count > 0) {
					var context = new ModelValidationContext(
						_actionContext,
						_metadata,
						_metadataProvider,
						_container,
						_model);

					var results = new List<ModelValidationResult>();
					for (var i = 0; i < count; i++) {
						results.AddRange(validators[i].Validate(context));
					}

					var resultsCount = results.Count;
					for (var i = 0; i < resultsCount; i++) {
						var result = results[i];
						var key = ModelNames.CreatePropertyModelName(_key, result.MemberName);

						// If this is a top-level parameter/property, the key would be empty,
						// so use the name of the top-level property
						if (string.IsNullOrEmpty(key) && _metadata.PropertyName != null) {
							key = _metadata.PropertyName;
						}

						_modelState.TryAddModelError(key, result.Message);
					}
				}
			}

			state = _modelState.GetFieldValidationState(_key);
			if (state == ModelValidationState.Invalid) {
				return false;
			} else {
				// If the field has an entry in ModelState, then record it as valid. Don't create
				// extra entries if they don't exist already.
				var entry = _modelState[_key];
				if (entry != null) {
					entry.ValidationState = ModelValidationState.Valid;
				}

				return true;
			}
		}

		private bool Visit(ModelMetadata metadata, string key, object model) {
			//RuntimeHelpers.EnsureSufficientExecutionStack();

			if (model != null && !_currentPath.Push(model)) {
				// This is a cycle, bail.
				return true;
			}

			var entry = GetValidationEntry(model);
			key = entry?.Key ?? key ?? string.Empty;
			metadata = entry?.Metadata ?? metadata;
			var strategy = entry?.Strategy;

			if (_modelState.HasReachedMaxErrors) {
				SuppressValidation(key);
				return false;
			} else if (entry != null && entry.SuppressValidation) {
				// Use the key on the entry, because we might not have entries in model state.
				SuppressValidation(entry.Key);
				_currentPath.Pop(model);
				return true;
			}

			using (StateManager.Recurse(this, key ?? string.Empty, metadata, model, strategy)) {
				if (_metadata.IsEnumerableType) {
					return VisitComplexType(DefaultCollectionValidationStrategy.Instance);
				}

				if (_metadata.IsComplexType) {
					return VisitComplexType(DefaultComplexObjectValidationStrategy.Instance);
				}

				return VisitSimpleType();
			}
		}

		// Covers everything VisitSimpleType does not i.e. both enumerations and complex types.
		private bool VisitComplexType(IValidationStrategy defaultStrategy) {
			var isValid = true;

			if (_model != null && _metadata.ValidateChildren) {
				var strategy = _strategy ?? defaultStrategy;
				isValid = VisitChildren(strategy);
			} else if (_model != null) {
				// Suppress validation for the entries matching this prefix. This will temporarily set
				// the current node to 'skipped' but we're going to visit it right away, so subsequent
				// code will set it to 'valid' or 'invalid'
				SuppressValidation(_key);
			}

			// Double-checking HasReachedMaxErrors just in case this model has no properties.
			if (/*isValid && */!_modelState.HasReachedMaxErrors) {
				isValid &= ValidateNode();
			}

			return isValid;
		}

		private bool VisitSimpleType() {
			if (_modelState.HasReachedMaxErrors) {
				SuppressValidation(_key);
				return false;
			}

			return ValidateNode();
		}

		private bool VisitChildren(IValidationStrategy strategy) {
			var isValid = true;
			var enumerator = strategy.GetChildren(_metadata, _key, _model);
			var parentEntry = new ValidationEntry(_metadata, _key, _model);

			while (enumerator.MoveNext()) {
				var entry = enumerator.Current;
				var metadata = entry.Metadata;
				var key = entry.Key;
//				if (metadata.PropertyValidationFilter?.ShouldValidateEntry(entry, parentEntry) == false) {
//					SuppressValidation(key);
//					continue;
//				}

				isValid &= Visit(metadata, key, entry.Model);
			}

			return isValid;
		}

		private void SuppressValidation(string key) {
			if (key == null) {
				// If the key is null, that means that we shouldn't expect any entries in ModelState for
				// this value, so there's nothing to do.
				return;
			}

			var entries = _modelState.FindKeysWithPrefix(key);
			foreach (var entry in entries) {
				entry.Value.ValidationState = ModelValidationState.Skipped;
			}
		}

		private ValidationStateEntry GetValidationEntry(object model) {
			if (model == null || _validationState == null) {
				return null;
			}

			ValidationStateEntry entry;
			_validationState.TryGetValue(model, out entry);
			return entry;
		}

		private struct StateManager : IDisposable {
			private readonly ValidationVisitor2 _visitor;
			private readonly object _container;
			private readonly string _key;
			private readonly ModelMetadata _metadata;
			private readonly object _model;
			private readonly object _newModel;
			private readonly IValidationStrategy _strategy;

			public static StateManager Recurse(
				ValidationVisitor2 visitor,
				string key,
				ModelMetadata metadata,
				object model,
				IValidationStrategy strategy) {
				var recursifier = new StateManager(visitor, model);

				visitor._container = visitor._model;
				visitor._key = key;
				visitor._metadata = metadata;
				visitor._model = model;
				visitor._strategy = strategy;

				return recursifier;
			}

			public StateManager(ValidationVisitor2 visitor, object newModel) {
				_visitor = visitor;
				_newModel = newModel;

				_container = _visitor._container;
				_key = _visitor._key;
				_metadata = _visitor._metadata;
				_model = _visitor._model;
				_strategy = _visitor._strategy;
			}

			public void Dispose() {
				_visitor._container = _container;
				_visitor._key = _key;
				_visitor._metadata = _metadata;
				_visitor._model = _model;
				_visitor._strategy = _strategy;

				_visitor._currentPath.Pop(_newModel);
			}
		}
	}

}