namespace FluentValidation.Tests {
	using System.Globalization;
	using Resources;
	using Validators;
	using Xunit;

	public class LanguageManagerTests {
		private LanguageManager _lanaguages;

		public LanguageManagerTests() {
			_lanaguages = new LanguageManager();
		}

		[Fact]
		public void Gets_translation_for_culture() {
			using (new CultureScope("fr")) {
				var msg = _lanaguages.GetStringForValidator<NotNullValidator>();
				msg.ShouldEqual("'{PropertyName}' ne doit pas avoir la valeur null.");
			}
		}

		[Fact]
		public void Gets_translation_for_specific_culture() {
			using (new CultureScope("zh-CN")) {
				var msg = _lanaguages.GetStringForValidator<NotNullValidator>();
				msg.ShouldEqual("请填写 '{PropertyName}'。");
			}
		}

		[Fact]
		public void Falls_back_to_parent_culture() {
			using (new CultureScope("fr-FR")) {
				var msg = _lanaguages.GetStringForValidator<NotNullValidator>();
				msg.ShouldEqual("'{PropertyName}' ne doit pas avoir la valeur null.");
			}
		}

		[Fact]
		public void Falls_back_to_english_when_culture_not_registered() {
			using (new CultureScope("gu-IN")) {
				var msg = _lanaguages.GetStringForValidator<NotNullValidator>();
				msg.ShouldEqual("'{PropertyName}' must not be empty.");
			}
		}

		[Fact]
		public void Falls_back_to_english_when_translation_missing() {
			// Chinese doesn't have enumvalidator translated
			using (new CultureScope("zh-CN")) {
				var msg = _lanaguages.GetStringForValidator<EnumValidator>();
				msg.ShouldEqual("'{PropertyName}' has a range of values which does not include '{PropertyValue}'.");
			}
		}

		[Fact]
		public void Uses_english_when_all_translations_removed() {
			using (new CultureScope("fr")) {
				_lanaguages.Clear();
				var msg = _lanaguages.GetStringForValidator<NotNullValidator>();
				msg.ShouldEqual("'{PropertyName}' must not be empty.");
			}
		}

		[Fact]
		public void Override_default_language() {
			using (new CultureScope("fr")) {
				_lanaguages.SetFallbackLanguage(new TestLanguage());
				var msg = _lanaguages.GetStringForValidator<NotTranslateValidator>();
				msg.ShouldEqual("foo");
			}

		}

		[Fact]
		public void Disables_localization() {
			using (new CultureScope("fr")) {
				_lanaguages.Enabled = false;
				var msg = _lanaguages.GetStringForValidator<NotNullValidator>();
				msg.ShouldEqual("'{PropertyName}' must not be empty.");
			}
		}

		private class NotTranslateValidator : PropertyValidator {
			protected override bool IsValid(PropertyValidatorContext context) {
				return true;
			}
		}

		private class TestLanguage : Language {
			public override string Name => "test";

			public TestLanguage() {
				Translate<NotTranslateValidator>("foo");
			}
		}
	}
}