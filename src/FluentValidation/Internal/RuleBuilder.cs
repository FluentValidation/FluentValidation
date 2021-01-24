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
	using Validators;

	/// <summary>
	/// Builds a validation rule and constructs a validator.
	/// </summary>
	/// <typeparam name="T">Type of object being validated</typeparam>
	/// <typeparam name="TProperty">Type of property being validated</typeparam>
	internal class RuleBuilder<T, TProperty> : IRuleBuilderOptions<T, TProperty>, IRuleBuilderInitial<T, TProperty>, IRuleBuilderInitialCollection<T,TProperty> {
		/// <summary>
		/// The rule being created by this RuleBuilder.
		/// </summary>
		public IValidationRule<T, TProperty> Rule { get; }

		/// <summary>
		/// Parent validator
		/// </summary>
		public AbstractValidator<T> ParentValidator { get; }

		/// <summary>
		/// Creates a new instance of the <see cref="RuleBuilder{T,TProperty}">RuleBuilder</see> class.
		/// </summary>
		public RuleBuilder(IValidationRule<T, TProperty> rule, AbstractValidator<T> parent) {
			Rule = rule;
			ParentValidator = parent;
		}

		/// <summary>
		/// Sets the validator associated with the rule. Use with complex properties where an IValidator instance is already declared for the property type.
		/// </summary>
		/// <param name="validator">The validator to set</param>
		/// <param name="ruleSets"></param>
		public IRuleBuilderOptions<T, TProperty> SetValidator(IValidator<TProperty> validator, params string[] ruleSets) {
			validator.Guard("Cannot pass a null validator to SetValidator", nameof(validator));
			var adaptor = new ChildValidatorAdaptor<T,TProperty>(validator, validator.GetType()) {
				RuleSets = ruleSets
			};
			this.SetValidator(adaptor);
			return this;
		}

		/// <summary>
		/// Sets the validator associated with the rule. Use with complex properties where an IValidator instance is already declared for the property type.
		/// </summary>
		/// <param name="validatorProvider">The validator provider to set</param>
		/// <param name="ruleSets"></param>
		public IRuleBuilderOptions<T, TProperty> SetValidator<TValidator>(Func<T, TValidator> validatorProvider, params string[] ruleSets)
			where TValidator : IValidator<TProperty> {
			validatorProvider.Guard("Cannot pass a null validatorProvider to SetValidator", nameof(validatorProvider));
			this.SetValidator(new ChildValidatorAdaptor<T,TProperty>((context, _) => validatorProvider(context.InstanceToValidate), typeof (TValidator)) {
				RuleSets = ruleSets
			});
			return this;
		}

		/// <summary>
		/// Associates a validator provider with the current property rule.
		/// </summary>
		/// <param name="validatorProvider">The validator provider to use</param>
		/// <param name="ruleSets"></param>
		public IRuleBuilderOptions<T, TProperty> SetValidator<TValidator>(Func<T, TProperty, TValidator> validatorProvider, params string[] ruleSets) where TValidator : IValidator<TProperty> {
			validatorProvider.Guard("Cannot pass a null validatorProvider to SetValidator", nameof(validatorProvider));
			this.SetValidator(new ChildValidatorAdaptor<T,TProperty>((context, val) => validatorProvider(context.InstanceToValidate, val), typeof (TValidator)) {
				RuleSets = ruleSets
			});
			return this;
		}

		/// <summary>
		/// Creates a scope for declaring dependent rules.
		/// </summary>
		public IRuleBuilderOptions<T, TProperty> DependentRules(Action action) {
			var dependencyContainer = new List<IExecutableValidationRule<T>>();

			// Capture any rules added to the parent validator inside this delegate.
			using (ParentValidator.Rules.Capture(dependencyContainer.Add)) {
				action();
			}

			if (Rule.RuleSets != null && Rule.RuleSets.Length > 0) {
				foreach (var dependentRule in dependencyContainer) {
					if (dependentRule is PropertyRule<T, TProperty> propRule && propRule.RuleSets == null) {
						propRule.RuleSets = Rule.RuleSets;
					}
				}
			}

			((IExecutableValidationRule<T>) Rule).AddDependentRules(dependencyContainer);
			return this;
		}
	}

}
