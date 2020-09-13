﻿#region License
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
	using System.Globalization;
	using Validators;

	/// <summary>
	/// IStringSource implementation that uses the default language manager.
	/// </summary>
	[Obsolete("LanguageStringSource is deprecated and will be removed in FluentValidation 10. Use a Func<PropertyValidatorContext, string> instead.")]
	public class LanguageStringSource : IStringSource {
		private readonly string _key;
		internal Func<ICommonContext, string> ErrorCodeFunc { get; set; }

		public LanguageStringSource(string key) {
			_key = key;
		}

		public LanguageStringSource(Func<ICommonContext, string> errorCodeFunc, string fallbackKey) {
			ErrorCodeFunc = errorCodeFunc;
			_key = fallbackKey;
		}

		public virtual string GetString(ICommonContext context) {
				var errorCode = ErrorCodeFunc?.Invoke(context);

				if (errorCode != null) {
					string result = ValidatorOptions.Global.LanguageManager.GetString(errorCode);

					if (!string.IsNullOrEmpty(result)) {
						return result;
					}
				}

				return ValidatorOptions.Global.LanguageManager.GetString(_key);
		}
	}
}
