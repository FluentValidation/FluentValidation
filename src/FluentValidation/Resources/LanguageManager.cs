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
	using System.Collections.Generic;
	using System.Globalization;
	using Validators;

	/// <summary>
	/// Allows the default error message translations to be managed.
	/// </summary>
	public class LanguageManager : ILanguageManager {
		private readonly Dictionary<string, Language> _languages = new Dictionary<string, Language>();
		private Language _fallback;

		/// <summary>
		/// Creates a new instance of the LanguageManager class.
		/// </summary>
		public LanguageManager() {
			var languages = new Language[] {
				new EnglishLanguage(),
				new ChineseSimplifiedLanguage(),
				new ChineseTraditionalLanguage(),
				new CroatianLanguage(),
				new CzechLanguage(),
				new DanishLanguage(),
				new DutchLanguage(),
				new FinnishLanguage(),
				new FrenchLanguage(),
				new GermanLanguage(),
				new GeorgianLanguage(),
				new HebrewLanguage(),
				new HindiLanguage(),
				new ItalianLanguage(),
				new KoreanLanguage(),
				new MacedonianLanguage(),
				new PersianLanguage(),
				new PolishLanguage(),
				new PortugueseLanguage(),
				new PortugueseBrazilLanguage(),
				new RomanianLanguage(),
				new RussianLanguage(),
				new SlovakLanguage(),
				new SpanishLanguage(),
				new SwedishLanguage(),
				new TurkishLanguage(),
				new UkrainianLanguage(),
				new ArabicLanguage(),
				new AlbanianLanguage(),
				new GreekLanguage(),
			};

			foreach (var language in languages) {
				_languages[language.Name] = language;
			}

			_fallback = _languages["en"];
		}

		/// <summary>
		/// Whether localization is enabled.
		/// </summary>
		public bool Enabled { get; set; } = true;

		/// <summary>
		/// Default culture to use for all requests to the LanguageManager. If not specified, uses the current UI culture.
		/// </summary>
		public CultureInfo Culture { get; set; }

		/// <summary>
		/// Provides a collection of all supported languages.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetSupportedLanguages() {
			return _languages.Keys;
		}


		/// <summary>
		/// Removes all languages except the default.
		/// </summary>
		public void Clear() {
			_languages.Clear();
		}

		/// <summary>
		/// Gets a translated string based on its key. If the culture is specific and it isn't registered, we try the neutral culture instead.
		/// If no matching culture is found  to be registered we use English.
		/// </summary>
		/// <param name="key">The key</param>
		/// <param name="culture">The culture to translate into</param>
		/// <returns></returns>
		public virtual string GetString(string key, CultureInfo culture=null) {
			// For backwards compatibility with < 7.0 ResourceProvider
			culture = culture ?? Culture ?? CultureInfo.CurrentUICulture;

			string code = culture.Name;

			if (!culture.IsNeutralCulture && !_languages.ContainsKey(code)) {
				code = culture.Parent.Name;
			}

			var languageToUse = Enabled && _languages.ContainsKey(code)
				? _languages[code]
				: _fallback;

			string value = languageToUse.GetTranslation(key);

			if (string.IsNullOrEmpty(value) && languageToUse != _fallback) {
				value = _fallback.GetTranslation(key);
			}

			return value ?? string.Empty;
		}

		public IEnumerable<string> GetSupportedTranslationKeys() {
			return _fallback.GetSupportedKeys();
		}

		public void AddTranslation(string language, string key, string message) {
			if(string.IsNullOrEmpty(language)) throw new ArgumentNullException(nameof(language));
			if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
			if (string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message));

			if (!_languages.ContainsKey(language)) {
				_languages[language] = new GenericLanguage(language);
			}

			_languages[language].Translate(key, message);

		}
	}
}
