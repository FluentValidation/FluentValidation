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

	public class ComplexPropertyRule<T, TProperty> : IPropertyRule<T> {
		readonly IValidator<TProperty> validator;
		readonly PropertyModel<T, TProperty> model;

		public ComplexPropertyRule(IValidator<TProperty> validator, PropertyModel<T, TProperty> model) {
			this.validator = validator;
			this.model = model;
		}

		public string PropertyName { 
			get { return model.PropertyName; }
			set { model.PropertyName=value; } 
		}

		public string CustomPropertyName {
			get { return model.CustomPropertyName; }
			set { model.CustomPropertyName = value; }
		}

		public string PropertyDescription {
			get { return model.PropertyDescription; }
		}

		public MemberInfo Member {
			get { return model.Member; }
		}

		public IEnumerable<ValidationFailure> Validate(ValidationContext<T> context) {
			if(Member == null) {
				throw new InvalidOperationException(string.Format("Nested validators can only be used with Member Expressions. '{0}' is not a MemberExpression.", model.Expression));
			}

			string propertyName = BuildPropertyName(context);

			if(! context.Selector.CanExecute(this, propertyName)) {
				return Enumerable.Empty<ValidationFailure>();
			}

			var instanceToValidate = model.PropertyFunc(context.InstanceToValidate);

			if (instanceToValidate == null) {
				return Enumerable.Empty<ValidationFailure>();
			}

			var propertyChain = new PropertyChain(context.PropertyChain);
			propertyChain.Add(Member);

			//Selector should not propogate to complex properties. 
			//If this property has been included then all child properties should be included.

			var newContext = new ValidationContext<TProperty>(instanceToValidate, propertyChain, new DefaultValidatorSelector());
			var results = validator.SelectMany(x => x.Validate(newContext));

			return results;
		}

		private string BuildPropertyName(ValidationContext<T> context) {
			return context.PropertyChain.BuildPropertyName(model.PropertyName);
		}
	}
}