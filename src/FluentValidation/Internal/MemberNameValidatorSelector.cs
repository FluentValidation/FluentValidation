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

namespace FluentValidation.Internal {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;

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

			return (isChildContext && cascadeEnabled && !_memberNames.Any(x => x.Contains(".")))
			       || rule is IIncludeRule
			       || ( _memberNames.Any(x => x == propertyPath || propertyPath.StartsWith(x + ".") || x.StartsWith(propertyPath + ".")));
		}

		///<summary>
		/// Creates a MemberNameValidatorSelector from a collection of expressions.
		///</summary>
		[Obsolete("This method will be removed from FluentValidation in 10.0")]
		public static MemberNameValidatorSelector FromExpressions<T>(params Expression<Func<T, object>>[] propertyExpressions) {
			var members = propertyExpressions.Select(MemberFromExpression).ToList();
			return new MemberNameValidatorSelector(members);
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
			var chain = PropertyChain.FromExpression(expression);

			if (chain.Count == 0) {
				throw new ArgumentException($"Expression '{expression}' does not specify a valid property or field.");
			}

			return chain.ToString();
		}
	}
}
