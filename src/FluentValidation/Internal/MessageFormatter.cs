namespace FluentValidation.Internal {
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;

	/// <summary>
	/// Assists in the construction of validation messages.
	/// </summary>
	public class MessageFormatter {
		readonly Dictionary<string, object> _placeholderValues = new Dictionary<string, object>(2);
		object[] _additionalArguments = new object[0];
		private bool _shouldUseAdditionalArgs;

		/// <summary>
		/// Default Property Name placeholder.
		/// </summary>
		public const string PropertyName = "PropertyName";

		/// <summary>
		/// Default Property Value placeholder.
		/// </summary>
		public const string PropertyValue = "PropertyValue";

		public MessageFormatter() {
			
		}

		/// <summary>
		/// Adds a value for a validation message placeholder.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public MessageFormatter AppendArgument(string name, object value) {
			_placeholderValues[name] = value;
			return this;
		}

		/// <summary>
		/// Appends a property name to the message.
		/// </summary>
		/// <param name="name">The name of the property</param>
		/// <returns></returns>
		public MessageFormatter AppendPropertyName(string name) {
			return AppendArgument(PropertyName, name);
		}

		/// <summary>
		/// Appends a property value to the message.
		/// </summary>
		/// <param name="value">The value of the property</param>
		/// <returns></returns>
		public MessageFormatter AppendPropertyValue(object value) {
			return AppendArgument(PropertyValue, value);
		}

		/// <summary>
		/// Adds additional arguments to the message for use with standard string placeholders.
		/// </summary>
		/// <param name="additionalArgs">Additional arguments</param>
		/// <returns></returns>
		public MessageFormatter AppendAdditionalArguments(params object[] additionalArgs) {
			_additionalArguments = additionalArgs;
			_shouldUseAdditionalArgs = _additionalArguments != null && _additionalArguments.Length > 0;
			return this;
		}

		
#if NETSTANDARD1_1 || NETSTANDARD1_6
		private static Regex _templateRegex = new Regex("{[^{}]+:.+}");
#else
		private static Regex _templateRegex = new Regex("{[^{}]+:.+}"/*, RegexOptions.Compiled*/); 
#endif
		/// <summary>
		/// Constructs the final message from the specified template. 
		/// </summary>
		/// <param name="messageTemplate">Message template</param>
		/// <returns>The message with placeholders replaced with their appropriate values</returns>
		public virtual string BuildMessage(string messageTemplate) {

			string result = messageTemplate;

			if (_templateRegex.Match(result).Success)
				//_placeholderValues.Keys.Any(x => x?.ToString().Contains(":") ?? false))
				result = ReplacePlaceholdersWithValues(result, _placeholderValues);
			else
				foreach (var pair in _placeholderValues)
					result = ReplacePlaceholderWithValue(result, pair.Key, pair.Value);
				

			if (_shouldUseAdditionalArgs) {
				return string.Format(result, _additionalArguments);
			}
			return result;
		}

		/// <summary>
		/// Additional arguments to use
		/// </summary>
		public object[] AdditionalArguments => _additionalArguments;

		/// <summary>
		/// Additional placeholder values
		/// </summary>
		public Dictionary<string, object> PlaceholderValues => _placeholderValues;

		protected virtual string ReplacePlaceholderWithValue(string template, string key, object value)	{
			string placeholder = GetPlaceholder(key);
			return template.Replace(placeholder, value?.ToString());
		}

		protected string GetPlaceholder(string key)	{
			// Performance: String concat causes much overhead when not needed. Concatting constants results in constants being compiled.
			switch (key)
			{
				case PropertyName:
					return "{" + PropertyName + "}";
				case PropertyValue:
					return "{" + PropertyValue + "}";
				default:
					return "{" + key + "}";
			}
		}

#if NETSTANDARD1_1 || NETSTANDARD1_6
		private static Regex _keyRegex = new Regex("{([^{}:]+)(?::([^{}]+))?}");
#else
		private static Regex _keyRegex = new Regex("{([^{}:]+)(?::([^{}]+))?}", RegexOptions.Compiled); 
#endif

		protected virtual string ReplacePlaceholdersWithValues(string template, IDictionary<string, object> values)	{
			return _keyRegex.Replace(template, m =>
			{
				var key = m.Groups[1].Value;

				if (!values.ContainsKey(key))
					return m.Value; // No placeholder / value

				var format = m.Groups[2].Success // Format specified?
					? $"{{0:{m.Groups[2].Value}}}"
					: null;

				return format == null
					? values[key]?.ToString()
					: string.Format(format, values[key]);
			});
		}
	}
}