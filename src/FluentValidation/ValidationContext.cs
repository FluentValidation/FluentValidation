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
	using System.Collections.Generic;
	using Internal;

	/// <summary>
	/// Validation context
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ValidationContext<T> : ValidationContext {
		/// <summary>
		/// Creates a new validation context
		/// </summary>
		/// <param name="instanceToValidate"></param>
		public ValidationContext(T instanceToValidate) : this(instanceToValidate, new PropertyChain(), ValidatorOptions.ValidatorSelectors.DefaultValidatorSelectorFactory()) {
			
		}

		/// <summary>
		/// Creates a new validation context with a custom property chain and selector
		/// </summary>
		/// <param name="instanceToValidate"></param>
		/// <param name="propertyChain"></param>
		/// <param name="validatorSelector"></param>
		public ValidationContext(T instanceToValidate, PropertyChain propertyChain, IValidatorSelector validatorSelector)
			: base(instanceToValidate, propertyChain, validatorSelector) {

			InstanceToValidate = instanceToValidate;
		}

		/// <summary>
		/// The object to validate
		/// </summary>
		public new T InstanceToValidate { get; private set; }
	}

	/// <summary>
	/// Validation context
	/// </summary>
	public class ValidationContext {

		public Dictionary<string, object> RootContextData { get; internal set; } = new Dictionary<string, object>();

		/// <summary>
		/// Creates a new validation context
		/// </summary>
		/// <param name="instanceToValidate"></param>
		public ValidationContext(object instanceToValidate)
		 : this (instanceToValidate, new PropertyChain(), ValidatorOptions.ValidatorSelectors.DefaultValidatorSelectorFactory()){
			
		}

		/// <summary>
		/// Creates a new validation context with a property chain and validation selector
		/// </summary>
		/// <param name="instanceToValidate"></param>
		/// <param name="propertyChain"></param>
		/// <param name="validatorSelector"></param>
		public ValidationContext(object instanceToValidate, PropertyChain propertyChain, IValidatorSelector validatorSelector) {
			PropertyChain = new PropertyChain(propertyChain);
			InstanceToValidate = instanceToValidate;
			Selector = validatorSelector;
		}

		/// <summary>
		/// Property chain
		/// </summary>
		public PropertyChain PropertyChain { get; private set; }
		/// <summary>
		/// Object being validated
		/// </summary>
		public object InstanceToValidate { get; private set; }
		/// <summary>
		/// Selector
		/// </summary>
		public IValidatorSelector Selector { get; private set; }
		/// <summary>
		/// Whether this is a child context
		/// </summary>
		public virtual bool IsChildContext { get; internal set; }

		/// <summary>
		/// Whether this is a child collection context.
		/// </summary>
		public virtual bool IsChildCollectionContext { get; internal set; }

		/// <summary>
		/// Creates a new ValidationContext based on this one
		/// </summary>
		/// <param name="chain"></param>
		/// <param name="instanceToValidate"></param>
		/// <param name="selector"></param>
		/// <returns></returns>
		public ValidationContext Clone(PropertyChain chain = null, object instanceToValidate = null, IValidatorSelector selector = null) {
			return new ValidationContext(instanceToValidate ?? this.InstanceToValidate, chain ?? this.PropertyChain, selector ?? this.Selector) {
				RootContextData = RootContextData
			};
		}

		/// <summary>
		/// Creates a new validation context for use with a child validator
		/// </summary>
		/// <param name="instanceToValidate"></param>
		/// <returns></returns>
		public ValidationContext CloneForChildValidator(object instanceToValidate) {
			return new ValidationContext(instanceToValidate, PropertyChain, Selector) {
				IsChildContext = true,
				RootContextData = RootContextData
			};
		}

		/// <summary>
		/// Creates a new validation context for use with a child collection validator
		/// </summary>
		/// <param name="instanceToValidate"></param>
		/// <returns></returns>
		public ValidationContext CloneForChildCollectionValidator(object instanceToValidate) {
			return new ValidationContext(instanceToValidate, null, Selector) {
				IsChildContext = true,
				IsChildCollectionContext = true,
				RootContextData = RootContextData
			};
		}

	}
}