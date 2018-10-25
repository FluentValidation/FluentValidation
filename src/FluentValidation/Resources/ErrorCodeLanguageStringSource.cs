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

	/// <summary>
	/// Uses error code as the lookup for language manager, falling back to the default LanguageStringSource if not found.
	/// Internal as the api may change.
	/// </summary>
	internal class ErrorCodeLanguageStringSource : IStringSource {
		private readonly Func<IValidationContext, string> _errorCodeFunc;
		private readonly LanguageStringSource _inner;

		public ErrorCodeLanguageStringSource(Func<IValidationContext, string> errorCodeFunc, LanguageStringSource inner) {
			_errorCodeFunc = errorCodeFunc;
			_inner = inner;
		}

		public string GetString(IValidationContext context) {
			string result = null;
			var errorCode = _errorCodeFunc(context);
			if (errorCode != null) {
				result = ValidatorOptions.LanguageManager.GetString(errorCode);
			}
			
			return string.IsNullOrEmpty(result) ? _inner.GetString(context) : result;
		}

		public string ResourceName => _errorCodeFunc(null) ?? _inner.ResourceName; // null is ok here as LanguageStringSource doesn't use context.
		public Type ResourceType => typeof(LanguageManager);
	}
}