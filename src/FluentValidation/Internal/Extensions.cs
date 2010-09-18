#region License
// Copyright 2008-2009 Jeremy Skinner (http://www.jeremyskinner.co.uk)
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
// The latest version of this file can be found at http://www.codeplex.com/FluentValidation
#endregion

namespace FluentValidation.Internal {
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Text.RegularExpressions;

	public static class Extensions {
		internal static void Guard(this object obj, string message) {
			if (obj == null) {
				throw new ArgumentNullException(message);
			}
		}

		internal static void Guard(this string str, string message) {
			if (string.IsNullOrEmpty(str)) {
				throw new ArgumentNullException(message);
			}
		}

		public static MemberInfo GetMember(this LambdaExpression expression) {
			var memberExp = RemoveUnary(expression.Body);

			if (memberExp == null) {
				return null;
			}

			return memberExp.Member;
		}

		public static MemberInfo GetMember<T, TProperty>(this Expression<Func<T, TProperty>> expression) {
			var memberExp = RemoveUnary(expression.Body);

			if (memberExp == null) {
				return null;
			}

			return memberExp.Member;
		}

		private static MemberExpression RemoveUnary(Expression toUnwrap) {
			if (toUnwrap is UnaryExpression) {
				return ((UnaryExpression)toUnwrap).Operand as MemberExpression;
			}

			return toUnwrap as MemberExpression;
		}


		public static string SplitPascalCase(this string input) {
			if (string.IsNullOrEmpty(input)) {
				return input;
			}
			return Regex.Replace(input, "([A-Z])", " $1").Trim();
		}
		/// <summary>
		/// Helper method to construct a constant expression from a constant.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="valueToCompare">The value being compared</param>
		/// <returns></returns>
		internal static Expression<Func<T, TProperty>> GetConstantExpresionFromConstant<T, TProperty>(TProperty valueToCompare) {
			Expression constant = Expression.Constant(valueToCompare, typeof(TProperty));
			ParameterExpression parameter = Expression.Parameter(typeof(T), "t");
			return Expression.Lambda<Func<T, TProperty>>(constant, parameter);
		}

		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
			foreach(var item in source) {
				action(item);	
			}
		}

#if WINDOWS_PHONE
		// WP7 doesn't support expression tree compilation.
		// As a workaround, this extension method falls back to delegate compilation. 
		// However, it only supports simple property references, ie x => x.SomeProperty

		internal static TDelegate Compile<TDelegate>(this Expression<TDelegate> expression) {
			var compiledDelegate = CompilePropertyGetterExpression(expression, typeof(TDelegate));
			return (TDelegate)compiledDelegate;
		}

		static object CompilePropertyGetterExpression(LambdaExpression expression, Type delegateType) {
			var member = expression.GetMember() as PropertyInfo;

			if (member == null) {
				throw new NotSupportedException("FluentValidation for WP7 can only be used with expressions that reference public properties, ie x => x.SomeProperty");
			}

			var compiledDelegate = Delegate.CreateDelegate(delegateType, member.GetGetMethod());
			return compiledDelegate;
		}
#endif
	}
}