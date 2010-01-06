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
	using System.Web.Mvc;
	using Mvc;

	/// <summary>
	/// Model Binder implementation that integrated with FluentValidation. 
	/// After binding takes place a validator will be instantiated using the specified validator factory
	/// and the bound object will be validated. Any validation errors are added to ModelState.
	/// </summary>
	public class FluentValidationModelBinder : FluentValidationModelBinderDecorator {
		public FluentValidationModelBinder(IValidatorFactory validatorFactory)
			: base(validatorFactory, new IgnoreDataAnnotationsModelBinder()) {
		}

		//It's easier to just bypass the DataAnnotations validator rather than try to play nice with it
		private class IgnoreDataAnnotationsModelBinder : DefaultModelBinder {
			protected override void OnModelUpdated(ControllerContext controllerContext, ModelBindingContext bindingContext) {
				//no-op
			}

			protected override void OnPropertyValidated(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor, object value) {
				//no-op
			}
		}
	}
}

