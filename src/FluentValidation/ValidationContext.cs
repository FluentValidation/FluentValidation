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
	/// Validation context
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ValidationContext<T> : ValidationContext, IValidationContext<T> {
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
		/// <param name="isChildContext"></param>
		public ValidationContext(T instanceToValidate, PropertyChain propertyChain, IValidatorSelector validatorSelector, bool isChildContext = false)
			: base(instanceToValidate, propertyChain, validatorSelector) {
			InstanceToValidate = instanceToValidate;
			IsChildContext = isChildContext;
		}

		/// <summary>
		/// The object to validate
		/// </summary>
		public new T InstanceToValidate { get; private set; }
	}

	/// <summary>
	/// Validation context
	/// </summary>
	public class ValidationContext : IValidationContext {

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
		public bool IsChildContext { get; protected internal set; }

		/// <summary>
		/// Creates a new ValidationContext based on this one
		/// </summary>
		/// <param name="chain"></param>
		/// <param name="instanceToValidate"></param>
		/// <param name="selector"></param>
		/// <param name="isChildContext"></param>
		/// <returns></returns>
		public virtual IValidationContext<TType> Clone<TType>(PropertyChain chain = null, TType instanceToValidate = default(TType), IValidatorSelector selector = null, bool? isChildContext = null)
		{
			return ValidatorOptions.ValidationContextFactory.Get(Equals(instanceToValidate, default(TType)) ? (TType)InstanceToValidate : instanceToValidate, chain ?? PropertyChain, selector ?? Selector, isChildContext ?? IsChildContext);
		}

		/// <summary>
		/// Creates a new ValidationContext based on this one
		/// </summary>
		/// <param name="chain"></param>
		/// <param name="instanceToValidate"></param>
		/// <param name="selector"></param>
		/// <returns></returns>
		public virtual IValidationContext Clone(PropertyChain chain = null, object instanceToValidate = null, IValidatorSelector selector = null) {
			return ValidatorOptions.ValidationContextFactory.Get(instanceToValidate ?? InstanceToValidate, chain ?? PropertyChain, selector ?? Selector);
		}

		/// <summary>
		/// Creates a new validation context for use with a child validator
		/// </summary>
		/// <param name="instanceToValidate"></param>
		/// <returns></returns>
		public virtual IValidationContext CloneForChildValidator(object instanceToValidate) {
			return ValidatorOptions.ValidationContextFactory.Get(instanceToValidate, PropertyChain, Selector, true);
		}
	}
}