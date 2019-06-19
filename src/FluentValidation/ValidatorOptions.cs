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
	/// Configuration options for validators.
	/// </summary>
	public class ValidatorConfiguration {
		private Func<Type, MemberInfo, LambdaExpression, string> _propertyNameResolver = DefaultPropertyNameResolver;
		private Func<Type, MemberInfo, LambdaExpression, string> _displayNameResolver = DefaultDisplayNameResolver;
		private Func<MessageFormatter> _messageFormatterFactory = () => new MessageFormatter();
		private Func<PropertyValidator, string> _errorCodeResolver = DefaultErrorCodeResolver;
		private ILanguageManager _languageManager = new LanguageManager();

		/// <summary>
		/// Default cascade mode
		/// </summary>
		public CascadeMode CascadeMode { get; set; } = CascadeMode.Continue;

		/// <summary>
		/// Default property chain separator
		/// </summary>
		public string PropertyChainSeparator { get; set; } = ".";

		/// <summary>
		/// Default language manager
		/// </summary>
		public ILanguageManager LanguageManager {
			get => _languageManager;
			set => _languageManager = value ?? throw new ArgumentNullException(nameof(value));
		}

		/// <summary>
		/// Customizations of validator selector
		/// </summary>
		public ValidatorSelectorOptions ValidatorSelectors { get; } = new ValidatorSelectorOptions();

		/// <summary>
		/// Specifies a factory for creating MessageFormatter instances.
		/// </summary>
		public Func<MessageFormatter> MessageFormatterFactory {
			get => _messageFormatterFactory;
			set => _messageFormatterFactory = value ?? (() => new MessageFormatter());
		}

		/// <summary>
		/// Pluggable logic for resolving property names
		/// </summary>
		public Func<Type, MemberInfo, LambdaExpression, string> PropertyNameResolver {
			get => _propertyNameResolver;
			set => _propertyNameResolver = value ?? DefaultPropertyNameResolver;
		}

		/// <summary>
		/// Pluggable logic for resolving display names
		/// </summary>
		public Func<Type, MemberInfo, LambdaExpression, string> DisplayNameResolver {
			get => _displayNameResolver;
			set => _displayNameResolver = value ?? DefaultDisplayNameResolver;
		}

		/// <summary>
		/// Disables the expression accessor cache. Not recommended.
		/// </summary>
		public bool DisableAccessorCache { get; set; }

		/// <summary>
		/// Pluggable resolver for default error codes
		/// </summary>
		public Func<PropertyValidator, string> ErrorCodeResolver {
			get => _errorCodeResolver;
			set => _errorCodeResolver = value ?? DefaultErrorCodeResolver;
		}

		static string DefaultPropertyNameResolver(Type type, MemberInfo memberInfo, LambdaExpression expression) {
			if (expression != null) {
				var chain = PropertyChain.FromExpression(expression);
				if (chain.Count > 0) return chain.ToString();
			}

			return memberInfo?.Name;
		}

		static string DefaultDisplayNameResolver(Type type, MemberInfo memberInfo, LambdaExpression expression) => null;

		static string DefaultErrorCodeResolver(PropertyValidator validator) {
			return validator.GetType().Name;
		}
	}

	/// <summary>
	/// Validator runtime options
	/// </summary>
	public static class ValidatorOptions {

		/// <summary>
		/// Global configuration for all validators.
		/// </summary>
		public static ValidatorConfiguration Global { get; } = new ValidatorConfiguration();

		/// <summary>
		/// Default cascade mode
		/// </summary>
		[Obsolete("This property will be removed in FluentValidation 10. Use ValidatorOptions.Global.CascadeMode instead.")]
		public static CascadeMode CascadeMode {
			get => Global.CascadeMode;
			set => Global.CascadeMode = value;
		}

		/// <summary>
		/// Default property chain separator
		/// </summary>
		[Obsolete("This property will be removed in FluentValidation 10. Use ValidatorOptions.Global.PropertyChainSeparator instead.")]
		public static string PropertyChainSeparator {
			get => Global.PropertyChainSeparator;
			set => Global.PropertyChainSeparator = value;
		}

		/// <summary>
		/// Default language manager
		/// </summary>
		[Obsolete("This property will be removed in FluentValidation 10. Use ValidatorOptions.Global.LanguageManager instead.")]
		public static ILanguageManager LanguageManager {
			get => Global.LanguageManager;
			set => Global.LanguageManager = value;
		}

		/// <summary>
		/// Customizations of validator selector
		/// </summary>
		[Obsolete("This property will be removed in FluentValidation 10. Use ValidatorOptions.Global.ValidatorSelectors instead.")]
		public static ValidatorSelectorOptions ValidatorSelectors => Global.ValidatorSelectors;

		/// <summary>
		/// Specifies a factory for creating MessageFormatter instances.
		/// </summary>
		[Obsolete("This property will be removed in FluentValidation 10. Use ValidatorOptions.Global.MessageFormatterFactory instead.")]
		public static Func<MessageFormatter> MessageFormatterFactory {
			get => Global.MessageFormatterFactory;
			set => Global.MessageFormatterFactory = value;
		}

		/// <summary>
		/// Pluggable logic for resolving property names
		/// </summary>
		[Obsolete("This property will be removed in FluentValidation 10. Use ValidatorOptions.Global.PropertyNameResolver instead.")]
		public static Func<Type, MemberInfo, LambdaExpression, string> PropertyNameResolver {
			get => Global.PropertyNameResolver;
			set => Global.PropertyNameResolver = value;
		}

		/// <summary>
		/// Pluggable logic for resolving display names
		/// </summary>
		[Obsolete("This property will be removed in FluentValidation 10. Use ValidatorOptions.Global.DisplayNameResolver instead.")]
		public static Func<Type, MemberInfo, LambdaExpression, string> DisplayNameResolver {
			get => Global.DisplayNameResolver;
			set => Global.DisplayNameResolver = value;
		}

		/// <summary>
		/// Disables the expression accessor cache. Not recommended.
		/// </summary>
		[Obsolete("This property will be removed in FluentValidation 10. Use ValidatorOptions.Global.DisableAccessorCache instead.")]
		public static bool DisableAccessorCache {
			get => Global.DisableAccessorCache;
			set => Global.DisableAccessorCache = value;
		}

		/// <summary>
		/// Pluggable resolver for default error codes
		/// </summary>
		[Obsolete("This property will be removed in FluentValidation 10. Use ValidatorOptions.Global.ErrorCodeResolver instead.")]
		public static Func<PropertyValidator, string> ErrorCodeResolver {
			get => Global.ErrorCodeResolver;
			set => Global.ErrorCodeResolver = value;
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
