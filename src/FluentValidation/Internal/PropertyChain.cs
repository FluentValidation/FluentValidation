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
		/// Creates a PropertyChain from a lambda expresion
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static PropertyChain FromExpression(LambdaExpression expression) {
			var memberNames = new Stack<string>();

			var getMemberExp = new Func<Expression, MemberExpression>(toUnwrap => {
				if (toUnwrap is UnaryExpression) {
					return ((UnaryExpression)toUnwrap).Operand as MemberExpression;
				}

				return toUnwrap as MemberExpression;
			});

			var memberExp = getMemberExp(expression.Body);

			while(memberExp != null) {
				memberNames.Push(memberExp.Member.Name);
				memberExp = getMemberExp(memberExp.Expression);
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
		public void AddIndexer(object indexer) {
			if(_memberNames.Count == 0) {
				throw new InvalidOperationException("Could not apply an Indexer because the property chain is empty.");
			}

			string last = _memberNames[_memberNames.Count - 1];
			last += "[" + indexer + "]";

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