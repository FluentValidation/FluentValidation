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

namespace FluentValidation {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Text.RegularExpressions;
	using Internal;
	using Results;
	using Validators;

	/// <summary>
	/// Extension methods that provide the default set of validators.
	/// </summary>
	public static class DefaultValidatorExtensions {
		/// <summary>
		/// Defines a 'not null' validator on the current rule builder. 
		/// Validation will fail if the property is null.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> NotNull<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder) {
			return ruleBuilder.SetValidator(new NotNullValidator());
		}

		/// <summary>
		/// Defines a 'not empty' validator on the current rule builder.
		/// Validation will fail if the property is null, an empty or the default value for the type (for example, 0 for integers)
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> NotEmpty<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder) {
			return ruleBuilder.SetValidator(new NotEmptyValidator(default(TProperty)));
		}

		/// <summary>
		/// Defines a length validator on the current rule builder, but only for string properties.
		/// Validation will fail if the length of the string is outside of the specifed range. The range is inclusive.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, string> Length<T>(this IRuleBuilder<T, string> ruleBuilder, int min, int max) {
			return ruleBuilder.SetValidator(new LengthValidator(min, max));
		}

		/// <summary>
		/// Defines a length validator on the current rule builder, but only for string properties.
		/// Validation will fail if the length of the string is not equal to the length specified.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, string> Length<T>(this IRuleBuilder<T, string> ruleBuilder, int exactLength) {
			return ruleBuilder.SetValidator(new ExactLengthValidator(exactLength));
		}

		/// <summary>
		/// Defines a regular expression validator on the current rule builder, but only for string properties.
		/// Validation will fail if the value returned by the lambda does not match the regular expression.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="expression">The regular expression to check the value against.</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, string> Matches<T>(this IRuleBuilder<T, string> ruleBuilder, string expression) {
			return ruleBuilder.SetValidator(new RegularExpressionValidator(expression));
		}


		/// <summary>
		/// Defines a regular expression validator on the current rule builder, but only for string properties.
		/// Validation will fail if the value returned by the lambda does not match the regular expression.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="regex">The regular expression to use</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, string> Matches<T>(this IRuleBuilder<T, string> ruleBuilder, Regex regex) {
			return ruleBuilder.SetValidator(new RegularExpressionValidator(regex));
		}


		/// <summary>
		/// Defines a regular expression validator on the current rule builder, but only for string properties.
		/// Validation will fail if the value returned by the lambda does not match the regular expression.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="expression">The regular expression to check the value against.</param>
		/// <param name="options">Regex options</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, string> Matches<T>(this IRuleBuilder<T, string> ruleBuilder, string expression, RegexOptions options) {
			return ruleBuilder.SetValidator(new RegularExpressionValidator(expression, options));
		}

		/// <summary>
		/// Defines a regular expression validator on the current rule builder, but only for string properties.
		/// Validation will fail if the value returned by the lambda is not a valid email address.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, string> EmailAddress<T>(this IRuleBuilder<T, string> ruleBuilder) {
			return ruleBuilder.SetValidator(new EmailValidator());
		}

		/// <summary>
		/// Defines a 'not equal' validator on the current rule builder.
		/// Validation will fail if the specified value is equal to the value of the property.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="toCompare">The value to compare</param>
		/// <param name="comparer">Equality comparer to use</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> NotEqual<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder,
																			   TProperty toCompare, IEqualityComparer comparer = null) {
			return ruleBuilder.SetValidator(new NotEqualValidator(toCompare, comparer));
		}
		
		/// <summary>
		/// Defines a 'not equal' validator on the current rule builder using a lambda to specify the value.
		/// Validation will fail if the value returned by the lambda is equal to the value of the property.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="expression">A lambda expression to provide the comparison value</param>
		/// <param name="comparer">Equality Comparer to use</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> NotEqual<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder,
																			   Expression<Func<T, TProperty>> expression, IEqualityComparer comparer = null) {
			var func = expression.Compile();
			return ruleBuilder.SetValidator(new NotEqualValidator(func.CoerceToNonGeneric(), expression.GetMember(), comparer));
		}

		/// <summary>
		/// Defines an 'equals' validator on the current rule builder. 
		/// Validation will fail if the specified value is not equal to the value of the property.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="toCompare">The value to compare</param>
		/// <param name="comparer">Equality Comparer to use</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> Equal<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, TProperty toCompare, IEqualityComparer comparer = null) {
			return ruleBuilder.SetValidator(new EqualValidator(toCompare, comparer));
		}

		/// <summary>
		/// Defines an 'equals' validator on the current rule builder using a lambda to specify the comparison value.
		/// Validation will fail if the value returned by the lambda is not equal to the value of the property.
		/// </summary>
		/// <typeparam name="T">The type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="expression">A lambda expression to provide the comparison value</param>
		/// <param name="comparer">Equality comparer to use</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> Equal<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Expression<Func<T, TProperty>> expression, IEqualityComparer comparer = null) {
			var func = expression.Compile();
			return ruleBuilder.SetValidator(new EqualValidator(func.CoerceToNonGeneric(), expression.GetMember(), comparer));
		}

		/// <summary>
		/// Defines a predicate validator on the current rule builder using a lambda expression to specify the predicate.
		/// Validation will fail if the specified lambda returns false. 
		/// Validation will succeed if the specifed lambda returns true.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="predicate">A lambda expression specifying the predicate</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> Must<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder,
		                                                                   Func<TProperty, bool> predicate) {
			predicate.Guard("Cannot pass a null predicate to Must.");

			return ruleBuilder.Must((x, val) => predicate(val));
		}

		/// <summary>
		/// Defines a predicate validator on the current rule builder using a lambda expression to specify the predicate.
		/// Validation will fail if the specified lambda returns false. 
		/// Validation will succeed if the specifed lambda returns true.
		/// This overload accepts the object being validated in addition to the property being validated.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="predicate">A lambda expression specifying the predicate</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> Must<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder,
		                                                                   Func<T, TProperty, bool> predicate) {
			predicate.Guard("Cannot pass a null predicate to Must.");
            return ruleBuilder.Must((x, val, propertyValidatorContext) => predicate(x, val));
		}

        /// <summary>
        /// Defines a predicate validator on the current rule builder using a lambda expression to specify the predicate.
        /// Validation will fail if the specified lambda returns false. 
        /// Validation will succeed if the specifed lambda returns true.
        /// This overload accepts the object being validated in addition to the property being validated.
        /// </summary>
        /// <typeparam name="T">Type of object being validated</typeparam>
        /// <typeparam name="TProperty">Type of property being validated</typeparam>
        /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
        /// <param name="predicate">A lambda expression specifying the predicate</param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, TProperty> Must<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, 
                                                                           Func<T, TProperty, PropertyValidatorContext, bool> predicate) {
            predicate.Guard("Cannot pass a null predicate to Must.");
            return ruleBuilder.SetValidator(new PredicateValidator((instance, property, propertyValidatorContext) => predicate((T)instance, (TProperty)property, propertyValidatorContext)));
        }

		/// <summary>
		/// Defines a 'less than' validator on the current rule builder. 
		/// The validation will succeed if the property value is less than the specified value.
		/// The validation will fail if the property value is greater than or equal to the specified value.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="valueToCompare">The value being compared</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> LessThan<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder,
		                                                                       TProperty valueToCompare)
			where TProperty : IComparable<TProperty>, IComparable {
			return ruleBuilder.SetValidator(new LessThanValidator(valueToCompare));
		}

		/// <summary>
		/// Defines a 'less than' validator on the current rule builder. 
		/// The validation will succeed if the property value is less than the specified value.
		/// The validation will fail if the property value is greater than or equal to the specified value.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="valueToCompare">The value being compared</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, Nullable<TProperty>> LessThan<T, TProperty>(this IRuleBuilder<T, Nullable<TProperty>> ruleBuilder,
																			   TProperty valueToCompare)
			where TProperty : struct, IComparable<TProperty>, IComparable {
			return ruleBuilder.SetValidator(new LessThanValidator(valueToCompare));
		}

		/// <summary>
		/// Defines a 'less than or equal' validator on the current rule builder. 
		/// The validation will succeed if the property value is less than or equal to the specified value.
		/// The validation will fail if the property value is greater than the specified value.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="valueToCompare">The value being compared</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> LessThanOrEqualTo<T, TProperty>(
			this IRuleBuilder<T, TProperty> ruleBuilder, TProperty valueToCompare) where TProperty : IComparable<TProperty>, IComparable {
			return ruleBuilder.SetValidator(new LessThanOrEqualValidator(valueToCompare));
		}

		/// <summary>
		/// Defines a 'less than or equal' validator on the current rule builder. 
		/// The validation will succeed if the property value is less than or equal to the specified value.
		/// The validation will fail if the property value is greater than the specified value.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="valueToCompare">The value being compared</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, Nullable<TProperty>> LessThanOrEqualTo<T, TProperty>(
			this IRuleBuilder<T, Nullable<TProperty>> ruleBuilder, TProperty valueToCompare) where TProperty : struct, IComparable<TProperty>, IComparable {
			return ruleBuilder.SetValidator(new LessThanOrEqualValidator(valueToCompare));
		}

		/// <summary>
		/// Defines a 'greater than' validator on the current rule builder. 
		/// The validation will succeed if the property value is greater than the specified value.
		/// The validation will fail if the property value is less than or equal to the specified value.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="valueToCompare">The value being compared</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> GreaterThan<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, TProperty valueToCompare)
			where TProperty : IComparable<TProperty>, IComparable {
			return ruleBuilder.SetValidator(new GreaterThanValidator(valueToCompare));
		}

		/// <summary>
		/// Defines a 'greater than' validator on the current rule builder. 
		/// The validation will succeed if the property value is greater than the specified value.
		/// The validation will fail if the property value is less than or equal to the specified value.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="valueToCompare">The value being compared</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty?> GreaterThan<T, TProperty>(this IRuleBuilder<T, TProperty?> ruleBuilder, TProperty valueToCompare)
			where TProperty : struct, IComparable<TProperty>, IComparable {
			return ruleBuilder.SetValidator(new GreaterThanValidator(valueToCompare));
		}

		/// <summary>
		/// Defines a 'greater than or equal' validator on the current rule builder. 
		/// The validation will succeed if the property value is greater than or equal the specified value.
		/// The validation will fail if the property value is less than the specified value.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="valueToCompare">The value being compared</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> GreaterThanOrEqualTo<T, TProperty>(
			this IRuleBuilder<T, TProperty> ruleBuilder, TProperty valueToCompare) where TProperty : IComparable<TProperty>, IComparable {
			return ruleBuilder.SetValidator(new GreaterThanOrEqualValidator(valueToCompare));
		}

		/// <summary>
		/// Defines a 'greater than or equal' validator on the current rule builder. 
		/// The validation will succeed if the property value is greater than or equal the specified value.
		/// The validation will fail if the property value is less than the specified value.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="valueToCompare">The value being compared</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, Nullable<TProperty>> GreaterThanOrEqualTo<T, TProperty>(
			this IRuleBuilder<T, Nullable<TProperty>> ruleBuilder, TProperty valueToCompare) where TProperty : struct, IComparable<TProperty>, IComparable {
			return ruleBuilder.SetValidator(new GreaterThanOrEqualValidator(valueToCompare));
		}


		/// <summary>
		/// Defines a 'less than' validator on the current rule builder using a lambda expression. 
		/// The validation will succeed if the property value is less than the specified value.
		/// The validation will fail if the property value is greater than or equal to the specified value.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="expression">A lambda that should return the value being compared</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> LessThan<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder,
		                                                                       Expression<Func<T, TProperty>> expression)
			where TProperty : IComparable<TProperty>, IComparable {
			expression.Guard("Cannot pass null to LessThan");

			var func = expression.Compile();

			return ruleBuilder.SetValidator(new LessThanValidator(func.CoerceToNonGeneric(), expression.GetMember()));
		}

		/// <summary>
		/// Defines a 'less than' validator on the current rule builder using a lambda expression. 
		/// The validation will succeed if the property value is less than the specified value.
		/// The validation will fail if the property value is greater than or equal to the specified value.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="expression">A lambda that should return the value being compared</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, Nullable<TProperty>> LessThan<T, TProperty>(this IRuleBuilder<T, Nullable<TProperty>> ruleBuilder,
		                                                                                 Expression<Func<T, TProperty>> expression)
			where TProperty : struct, IComparable<TProperty>, IComparable {
			expression.Guard("Cannot pass null to LessThan");

			var func = expression.Compile();

			return ruleBuilder.SetValidator(new LessThanValidator(func.CoerceToNonGeneric(), expression.GetMember()));
		}

		/// <summary>
		/// Defines a 'less than or equal' validator on the current rule builder using a lambda expression. 
		/// The validation will succeed if the property value is less than or equal to the specified value.
		/// The validation will fail if the property value is greater than the specified value.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="expression">The value being compared</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> LessThanOrEqualTo<T, TProperty>(
			this IRuleBuilder<T, TProperty> ruleBuilder, Expression<Func<T, TProperty>> expression)
			where TProperty : IComparable<TProperty>, IComparable {
			var func = expression.Compile();

			return ruleBuilder.SetValidator(new LessThanOrEqualValidator(func.CoerceToNonGeneric(), expression.GetMember()));
		}

		/// <summary>
		/// Defines a 'less than or equal' validator on the current rule builder using a lambda expression. 
		/// The validation will succeed if the property value is less than or equal to the specified value.
		/// The validation will fail if the property value is greater than the specified value.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="expression">The value being compared</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, Nullable<TProperty>> LessThanOrEqualTo<T, TProperty>(
			this IRuleBuilder<T, Nullable<TProperty>> ruleBuilder, Expression<Func<T, TProperty>> expression)
			where TProperty : struct, IComparable<TProperty>, IComparable
		{
			var func = expression.Compile();

			return ruleBuilder.SetValidator(new LessThanOrEqualValidator(func.CoerceToNonGeneric(), expression.GetMember()));
		}

    /// <summary>
    /// Defines a 'less than or equal' validator on the current rule builder using a lambda expression. 
    /// The validation will succeed if the property value is less than or equal to the specified value.
    /// The validation will fail if the property value is greater than the specified value.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="expression">The value being compared</param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, TProperty?> LessThanOrEqualTo<T, TProperty>(
      this IRuleBuilder<T, TProperty?> ruleBuilder, Expression<Func<T, TProperty?>> expression)
      where TProperty : struct, IComparable<TProperty>, IComparable {
      var func = expression.Compile();

      return ruleBuilder.SetValidator(new LessThanOrEqualValidator(func.CoerceToNonGeneric(), expression.GetMember()));
    }

		/// <summary>
		/// Defines a 'less than' validator on the current rule builder using a lambda expression. 
		/// The validation will succeed if the property value is greater than the specified value.
		/// The validation will fail if the property value is less than or equal to the specified value.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="expression">The value being compared</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> GreaterThan<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder,
		                                                                          Expression<Func<T, TProperty>> expression)
			where TProperty : IComparable<TProperty>, IComparable {
			var func = expression.Compile();

			return ruleBuilder.SetValidator(new GreaterThanValidator(func.CoerceToNonGeneric(), expression.GetMember()));
		}


		/// <summary>
		/// Defines a 'less than' validator on the current rule builder using a lambda expression. 
		/// The validation will succeed if the property value is greater than the specified value.
		/// The validation will fail if the property value is less than or equal to the specified value.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="expression">The value being compared</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, Nullable<TProperty>> GreaterThan<T, TProperty>(this IRuleBuilder<T, Nullable<TProperty>> ruleBuilder,
																				  Expression<Func<T, TProperty>> expression)
			where TProperty : struct, IComparable<TProperty>, IComparable
		{
			var func = expression.Compile();

			return ruleBuilder.SetValidator(new GreaterThanValidator(func.CoerceToNonGeneric(), expression.GetMember()));
		}


		/// <summary>
		/// Defines a 'less than' validator on the current rule builder using a lambda expression. 
		/// The validation will succeed if the property value is greater than or equal the specified value.
		/// The validation will fail if the property value is less than the specified value.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="valueToCompare">The value being compared</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> GreaterThanOrEqualTo<T, TProperty>(
			this IRuleBuilder<T, TProperty> ruleBuilder, Expression<Func<T, TProperty>> valueToCompare)
			where TProperty : IComparable<TProperty>, IComparable {
			var func = valueToCompare.Compile();

			return ruleBuilder.SetValidator(new GreaterThanOrEqualValidator(func.CoerceToNonGeneric(), valueToCompare.GetMember()));
		}

    /// <summary>
    /// Defines a 'greater than or equal to' validator on the current rule builder using a lambda expression. 
    /// The validation will succeed if the property value is greater than or equal the specified value.
    /// The validation will fail if the property value is less than the specified value.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="valueToCompare">The value being compared</param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, TProperty?> GreaterThanOrEqualTo<T, TProperty>(this IRuleBuilder<T, TProperty?> ruleBuilder, Expression<Func<T, TProperty?>> valueToCompare) 
      where TProperty : struct, IComparable<TProperty>, IComparable
    {
      var func = valueToCompare.Compile();
      return ruleBuilder.SetValidator(new GreaterThanOrEqualValidator(func.CoerceToNonGeneric(), valueToCompare.GetMember()));
    }

    /// <summary>
    /// Defines a 'greater than or equal to' validator on the current rule builder using a lambda expression. 
    /// The validation will succeed if the property value is greater than or equal the specified value.
    /// The validation will fail if the property value is less than the specified value.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="valueToCompare">The value being compared</param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, TProperty?> GreaterThanOrEqualTo<T, TProperty>(
      this IRuleBuilder<T, TProperty?> ruleBuilder, Expression<Func<T, TProperty>> valueToCompare)
      where TProperty : struct, IComparable<TProperty>, IComparable
    {
      var func = valueToCompare.Compile();

      return ruleBuilder.SetValidator(new GreaterThanOrEqualValidator(func.CoerceToNonGeneric(), valueToCompare.GetMember()));
    }

		/// <summary>
		/// Validates certain properties of the specified instance.
		/// </summary>
		/// <param name="validator">The current validator</param>
		/// <param name="instance">The object to validate</param>
		/// <param name="propertyExpressions">Expressions to specify the properties to validate</param>
		/// <returns>A ValidationResult object containing any validation failures</returns>
		public static ValidationResult Validate<T>(this IValidator<T> validator, T instance, params Expression<Func<T, object>>[] propertyExpressions) {
			var context = new ValidationContext<T>(instance, new PropertyChain(), MemberNameValidatorSelector.FromExpressions(propertyExpressions));
			return validator.Validate(context);
		}

		/// <summary>
		/// Validates certain properties of the specified instance.
		/// </summary>
		/// <param name="instance">The object to validate</param>
		/// <param name="properties">The names of the properties to validate.</param>
		/// <returns>A ValidationResult object containing any validation failures.</returns>
		public static ValidationResult Validate<T>(this IValidator<T> validator, T instance, params string[] properties) {
			var context = new ValidationContext<T>(instance, new PropertyChain(), new MemberNameValidatorSelector(properties));
			return validator.Validate(context);
		}

		public static ValidationResult Validate<T>(this IValidator<T> validator, T instance, IValidatorSelector selector = null, string ruleSet = null) {
			if(selector != null && ruleSet != null) {
				throw new InvalidOperationException("Cannot specify both an IValidatorSelector and a RuleSet.");
			}
			
			if(selector == null) {
				selector = new DefaultValidatorSelector();
			}
			
			if(ruleSet != null) {
				var ruleSetNames = ruleSet.Split(',', ';');
				selector = new RulesetValidatorSelector(ruleSetNames);
			}

			var context = new ValidationContext<T>(instance, new PropertyChain(), selector);
			return validator.Validate(context);
		}

		/// <summary>
		/// Performs validation and then throws an exception if validation fails.
		/// </summary>
		public static void ValidateAndThrow<T>(this IValidator<T> validator, T instance) {
			var result = validator.Validate(instance);

			if(! result.IsValid) {
				throw new ValidationException(result.Errors);	
			}
		}

		/// <summary>
		/// Defines an 'inclusive between' validator on the current rule builder, but only for properties of types that implement IComparable.
		/// Validation will fail if the value of the property is outside of the specifed range. The range is inclusive.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="from">The lowest allowed value</param>
		/// <param name="to">The highest allowed value</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> InclusiveBetween<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, TProperty from, TProperty to) where TProperty : IComparable<TProperty>, IComparable {
			return ruleBuilder.SetValidator(new InclusiveBetweenValidator(from, to));
		}

		/// <summary>
		/// Defines an 'inclusive between' validator on the current rule builder, but only for properties of types that implement IComparable.
		/// Validation will fail if the value of the property is outside of the specifed range. The range is inclusive.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="from">The lowest allowed value</param>
		/// <param name="to">The highest allowed value</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, Nullable<TProperty>> InclusiveBetween<T, TProperty>(this IRuleBuilder<T, Nullable<TProperty>> ruleBuilder, TProperty from, TProperty to) where TProperty : struct, IComparable<TProperty>, IComparable {
			return ruleBuilder.SetValidator(new InclusiveBetweenValidator(from, to));
		}

		/// <summary>
		/// Defines an 'exclusive between' validator on the current rule builder, but only for properties of types that implement IComparable.
		/// Validation will fail if the value of the property is outside of the specifed range. The range is exclusive.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="from">The lowest allowed value</param>
		/// <param name="to">The highest allowed value</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> ExclusiveBetween<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, TProperty from, TProperty to) where TProperty : IComparable<TProperty>, IComparable {
			return ruleBuilder.SetValidator(new ExclusiveBetweenValidator(from, to));
		}

		/// <summary>
		/// Defines an 'exclusive between' validator on the current rule builder, but only for properties of types that implement IComparable.
		/// Validation will fail if the value of the property is outside of the specifed range. The range is exclusive.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="from">The lowest allowed value</param>
		/// <param name="to">The highest allowed value</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, Nullable<TProperty>> ExclusiveBetween<T, TProperty>(this IRuleBuilder<T, Nullable<TProperty>> ruleBuilder, TProperty from, TProperty to) where TProperty : struct, IComparable<TProperty>, IComparable {
			return ruleBuilder.SetValidator(new ExclusiveBetweenValidator(from, to));
		}

		/// <summary>
		/// Defines a credit card validator for the current rule builder that ensures that the specified string is a valid credit card number.
		/// </summary>
		public static IRuleBuilderOptions<T,string> CreditCard<T>(this IRuleBuilder<T, string> ruleBuilder) {
			return ruleBuilder.SetValidator(new CreditCardValidator());
		}
	}
}