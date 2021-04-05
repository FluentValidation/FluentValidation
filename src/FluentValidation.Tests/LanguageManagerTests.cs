namespace FluentValidation.Tests {
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Reflection;
	using Internal;
	using Resources;
	using Validators;
	using Xunit;

	public class LanguageManagerTests {
		private ILanguageManager _languages;

		public LanguageManagerTests() {
			_languages = new LanguageManager();
		}

		[Theory]
		[InlineData("bs")]
		[InlineData("bs-Latn")]
		[InlineData("bs-Latn-BA")]
		public void Gets_translation_for_bosnian_latin_culture(string cultureName) {
			using (new CultureScope(cultureName)) {
				var msg = _languages.GetString("NotNullValidator");
				msg.ShouldEqual("'{PropertyName}' ne smije biti prazan.");
			}
		}

		[Theory]
		[InlineData("sr")]
		[InlineData("sr-Latn")]
		[InlineData("sr-Latn-RS")]
		public void Gets_translation_for_serbian_culture(string cultureName) {
			using (new CultureScope(cultureName)) {
				var msg = _languages.GetString("NotNullValidator");
				msg.ShouldEqual("'{PropertyName}' ne sme biti prazan.");
			}
		}

		[Fact]
		public void Gets_translation_for_culture() {
			using (new CultureScope("fr")) {
				var msg = _languages.GetString("NotNullValidator");
				msg.ShouldEqual("'{PropertyName}' ne doit pas avoir la valeur null.");
			}
		}

		[Fact]
		public void Gets_translation_for_specific_culture() {
			using (new CultureScope("zh-CN")) {
				var msg = _languages.GetString("NotNullValidator");
				msg.ShouldEqual("'{PropertyName}' 不能为Null。");
			}
		}

		[Fact]
		public void Gets_translation_for_croatian_culture() {
			using (new CultureScope("hr-HR")) {
				var msg = _languages.GetString("NotNullValidator");
				msg.ShouldEqual("Niste upisali '{PropertyName}'");
			}
		}

		[Fact]
		public void Falls_back_to_parent_culture() {
			using (new CultureScope("fr-FR")) {
				var msg = _languages.GetString("NotNullValidator");
				msg.ShouldEqual("'{PropertyName}' ne doit pas avoir la valeur null.");
			}
		}

		[Fact]
		public void Falls_back_to_english_when_culture_not_registered() {
			using (new CultureScope("gu-IN")) {
				var msg = _languages.GetString("NotNullValidator");
				msg.ShouldEqual("'{PropertyName}' must not be empty.");
			}
		}

		[Fact]
		public void Falls_back_to_english_when_translation_missing() {
			var l = new LanguageManager();
			l.AddTranslation("en", "TestValidator", "foo");

			using (new CultureScope("zh-CN")) {
				var msg = l.GetString("TestValidator");
				msg.ShouldEqual("foo");
			}
		}

		[Fact]
		public void Always_use_specific_language() {
			_languages.Culture = new CultureInfo("fr-FR");
			var msg = _languages.GetString("NotNullValidator");
			msg.ShouldEqual("'{PropertyName}' ne doit pas avoir la valeur null.");
		}

		[Fact]
		public void Always_use_specific_language_with_string_source() {
			ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("fr-FR");
			var validator = new InlineValidator<Person>();
			validator.RuleFor(x => x.Surname).NotNull();

			var component = (RuleComponent<Person,string>)validator.First().Components.First();
			var msg = component.GetErrorMessage(null, null);
			ValidatorOptions.Global.LanguageManager.Culture = null;

			msg.ShouldEqual("'{PropertyName}' ne doit pas avoir la valeur null.");
		}

		[Fact]
		public void Disables_localization() {
			using (new CultureScope("fr")) {
				_languages.Enabled = false;
				var msg = _languages.GetString("NotNullValidator");
				msg.ShouldEqual("'{PropertyName}' must not be empty.");
			}
		}

		[Fact]
		public void Can_replace_message() {
			using (new CultureScope("fr-FR")) {
				var custom = new CustomLanguageManager();
				var msg = custom.GetString("NotNullValidator");
				msg.ShouldEqual("foo");
			}
		}

		[Fact]
		public void Can_replace_message_without_overriding_all_languages() {
			using (new CultureScope("fr-FR")) {
				var custom = new LanguageManager();
				custom.AddTranslation("fr", "NotNullValidator", "foo");
				var msg = custom.GetString("NotNullValidator");
				msg.ShouldEqual("foo");

				// Using a custom translation should only override the single message.
				// Other messages in the language should be unaffected.
				// Need to do this test as non-english, as english is always loaded.
				msg = custom.GetString("NotEmptyValidator");
				msg.ShouldEqual("'{PropertyName}' ne doit pas être vide.");
			}
		}

		[Fact]
		public void All_localizations_have_same_parameters_as_English() {

			// Remember to update this test if new validators are added.
			string[] keys = {
#pragma warning disable 618
				"EmailValidator",
#pragma warning restore 618
				"GreaterThanOrEqualValidator",
				"GreaterThanValidator",
				"LengthValidator",
				"MinimumLengthValidator",
				"MaximumLengthValidator",
				"LessThanOrEqualValidator",
				"LessThanValidator",
				"NotEmptyValidator",
				"NotEqualValidator",
				"NotNullValidator",
				"PredicateValidator",
				"AsyncPredicateValidator",
				"RegularExpressionValidator",
				"EqualValidator",
				"ExactLengthValidator",
				"InclusiveBetweenValidator",
				"ExclusiveBetweenValidator",
				"CreditCardValidator",
				"ScalePrecisionValidator",
				"EmptyValidator",
				"NullValidator",
				"EnumValidator",
				"Length_Simple",
				"MinimumLength_Simple",
				"MaximumLength_Simple",
				"ExactLength_Simple",
				"InclusiveBetween_Simple",
			};


			var query = from type in typeof(LanguageManager).Assembly.GetTypes()
				where type.Namespace == "FluentValidation.Resources" && !type.IsPublic
				let cultureField = type.GetField("Culture", BindingFlags.Public | BindingFlags.Static)
				where cultureField != null && cultureField.IsLiteral
				select cultureField.GetValue(null);

			var languageNames = query.Cast<string>().ToList();

			Assert.All(languageNames, l => Assert.All(keys, k => CheckParametersMatch(l, k)));

			void CheckParametersMatch(string languageCode, string translationKey) {
				var referenceMessage = _languages.GetString(translationKey, new CultureInfo("en-US"));
				var translatedMessage = _languages.GetString(translationKey, new CultureInfo(languageCode));
				if (referenceMessage == translatedMessage) return;
				var referenceParameters = ExtractTemplateParameters(referenceMessage);
				var translatedParameters = ExtractTemplateParameters(translatedMessage);
				Assert.False(referenceParameters.Count() != translatedParameters.Count() ||
				             referenceParameters.Except(translatedParameters).Any(),
					$"Translation for language {languageCode}, key {translationKey} has parameters {string.Join(",", translatedParameters)}, expected {string.Join(",", referenceParameters)}");
			}

			IEnumerable<string> ExtractTemplateParameters(string message) {
				message = message.Replace("{{", "").Replace("}}", "");
				return message.Split('{').Skip(1).Select(s => s.Split('}').First());
			}
		}

		[Fact]
		public void All_languages_should_be_loaded() {
			var languages = from type in typeof(LanguageManager).Assembly.GetTypes()
				where type.Namespace == "FluentValidation.Resources" && !type.IsPublic
				let cultureField = type.GetField("Culture", BindingFlags.Public | BindingFlags.Static)
				where cultureField != null && cultureField.IsLiteral
				select new { Name = cultureField.GetValue(null) as string, Type = type.Name };

			string englishMessage = _languages.GetString("NotNullValidator", new CultureInfo("en"));

			foreach (var language in languages) {
				// Skip english as we know it's always loaded and will match.
				if (language.Name == "en") {
					continue;
				}

				// Get the message from the language manager from the culture. If it's in English, then it's hit the
				// fallback and means the culture hasn't been loaded.
				string message = _languages.GetString("NotNullValidator", new CultureInfo(language.Name));
				(message != englishMessage).ShouldBeTrue($"Language '{language.Name}' ({language.Type}) is not loaded in the LanguageManager");
			}
		}

		[Fact]
		public void Uses_error_code_as_localization_key() {
			var originalLanguageManager = ValidatorOptions.Global.LanguageManager;
			ValidatorOptions.Global.LanguageManager = new CustomLanguageManager();

			using (new CultureScope("fr-FR")) {
				var validator = new InlineValidator<Person>();
				validator.RuleFor(x => x.Forename).NotNull().WithErrorCode("CustomKey");
				var result = validator.Validate(new Person());

				ValidatorOptions.Global.LanguageManager = originalLanguageManager;

				result.Errors[0].ErrorMessage.ShouldEqual("bar");
			}
		}

		[Fact]
		public void Falls_back_to_default_localization_key_when_error_code_key_not_found() {
			var originalLanguageManager = ValidatorOptions.Global.LanguageManager;
			ValidatorOptions.Global.LanguageManager = new CustomLanguageManager();
			ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("fr-FR");
			var validator = new InlineValidator<Person>();
			validator.RuleFor(x => x.Forename).NotNull().WithErrorCode("DoesNotExist");
			var result = validator.Validate(new Person());

			ValidatorOptions.Global.LanguageManager = originalLanguageManager;

			result.Errors[0].ErrorMessage.ShouldEqual("foo");
		}

		public class CustomLanguageManager : LanguageManager {
			public CustomLanguageManager() {
				AddTranslation("fr", "NotNullValidator", "foo");
				AddTranslation("fr", "CustomKey", "bar");
			}
		}
	}
}
