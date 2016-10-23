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
	/// The default implementation for the validation context creation
	/// </summary>
	public class DefaultValidationContextFactory : IValidationContextFactory {

		/// <summary>
		/// Gets the validation context for the specified type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instanceToValidate">The instance to validate</param>
		/// <param name="propertyChain">The chain of properties</param>
		/// <param name="validatorSelector">The rule selector</param>
		public virtual IValidationContext<T> Get<T>(T instanceToValidate, PropertyChain propertyChain, IValidatorSelector validatorSelector) {
			return new ValidationContext<T>(instanceToValidate, propertyChain, validatorSelector);
		}

		/// <summary>
		/// Gets the validation context for the specified type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instanceToValidate">The instance to validate</param>
		/// <param name="propertyChain">The chain of properties</param>
		/// <param name="validatorSelector">The rule selector</param>
		/// <param name="isChildContext"></param>
		public IValidationContext<T> Get<T>(T instanceToValidate, PropertyChain propertyChain, IValidatorSelector validatorSelector, bool isChildContext) {
			return new ValidationContext<T>(instanceToValidate, propertyChain, validatorSelector, isChildContext);
		}

		/// <summary>
		/// Gets the validation context for the specified object.
		/// </summary>
		/// <param name="instanceToValidate">The instance to validate</param>
		/// <param name="propertyChain">The chain of properties</param>
		/// <param name="validatorSelector">The rule selector</param>
		public virtual IValidationContext Get(object instanceToValidate, PropertyChain propertyChain, IValidatorSelector validatorSelector) {
			return new ValidationContext(instanceToValidate, propertyChain, validatorSelector);
		}

		/// <summary>
		/// Gets the validation context for the specified object.
		/// </summary>
		/// <param name="instanceToValidate">The instance to validate</param>
		public virtual IValidationContext Get(object instanceToValidate) {
			return new ValidationContext(instanceToValidate);
		}
	}
}
