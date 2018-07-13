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
	using System;
	using Resources;
	using Validators;

	/// <summary>
	/// Validator metadata.
	/// </summary>
	public class PropertyValidatorOptions {
		private IStringSource _errorSource;
		private IStringSource _errorCodeSource;
		
		/// <summary>
		/// Function used to retrieve custom state for the validator
		/// </summary>
		public Func<PropertyValidatorContext, object> CustomStateProvider { get; set; }

		/// <summary>
		/// Severity of error.
		/// </summary>
		public Severity Severity { get; set; }
		
		/// <summary>
		/// Retrieves the unformatted error message template.
		/// </summary>
		public IStringSource ErrorMessageSource {
			get => _errorSource;
			set => _errorSource = value ?? throw new ArgumentNullException(nameof(value));
		}
		
		/// <summary>
		/// Retrieves the error code.
		/// </summary>
		public IStringSource ErrorCodeSource {
			get => _errorCodeSource;
			set => _errorCodeSource = value ?? throw new ArgumentNullException(nameof(value));
		}

		/// <summary>
		/// Empty metadata.
		/// </summary>
		public static PropertyValidatorOptions Empty { get; } = new PropertyValidatorOptions {
			_errorSource = new StaticStringSource(string.Empty),
		};
	}
}