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

namespace FluentValidation.Resources {
	using System;

	/// <summary>
	/// Implementation of IErrorMessageSource that uses a constant string.
	/// </summary>
	public class StringErrorMessageSource : IErrorMessageSource {
		readonly string errorMessage;

		/// <summary>
		/// Creates a new StringErrorMessageSource using the specified error message as the error template.
		/// </summary>
		/// <param name="errorMessage">The error message template.</param>
		public StringErrorMessageSource(string errorMessage) {
			this.errorMessage = errorMessage;
		}

		/// <summary>
		/// Construct the error message template
		/// </summary>
		/// <returns>Error message template</returns>
		public string BuildErrorMessage() {
			return errorMessage;
		}

		/// <summary>
		/// The name of the resource if localized.
		/// </summary>
		public string ResourceName {
			get { return null; }
		}

		/// <summary>
		/// The type of the resource provider if localized.
		/// </summary>
		public Type ResourceType {
			get { return null; }
		}
	}
}