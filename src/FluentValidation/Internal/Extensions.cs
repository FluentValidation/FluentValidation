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
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Text;
	using Resources;

	/// <summary>
	/// Useful extensions
	/// </summary>
	public static class Extensions {

		/// <summary>
		/// Gets a MemberInfo from a member expression.
		/// </summary>
		public static MemberInfo GetMember<T, TProperty>(this Expression<Func<T, TProperty>> expression) {
			var memberExp = RemoveUnary(expression.Body) as MemberExpression;

			if (memberExp == null) {
				return null;
			}

			Expression currentExpr = memberExp.Expression;

			// Unwind the expression to get the root object that the expression acts upon.
			while (true) {
				currentExpr = RemoveUnary(currentExpr);

				if (currentExpr != null && currentExpr.NodeType == ExpressionType.MemberAccess) {
					currentExpr = ((MemberExpression)currentExpr).Expression;
				} else {
					break;
				}
			}

			if (currentExpr == null || currentExpr.NodeType != ExpressionType.Parameter) {
				return null; // We don't care if we're not acting upon the model instance.
			}

			return memberExp.Member;
		}

		private static Expression RemoveUnary(Expression toUnwrap) {
			if (toUnwrap is UnaryExpression) {
				return ((UnaryExpression)toUnwrap).Operand;
			}

			return toUnwrap;
		}
	}

}
