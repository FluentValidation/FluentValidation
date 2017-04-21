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
	using System.Globalization;

	/// <summary>
	/// Allows the default error message translations to be managed. 
	/// </summary>
	public interface ILanguageManager {
		/// <summary>
		/// Whether localization is enabled.
		/// </summary>
		bool Enabled { get; set; }

		/// <summary>
		/// Default culture to use for all requests to the LanguageManager. If not specified, uses the current UI culture. 
		/// </summary>
		CultureInfo Culture { get; set; }

		/// <summary>
		/// Gets a translated string based on its key. If the culture is specific and it isn't registered, we try the neutral culture instead.
		/// If no matching culture is found  to be registered we use English.
		/// </summary>
		/// <param name="key">The key</param>
		/// <param name="culture">The culture to translate into</param>
		/// <returns></returns>
		string GetString(string key, CultureInfo culture = null);
	}
}