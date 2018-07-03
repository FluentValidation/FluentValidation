namespace FluentValidation.Resources {
	using System;
	using System.Globalization;
	using Validators;

	/// <summary>
	/// IStringSource implementation that uses the default language manager.
	/// </summary>
	public class LanguageStringSource : IStringSource {

		private readonly string _key;

		public LanguageStringSource(string key) {
			_key = key;
		}

		public string GetString(IValidationContext context) {
			return ValidatorOptions.LanguageManager.GetString(_key);
		}

		public string ResourceName => _key;
		public Type ResourceType => typeof(LanguageManager);
	}
}