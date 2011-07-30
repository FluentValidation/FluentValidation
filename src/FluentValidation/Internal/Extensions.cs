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
// The latest version of this file can be found at http://www.codeplex.com/FluentValidation
#endregion

namespace FluentValidation.Internal {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Text.RegularExpressions;
	using Validators;

	/// <summary>
	/// Useful extensions
	/// </summary>
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

		/// <summary>
		/// Gets a MemberInfo from a member expression.
		/// </summary>
		public static MemberInfo GetMember(this LambdaExpression expression) {
			var memberExp = RemoveUnary(expression.Body);

			if (memberExp == null) {
				return null;
			}

			return memberExp.Member;
		}

		/// <summary>
		/// Gets a MemberInfo from a member expression.
		/// </summary>
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


		/// <summary>
		/// Splits pascal case, so "FooBar" would become "Foo Bar"
		/// </summary>
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

		internal static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
			foreach(var item in source) {
				action(item);	
			}
		}

		/// <summary>
		/// Based on a child validator and a propery rule, infers whether the validator should be wrapped in a ChildValidatorAdaptor or a CollectionValidatorAdaptor
		/// </summary>
		internal static IPropertyValidator InferPropertyValidatorForChildValidator(PropertyRule rule, IValidator childValidator) {
			// If the property implements IEnumerable<T> and the validator validates T, assume it's a collection property validator
			// This is here for backwards compatibility with v2. V3 uses the new SetCollectionValidator method.
			if (DoesImplementCompatibleIEnumerable(rule.TypeToValidate, childValidator)) {
				return new ChildCollectionValidatorAdaptor(childValidator);
			}

			// Otherwise if the validator is allowed to validate the type, assume child validator
			if (childValidator.CanValidateInstancesOfType(rule.TypeToValidate)) {
				return new ChildValidatorAdaptor(childValidator);
			}

			throw new InvalidOperationException(string.Format("The validator '{0}' cannot validate members of type '{1}' - the types are not compatible.", childValidator.GetType().Name, rule.TypeToValidate.Name));
		}

		private static bool DoesImplementCompatibleIEnumerable(Type propertyType, IValidator childValidator) {
			//concatenate the property type itself, incase we're using IEnumerable directly (typeof(IEnumerable).GetInterfaces() obviously doesn't include IEnumerable)
			var interfaces = from i in propertyType.GetInterfaces().Concat(new[] { propertyType })
							 where i.IsGenericType
							 where i.GetGenericTypeDefinition() == typeof(IEnumerable<>)
							 let enumerableType = i.GetGenericArguments()[0]
							 where childValidator.CanValidateInstancesOfType(enumerableType)
							 select i;

			return interfaces.Any();
		}

		public static Func<object, object> CoerceToNonGeneric<T, TProperty>(this Func<T, TProperty> func) {
			return x => func((T)x);
		} 

		public static Func<object, bool> CoerceToNonGeneric<T>(this Func<T, bool> func) {
			return x => func((T)x);
		}

		public static Action<object> CoerceToNonGeneric<T>(this Action<T> action) {
			return x => action((T)x);
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