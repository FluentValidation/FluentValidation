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

	/// <summary>
	/// Useful extensions
	/// </summary>
	public static class Extensions {
		internal static void Guard(this object obj, string message, string paramName) {
			if (obj == null) {
				throw new ArgumentNullException(paramName, message);
			}
		}

		internal static void Guard(this string str, string message, string paramName) {
			if (str == null) {
				throw new ArgumentNullException(paramName, message);
			}

			if (string.IsNullOrEmpty(str)) {
				throw new ArgumentException(message, paramName);
			}
		}

		/// <summary>
		/// Checks if the expression is a parameter expression
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		internal static bool IsParameterExpression(this LambdaExpression expression) {
			return expression.Body.NodeType == ExpressionType.Parameter;
		}

		/// <summary>
		/// Gets a MemberInfo from a member expression.
		/// </summary>
		internal static MemberInfo GetMember<T, TProperty>(this Expression<Func<T, TProperty>> expression) {
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

		/// <summary>
		/// Splits pascal case, so "FooBar" would become "Foo Bar"
		/// </summary>
		internal static string SplitPascalCase(this string input) {
			if (string.IsNullOrEmpty(input))
				return input;

			var retVal = new StringBuilder(input.Length + 5);

			for (int i = 0; i < input.Length; ++i) {
				var currentChar = input[i];
				if (char.IsUpper(currentChar)) {
					if ((i > 1 && !char.IsUpper(input[i - 1]))
							|| (i + 1 < input.Length && !char.IsUpper(input[i + 1])))
						retVal.Append(' ');
				}

				retVal.Append(currentChar);
			}

			return retVal.ToString().Trim();
		}

		internal static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
			foreach (var item in source) {
				action(item);
			}
		}

		[Obsolete]
		internal static Func<object, object> CoerceToNonGeneric<T, TProperty>(this Func<T, TProperty> func) {
			return x => func((T)x);
		}

		[Obsolete]
		internal static Func<object, int> CoerceToNonGeneric<T>(this Func<T, int> func) {
			return x => func((T)x);
		}

		[Obsolete]
		internal static Func<object, string> CoerceToNonGeneric<T>(this Func<T, string> func) {
			return x => func((T)x);
		}

		[Obsolete]
		internal static Func<object, System.Text.RegularExpressions.Regex> CoerceToNonGeneric<T>(this Func<T, System.Text.RegularExpressions.Regex> func) {
			return x => func((T)x);
		}

		/// <summary>
		/// Checks whether this is an asynchronous validation run.
		/// </summary>
		/// <param name="ctx"></param>
		/// <returns></returns>
		public static bool IsAsync(this IValidationContext ctx) {
			if (ctx.RootContextData.ContainsKey("__FV_IsAsyncExecution")) {
				return (ctx.RootContextData["__FV_IsAsyncExecution"] as bool?).GetValueOrDefault();
			}

			return false;
		}

		internal static T GetOrAdd<T>(this IDictionary<string, object> dict, string key, Func<T> value) {
			if (dict.TryGetValue(key, out var tmp)) {
				if (tmp is T result) {
					return result;
				}
			}

			var val = value();
			dict[key] = val;
			return val;
		}
	}
}
