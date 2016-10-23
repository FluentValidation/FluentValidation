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

namespace FluentValidation {
	using Internal;

	/// <summary>
	/// Gets a validation context for a particular type.
	/// </summary>
	public interface IValidationContextFactory {
		/// <summary>
		/// Gets the validation context for the specified type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instanceToValidate">The instance to validate</param>
		/// <param name="propertyChain">The chain of properties</param>
		/// <param name="validatorSelector">The rule selector</param>
		IValidationContext<T> Get<T>(T instanceToValidate, PropertyChain propertyChain, IValidatorSelector validatorSelector);

		/// <summary>
		/// Gets the validation context for the specified type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instanceToValidate">The instance to validate</param>
		/// <param name="propertyChain">The chain of properties</param>
		/// <param name="validatorSelector">The rule selector</param>
		/// <param name="isChildContext"></param>
		IValidationContext<T> Get<T>(T instanceToValidate, PropertyChain propertyChain, IValidatorSelector validatorSelector, bool isChildContext);

		/// <summary>
		/// Gets the validation context for the specified object.
		/// </summary>
		/// <param name="instanceToValidate">The instance to validate</param>
		/// <param name="propertyChain">The chain of properties</param>
		/// <param name="validatorSelector">The rule selector</param>
		IValidationContext Get(object instanceToValidate, PropertyChain propertyChain, IValidatorSelector validatorSelector);

		/// <summary>
		/// Gets the validation context for the specified object.
		/// </summary>
		/// <param name="instanceToValidate">The instance to validate</param>
		IValidationContext Get(object instanceToValidate);
	}
}
