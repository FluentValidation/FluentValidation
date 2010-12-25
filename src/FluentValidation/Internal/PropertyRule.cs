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
	using Resources;
	using Results;
	using Validators;

	//TODO: For FluentValidation v3, remove the generic version of this class.

	public class PropertyRule<T> : PropertyRule, IValidationRule<T> {

		public PropertyRule(MemberInfo member, PropertySelector propertyFunc, Expression expression, Func<CascadeMode> cascadeModeThunk, Type typeToValidate)
			: base(member, propertyFunc, expression, cascadeModeThunk, typeToValidate, typeof(T)) {
		}

		public static PropertyRule<T> Create<TProperty>(Expression<Func<T, TProperty>> expression) {
			return Create(expression, () => ValidatorOptions.CascadeMode);
		}

		public static PropertyRule<T> Create<TProperty>(Expression<Func<T, TProperty>> expression, Func<CascadeMode> cascadeModeThunk) {
			var member = expression.GetMember();
			var compiled = expression.Compile();
			PropertySelector propertySelector = x => compiled((T)x);

			return new PropertyRule<T>(member, propertySelector, expression, cascadeModeThunk, typeof(TProperty));
		}

		public virtual IEnumerable<ValidationFailure> Validate(ValidationContext<T> context) {
			return base.Validate(context);
		}
	}

	public class PropertyRule : IValidationRule {
		readonly List<IPropertyValidator> validators = new List<IPropertyValidator>();
		Func<CascadeMode> cascadeModeThunk = () => ValidatorOptions.CascadeMode;

		public MemberInfo Member { get; private set; }
		public PropertySelector PropertyFunc { get; private set; }
		public Expression Expression { get; private set; }

		public IStringSource CustomPropertyName { get; set; }
		public string RuleSet { get; set; }

		public Action<object> OnFailure { get; set; }
		public IPropertyValidator CurrentValidator { get; private set; }
		public Type TypeToValidate { get; private set; }

		public CascadeMode CascadeMode {
			get { return cascadeModeThunk(); }
			set { cascadeModeThunk = () => value; }
		}

		public IEnumerable<IPropertyValidator> Validators {
			get { return validators.AsReadOnly(); }
		}

		public PropertyRule(MemberInfo member, PropertySelector propertyFunc, Expression expression, Func<CascadeMode> cascadeModeThunk, Type typeToValidate, Type containerType) {
			Member = member;
			PropertyFunc = propertyFunc;
			Expression = expression;
			OnFailure = x => { };
			TypeToValidate = typeToValidate;
			this.cascadeModeThunk = cascadeModeThunk;

			PropertyName = ValidatorOptions.PropertyNameResolver(containerType, member);
		}

		public void AddValidator(IPropertyValidator validator) {
			CurrentValidator = validator;
			validators.Add(validator);
		}

		public void ReplaceValidator(IPropertyValidator original, IPropertyValidator newValidator) {
			var index = validators.IndexOf(original);

			if (index > -1) {
				validators[index] = newValidator;

				if (ReferenceEquals(CurrentValidator, original)) {
					CurrentValidator = newValidator;
				}
			}
		}

		/// <summary>
		/// Returns the property name for the property being validated.
		/// Returns null if it is not a property being validated (eg a method call)
		/// </summary>
		public string PropertyName { get; set; }

		public string PropertyDescription {
			get {

				if (CustomPropertyName != null) {
					return CustomPropertyName.GetString();
				}

				return PropertyName.SplitPascalCase();
			}
		}

		public virtual IEnumerable<ValidationFailure> Validate(ValidationContext context) {
			EnsureValidPropertyName();

			// Construct the full name of the property, taking into account overriden property names and the chain (if we're in a nested validator)
			string propertyName = BuildPropertyName(context);

			// Ensure that this rule is allowed to run. 
			// The validatselector has the opportunity to veto this before any of the validators execute.
			if(! context.Selector.CanExecute(this, propertyName, context)) {
				yield break;
			}

			var cascade = cascadeModeThunk();
			bool hasAnyFailure = false;

			// Invoke each validator and collect its results.
			foreach (var validator in validators) {
				var results = InvokePropertyValidator(context, validator, propertyName);

				bool hasFailure = false;

				foreach (var result in results) {
					hasAnyFailure = true;
					hasFailure = true;
					yield return result;
				}

				// If there has been at least one failure, and our CascadeMode has been set to StopOnFirst
				// then don't continue to the next rule
				if (cascade == FluentValidation.CascadeMode.StopOnFirstFailure && hasFailure) {
					break;
				}
			}

			if (hasAnyFailure) {
				// Callback if there has been at least one property validator failed.
				OnFailure(context.InstanceToValidate);
			}
		}

		protected virtual IEnumerable<ValidationFailure> InvokePropertyValidator(ValidationContext context, IPropertyValidator validator, string propertyName) {
			var validationContext = new PropertyValidatorContext(PropertyDescription, context.InstanceToValidate, x => PropertyFunc(x), propertyName, Member);
			validationContext.PropertyChain = context.PropertyChain;
			validationContext.IsChildContext = context.IsChildContext;
			return validator.Validate(validationContext);
		}

		private void EnsureValidPropertyName() {
			if (PropertyName == null && CustomPropertyName == null) {
				throw new InvalidOperationException(string.Format("Property name could not be automatically determined for expression {0}. Please specify either a custom property name by calling 'WithName'.", Expression));
			}
		}

		private string BuildPropertyName(ValidationContext context) {
			return context.PropertyChain.BuildPropertyName(PropertyName ?? CustomPropertyName.GetString());
		}
	}
}