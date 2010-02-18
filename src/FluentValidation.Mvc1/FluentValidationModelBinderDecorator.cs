#region License
// Copyright 2008-2009 Jeremy Skinner (http://www.jeremyskinner.co.uk)
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
// The latest version of this file can be found at http://www.codeplex.com/FluentValidation
#endregion

namespace FluentValidation.Mvc {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;

	/// <summary>
	/// Model Binder implementation that integrated with FluentValidation.
	/// After binding takes place a validator will be instantiated using the specified validator factory
	/// and the bound object will be validated. Any validation errors are added to ModelState.
	/// </summary>
	public class FluentValidationModelBinderDecorator : IModelBinder {
		readonly IValidatorFactory validatorFactory;
		readonly IModelBinder wrappedBinder;

		public FluentValidationModelBinderDecorator(IValidatorFactory validatorFactory, IModelBinder wrappedBinder) {
			this.validatorFactory = validatorFactory;
			this.wrappedBinder = wrappedBinder;
		}

		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
			var boundInstance = wrappedBinder.BindModel(controllerContext, bindingContext);

			if (boundInstance != null) {
				var validator = CreateValidator(bindingContext);

				if (validator != null) {
					PerformValidation(boundInstance, validator, bindingContext);
				}
			}

			return boundInstance;
		}

		protected virtual void PerformValidation(object instance, IValidator validator, ModelBindingContext context) {
			string modelName = WasFallbackPerformed(context) ? string.Empty : context.ModelName;
			var result = validator.Validate(instance);
			result.AddToModelState(context.ModelState, modelName);
		}

		protected virtual IValidator CreateValidator(ModelBindingContext context) {
			return validatorFactory.GetValidator(context.ModelType);
		}

		protected bool WasFallbackPerformed(ModelBindingContext context) {
			if (!string.IsNullOrEmpty(context.ModelName)
			&& !DoesAnyKeyHavePrefix(context.ValueProvider, context.ModelName)
			&& context.FallbackToEmptyPrefix) {
				return true;
			}

			return false;
		}

		//From the internal DictionaryHelpers class in the ASP.NET MVC 1.0 source code
		private static bool DoesAnyKeyHavePrefix<TValue>(IDictionary<string, TValue> dictionary, string prefix) {
			return FindKeysWithPrefix(dictionary, prefix).Any();
		}

		//From the internal DictionaryHelpers class in the ASP.NET MVC 1.0 source code
		private static IEnumerable<KeyValuePair<string, TValue>> FindKeysWithPrefix<TValue>(IDictionary<string, TValue> dictionary, string prefix) {
			TValue exactMatchValue;
			if (dictionary.TryGetValue(prefix, out exactMatchValue)) {
				yield return new KeyValuePair<string, TValue>(prefix, exactMatchValue);
			}

			foreach (var entry in dictionary) {
				string key = entry.Key;

				if (key.Length <= prefix.Length) {
					continue;
				}

				if (!key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) {
					continue;
				}

				char charAfterPrefix = key[prefix.Length];
				switch (charAfterPrefix) {
					case '[':
					case '.':
						yield return entry;
						break;
				}
			}
		}
	}
}