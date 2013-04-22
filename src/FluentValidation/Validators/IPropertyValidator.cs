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

namespace FluentValidation.Validators {
	using System;
	using System.Collections.Generic;
	using Resources;
	using Results;

	/// <summary>
	/// A custom property validator.
	/// This interface should not be implemented directly in your code as it is subject to change.
	/// Please inherit from <see cref="PropertyValidator">PropertyValidator</see> instead.
	/// </summary>
	public interface IPropertyValidator {
		IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context);

		/// <summary>
		/// Custom message arguments. 
		/// Arg 1: Instance being validated
		/// Arg 2: Property value
		/// </summary>
		ICollection<Func<object, object, object>> CustomMessageFormatArguments { get; }
		Func<object, object> CustomStateProvider { get; set; }
		IStringSource ErrorMessageSource { get; set; }
	}
}