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

namespace FluentValidation {
	using System.Collections.Generic;
	using Internal;
	using Results;

	public interface IValidator<T> : IValidator, IEnumerable<IValidationRule<T>> {
		ValidationResult Validate(T instance);
		/// <summary>
		/// Validates the specified instance.
		/// </summary>
		/// <param name="instance">The object to validate</param>
		/// <param name="selector">An IValidatorSelector that determines which rules should execute.</param>
		/// <returns>A ValidationResult object containing any validation failures.</returns>
		ValidationResult Validate(T instance, IValidatorSelector selector);
	}

	public interface IValidator {
		ValidationResult Validate(object instance);
		ValidationResult Validate(object instance, IValidatorSelector selector);

		/// <summary>
		/// Creates a hook to access various meta data properties
		/// </summary>
		/// <returns>A IValidatorDescriptor object which contains methods to access metadata</returns>
		IValidatorDescriptor CreateDescriptor();
	}
}