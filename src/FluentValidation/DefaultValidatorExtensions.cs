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

namespace FluentValidation {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Text.RegularExpressions;
	using System.Threading;
	using System.Threading.Tasks;
	using Internal;
	using Results;
	using Validators;

	/// <summary>
	/// Extension methods that provide the default set of validators.
	/// </summary>
	public static partial class DefaultValidatorExtensions {
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
		/// Defines a 'null' validator on the current rule builder.
		/// Validation will fail if the property is not null.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> Null<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder) {
			return ruleBuilder.SetValidator(new NullValidator());
		}

		/// <summary>
		/// Defines a 'not empty' validator on the current rule builder.
		/// Validation will fail if the property is null, an empty string, whitespace, an empty collection or the default value for the type (for example, 0 for integers but null for nullable integers)
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> NotEmpty<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder) {
			return ruleBuilder.SetValidator(new NotEmptyValidator(default(TProperty)));
		}

		/// <summary>
		/// Defines a 'empty' validator on the current rule builder.
		/// Validation will fail if the property is not null, an empty or the default value for the type (for example, 0 for integers)
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> Empty<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder) {
			return ruleBuilder.SetValidator(new EmptyValidator(default(TProperty)));
		}

		/// <summary>
		/// Defines a length validator on the current rule builder, but only for string properties.
		/// Validation will fail if the length of the string is outside of the specified range. The range is inclusive.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, string> Length<T>(this IRuleBuilder<T, string> ruleBuilder, int min, int max) {
			return ruleBuilder.SetValidator(new LengthValidator(min, max));
		}

		/// <summary>
		/// Defines a length validator on the current rule builder, but only for string properties.
		/// Validation will fail if the length of the string is outside of the specified range. The range is inclusive.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, string> Length<T>(this IRuleBuilder<T, string> ruleBuilder, Func<T, int> min, Func<T, int> max) {
			return ruleBuilder.SetValidator(new LengthValidator(min.CoerceToNonGeneric(), max.CoerceToNonGeneric()));
		}

		/// <summary>
		/// Defines a length validator on the current rule builder, but only for string properties.
		/// Validation will fail if the length of the string is not equal to the length specified.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="exactLength"></param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, string> Length<T>(this IRuleBuilder<T, string> ruleBuilder, int exactLength) {
			return ruleBuilder.SetValidator(new ExactLengthValidator(exactLength));
		}

		/// <summary>
		/// Defines a length validator on the current rule builder, but only for string properties.
		/// Validation will fail if the length of the string is not equal to the length specified.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="exactLength"></param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, string> Length<T>(this IRuleBuilder<T, string> ruleBuilder, Func<T, int> exactLength) {
			return ruleBuilder.SetValidator(new ExactLengthValidator(exactLength.CoerceToNonGeneric()));
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
		/// Defines a length validator on the current rule builder, but only for string properties.
		/// Validation will fail if the length of the string is larger than the length specified.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="maximumLength"></param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, string> MaximumLength<T>(this IRuleBuilder<T, string> ruleBuilder, int maximumLength) {
			return ruleBuilder.SetValidator(new MaximumLengthValidator(maximumLength));
		}

		/// <summary>
		/// Defines a length validator on the current rule builder, but only for string properties.
		/// Validation will fail if the length of the string is less than the length specified.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="minimumLength"></param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, string> MinimumLength<T>(this IRuleBuilder<T, string> ruleBuilder, int minimumLength) {
			return ruleBuilder.SetValidator(new MinimumLengthValidator(minimumLength));
		}

		/// <summary>
		/// Defines a regular expression validator on the current rule builder, but only for string properties.
		/// Validation will fail if the value returned by the lambda does not match the regular expression.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="expression">The regular expression to check the value against.</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, string> Matches<T>(this IRuleBuilder<T, string> ruleBuilder, Func<T, string> expression) {
			return ruleBuilder.SetValidator(new RegularExpressionValidator(expression.CoerceToNonGeneric()));
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
		/// <param name="regex">The regular expression to use</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, string> Matches<T>(this IRuleBuilder<T, string> ruleBuilder, Func<T, Regex> regex) {
			return ruleBuilder.SetValidator(new RegularExpressionValidator(regex.CoerceToNonGeneric()));
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
		/// Validation will fail if the value returned by the lambda does not match the regular expression.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="expression">The regular expression to check the value against.</param>
		/// <param name="options">Regex options</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, string> Matches<T>(this IRuleBuilder<T, string> ruleBuilder, Func<T, string> expression, RegexOptions options) {
			return ruleBuilder.SetValidator(new RegularExpressionValidator(expression.CoerceToNonGeneric(), options));
		}

		/// <summary>
		/// Defines an email validator on the current rule builder for string properties.
		/// Validation will fail if the value returned by the lambda is not a valid email address.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="mode">The mode to use for email validation. If set to <see cref="EmailValidationMode.Net4xRegex"/>, then a regular expression will be used. This is the same regex used by <see cref="System.ComponentModel.DataAnnotations.EmailAddressAttribute"/> in .NET 4.x. If set to <see cref="EmailValidationMode.AspNetCoreCompatible"/> then this uses the simplified ASP.NET Core logic for checking an email address, which just checks for the presence of an @ sign.</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, string> EmailAddress<T>(this IRuleBuilder<T, string> ruleBuilder, EmailValidationMode mode = EmailValidationMode.AspNetCoreCompatible) {
			var validator = mode == EmailValidationMode.AspNetCoreCompatible ? new AspNetCoreCompatibleEmailValidator() : (PropertyValidator)new EmailValidator();
			return ruleBuilder.SetValidator(validator);
		}

		/// <summary>
		/// Defines a 'not equal' validator on the current rule builder.
		/// Validation will fail if the specified value is equal to the value of the property.
		/// For strings, this performs an ordinal comparison unless you specify a different comparer.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="toCompare">The value to compare</param>
		/// <param name="comparer">Equality comparer to use</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> NotEqual<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder,
																			   TProperty toCompare, IEqualityComparer comparer = null) {
			if (comparer == null && typeof(TProperty) == typeof(string)) {
				comparer = StringComparer.Ordinal;
			}
			return ruleBuilder.SetValidator(new NotEqualValidator(toCompare, comparer));
		}

		/// <summary>
		/// Defines a 'not equal' validator on the current rule builder using a lambda to specify the value.
		/// Validation will fail if the value returned by the lambda is equal to the value of the property.
		/// For strings, this performs an ordinal comparison unless you specify a different comparer.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="expression">A lambda expression to provide the comparison value</param>
		/// <param name="comparer">Equality Comparer to use</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> NotEqual<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder,
																			   Expression<Func<T, TProperty>> expression, IEqualityComparer comparer = null) {
			if (comparer == null && typeof(TProperty) == typeof(string)) {
				comparer = StringComparer.Ordinal;
			}

			var member = expression.GetMember();
			var func = AccessorCache<T>.GetCachedAccessor(member, expression);
			string comparisonPropertyName = GetDisplayName(member, expression);
			return ruleBuilder.SetValidator(new NotEqualValidator(func.CoerceToNonGeneric(), member, comparisonPropertyName, comparer));
		}

		/// <summary>
		/// Defines an 'equals' validator on the current rule builder.
		/// Validation will fail if the specified value is not equal to the value of the property.
		/// For strings, this performs an ordinal comparison unless you specify a different comparer.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="toCompare">The value to compare</param>
		/// <param name="comparer">Equality Comparer to use</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> Equal<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, TProperty toCompare, IEqualityComparer comparer = null) {
			if (comparer == null && typeof(TProperty) == typeof(string)) {
				comparer = StringComparer.Ordinal;
			}
			return ruleBuilder.SetValidator(new EqualValidator(toCompare, comparer));
		}

		/// <summary>
		/// Defines an 'equals' validator on the current rule builder using a lambda to specify the comparison value.
		/// Validation will fail if the value returned by the lambda is not equal to the value of the property.
		/// For strings, this performs an ordinal comparison unless you specify a different comparer.
		/// </summary>
		/// <typeparam name="T">The type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="expression">A lambda expression to provide the comparison value</param>
		/// <param name="comparer">Equality comparer to use</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> Equal<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Expression<Func<T, TProperty>> expression, IEqualityComparer comparer = null) {
			if (comparer == null && typeof(TProperty) == typeof(string)) {
				comparer = StringComparer.Ordinal;
			}

			var member = expression.GetMember();
			var func = AccessorCache<T>.GetCachedAccessor(member, expression);
			var name = GetDisplayName(member, expression);
			return ruleBuilder.SetValidator(new EqualValidator(func.CoerceToNonGeneric(), member, name, comparer));
		}

		/// <summary>
		/// Defines a predicate validator on the current rule builder using a lambda expression to specify the predicate.
		/// Validation will fail if the specified lambda returns false.
		/// Validation will succeed if the specified lambda returns true.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="predicate">A lambda expression specifying the predicate</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> Must<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Func<TProperty, bool> predicate) {
			predicate.Guard("Cannot pass a null predicate to Must.", nameof(predicate));
			return ruleBuilder.Must((x, val) => predicate(val));
		}

		/// <summary>
		/// Defines a predicate validator on the current rule builder using a lambda expression to specify the predicate.
		/// Validation will fail if the specified lambda returns false.
		/// Validation will succeed if the specified lambda returns true.
		/// This overload accepts the object being validated in addition to the property being validated.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="predicate">A lambda expression specifying the predicate</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> Must<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Func<T, TProperty, bool> predicate) {
			predicate.Guard("Cannot pass a null predicate to Must.", nameof(predicate));
			return ruleBuilder.Must((x, val, propertyValidatorContext) => predicate(x, val));
		}

		/// <summary>
		/// Defines a predicate validator on the current rule builder using a lambda expression to specify the predicate.
		/// Validation will fail if the specified lambda returns false.
		/// Validation will succeed if the specified lambda returns true.
		/// This overload accepts the object being validated in addition to the property being validated.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="predicate">A lambda expression specifying the predicate</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> Must<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Func<T, TProperty, PropertyValidatorContext, bool> predicate) {
			predicate.Guard("Cannot pass a null predicate to Must.", nameof(predicate));
			return ruleBuilder.SetValidator(new PredicateValidator((instance, property, propertyValidatorContext) => predicate((T) instance, (TProperty) property, propertyValidatorContext)));
		}

		/// <summary>
		/// Defines an asynchronous predicate validator on the current rule builder using a lambda expression to specify the predicate.
		/// Validation will fail if the specified lambda returns false.
		/// Validation will succeed if the specified lambda returns true.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="predicate">A lambda expression specifying the predicate</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> MustAsync<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Func<TProperty, CancellationToken, Task<bool>> predicate) {
			predicate.Guard("Cannot pass a null predicate to Must.", nameof(predicate));

			return ruleBuilder.MustAsync((x, val, ctx, cancel) => predicate(val, cancel));
		}

		/// <summary>
		/// Defines an asynchronous predicate validator on the current rule builder using a lambda expression to specify the predicate.
		/// Validation will fail if the specified lambda returns false.
		/// Validation will succeed if the specified lambda returns true.
		/// This overload accepts the object being validated in addition to the property being validated.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="predicate">A lambda expression specifying the predicate</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> MustAsync<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Func<T, TProperty, CancellationToken, Task<bool>> predicate) {
			predicate.Guard("Cannot pass a null predicate to Must.", nameof(predicate));
			return ruleBuilder.MustAsync((x, val, propertyValidatorContext, cancel) => predicate(x, val, cancel));
		}

		/// <summary>
		/// Defines an asynchronous predicate validator on the current rule builder using a lambda expression to specify the predicate.
		/// Validation will fail if the specified lambda returns false.
		/// Validation will succeed if the specified lambda returns true.
		/// This overload accepts the object being validated in addition to the property being validated.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="predicate">A lambda expression specifying the predicate</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> MustAsync<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Func<T, TProperty, PropertyValidatorContext, CancellationToken, Task<bool>> predicate) {
			predicate.Guard("Cannot pass a null predicate to Must.", nameof(predicate));
			return ruleBuilder.SetValidator(new AsyncPredicateValidator((instance, property, propertyValidatorContext, cancel) => predicate((T) instance, (TProperty) property, propertyValidatorContext, cancel)));
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
			expression.Guard("Cannot pass null to LessThan", nameof(expression));

			var member = expression.GetMember();
			var func = AccessorCache<T>.GetCachedAccessor(member, expression);
			var name = GetDisplayName(member, expression);
			return ruleBuilder.SetValidator(new LessThanValidator(func.CoerceToNonGeneric(), member, name));
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
																			   Expression<Func<T, Nullable<TProperty>>> expression)
			where TProperty : struct, IComparable<TProperty>, IComparable {
			expression.Guard("Cannot pass null to LessThan", nameof(expression));

			var member = expression.GetMember();
			var func = AccessorCache<T>.GetCachedAccessor(member, expression);
			var name = GetDisplayName(member, expression);

			return ruleBuilder.SetValidator(new LessThanValidator(func.CoerceToNonGeneric(), member, name));
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
			expression.Guard("Cannot pass null to LessThan", nameof(expression));

			var member = expression.GetMember();
			var func = AccessorCache<T>.GetCachedAccessor(member, expression);
			var name = GetDisplayName(member, expression);

			return ruleBuilder.SetValidator(new LessThanValidator(func.CoerceToNonGeneric(), member, name));
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
																						 Expression<Func<T, Nullable<TProperty>>> expression)
			where TProperty : struct, IComparable<TProperty>, IComparable {
			expression.Guard("Cannot pass null to LessThan", nameof(expression));

			var member = expression.GetMember();
			var func = AccessorCache<T>.GetCachedAccessor(member, expression);
			var name = GetDisplayName(member, expression);

			return ruleBuilder.SetValidator(new LessThanValidator(func.CoerceToNonGeneric(), member, name));
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
			var member = expression.GetMember();
			var func = AccessorCache<T>.GetCachedAccessor(member, expression);
			var name = GetDisplayName(member, expression);

			return ruleBuilder.SetValidator(new LessThanOrEqualValidator(func.CoerceToNonGeneric(), member, name));
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
			this IRuleBuilder<T, TProperty> ruleBuilder, Expression<Func<T, Nullable<TProperty>>> expression)
			where TProperty : struct, IComparable<TProperty>, IComparable {
			var member = expression.GetMember();
			var func = AccessorCache<T>.GetCachedAccessor(member, expression);
			var name = GetDisplayName(member, expression);

			return ruleBuilder.SetValidator(new LessThanOrEqualValidator(func.CoerceToNonGeneric(), member, name));
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
			where TProperty : struct, IComparable<TProperty>, IComparable {
			var member = expression.GetMember();
			var func = AccessorCache<T>.GetCachedAccessor(member, expression);
			var name = GetDisplayName(member, expression);

			return ruleBuilder.SetValidator(new LessThanOrEqualValidator(func.CoerceToNonGeneric(), member, name));
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
			var member = expression.GetMember();
			var func = AccessorCache<T>.GetCachedAccessor(member, expression);
			var name = GetDisplayName(member, expression);

			return ruleBuilder.SetValidator(new LessThanOrEqualValidator(func.CoerceToNonGeneric(), member, name));
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
			var member = expression.GetMember();
			var func = AccessorCache<T>.GetCachedAccessor(member, expression);
			var name = GetDisplayName(member, expression);

			return ruleBuilder.SetValidator(new GreaterThanValidator(func.CoerceToNonGeneric(), member, name));
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
																				  Expression<Func<T, Nullable<TProperty>>> expression)
			where TProperty : struct, IComparable<TProperty>, IComparable {
			var member = expression.GetMember();
			var func = AccessorCache<T>.GetCachedAccessor(member, expression);
			var name = GetDisplayName(member, expression);

			return ruleBuilder.SetValidator(new GreaterThanValidator(func.CoerceToNonGeneric(), member, name));
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
			where TProperty : struct, IComparable<TProperty>, IComparable {
			var member = expression.GetMember();
			var func = AccessorCache<T>.GetCachedAccessor(member, expression);
			var name = GetDisplayName(member, expression);

			return ruleBuilder.SetValidator(new GreaterThanValidator(func.CoerceToNonGeneric(), member, name));
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
																				  Expression<Func<T, Nullable<TProperty>>> expression)
			where TProperty : struct, IComparable<TProperty>, IComparable {
			var member = expression.GetMember();
			var func = AccessorCache<T>.GetCachedAccessor(member, expression);
			var name = GetDisplayName(member, expression);

			return ruleBuilder.SetValidator(new GreaterThanValidator(func.CoerceToNonGeneric(), member, name));
		}

		/// <summary>
		/// Defines a 'greater than' validator on the current rule builder using a lambda expression.
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
			var member = valueToCompare.GetMember();
			var func = AccessorCache<T>.GetCachedAccessor(member, valueToCompare);
			var name = GetDisplayName(member, valueToCompare);

			return ruleBuilder.SetValidator(new GreaterThanOrEqualValidator(func.CoerceToNonGeneric(), member, name));
		}

		/// <summary>
		/// Defines a 'greater than' validator on the current rule builder using a lambda expression.
		/// The validation will succeed if the property value is greater than or equal the specified value.
		/// The validation will fail if the property value is less than the specified value.
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="valueToCompare">The value being compared</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> GreaterThanOrEqualTo<T, TProperty>(
			this IRuleBuilder<T, TProperty> ruleBuilder, Expression<Func<T, Nullable<TProperty>>> valueToCompare)
			where TProperty : struct, IComparable<TProperty>, IComparable {
			var member = valueToCompare.GetMember();
			var func = AccessorCache<T>.GetCachedAccessor(member, valueToCompare);
			var name = GetDisplayName(member, valueToCompare);

			return ruleBuilder.SetValidator(new GreaterThanOrEqualValidator(func.CoerceToNonGeneric(), member, name));
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
		  where TProperty : struct, IComparable<TProperty>, IComparable {
			var member = valueToCompare.GetMember();
			var func = AccessorCache<T>.GetCachedAccessor(member, valueToCompare);
			var name = GetDisplayName(member, valueToCompare);

			return ruleBuilder.SetValidator(new GreaterThanOrEqualValidator(func.CoerceToNonGeneric(), member, name));
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
		  where TProperty : struct, IComparable<TProperty>, IComparable {
			var member = valueToCompare.GetMember();
			var func = AccessorCache<T>.GetCachedAccessor(member, valueToCompare);
			var name = GetDisplayName(member, valueToCompare);

			return ruleBuilder.SetValidator(new GreaterThanOrEqualValidator(func.CoerceToNonGeneric(), member, name));
		}

		/// <summary>
		/// Defines an 'inclusive between' validator on the current rule builder, but only for properties of types that implement IComparable.
		/// Validation will fail if the value of the property is outside of the specified range. The range is inclusive.
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
		/// Validation will fail if the value of the property is outside of the specified range. The range is inclusive.
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
		/// Validation will fail if the value of the property is outside of the specified range. The range is exclusive.
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
		/// Validation will fail if the value of the property is outside of the specified range. The range is exclusive.
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
		public static IRuleBuilderOptions<T, string> CreditCard<T>(this IRuleBuilder<T, string> ruleBuilder) {
			return ruleBuilder.SetValidator(new CreditCardValidator());
		}

		/// <summary>
		/// Defines a enum value validator on the current rule builder that ensures that the specific value is a valid enum value.
		/// </summary>
		/// <typeparam name="T">Type of Enum being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> IsInEnum<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder) {
			return ruleBuilder.SetValidator(new EnumValidator(typeof(TProperty)));
		}

		/// <summary>
		/// Defines a scale precision validator on the current rule builder that ensures that the specific value has a certain scale and precision
		/// </summary>
		/// <typeparam name="T">Type of object being validated</typeparam>
		/// <typeparam name="TProperty">Type of property being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="scale">Allowed scale of the value</param>
		/// <param name="precision">Allowed precision of the value</param>
		/// <param name="ignoreTrailingZeros">Whether the validator will ignore trailing zeros.</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> ScalePrecision<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, int scale, int precision, bool ignoreTrailingZeros = false) {
			return ruleBuilder.SetValidator(new ScalePrecisionValidator(scale, precision) { IgnoreTrailingZeros = ignoreTrailingZeros });
		}

		/// <summary>
		/// Defines a custom validation rule
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static IRuleBuilderInitial<T, TProperty> Custom<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Action<TProperty, CustomContext> action) {
			return (IRuleBuilderInitial<T, TProperty>) ruleBuilder.SetValidator(new CustomValidator<TProperty>(action));
		}

		/// <summary>
		/// Defines a custom validation rule
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="ruleBuilder"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static IRuleBuilderInitial<T, TProperty> CustomAsync<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Func<TProperty, CustomContext, CancellationToken, Task> action) {
			return (IRuleBuilderInitial<T, TProperty>) ruleBuilder.SetValidator(new CustomValidator<TProperty>(action));
		}

		/// <summary>
		/// Allows rules to be built against individual elements in the collection.
		/// </summary>
		/// <param name="ruleBuilder"></param>
		/// <param name="action"></param>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TElement"></typeparam>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, IEnumerable<TElement>> ForEach<T, TElement>(this IRuleBuilder<T, IEnumerable<TElement>> ruleBuilder,
			Action<IRuleBuilderInitialCollection<IEnumerable<TElement>, TElement>> action) {
			var innerValidator = new InlineValidator<IEnumerable<TElement>>();
			action(innerValidator.RuleForEach(x => x));
			return ruleBuilder.SetValidator(innerValidator);
		}

		/// <summary>
		/// Defines a enum value validator on the current rule builder that ensures that the specific value is a valid enum name.
		/// </summary>
		/// <typeparam name="T">Type of Enum being validated</typeparam>
		/// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
		/// <param name="enumType">The enum whose the string should match any name</param>
		/// <param name="caseSensitive">If the comparison between the string and the enum names should be case sensitive</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, string> IsEnumName<T>(this IRuleBuilder<T, string> ruleBuilder, Type enumType, bool caseSensitive = true) {
			return ruleBuilder.SetValidator(new StringEnumValidator(enumType, caseSensitive));
		}

		/// <summary>
		/// Defines child rules for a nested property.
		/// </summary>
		/// <param name="ruleBuilder">The rule builder.</param>
		/// <param name="action">Callback that will be invoked to build the rules.</param>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static IRuleBuilderOptions<T, TProperty> ChildRules<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Action<InlineValidator<TProperty>> action) {
			if (action == null) throw new ArgumentNullException(nameof(action));
			var validator = new InlineValidator<TProperty>();
			action(validator);
			return ruleBuilder.SetValidator(validator);
		}

		/// <summary>
		/// Defines one or more validators that can be used to validate sub-classes or implementors
		/// in an inheritance hierarchy. This is useful when the property being validated is an interface
		/// or base-class, but you want to define rules for properties of a specific subclass.
		/// </summary>
		/// <param name="ruleBuilder"></param>
		/// <param name="validatorConfiguration">Callback for setting up the inheritance validators.</param>
		/// <returns></returns>
		public static IRuleBuilderOptions<T, TProperty> SetInheritanceValidator<T,TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Action<PolymorphicValidator<T, TProperty>> validatorConfiguration) {
			if (validatorConfiguration == null) throw new ArgumentNullException(nameof(validatorConfiguration));
			var validator = new PolymorphicValidator<T, TProperty>();
			validatorConfiguration(validator);
			return ruleBuilder.SetValidator(validator);
		}

		private static string GetDisplayName<T, TProperty>(MemberInfo member, Expression<Func<T, TProperty>> expression) {
			return ValidatorOptions.Global.DisplayNameResolver(typeof(T), member, expression) ?? member?.Name.SplitPascalCase();
		}
	}
}
