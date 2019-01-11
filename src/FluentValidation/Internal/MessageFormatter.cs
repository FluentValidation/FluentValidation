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
namespace FluentValidation.Internal {
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;

	/// <summary>
	/// Assists in the construction of validation messages.
	/// </summary>
	public class MessageFormatter {
		readonly Dictionary<string, object> _placeholderValues = new Dictionary<string, object>(2);
		object[] _additionalArguments = new object[0];
		private bool _shouldUseAdditionalArgs;

		private static readonly Regex _templateRegex = new Regex("{[^{}]+:.+}", RegexOptions.Compiled); 
		private static readonly Regex _keyRegex = new Regex("{([^{}:]+)(?::([^{}]+))?}", RegexOptions.Compiled); 
		
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

		/// <summary>
		/// Constructs the final message from the specified template. 
		/// </summary>
		/// <param name="messageTemplate">Message template</param>
		/// <returns>The message with placeholders replaced with their appropriate values</returns>
		public virtual string BuildMessage(string messageTemplate) {

			string result = messageTemplate;

			if (_templateRegex.Match(result).Success) {
				result = ReplacePlaceholdersWithValues(result, _placeholderValues);
			}
			else {
				foreach (var pair in _placeholderValues) {
#pragma warning disable 618
					result = ReplacePlaceholderWithValue(result, pair.Key, pair.Value);
#pragma warning restore 618
				}
			}

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

		[Obsolete("Use of ReplacePlaceholderWithValue is deprecated and will be removed from future versions. Use ReplacePlaceholdersWithValues instead.")]
		protected virtual string ReplacePlaceholderWithValue(string template, string key, object value)	{
			string placeholder = GetPlaceholder(key);
			return template.Replace(placeholder, value?.ToString());
		}

		[Obsolete("Use of GetPlaceholder is deprecated and will be removed from future versions.")]
		protected string GetPlaceholder(string key)	{
			// Performance: String concat causes much overhead when not needed. Concatenating constants results in constants being compiled.
			switch (key) {
				case PropertyName:
					return "{" + PropertyName + "}";
				case PropertyValue:
					return "{" + PropertyValue + "}";
				default:
					return "{" + key + "}";
			}
		}

		protected virtual string ReplacePlaceholdersWithValues(string template, IDictionary<string, object> values)	{
			return _keyRegex.Replace(template, m =>	{
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