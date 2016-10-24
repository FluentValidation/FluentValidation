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
	public interface IValidationContext<out T> : IValidationContext {
		/// <summary>
		/// Object being validated
		/// </summary>
		new T InstanceToValidate { get; }
	}

	/// <summary>
	/// Validation context
	/// </summary>
	public interface IValidationContext {
		/// <summary>
		/// Property chain
		/// </summary>
		PropertyChain PropertyChain { get; }

		/// <summary>
		/// Object being validated
		/// </summary>
		object InstanceToValidate { get; }

		/// <summary>
		/// Selector
		/// </summary>
		IValidatorSelector Selector { get; }

		/// <summary>
		/// Whether this is a child context
		/// </summary>
		bool IsChildContext { get; }

		/// <summary>
		/// Creates a new <see cref="IValidationContext" /> based on this one
		/// </summary>
		/// <param name="chain"></param>
		/// <param name="instanceToValidate"></param>
		/// <param name="selector"></param>
		/// <returns></returns>
		IValidationContext Clone(PropertyChain chain = null, object instanceToValidate = null, IValidatorSelector selector = null);

		/// <summary>
		/// Creates a new <see cref="IValidationContext{T}" /> ValidationContext based on this one
		/// </summary>
		/// <param name="chain"></param>
		/// <param name="instanceToValidate"></param>
		/// <param name="selector"></param>
		/// <param name="isChildContext"></param>
		/// <returns></returns>
		IValidationContext<TType> Clone<TType>(PropertyChain chain = null, TType instanceToValidate = default(TType), IValidatorSelector selector = null, bool? isChildContext = null);

		/// <summary>
		/// Creates a new validation context for use with a child validator
		/// </summary>
		/// <param name="instanceToValidate"></param>
		/// <returns></returns>
		IValidationContext CloneForChildValidator(object instanceToValidate);
	}
}
