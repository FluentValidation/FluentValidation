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

namespace FluentValidation.Resources {
	using System;
	using Validators;

	/// <summary>
	/// Represents a static string.
	/// </summary>
	[Obsolete("StaticStringSource is deprecated and will be removed in FluentValidation 10. Use a Func<PropertyValidatorContext, string> instead.")]
	public class StaticStringSource : IStringSource {
		readonly string _message;

		internal string String => _message;

		/// <summary>
		/// Creates a new StringErrorMessageSource using the specified error message as the error template.
		/// </summary>
		/// <param name="message">The error message template.</param>
		public StaticStringSource(string message) {
			_message = message;
		}

		/// <summary>
		/// Construct the error message template
		/// </summary>
		/// <returns>Error message template</returns>
		public string GetString(ICommonContext context) {
			return _message;
		}
	}
}
