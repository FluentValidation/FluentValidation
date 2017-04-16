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
// The latest version of this file can be found at https://github.com/JeremySkinner/FluentValidation
#endregion

namespace FluentValidation.Mvc {
	using System;
	using System.Web.Mvc;
	using Internal;

	public class CustomizeValidatorAttribute : CustomModelBinderAttribute, IModelBinder {
		public string RuleSet { get; set; }
		public string Properties { get; set; }
		public Type Interceptor { get; set; }

		private const string key = "_FV_CustomizeValidator" ;

		public override IModelBinder GetBinder() {
			return this;
		}

		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
			// Originally I thought about storing this inside ModelMetadata.AdditionalValues.
			// Unfortunately, DefaultModelBinder overwrites this property internally.
			// So anything added to AdditionalValues will not be passed to the ValidatorProvider.
			// This is a very poor design decision. 
			// The only piece of information that is passed all the way down to the validator is the controller context.
			// So we resort to storing the attribute in HttpContext.Items. 
			// Horrible, horrible, horrible hack. Horrible.
			controllerContext.HttpContext.Items[key] = this;

			var innerBinder = ModelBinders.Binders.GetBinder(bindingContext.ModelType);
			var result = innerBinder.BindModel(controllerContext, bindingContext);

			controllerContext.HttpContext.Items.Remove(key);

			return result;
		}

		public static CustomizeValidatorAttribute GetFromControllerContext(ControllerContext context) {
			return context.HttpContext.Items[key] as CustomizeValidatorAttribute;
		}

		/// <summary>
		/// Builds a validator selector from the options specified in the attribute's properties.
		/// </summary>
		public IValidatorSelector ToValidatorSelector() {
			IValidatorSelector selector;

			if(! string.IsNullOrEmpty(RuleSet)) {
				var rulesets = RuleSet.Split(',', ';');
				selector = CreateRulesetValidatorSelector(rulesets);
			}
			else if(! string.IsNullOrEmpty(Properties)) {
				var properties = Properties.Split(',', ';');
				selector = CreateMemberNameValidatorSelector(properties);
			}
			else {
				selector = CreateDefaultValidatorSelector();
			}

			return selector;

		}

		protected virtual IValidatorSelector CreateRulesetValidatorSelector(string[] ruleSets) {
			return ValidatorOptions.ValidatorSelectors.RulesetValidatorSelectorFactory(ruleSets);
		}

		protected virtual IValidatorSelector CreateMemberNameValidatorSelector(string[] properties) {
			return ValidatorOptions.ValidatorSelectors.MemberNameValidatorSelectorFactory(properties);
		}

		protected virtual IValidatorSelector CreateDefaultValidatorSelector() {
			return ValidatorOptions.ValidatorSelectors.DefaultValidatorSelectorFactory();
		}

		public IValidatorInterceptor GetInterceptor() {
			if (Interceptor == null) return null;

			if(! typeof(IValidatorInterceptor).IsAssignableFrom(Interceptor)) {
				throw new InvalidOperationException("Type {0} is not an IValidatorInterceptor. The Interceptor property of CustomizeValidatorAttribute must implement IValidatorInterceptor.");
			}

			var instance = Activator.CreateInstance(Interceptor) as IValidatorInterceptor;

			if(instance == null) {
				throw new InvalidOperationException("Type {0} is not an IValidatorInterceptor. The Interceptor property of CustomizeValidatorAttribute must implement IValidatorInterceptor.");
			}

			return instance;
		}
	}
}