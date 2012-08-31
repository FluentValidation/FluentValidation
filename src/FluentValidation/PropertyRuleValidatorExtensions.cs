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
// The latest version of this file can be found at http://fluentvalidation.codeplex.com
#endregion

namespace FluentValidation {
	using System;
	using System.Linq;
	using System.Linq.Expressions;
	using Internal;
	using Validators;

	public static class PropertyRuleValidatorExtensions {
		/// <summary>
		/// Replace the first validator of this type and remove all the others.
		/// </summary>
		public static void ReplaceRule<T>(this IValidator<T> validators,
		                                  Expression<Func<T, object>> expression,
		                                  IPropertyValidator newValidator) {
			var property = expression.GetMember();
			if(property == null) throw new ArgumentException("Property could not be identified", "expression");
			var type = newValidator.GetType();

			// replace the first validator of this type, then remove the others
			bool replaced = false;
			foreach (var rule in validators.OfType<PropertyRule>()) {
				if (rule.Member == property) {
					foreach (var original in rule.Validators.Where(v => v.GetType() == type).ToArray()) {
						if (!replaced) {
							rule.ReplaceValidator(original, newValidator);
							replaced = true;
						}
						else {
							rule.RemoveValidator(original);
						}
					}
				}
			}
		}

		/// <summary>
		/// Remove all validators of the specifed type.
		/// </summary>
		public static void RemoveRule<T>(this IValidator<T> validators,
		                                 Expression<Func<T, object>> expression, Type oldValidatorType) {
			var property = expression.GetMember();
			if (property == null) throw new ArgumentException("Property could not be identified", "expression");

			foreach (var rule in validators.OfType<PropertyRule>()) {
				if (rule.Member == property) {
					foreach (var original in rule.Validators.Where(v => v.GetType() == oldValidatorType).ToArray()) {
						rule.RemoveValidator(original);
					}
				}
			}
		}

		/// <summary>
		/// Remove all validators for the given property.
		/// </summary>
		public static void ClearRules<T>(this IValidator<T> validators,
		                                 Expression<Func<T, object>> expression) {
			var property = expression.GetMember();
			if (property == null) throw new ArgumentException("Property could not be identified", "expression");

			foreach (var rule in validators.OfType<PropertyRule>()) {
				if (rule.Member == property) {
					rule.ClearValidators();
				}
			}
		}
	}
}