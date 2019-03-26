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
	using System.Globalization;
	using Validators;

	/// <summary>
	/// IStringSource implementation that uses the default language manager.
	/// </summary>
	public class LanguageStringSource : IStringSource {
		private readonly string _key;
		internal Func<IValidationContext, string> ErrorCodeFunc { get; set; }

		public LanguageStringSource(string key) {
			_key = key;
		}
		
		public LanguageStringSource(Func<IValidationContext, string> errorCodeFunc, string fallbackKey) {
			ErrorCodeFunc = errorCodeFunc;
			_key = fallbackKey;
		}

		public virtual string GetString(IValidationContext context) {
			var errorCode = ErrorCodeFunc?.Invoke(context);
			
			if (errorCode != null) {
				string result = ValidatorOptions.LanguageManager.GetString(errorCode);
				
				if (!string.IsNullOrEmpty(result)) {
					return result;
				}
			}

			return ValidatorOptions.LanguageManager.GetString(_key);
		}
	}
}