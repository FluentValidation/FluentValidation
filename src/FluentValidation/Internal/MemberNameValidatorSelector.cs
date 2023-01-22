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

namespace FluentValidation.Internal;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

/// <summary>
/// Selects validators that are associated with a particular property.
/// </summary>
public class MemberNameValidatorSelector : IValidatorSelector {
	internal const string DisableCascadeKey = "_FV_DisableSelectorCascadeForChildRules";
	readonly IEnumerable<string> _memberNames;

	/// <summary>
	/// Creates a new instance of MemberNameValidatorSelector.
	/// </summary>
	public MemberNameValidatorSelector(IEnumerable<string> memberNames) {
		_memberNames = memberNames;
	}

	/// <summary>
	/// Member names that are validated.
	/// </summary>
	public IEnumerable<string> MemberNames => _memberNames;

	/// <summary>
	/// Determines whether or not a rule should execute.
	/// </summary>
	/// <param name="rule">The rule</param>
	/// <param name="propertyPath">Property path (eg Customer.Address.Line1)</param>
	/// <param name="context">Contextual information</param>
	/// <returns>Whether or not the validator can execute.</returns>
	public bool CanExecute (IValidationRule rule, string propertyPath, IValidationContext context) {
		// Validator selector only applies to the top level.
		// If we're running in a child context then this means that the child validator has already been selected
		// Because of this, we assume that the rule should continue (ie if the parent rule is valid, all children are valid)
		bool isChildContext = context.IsChildContext;
		bool cascadeEnabled = !context.RootContextData.ContainsKey(DisableCascadeKey);

		// If a child validator is being executed and the cascade is enabled (which is the default)
		// then the child validator's rule should always be included.
		// The only time this isn't the case is if the member names contained for inclusion are for child
		// properties (which is indicated by them containing a period).
		if (isChildContext && cascadeEnabled && !_memberNames.Any(x => x.Contains('.'))) {
			return true;
		}

		// Rules included via Include() are always executed.
		if (rule is IIncludeRule) {
			return true;
		}

		// If the current property path is equal to any of the member names for inclusion
		// or it's a child property path (indicated by a period) where we have a partial match.
		foreach (var memberName in _memberNames) {
			// If the property path is equal to any of the input member names then it should be executed.
			if (memberName == propertyPath) {
				return true;
			}

			// If the property path is for a child property,
			// and the parent property is selected for inclusion,
			// then it should be allowed to execute.
			if (propertyPath.StartsWith(memberName + ".")) {
				return true;
			}

			// If the property path is for a parent property,
			// and any of its child properties are selected for inclusion
			// then it should be allowed to execute
			if (memberName.StartsWith(propertyPath + ".")) {
				return true;
			}

			// If the property path is for parent property,
			// it's a collection
			if (memberName.StartsWith(propertyPath + "[")) {
				return true;
			}
			// If property path is for child property within collection,
			// and member path contains wildcard []
			// then it should be allowed to execute
			if (Regex.IsMatch(propertyPath, memberName.Replace("[]", @"\[\d+\]"))) {
				return true;
			}

		}

		return false;
	}

	/// <summary>
	/// Gets member names from expressions
	/// </summary>
	/// <param name="propertyExpressions"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static string[] MemberNamesFromExpressions<T>(params Expression<Func<T, object>>[] propertyExpressions) {
		var members = propertyExpressions.Select(MemberFromExpression).ToArray();
		return members;
	}

	private static string MemberFromExpression<T>(Expression<Func<T, object>> expression) {
		var propertyName = ValidatorOptions.Global.PropertyNameResolver(typeof(T), expression.GetMember(), expression);

		if (string.IsNullOrEmpty(propertyName)) {
			throw new ArgumentException($"Expression '{expression}' does not specify a valid property or field.");
		}

		return propertyName;
	}
}
