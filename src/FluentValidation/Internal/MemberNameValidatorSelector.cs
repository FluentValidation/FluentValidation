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

	// Regex for normalizing collection indicies from Orders[0].Name to Orders[].Name
	private static Regex _collectionIndexNormalizer = new Regex(@"\[.*?\]", RegexOptions.Compiled);

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

		// Stores the normalized property name if we're working with collection properties
		// eg Orders[0].Name -> Orders[].Name. This is only initialized if needed (see below).
		string normalizedPropertyPath = null;

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

			// If the property path is for a collection property
			// and a child property for this collection has been passed in for inclusion.
			// For example, memberName is "Orders[0].Amount"
			// and propertyPath is "Orders" then it should be allowed to execute.
			if (memberName.StartsWith(propertyPath + "[")) {
				return true;
			}

			// If property path is for child property within collection,
			// and member path contains wildcard [] then this means that we want to match
			// with all items in the collection, but we need to normalize the property path
			// in order to match. For example, if the propertyPath is "Orders[0].Name"
			// and the memberName for inclusion is "Orders[].Name" then this should
			// be allowed to match.
			if (memberName.Contains("[]")) {
				if (normalizedPropertyPath == null) {
					// Normalize the property path using a regex. Orders[0].Name -> Orders[].Name.
					normalizedPropertyPath = _collectionIndexNormalizer.Replace(propertyPath, "[]");
				}

				if (memberName == normalizedPropertyPath) {
					return true;
				}

				if (memberName.StartsWith(normalizedPropertyPath + ".")) {
					return true;
				}

				if (memberName.StartsWith(normalizedPropertyPath + "[")) {
					return true;
				}
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
