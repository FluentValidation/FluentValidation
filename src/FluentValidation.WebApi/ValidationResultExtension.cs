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

namespace FluentValidation.WebApi {
    using System.Collections.Generic;
    using System.Globalization;
    using Results;
    using System.Web.Http.ModelBinding;
    using System.Web.Http.ValueProviders;

    public static class ValidationResultExtension {
		/// <summary>
		/// Stores the errors in a ValidationResult object to the specified modelstate dictionary.
		/// </summary>
		/// <param name="result">The validation result to store</param>
		/// <param name="modelState">The ModelStateDictionary to store the errors in.</param>
		/// <param name="prefix">An optional prefix. If ommitted, the property names will be the keys. If specified, the prefix will be concatenatd to the property name with a period. Eg "user.Name"</param>
		public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState, string prefix) {
			if (!result.IsValid) {
				foreach (var error in result.Errors) {
					string key = string.IsNullOrEmpty(prefix) ? error.PropertyName : prefix + "." + error.PropertyName;
					modelState.AddModelError(key, error.ErrorMessage);
					//To work around an issue with MVC: SetModelValue must be called if AddModelError is called.
					modelState.SetModelValue(key, new ValueProviderResult(error.AttemptedValue ?? "", (error.AttemptedValue ?? "").ToString(), CultureInfo.CurrentCulture));
				}
			}
		}
	}
}