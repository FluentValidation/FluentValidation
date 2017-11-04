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
	using System.Collections.Generic;

	/// <summary>
	/// Base class for lanaguages
	/// </summary>
	internal abstract class Language {

		/// <summary>
		/// Name of language (culture code)
		/// </summary>
		public abstract string Name { get; }

		private readonly Dictionary<string, string> _translations = new Dictionary<string, string>();

		/// <summary>
		/// Adds a translation
		/// </summary>
		/// <param name="key"></param>
		/// <param name="message"></param>
		public virtual void Translate(string key, string message) {
			_translations[key] = message;
		}
		
		/// <summary>
		/// Adds a translation for a type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="message"></param>
		public void Translate<T>(string message) {
			Translate(typeof(T).Name, message);
		}

		/// <summary>
		/// Gets the localized version of a string with a specific key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public virtual string GetTranslation(string key) {
			string value;

			if (_translations.TryGetValue(key, out value)) {
				return value;
			}

			return null;
		}

		internal IEnumerable<string> GetSupportedKeys() {
			return _translations.Keys;
		}
	}

	internal class GenericLanguage : Language {
		public GenericLanguage(string name) {
			Name = name;
		}

		public override string Name { get; }
	}
}