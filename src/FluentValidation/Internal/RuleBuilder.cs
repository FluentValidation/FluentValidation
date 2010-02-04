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
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using Results;
	using Validators;

	/// <summary>
	/// Builds a validation rule and constructs a validator.
	/// </summary>
	/// <typeparam name="T">Type of object being validated</typeparam>
	/// <typeparam name="TProperty">Type of property being validated</typeparam>
	public class RuleBuilder<T, TProperty> : IRuleBuilderOptions<T, TProperty>, IValidationRuleCollection<T>, IRuleBuilderInitial<T, TProperty> {
		PropertyRule<T, TProperty> currentRule;
		readonly PropertyModel<T, TProperty> model;
		readonly List<IValidationRule<T>> rules = new List<IValidationRule<T>>();
		Func<CascadeMode> cascadeMode = () => ValidatorOptions.CascadeMode;

		public CascadeMode CascadeMode {
			get { return cascadeMode(); }
			set { cascadeMode = () => value; }
		}

		public PropertyModel<T, TProperty> Model {
			get { return model; }
		}

		/// <summary>
		/// Creates a new instance of the <see cref="RuleBuilder{T,TProperty}">RuleBuilder</see> class.
		/// </summary>
		/// <param name="expression">Property expression used to initialise the rule builder.</param>
		public RuleBuilder(Expression<Func<T, TProperty>> expression) {
			model = new PropertyModel<T, TProperty>(expression.GetMember(), expression.Compile(), expression);
		}

		/// <summary>
		/// Sets the validator associated with the rule.
		/// </summary>
		/// <param name="validator">The validator to set</param>
		/// <returns></returns>
		public IRuleBuilderOptions<T, TProperty> SetValidator(IPropertyValidator validator) {
			validator.Guard("Cannot pass a null validator to SetValidator.");
			var rule = new PropertyRule<T, TProperty>(model, validator);
			rules.Add(rule);
			currentRule = rule;
			return this;
		}

		/// <summary>
		/// Sets the validator associated with the rule. Use with complex properties where an IValidator instance is already declared for the property type.
		/// </summary>
		/// <param name="validator">The validator to set</param>
		public void SetValidator(IValidator<TProperty> validator) {
			validator.Guard("Cannot pass a null validator to SetValidator");
			var rule = new ComplexPropertyRule<T, TProperty>(validator, model);
			rules.Add(rule);
		}

		public IRuleBuilderOptions<T, TProperty> Configure(Action<ISimplePropertyRule<T>> configurator) {
			configurator(currentRule);
			return this;
		}

		public IEnumerator<IValidationRule<T>> GetEnumerator() {
			return rules.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public virtual IEnumerable<ValidationFailure> Validate(ValidationContext<T> context) {
			var cascade = cascadeMode();
			bool hasAnyFailure = false;

			foreach(var rule in rules) {
				var results = rule.Validate(context);

				bool hasFailure = false;

				foreach(var result in results) {
					hasAnyFailure=true;
					hasFailure = true;
					yield return result;
				}

				if(cascade == CascadeMode.StopOnFirstFailure && hasFailure) {
					break;
				}
			}

			if (hasAnyFailure) {
				model.OnFailure(context.InstanceToValidate);
			}
		}

		public IRuleBuilderInitial<T, TProperty> Configure(Action<RuleBuilder<T, TProperty>> configurator) {
			configurator(this);
			return this;
		}
	}
}