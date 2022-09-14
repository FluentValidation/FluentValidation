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

namespace FluentValidation;

using System;
using System.Collections.Generic;
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
	private Func<IPropertyValidator, string> _errorCodeResolver = DefaultErrorCodeResolver;
	private ILanguageManager _languageManager = new LanguageManager();

	/// <summary>
	/// <para>
	/// Sets the default value for <see cref="AbstractValidator{T}.ClassLevelCascadeMode"/>.
	/// Defaults to <see cref="FluentValidation.CascadeMode.Continue"/> if not set.
	/// </para>
	/// </summary>
	public CascadeMode DefaultClassLevelCascadeMode { get; set; } = CascadeMode.Continue;

	/// <summary>
	/// <para>
	/// Sets the default value for <see cref="AbstractValidator{T}.RuleLevelCascadeMode"/>
	/// Defaults to <see cref="FluentValidation.CascadeMode.Continue"/> if not set.
	/// </para>
	/// </summary>
	public CascadeMode DefaultRuleLevelCascadeMode { get; set; } = CascadeMode.Continue;

	/// <summary>
	/// Default severity level
	/// </summary>
	public Severity Severity { get; set; } = Severity.Error;

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
	public Func<IPropertyValidator, string> ErrorCodeResolver {
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

	static string DefaultErrorCodeResolver(IPropertyValidator validator) {
		return validator.Name;
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
}

/// <summary>
/// ValidatorSelector options
/// </summary>
public class ValidatorSelectorOptions {
	private static readonly IValidatorSelector DefaultSelector = new DefaultValidatorSelector();

	private Func<IValidatorSelector> _defaultValidatorSelector = () => DefaultSelector;
	private Func<IEnumerable<string>, IValidatorSelector> _memberNameValidatorSelector = properties => new MemberNameValidatorSelector(properties);
	private Func<IEnumerable<string>, IValidatorSelector> _rulesetValidatorSelector = ruleSets => new RulesetValidatorSelector(ruleSets);
	private Func<IEnumerable<IValidatorSelector>, IValidatorSelector> _compositeValidatorSelectorFactory = selectors => new CompositeValidatorSelector(selectors);

	/// <summary>
	/// Factory func for creating the default validator selector
	/// </summary>
	public Func<IValidatorSelector> DefaultValidatorSelectorFactory {
		get => _defaultValidatorSelector;
		set => _defaultValidatorSelector = value ?? (() => DefaultSelector);
	}

	/// <summary>
	/// Factory func for creating the member validator selector
	/// </summary>
	public Func<IEnumerable<string>, IValidatorSelector> MemberNameValidatorSelectorFactory {
		get => _memberNameValidatorSelector;
		set => _memberNameValidatorSelector = value ?? (properties => new MemberNameValidatorSelector(properties));
	}

	/// <summary>
	/// Factory func for creating the ruleset validator selector
	/// </summary>
	public Func<IEnumerable<string>, IValidatorSelector> RulesetValidatorSelectorFactory {
		get => _rulesetValidatorSelector;
		set => _rulesetValidatorSelector = value ?? (ruleSets => new RulesetValidatorSelector(ruleSets));
	}

	/// <summary>
	/// Factory func for creating the composite validator selector
	/// </summary>
	public Func<IEnumerable<IValidatorSelector>, IValidatorSelector> CompositeValidatorSelectorFactory {
		get => _compositeValidatorSelectorFactory;
		set => _compositeValidatorSelectorFactory = value ?? (selectors => new CompositeValidatorSelector(selectors));
	}
}
