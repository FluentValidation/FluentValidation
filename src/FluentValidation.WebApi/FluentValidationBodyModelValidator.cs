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

namespace FluentValidation.WebApi
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Globalization;
	using System.Linq;
	using System.Net.Http.Formatting;
	using System.Web.Http.Controllers;
	using System.Web.Http.Metadata;
	using System.Web.Http.ModelBinding;
	using System.Web.Http.Validation;

	/// <summary>
	/// Recursively validate an object.
	/// Is just a copy of the DefaultBodyModelValidator but includes a "patch" for the FluentValidationModelValidator
	/// When the whole model is validated the errors get added to the ModelState grouped by property instead of all under
	/// the model. It also changes the check of previous errors to avoid missing error messages when there are bind errors
	/// If the FluentValidationModelValidator is not used to validate, fallbacks to the DefaultBodyModelValidator behaviour
	/// Another difference is that the IModelValidatorCache had to be removed because it's internal to the asp net framework
	/// (affects performance but not the behaviour)
	/// </summary>
	public class FluentValidationBodyModelValidator : IBodyModelValidator {
		private interface IKeyBuilder {
			string AppendTo(string prefix);
		}

		public bool Validate(object model, Type type, ModelMetadataProvider metadataProvider, HttpActionContext actionContext, string keyPrefix) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}

			if (metadataProvider == null) {
				throw new ArgumentNullException("metadataProvider");
			}

			if (actionContext == null) {
				throw new ArgumentNullException("actionContext");
			}

			if (model != null && MediaTypeFormatterCollection.IsTypeExcludedFromValidation(model.GetType())) {
				// no validation for some DOM like types
				return true;
			}

			ModelValidatorProvider[] validatorProviders = actionContext.GetValidatorProviders().ToArray();
			// Optimization : avoid validating the object graph if there are no validator providers
			if (validatorProviders == null || validatorProviders.Length == 0) {
				return true;
			}

			ModelMetadata metadata = metadataProvider.GetMetadataForType(() => model, type);
			ValidationContext validationContext = new ValidationContext {
				MetadataProvider = metadataProvider,
				ActionContext = actionContext,
				ModelState = actionContext.ModelState,
				Visited = new HashSet<object>(),
				KeyBuilders = new Stack<IKeyBuilder>(),
				RootPrefix = keyPrefix
			};
			return this.ValidateNodeAndChildren(metadata, validationContext, container: null);
		}

		internal static bool IsSimpleType(Type type)
		{
            return type.IsPrimitive ||
                   (type.IsArray && type.GetElementType().IsPrimitive) ||
                   type.Equals(typeof(string)) ||
				   type.Equals(typeof(DateTime)) ||
				   type.Equals(typeof(Decimal)) ||
				   type.Equals(typeof(Guid)) ||
				   type.Equals(typeof(DateTimeOffset)) ||
				   type.Equals(typeof(TimeSpan));
        }
		
		private bool ValidateNodeAndChildren(ModelMetadata metadata, ValidationContext validationContext, object container) {

			object model = null;

			try {
				model = metadata.Model;
			}
			catch {
				// Retrieving the model failed - typically caused by a property getter throwing
				// Being unable to retrieve a property is not a validation error - many properties can only be retrieved if certain conditions are met
				// For example, Uri.AbsoluteUri throws for relative URIs but it shouldn't be considered a validation error
				return true;
			}

			bool isValid = true;

			// Optimization: we don't need to recursively traverse the graph for null and primitive types
			if (model == null || IsSimpleType(model.GetType())) {
				return ShallowValidate(metadata, validationContext, container);
			}

			// Check to avoid infinite recursion. This can happen with cycles in an object graph.
			if (validationContext.Visited.Contains(model)) {
				return true;
			}
			validationContext.Visited.Add(model);

			// Validate the children first - depth-first traversal
			IEnumerable enumerableModel = model as IEnumerable;
			if (enumerableModel == null) {
				isValid = this.ValidateProperties(metadata, validationContext);
			}
			else {
				isValid = this.ValidateElements(enumerableModel, validationContext);
			}

            // Validate this node as well
            isValid = ShallowValidate(metadata, validationContext, container) && isValid;
			
			// Pop the object so that it can be validated again in a different path
			validationContext.Visited.Remove(model);

			return isValid;
		}

		private bool ValidateProperties(ModelMetadata metadata, ValidationContext validationContext) {
			bool isValid = true;
			PropertyScope propertyScope = new PropertyScope();
			validationContext.KeyBuilders.Push(propertyScope);
			
			foreach (ModelMetadata childMetadata in validationContext.MetadataProvider.GetMetadataForProperties(metadata.Model, metadata.ModelType)) {
				propertyScope.PropertyName = childMetadata.PropertyName;
			
				if (!this.ValidateNodeAndChildren(childMetadata, validationContext, metadata.Model)) {
					isValid = false;
				}
			}

			validationContext.KeyBuilders.Pop();
			return isValid;
		}

		private bool ValidateElements(IEnumerable model, ValidationContext validationContext) {
			bool isValid = true;
			Type elementType = GetElementType(model.GetType());
			ModelMetadata elementMetadata = validationContext.MetadataProvider.GetMetadataForType(null, elementType);

			ElementScope elementScope = new ElementScope() { Index = 0 };
			validationContext.KeyBuilders.Push(elementScope);
			
			foreach (object element in model) {
				elementMetadata.Model = element;
				
				if (!this.ValidateNodeAndChildren(elementMetadata, validationContext, model)) {
					isValid = false;
				}

				elementScope.Index++;
			}

			validationContext.KeyBuilders.Pop();
			return isValid;
		}

		// Validates a single node (not including children)
		// Returns true if validation passes successfully
		private static bool ShallowValidate(ModelMetadata metadata, ValidationContext validationContext, object container) {
			bool isValid = true;
			string key = null;

			foreach (ModelValidator validator in validationContext.ActionContext.GetValidators(metadata)) {
				// we use this flag to determine if we use the "patched" version or the default
				var isFluentModelValidator = validator is FluentValidationModelValidator;
				
				foreach (ModelValidationResult error in validator.Validate(metadata, container)) {
					if (key == null) {
						key = validationContext.RootPrefix;

						foreach (IKeyBuilder keyBuilder in validationContext.KeyBuilders.Reverse()) {
							key = keyBuilder.AppendTo(key);
						}

						// Avoid adding model errors if the model state already contains model errors for that key
						// We can't perform this check earlier because we compute the key string only when we detect an error
						/*
						 The default condition is: !validationContext.ModelState.IsValidField(key)
						 For fluent validation we use validationContext.ModelState.ContainsKey(key)
						 This is to avoid missing errors in the ModelState in case the binder added errors before the validation
						*/
						if ((!isFluentModelValidator && !validationContext.ModelState.IsValidField(key)) ||
							(isFluentModelValidator && validationContext.ModelState.ContainsKey(key))) {
							return false;
						}
					}

					/*
					 The default key is: key
					 For fluent validation we use: CreatePropertyModelName(key, error.MemberName)
					 This gets the errors group by property
					*/
					validationContext.ModelState.AddModelError(isFluentModelValidator ? CreatePropertyModelName(key, error.MemberName) : key, error.Message);
					isValid = false;
				}
			}
			return isValid;
		}

		private static Type GetElementType(Type type) {
			Contract.Assert(typeof(IEnumerable).IsAssignableFrom(type));
			if (type.IsArray) {
				return type.GetElementType();
			}

			foreach (Type implementedInterface in type.GetInterfaces()) {
				if (implementedInterface.IsGenericType && implementedInterface.GetGenericTypeDefinition() == typeof(IEnumerable<>)) {
					return implementedInterface.GetGenericArguments()[0];
				}
			}

			return typeof(object);
		}

		internal static string CreatePropertyModelName(string prefix, string propertyName) {
			if (String.IsNullOrEmpty(prefix)) {
				return propertyName ?? String.Empty;
			}
			else if (String.IsNullOrEmpty(propertyName)) {
				return prefix ?? String.Empty;
			}
			else {
				return prefix + "." + propertyName;
			}
		}

		private class PropertyScope : IKeyBuilder {
			public string PropertyName { get; set; }

			public string AppendTo(string prefix) {
				return CreatePropertyModelName(prefix, this.PropertyName);
			}
		}

		internal static string CreateIndexModelName(string parentName, int index) {
			return CreateIndexModelName(parentName, index.ToString(CultureInfo.InvariantCulture));
		}

		internal static string CreateIndexModelName(string parentName, string index) {
			return (parentName.Length == 0) ? "[" + index + "]" : parentName + "[" + index + "]";
		}

		private class ElementScope : IKeyBuilder {
			public int Index { get; set; }

			public string AppendTo(string prefix)
			{
				return CreateIndexModelName(prefix, this.Index);
			}
		}

		private class ValidationContext {
			public ModelMetadataProvider MetadataProvider { get; set; }
			public HttpActionContext ActionContext { get; set; }
			public ModelStateDictionary ModelState { get; set; }
			public HashSet<object> Visited { get; set; }
			public Stack<IKeyBuilder> KeyBuilders { get; set; }
			public string RootPrefix { get; set; }
		}
	}
}
