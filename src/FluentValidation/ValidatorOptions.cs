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
// The latest version of this file can be found at https://github.com/jeremyskinner/FluentValidation
#endregion

namespace FluentValidation {
	using System;
	using System.Linq.Expressions;
	using System.Reflection;
	using Internal;
	using Resources;
	using Validators;

	/// <summary>
	/// Validator runtime options
	/// </summary>
	public static class ValidatorOptions {
		/// <summary>
		/// Default cascade mode
		/// </summary>
		public static CascadeMode CascadeMode = CascadeMode.Continue;

		/// <summary>
		/// Default property chain separator
		/// </summary>
		public static string PropertyChainSeparator = ".";

		/// <summary>
		/// Default language manager 
		/// </summary>
		public static ILanguageManager LanguageManager {
			get => _languageManager;
			set => _languageManager = value ?? throw new ArgumentNullException(nameof(value));
		}

		/// <summary>
		/// Customizations of validator selector
		/// </summary>
		public static ValidatorSelectorOptions ValidatorSelectors { get; } = new ValidatorSelectorOptions();

		private static Func<Type, MemberInfo, LambdaExpression, string> _propertyNameResolver = DefaultPropertyNameResolver;
		private static Func<Type, MemberInfo, LambdaExpression, string> _displayNameResolver = DefaultDisplayNameResolver;
		private static ILanguageManager _languageManager = new LanguageManager();
		private static Func<MessageFormatter> _messageFormatterFactory = () => new MessageFormatter();
		private static Func<PropertyValidator, string> _errorCodeResolver = DefaultErrorCodeResolver;

		/// <summary>
		/// Specifies a factory for creating MessageFormatter instances.
		/// </summary>
		public static Func<MessageFormatter> MessageFormatterFactory {
			get => _messageFormatterFactory;
			set => _messageFormatterFactory = value ?? (() => new MessageFormatter());
		}

		/// <summary>
		/// Pluggable logic for resolving property names
		/// </summary>
		public static Func<Type, MemberInfo, LambdaExpression, string> PropertyNameResolver {
			get => _propertyNameResolver;
			set => _propertyNameResolver = value ?? DefaultPropertyNameResolver;
		}

		/// <summary>
		/// Pluggable logic for resolving display names
		/// </summary>
		public static Func<Type, MemberInfo, LambdaExpression, string> DisplayNameResolver
		{
			get => _displayNameResolver;
			set => _displayNameResolver = value ?? DefaultDisplayNameResolver;
		}

		static string DefaultPropertyNameResolver(Type type, MemberInfo memberInfo, LambdaExpression expression) {
			if (expression != null) {
				var chain = PropertyChain.FromExpression(expression);
				if (chain.Count > 0) return chain.ToString();
			}

			return memberInfo?.Name;
		}	
		
		static string DefaultDisplayNameResolver(Type type, MemberInfo memberInfo, LambdaExpression expression) {
			return memberInfo == null ? null : DisplayNameCache.GetCachedDisplayName(memberInfo);
		}




		/// <summary>
		/// Disables the expression accessor cache. Not recommended.
		/// </summary>
		public static bool DisableAccessorCache { get; set; }

		/// <summary>
		/// Pluggable resolver for default error codes
		/// </summary>
		public static Func<PropertyValidator, string> ErrorCodeResolver {
			get => _errorCodeResolver;
			set => _errorCodeResolver = value ?? DefaultErrorCodeResolver;
		}

		static string DefaultErrorCodeResolver(PropertyValidator validator) {
			return validator.GetType().Name;
		}
	}

	/// <summary>
	/// ValidatorSelector options
	/// </summary>
	public class ValidatorSelectorOptions {
		private Func<IValidatorSelector>  _defaultValidatorSelector = () => new DefaultValidatorSelector();
		private Func<string[], IValidatorSelector> _memberNameValidatorSelector = properties => new MemberNameValidatorSelector(properties);
		private Func<string[], IValidatorSelector> _rulesetValidatorSelector = ruleSets => new RulesetValidatorSelector(ruleSets);

		/// <summary>
		/// Factory func for creating the default validator selector
		/// </summary>
		public Func<IValidatorSelector> DefaultValidatorSelectorFactory {
			get => _defaultValidatorSelector;
			set => _defaultValidatorSelector = value ?? (() => new DefaultValidatorSelector());
		}

		/// <summary>
		/// Factory func for creating the member validator selector
		/// </summary>
		public Func<string[], IValidatorSelector> MemberNameValidatorSelectorFactory {
			get => _memberNameValidatorSelector;
			set => _memberNameValidatorSelector = value ?? (properties => new MemberNameValidatorSelector(properties));
		}

		/// <summary>
		/// Factory func for creating the ruleset validator selector
		/// </summary>
		public Func<string[], IValidatorSelector> RulesetValidatorSelectorFactory {
			get => _rulesetValidatorSelector;
			set => _rulesetValidatorSelector = value ?? (ruleSets => new RulesetValidatorSelector(ruleSets));
		}
	}
}
