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
				new CzechLanguage(),
				new DanishLanguage(),
				new DutchLanguage(),
				new FinnishLanguage(),
				new FrenchLanguage(),
				new GermanLanguage(),
				new GeorgianLanguage(),
				new ItalianLanguage(),
				new KoreanLanguage(),
				new MacedonianLanguage(),
				new PersianLanguage(),
				new PolishLanguage(),
				new PortugueseLanguage(),
				new RomanianLanguage(),
				new RussianLanguage(),
				new SpanishLanguage(),
				new SwedishLanguage(),
				new TurkishLanguage(),
				new HindiLanguage(),
				new CroatianLanguage()
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
#pragma warning disable 618
			if (ValidatorOptions.ResourceProviderType != null) {
				try {
					var localizedStringSource = new LocalizedStringSource(ValidatorOptions.ResourceProviderType, BackwardsCompatibilityCodeMapping(key));
					return localizedStringSource.GetString(null);
				}
				catch(InvalidOperationException) {  } // If something went wrong with the backwards compat override, just allow it to carry on to the normal behaviour.
			}
#pragma warning restore 618

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

		// Prior to 7.0 the error message resource names were string values such as "notnull_error" rather than the type name (NotNullValidator).
		// For internal usage, the change is fine but it's a breaking change who relied on the keys being set in the ErrorCode property on the validation failure
		// This mapping ensures that the original resource names are used for generating error codes.
		internal static string BackwardsCompatibilityCodeMapping(string name) {
			switch (name) {
				case nameof(EnumValidator): return "enum_error";
				case nameof(NullValidator): return "null_error";
				case nameof(EmptyValidator): return "empty_error";
				case nameof(ScalePrecisionValidator): return "scale_precision_error";
				case nameof(CreditCardValidator): return "CreditCardError";
				case nameof(ExclusiveBetweenValidator): return "exclusivebetween_error";
				case nameof(InclusiveBetweenValidator): return "inclusivebetween_error";
				case nameof(ExactLengthValidator): return "exact_length_error";
				case nameof(EqualValidator): return "equal_error";
				case nameof(RegularExpressionValidator): return "regex_error";
				case nameof(PredicateValidator): return "predicate_error";
				case nameof(AsyncPredicateValidator): return "predicate_error";
				case nameof(NotNullValidator): return "notnull_error";
				case nameof(NotEqualValidator): return "notequal_error";
				case nameof(NotEmptyValidator): return "notempty_error";
				case nameof(LessThanValidator): return "lessthan_error";
				case nameof(LessThanOrEqualValidator): return "lessthanorequal_error";
				case nameof(LengthValidator): return "length_error";
				case nameof(MinimumLengthValidator): return "length_error";
				case nameof(MaximumLengthValidator): return "length_error";
				case nameof(GreaterThanValidator): return "greaterthan_error";
				case nameof(GreaterThanOrEqualValidator): return "greaterthanorequal_error";
				case nameof(EmailValidator): return "email_error";
			}

			return name;
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