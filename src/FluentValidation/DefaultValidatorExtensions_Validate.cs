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
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading;
	using System.Threading.Tasks;
	using Internal;
	using Results;

	public static partial class DefaultValidatorExtensions {

		/// <summary>
		/// Validates the specified instance using a combination of extra options
		/// </summary>
		/// <param name="validator">The validator</param>
		/// <param name="instance">The instance to validate</param>
		/// <param name="options">Callback to configure additional options</param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static ValidationResult Validate<T>(this IValidator<T> validator, T instance, Action<ValidationStrategy<T>> options) {
			return validator.Validate(ValidationContext<T>.CreateWithOptions(instance, options));
		}

		/// <summary>
		/// Validates the specified instance using a combination of extra options
		/// </summary>
		/// <param name="validator">The validator</param>
		/// <param name="instance">The instance to validate</param>
		/// <param name="cancellation">Cancellation token</param>
		/// <param name="options">Callback to configure additional options</param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Task<ValidationResult> ValidateAsync<T>(this IValidator<T> validator, T instance, Action<ValidationStrategy<T>> options, CancellationToken cancellation = default) {
			return validator.ValidateAsync(ValidationContext<T>.CreateWithOptions(instance, options), cancellation);
		}

		/// <summary>
		/// Performs validation and then throws an exception if validation fails.
		/// This method is a shortcut for: Validate(instance, options => options.ThrowOnFailures());
		/// </summary>
		/// <param name="validator">The validator this method is extending.</param>
		/// <param name="instance">The instance of the type we are validating.</param>
		public static void ValidateAndThrow<T>(this IValidator<T> validator, T instance) {
			validator.Validate(instance, options => {
				options.ThrowOnFailures();
			});
		}

		/// <summary>
		/// Performs validation asynchronously and then throws an exception if validation fails.
		/// This method is a shortcut for: ValidateAsync(instance, options => options.ThrowOnFailures());
		/// </summary>
		/// <param name="validator">The validator this method is extending.</param>
		/// <param name="instance">The instance of the type we are validating.</param>
		/// <param name="cancellationToken"></param>
		public static async Task ValidateAndThrowAsync<T>(this IValidator<T> validator, T instance, CancellationToken cancellationToken = default) {
			await validator.ValidateAsync(instance, options => {
				options.ThrowOnFailures();
			}, cancellationToken);
		}

		/// <summary>
		/// Validates certain properties of the specified instance.
		/// </summary>
		/// <param name="validator">The current validator</param>
		/// <param name="instance">The object to validate</param>
		/// <param name="propertyExpressions">Expressions to specify the properties to validate</param>
		/// <returns>A ValidationResult object containing any validation failures</returns>
		[Obsolete("This method will be removed in FluentValidation 10. Instead use Validate(instance, options => options.IncludeProperties(expressions))")]
		public static ValidationResult Validate<T>(this IValidator<T> validator, T instance, params Expression<Func<T, object>>[] propertyExpressions) {
			return validator.Validate(instance, options => {
				options.IncludeProperties(propertyExpressions);
			});
		}

		/// <summary>
		/// Validates certain properties of the specified instance.
		/// </summary>
		/// <param name="validator"></param>
		/// <param name="instance">The object to validate</param>
		/// <param name="properties">The names of the properties to validate.</param>
		/// <returns>A ValidationResult object containing any validation failures.</returns>
		[Obsolete("This method will be removed in FluentValidation 10. Instead use Validate(instance, options => options.IncludeProperties(properties))")]
		public static ValidationResult Validate<T>(this IValidator<T> validator, T instance, params string[] properties) {
			return validator.Validate(instance, options => {
				options.IncludeProperties(properties);
			});
		}

		/// <summary>
		/// Validates an object using either a custom validator selector or a ruleset.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="validator"></param>
		/// <param name="instance"></param>
		/// <param name="selector"></param>
		/// <param name="ruleSet"></param>
		/// <returns></returns>
		[Obsolete("This method will be removed in FluentValidation 10. Instead call Validate(instance, options => options.IncludeRuleSets(\"someRuleSet\",\"anotherRuleSet\")). Be sure to pass in separate strings rather than a comma-separated string.")]
		public static ValidationResult Validate<T>(this IValidator<T> validator, T instance, IValidatorSelector selector = null, string ruleSet = null) {

			return validator.Validate(instance, options => {
				if (selector != null) {
					options.UseCustomSelector(selector);
				}

				if (ruleSet != null) {
					options.IncludeRuleSets(RulesetValidatorSelector.LegacyRulesetSplit(ruleSet));
				}
			});
		}

		/// <summary>
		/// Validates certain properties of the specified instance asynchronously.
		/// </summary>
		/// <param name="validator">The current validator</param>
		/// <param name="instance">The object to validate</param>
		/// <param name="cancellationToken"></param>
		/// <param name="propertyExpressions">Expressions to specify the properties to validate</param>
		/// <returns>A ValidationResult object containing any validation failures</returns>
		[Obsolete("This method will be removed in FluentValidation 10. Instead use ValidateAsync(instance, options => options.IncludeProperties(expressions), cancellationToken)")]
		public static Task<ValidationResult> ValidateAsync<T>(this IValidator<T> validator, T instance, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] propertyExpressions) {
			return validator.ValidateAsync(instance, options => options.IncludeProperties(propertyExpressions), cancellationToken);
		}

		/// <summary>
		/// Validates certain properties of the specified instance asynchronously.
		/// </summary>
		/// <param name="validator"></param>
		/// <param name="instance">The object to validate</param>
		/// <param name="cancellationToken"></param>
		/// <param name="properties">The names of the properties to validate.</param>
		/// <returns>A ValidationResult object containing any validation failures.</returns>
		[Obsolete("This method will be removed in FluentValidation 10. Instead use ValidateAsync(instance, options => options.IncludeProperties(properties), cancellationToken)")]
		public static Task<ValidationResult> ValidateAsync<T>(this IValidator<T> validator, T instance, CancellationToken cancellationToken = default, params string[] properties) {
			return validator.ValidateAsync(instance, options => options.IncludeProperties(properties), cancellationToken);
		}

		/// <summary>
		/// Validates an object asynchronously using a custom validator selector or a ruleset
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="validator"></param>
		/// <param name="instance"></param>
		/// <param name="cancellationToken"></param>
		/// <param name="selector"></param>
		/// <param name="ruleSet"></param>
		/// <returns></returns>
		[Obsolete("This method will be removed in FluentValidation 10. Instead call ValidateAsync(instance, options => options.IncludeRuleSets(\"someRuleSet\",\"anotherRuleSet\"), cancellationToken). Be sure to pass in separate strings rather than a comma-separated string.")]
		public static Task<ValidationResult> ValidateAsync<T>(this IValidator<T> validator, T instance, CancellationToken cancellationToken = default, IValidatorSelector selector = null, string ruleSet = null) {
			return validator.ValidateAsync(instance, options => {
				if (selector != null) {
					options.UseCustomSelector(selector);
				}

				if (ruleSet != null) {
					options.IncludeRuleSets(RulesetValidatorSelector.LegacyRulesetSplit(ruleSet));
				}
			}, cancellationToken);
		}

		/// <summary>
		/// Performs validation and then throws an exception if validation fails.
		/// </summary>
		/// <param name="validator">The validator this method is extending.</param>
		/// <param name="instance">The instance of the type we are validating.</param>
		/// <param name="ruleSet">Optional: a ruleset when need to validate against.</param>
		[Obsolete("This method will be removed in FluentValidation 10. Instead call Validate(instance, options => options.IncludeRuleSets(\"someRuleSet\",\"anotherRuleSet\").ThrowOnFailures()). Be sure to pass in separate strings rather than a comma-separated string for rulesets.")]
		public static void ValidateAndThrow<T>(this IValidator<T> validator, T instance, string ruleSet) {
			validator.Validate(instance, options => {
				if (ruleSet != null) {
					options.IncludeRuleSets(RulesetValidatorSelector.LegacyRulesetSplit(ruleSet));
				}

				options.ThrowOnFailures();
			});
		}

		/// <summary>
		/// Performs validation asynchronously and then throws an exception if validation fails.
		/// </summary>
		/// <param name="validator">The validator this method is extending.</param>
		/// <param name="instance">The instance of the type we are validating.</param>
		/// <param name="cancellationToken"></param>
		/// <param name="ruleSet">Optional: a ruleset when need to validate against.</param>
		[Obsolete("This method will be removed in FluentValidation 10. Instead call ValidateAsync(instance, options => options.IncludeRuleSets(\"someRuleSet\",\"anotherRuleSet\").ThrowOnFailures(), cancellationToken). Be sure to pass in separate strings rather than a comma-separated string for rulesets.")]
		public static async Task ValidateAndThrowAsync<T>(this IValidator<T> validator, T instance, string ruleSet, CancellationToken cancellationToken = default) {
			await validator.ValidateAsync(instance, options => {
				if (ruleSet != null) {
					options.IncludeRuleSets(RulesetValidatorSelector.LegacyRulesetSplit(ruleSet));
				}

				options.ThrowOnFailures();
			}, cancellationToken);
		}
	}
}
