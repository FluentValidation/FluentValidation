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
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;

	/// <summary>
	/// Allows the default error message translations to be managed.
	/// </summary>
	public class LanguageManager : ILanguageManager {
		private readonly ConcurrentDictionary<string, string> _languages;

		/// <summary>
		/// Creates a new instance of the LanguageManager class.
		/// </summary>
		public LanguageManager() {
			// Initialize with English as the default. Others will be lazily loaded as needed.
			_languages = new ConcurrentDictionary<string, string>();
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
				BengaliLanguage.Culture => new BengaliLanguage(),
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
				IcelandicLanguage.Culture => new IcelandicLanguage(),
				ItalianLanguage.Culture => new ItalianLanguage(),
				IndonesianLanguage.Culture => new IndonesianLanguage(),
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
				SlovenianLanguage.Culture => new SlovenianLanguage(),
				SpanishLanguage.Culture => new SpanishLanguage(),
				SwedishLanguage.Culture => new SwedishLanguage(),
				TurkishLanguage.Culture => new TurkishLanguage(),
				UkrainianLanguage.Culture => new UkrainianLanguage(),
				WelshLanguage.Culture => new WelshLanguage(),
				_ => (Language) null,
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
		public virtual string GetString(string key, CultureInfo culture = null) {
			string value;

			if (Enabled) {
				culture = culture ?? Culture ?? CultureInfo.CurrentUICulture;

				string currentCultureKey = culture.Name + ":" + key;

				value = _languages.GetOrAdd(currentCultureKey, k => {
					// TODO: Move CreateLanguage().GetTranslation() to static methods.
					string result = CreateLanguage(culture.Name)?.GetTranslation(key);

					if (result == null && !culture.IsNeutralCulture) {
						string parentCultureKey = culture.Parent.Name + ":" + key;
						result = _languages.GetOrAdd(parentCultureKey, k2 => {
							string result2 = CreateLanguage(culture.Parent.Name)?.GetTranslation(key);
							return result2;
						});
					}

					return result;
				});

				if (value == null && culture.Name != "en") {
					// If it couldn't be found, try the fallback English (if we haven't tried it already).
					if (!culture.IsNeutralCulture && culture.Parent.Name != "en") {
						value = _languages.GetOrAdd("en:" + key, k => {
							return new EnglishLanguage().GetTranslation(key);
						});
					}
				}
			}
			else {
				value = _languages.GetOrAdd("en:" + key, k => {
					return new EnglishLanguage().GetTranslation(key);
				});
			}

			return value ?? string.Empty;
		}

		public void AddTranslation(string language, string key, string message) {
			if (string.IsNullOrEmpty(language)) throw new ArgumentNullException(nameof(language));
			if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
			if (string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message));

			_languages[language + ":" + key] = message;
		}
	}
}
