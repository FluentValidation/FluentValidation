#region License
// Copyright (c) .NET Foundation and contributors.
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
// The latest version of this file can be found at https://github.com/FluentValidation/FluentValidation
#endregion

namespace FluentValidation.Validators {
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;
	using Resources;
	using Results;

	public interface IAsyncPropertyValidator<T, in TProperty> : IPropertyValidator {
		/// <summary>
		/// Validates a specific property value asynchronously.
		/// </summary>
		/// <param name="context">The validation context. The parent object can be obtained from here.</param>
		/// <param name="value">The current property value to validate</param>
		/// <param name="cancellation">Cancellation token</param>
		/// <returns>True if valid, otherwise false.</returns>
		Task<bool> IsValidAsync(ValidationContext<T> context, TProperty value, CancellationToken cancellation);
	}

	public interface IPropertyValidator<T, in TProperty> : IPropertyValidator {
		/// <summary>
		/// Validates a specific property value.
		/// </summary>
		/// <param name="context">The validation context. The parent object can be obtained from here.</param>
		/// <param name="value">The current property value to validate</param>
		/// <returns>True if valid, otherwise false.</returns>
		bool IsValid(ValidationContext<T> context, TProperty value);
	}

	/// <summary>
	/// A custom property validator.
	/// This interface should not be implemented directly in your code as it is subject to change.
	/// Please inherit from <see cref="PropertyValidator{T,TProperty}">PropertyValidator</see> instead.
	/// </summary>
	public interface IPropertyValidator {
		/// <summary>
		/// The name of the validator. This is usually the type name without any generic parameters.
		/// This is used as the default Error Code for the validator.
		/// </summary>
		string Name { get; }


		/// <summary>
		/// Returns the default error message template for this validator, when not overridden.
		/// </summary>
		/// <param name="errorCode"></param>
		/// <returns></returns>
		string GetDefaultMessageTemplate(string errorCode);
	}

}
