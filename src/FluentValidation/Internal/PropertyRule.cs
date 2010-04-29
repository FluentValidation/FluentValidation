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
	using System.Linq;
	using System.Reflection;
	using Results;
	using Validators;

	/// <summary>
	/// Defines a validation rule for a property.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TProperty"></typeparam>
	public class PropertyRule<T, TProperty> : IPropertyRule<T> {
		readonly PropertyModel<T, TProperty> model;

		public PropertyRule(PropertyModel<T, TProperty> propertyModel, IPropertyValidator validator) {
			this.model = propertyModel;
			Validator = validator;
		}

		public IPropertyValidator Validator { get; set; }

		public Action<T> OnFailure {
			get { return model.OnFailure; }
			set { model.OnFailure=value; }
		}

		public string CustomPropertyName {
			get { return model.CustomPropertyName; }
			set { model.CustomPropertyName = value; }
		}

		public string PropertyName {
			get { return model.PropertyName; }
			set { model.PropertyName = value; }
		}

		public string PropertyDescription {
			get { return model.PropertyDescription; }
		}

		public MemberInfo Member {
			get { return model.Member; }
		}

		/// <summary>
		/// Executes the validator associated with this rule.
		/// </summary>
		/// <param name="instance">The object to validate</param>
		/// <returns>Will return a <see cref="ValidationFailure">ValidationFailure</see> if validation fails, otherwise null.</returns>
		public IEnumerable<ValidationFailure> Validate(ValidationContext<T> context) {
			//Property Name cannot be determined for non-MemberExpressions. 
			if (model.PropertyName == null && model.CustomPropertyName == null) {
				throw new InvalidOperationException(string.Format("Property name could not be automatically determined for expression {0}. Please specify either a custom property name by calling 'WithName'.", model.Expression));
			}

			string propertyName = BuildPropertyName(context);

			if (context.Selector.CanExecute(this, propertyName)) {
				var validationContext = new PropertyValidatorContext(model.PropertyDescription, context.InstanceToValidate, x => model.PropertyFunc((T)x), propertyName, Member);
				validationContext.PropertyChain = context.PropertyChain;
				return Validator.Validate(validationContext);
			}

			return Enumerable.Empty<ValidationFailure>();
		}

		private string BuildPropertyName(ValidationContext<T> context) {
			return context.PropertyChain.BuildPropertyName(model.PropertyName ?? model.CustomPropertyName);
		}
	}
}