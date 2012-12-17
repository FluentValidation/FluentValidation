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
namespace FluentValidation.Internal {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using Results;
	using Validators;

	public class CollectionPropertyRule<TProperty> : PropertyRule {
		public CollectionPropertyRule(MemberInfo member, Func<object, object> propertyFunc, LambdaExpression expression, Func<CascadeMode> cascadeModeThunk, Type typeToValidate, Type containerType) : base(member, propertyFunc, expression, cascadeModeThunk, typeToValidate, containerType) {
		}

		/// <summary>
		/// Creates a new property rule from a lambda expression.
		/// </summary>
		public new static CollectionPropertyRule<TProperty> Create<T>(Expression<Func<T, IEnumerable<TProperty>>> expression, Func<CascadeMode> cascadeModeThunk) {
			var member = expression.GetMember();
			var compiled = expression.Compile();

			return new CollectionPropertyRule<TProperty>(member, compiled.CoerceToNonGeneric(), expression, cascadeModeThunk, typeof(TProperty), typeof(T));
		}

		protected override IEnumerable<Results.ValidationFailure> InvokePropertyValidator(ValidationContext context, Validators.IPropertyValidator validator, string propertyName) {
			var propertyContext = new PropertyValidatorContext(context, this, propertyName);
			var collectionPropertyValue = propertyContext.PropertyValue as IEnumerable<TProperty>;
			var results = new List<ValidationFailure>();

			int count = 0;

			if (collectionPropertyValue != null) {
				foreach (var element in collectionPropertyValue) {
					var newContext = context.CloneForChildValidator(context.InstanceToValidate);
					newContext.PropertyChain.Add(propertyName);
					newContext.PropertyChain.AddIndexer(count++);

					var newPropertyContext = new PropertyValidatorContext(newContext, this, newContext.PropertyChain.ToString());
					newPropertyContext.PropertyValue = element;

					results.AddRange(validator.Validate(newPropertyContext));
				}
			}
			return results;
		}
	}
}