#region License
// Copyright (c) Jeremy Skinner (http://www.jeremyskinner.co.uk) and contributors.
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
	using System.Linq.Expressions;
	using System.Reflection;

	/// <summary>
	/// Represents a chain of properties
	/// </summary>
	public class PropertyChain {
		readonly List<string> _memberNames = new List<string>(2);

		/// <summary>
		/// Creates a new PropertyChain.
		/// </summary>
		public PropertyChain() {
		}

		/// <summary>
		/// Creates a new PropertyChain based on another.
		/// </summary>
		public PropertyChain(PropertyChain parent) {
			if(parent != null
				&& parent._memberNames.Count > 0) {
				_memberNames.AddRange(parent._memberNames);
			}
		}

		/// <summary>
		/// Creates a new PropertyChain
		/// </summary>
		/// <param name="memberNames"></param>
		public PropertyChain(IEnumerable<string> memberNames) {
			this._memberNames.AddRange(memberNames);
		}

		/// <summary>
		/// Creates a PropertyChain from a lambda expression
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="throwOnInvalid">Whether to throw an exception if the expression is not supported (true) or to return an empty property chain (false). Defaults to false (returning an empty property chain)</param>
		/// <returns></returns>
		public static PropertyChain FromExpression<T, TProperty>(Expression<Func<T, TProperty>> expression, bool throwOnInvalid = false) {
			return FromExpression((LambdaExpression) expression, throwOnInvalid);
		}

		/// <summary>
		/// Creates a PropertyChain from a lambda expression
		/// </summary>
		/// <param name="expression">The expression to convert</param>
		/// <param name="throwOnInvalid">Whether to throw an exception if the expression is not supported (true) or to return an empty property chain (false). Defaults to false (returning an empty property chain)</param>
		/// <returns></returns>
		public static PropertyChain FromExpression(LambdaExpression expression, bool throwOnInvalid = false) {
			var memberExp = Extensions.RemoveUnary(expression.Body) as MemberExpression;

			// If it's not a MemberExpression, either throw an exception or return an empty property chain.
			if (memberExp == null) {
				if (throwOnInvalid) {
					throw new NotSupportedException("The expression you passed in isn't supported. Only member-access expressions can be used to create a property chain.");
				}
				return new PropertyChain();
			}

			var memberNames = new Stack<string>();
			Expression currentExpr = memberExp;
			object currentIndexer = null;

			while (true) {
				currentExpr = Extensions.RemoveUnary(currentExpr);
				object indexerToUse = currentIndexer;
				currentIndexer = null;

				if (currentExpr != null && currentExpr is MemberExpression me) {
					currentExpr = me.Expression;
					var memberName = me.Member.Name;

					if (indexerToUse != null) {
						memberName += "[" + indexerToUse + "]";
					}

					memberNames.Push(memberName);
				}
				else if (currentExpr != null && currentExpr is MethodCallExpression methodExp) {
					// Method calls are OK so long as they're not the last item.

					// Is it an indexer method? If so, normalize it to C# style indexer.
					// Must be called get_Item, have 1 argument and that argument is constant.
					if (methodExp.Method.IsSpecialName && methodExp.Method.Name == "get_Item" && methodExp.Arguments.Count == 1 && methodExp.Arguments[0] is ConstantExpression ce) {
						// TODO: Do we need to care about multi-argument indexers?
						// Record the indexer for the next round of the loop.
						currentIndexer = ce.Value;
					}
					else {
						string methodName = methodExp.Method.Name;
						if (indexerToUse != null) {
							methodName += "[" + indexerToUse + "]";
						}
						memberNames.Push(methodName);
					}

					currentExpr = methodExp.Object;
				}
				else {
					break;
				}
			}

			// The root of the expression must be a ParameterExpression (ie x => x.<something>)
			if (currentExpr == null || currentExpr.NodeType != ExpressionType.Parameter) {
				if (throwOnInvalid) {
					throw new NotSupportedException("The expression you passed in isn't supported. Only member-access expressions can be used to create a property chain.");
				}
				return new PropertyChain();
			}

			return new PropertyChain(memberNames);
		}

		/// <summary>
		/// Adds a MemberInfo instance to the chain
		/// </summary>
		/// <param name="member">Member to add</param>
		public void Add(MemberInfo member) {
			if(member != null)
				_memberNames.Add(member.Name);
		}

		/// <summary>
		/// Adds a property name to the chain
		/// </summary>
		/// <param name="propertyName">Name of the property to add</param>
		public void Add(string propertyName) {
			if(!string.IsNullOrEmpty(propertyName))
				_memberNames.Add(propertyName);
		}

		/// <summary>
		/// Adds an indexer to the property chain. For example, if the following chain has been constructed:
		/// Parent.Child
		/// then calling AddIndexer(0) would convert this to:
		/// Parent.Child[0]
		/// </summary>
		/// <param name="indexer"></param>
		/// <param name="surroundWithBrackets">Whether square brackets should be applied before and after the indexer. Default true.</param>
		public void AddIndexer(object indexer, bool surroundWithBrackets = true) {
			if(_memberNames.Count == 0) {
				throw new InvalidOperationException("Could not apply an Indexer because the property chain is empty.");
			}

			string last = _memberNames[_memberNames.Count - 1];
			last += surroundWithBrackets ? "[" + indexer + "]" : indexer;

			_memberNames[_memberNames.Count - 1] = last;
		}

		/// <summary>
		/// Creates a string representation of a property chain.
		/// </summary>
		public override string ToString() {
			// Performance: Calling string.Join causes much overhead when it's not needed.
			switch (_memberNames.Count) {
				case 0:
					return string.Empty;
				case 1:
					return _memberNames[0];
				default:
					return string.Join(ValidatorOptions.PropertyChainSeparator, _memberNames);
			}
		}

		/// <summary>
		/// Checks if the current chain is the child of another chain.
		/// For example, if chain1 were for "Parent.Child" and chain2 were for "Parent.Child.GrandChild" then
		/// chain2.IsChildChainOf(chain1) would be true.
		/// </summary>
		/// <param name="parentChain">The parent chain to compare</param>
		/// <returns>True if the current chain is the child of the other chain, otherwise false</returns>
		public bool IsChildChainOf(PropertyChain parentChain) {
			return ToString().StartsWith(parentChain.ToString());
		}

		/// <summary>
		/// Builds a property path.
		/// </summary>
		public string BuildPropertyName(string propertyName) {
			if (_memberNames.Count == 0) {
				return propertyName;
			}

			var chain = new PropertyChain(this);
			chain.Add(propertyName);
			return chain.ToString();
		}

		/// <summary>
		/// Number of member names in the chain
		/// </summary>
		public int Count => _memberNames.Count;
	}
}
