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
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Globalization;
	using Validators;

	/// <summary>
	/// Allows the default error message translations to be managed.
	/// </summary>
	public class LanguageManager : ILanguageManager {
		private readonly ConcurrentDictionary<string, Language> _languages;
		private readonly Language _fallback = new EnglishLanguage();

		/// <summary>
		/// Creates a new instance of the LanguageManager class.
		/// </summary>
		public LanguageManager() {
			// Initialize with English as the default. Others will be lazily loaded as needed.
			_languages = new ConcurrentDictionary<string, Language>(new[] {
				new KeyValuePair<string, Language>(EnglishLanguage.Culture, _fallback),
			});
		}

		/// <summary>
		/// Language factory.
		/// </summary>
		/// <param name="culture">The culture code.</param>
		/// <returns>The corresponding Language instance or null.</returns>
		private static Language CreateLanguage(string culture) {
			return culture switch {
				EnglishLanguage.Culture => new EnglishLanguage(),
				AlbanianLanguage.Culture => new AlbanianLanguage(),
				ArabicLanguage.Culture => new ArabicLanguage(),
				ChineseSimplifiedLanguage.Culture => new ChineseSimplifiedLanguage(),
				ChineseTraditionalLanguage.Culture => new ChineseTraditionalLanguage(),
				CroatianLanguage.Culture => new CroatianLanguage(),
				CzechLanguage.Culture => new CzechLanguage(),
				DanishLanguage.Culture => new DanishLanguage(),
				DutchLanguage.Culture => new DutchLanguage(),
				FinnishLanguage.Culture => new FinnishLanguage(),
				FrenchLanguage.Culture => new FrenchLanguage(),
				GermanLanguage.Culture => new GermanLanguage(),
				GeorgianLanguage.Culture => new GeorgianLanguage(),
				GreekLanguage.Culture => new GreekLanguage(),
				HebrewLanguage.Culture => new HebrewLanguage(),
				HindiLanguage.Culture => new HindiLanguage(),
				HungarianLanguage.Culture => new HungarianLanguage(),
				ItalianLanguage.Culture => new ItalianLanguage(),
				JapaneseLanguage.Culture => new JapaneseLanguage(),
				KoreanLanguage.Culture => new KoreanLanguage(),
				MacedonianLanguage.Culture => new MacedonianLanguage(),
				NorwegianBokmalLanguage.Culture => new NorwegianBokmalLanguage(),
				PersianLanguage.Culture => new PersianLanguage(),
				PolishLanguage.Culture => new PolishLanguage(),
				PortugueseLanguage.Culture => new PortugueseLanguage(),
				PortugueseBrazilLanguage.Culture => new PortugueseBrazilLanguage(),
				RomanianLanguage.Culture => new RomanianLanguage(),
				RussianLanguage.Culture => new RussianLanguage(),
				SlovakLanguage.Culture => new SlovakLanguage(),
				SpanishLanguage.Culture => new SpanishLanguage(),
				SwedishLanguage.Culture => new SwedishLanguage(),
				TurkishLanguage.Culture => new TurkishLanguage(),
				UkrainianLanguage.Culture => new UkrainianLanguage(),
				WelshLanguage.Culture => new WelshLanguage(),
				_=> (Language)null,
			};
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
			string value;

			if (Enabled) {
				culture = culture ?? Culture ?? CultureInfo.CurrentUICulture;
				var languageToUse = GetCachedLanguage(culture) ?? _fallback;
				value = languageToUse.GetTranslation(key);

				// Selected language is missing a translation for this key - fall back to English translation
				// if we're not using english already.
				if (string.IsNullOrEmpty(value) && languageToUse != _fallback) {
					value = _fallback.GetTranslation(key);
				}
			}
			else {
				value = _fallback.GetTranslation(key);
			}

			return value ?? string.Empty;
		}

		private Language GetCachedLanguage(CultureInfo culture) {
			// Find matching translations.
			var languageToUse = _languages.GetOrAdd(culture.Name, CreateLanguage);

			// If we couldn't find translations for this culture, and it's not a neutral culture
			// then try and find translations for the parent culture instead.
			if (languageToUse == null && !culture.IsNeutralCulture) {
				languageToUse = _languages.GetOrAdd(culture.Parent.Name, CreateLanguage);
			}

			return languageToUse;
		}

		public void AddTranslation(string language, string key, string message) {
			if (string.IsNullOrEmpty(language)) throw new ArgumentNullException(nameof(language));
			if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
			if (string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message));

			var languageInstance = _languages.GetOrAdd(language, c => CreateLanguage(c) ?? new GenericLanguage(c));
			languageInstance.Translate(key, message);
		}
	}
}
