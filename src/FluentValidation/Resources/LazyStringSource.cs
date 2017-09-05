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
// The latest version of this file can be found at https://github.com/JeremySkinner/FluentValidation
#endregion
namespace FluentValidation.Resources {
	using System;
	using Validators;

	/// <summary>
	/// Lazily loads the string
	/// </summary>
	public class LazyStringSource : IStringSource {
		readonly Func<object, string> _stringProvider;

		/// <summary>
		/// Creates a LazyStringSource
		/// </summary>
		public LazyStringSource(Func<object, string> stringProvider) {
			_stringProvider = stringProvider;
		}

		/// <summary>
		/// Gets the value
		/// </summary>
		/// <returns></returns>
		public string GetString(object context) {
			try {
				return _stringProvider(context);
			}
			catch (NullReferenceException ex) {
				throw new FluentValidationMessageFormatException("Could not build error message- the message makes use of properties from the containing object, but the containing object was null.", ex);
			}
		}

		/// <summary>
		/// Resource type
		/// </summary>
		public string ResourceName => null;

		/// <summary>
		/// Resource name
		/// </summary>
		public Type ResourceType => null;
	}

	// Internal for now as I'm not sure I like the duplication. Might be better to have the breaking change and merge this with LazyStringSource.
	internal class ContextAwareLazyStringSource : IStringSource, IContextAwareStringSource {
		readonly Func<PropertyValidatorContext, string> _stringProvider;

		/// <summary>
		/// Creates a LazyStringSource
		/// </summary>
		public ContextAwareLazyStringSource(Func<PropertyValidatorContext, string> stringProvider) {
			_stringProvider = stringProvider;
		}

		/// <summary>
		/// Gets the value
		/// </summary>
		/// <returns></returns>
		public string GetString(object context) {
			try {
				return _stringProvider(context as PropertyValidatorContext);
			} catch (NullReferenceException ex) {
				throw new FluentValidationMessageFormatException("Could not build error message- the message makes use of properties from the containing object, but the containing object was null.", ex);
			}
		}

		/// <summary>
		/// Resource type
		/// </summary>
		public string ResourceName => null;

		/// <summary>
		/// Resource name
		/// </summary>
		public Type ResourceType => null;
	}

	public class FluentValidationMessageFormatException : Exception {
		public FluentValidationMessageFormatException(string message) : base(message) {
		}

		public FluentValidationMessageFormatException(string message, Exception innerException) : base(message, innerException) {
		}
	}
}