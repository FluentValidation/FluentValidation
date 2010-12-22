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
// The latest version of this file can be found at http://www.codeplex.com/FluentValidation
#endregion

namespace FluentValidation {
	using System.Collections.Generic;
	using Internal;
	using Results;
	using Validators;

	public interface IValidationRule {
		IEnumerable<IPropertyValidator> Validators { get; }

		/// <summary>
		/// Performs validation using a validation context and returns a collection of Validation Failures.
		/// </summary>
		/// <param name="instance">The instance to validate</param>
		/// <param name="context">Validation Context</param>
		/// <returns>A collection of validation failures</returns>
		IEnumerable<ValidationFailure> Validate(ValidationContext context);
	}

	//TODO: Deprecate the generic version of this interface for FluentValidation v3.

	/// <summary>
	/// Validation Rule
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IValidationRule<T> : IValidationRule {
		/// <summary>
		/// Performs validation using a validation context and returns a collection of Validation Failures.
		/// </summary>
		/// <param name="instance">The instance to validate</param>
		/// <param name="context">Validation Context</param>
		/// <returns>A collection of validation failures</returns>
		IEnumerable<ValidationFailure> Validate(ValidationContext<T> context);
	}
}