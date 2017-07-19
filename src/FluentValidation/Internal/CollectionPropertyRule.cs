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
// The latest version of this file can be found at https://github.com/JeremySkinner/FluentValidation
#endregion
namespace FluentValidation.Internal {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Threading;
	using System.Threading.Tasks;
	using Results;
	using Validators;

	/// <summary>
	/// Rule definition for collection properties
	/// </summary>
	/// <typeparam name="TProperty"></typeparam>
	public class CollectionPropertyRule<TProperty> : PropertyRule {
		/// <summary>
		/// Initializes new instance of the CollectionPropertyRule class
		/// </summary>
		/// <param name="member"></param>
		/// <param name="propertyFunc"></param>
		/// <param name="expression"></param>
		/// <param name="cascadeModeThunk"></param>
		/// <param name="typeToValidate"></param>
		/// <param name="containerType"></param>
		public CollectionPropertyRule(MemberInfo member, Func<object, object> propertyFunc, LambdaExpression expression, Func<CascadeMode> cascadeModeThunk, Type typeToValidate, Type containerType) : base(member, propertyFunc, expression, cascadeModeThunk, typeToValidate, containerType) {
		}

		/// <summary>
		/// Creates a new property rule from a lambda expression.
		/// </summary>
		public static CollectionPropertyRule<TProperty> Create<T>(Expression<Func<T, IEnumerable<TProperty>>> expression, Func<CascadeMode> cascadeModeThunk) {
			var member = expression.GetMember();
			var compiled = expression.Compile();

			return new CollectionPropertyRule<TProperty>(member, compiled.CoerceToNonGeneric(), expression, cascadeModeThunk, typeof(TProperty), typeof(T));
		}

		/// <summary>
		/// Invokes the validator asynchronously
		/// </summary>
		/// <param name="context"></param>
		/// <param name="validator"></param>
		/// <param name="propertyName"></param>
		/// <param name="cancellation"></param>
		/// <returns></returns>
		protected override Task<IEnumerable<ValidationFailure>> InvokePropertyValidatorAsync(ValidationContext context, IPropertyValidator validator, string propertyName, CancellationToken cancellation) {

			var propertyContext = new PropertyValidatorContext(context, this, propertyName);
			var results = new List<ValidationFailure>();
			var delegatingValidator = validator as IDelegatingValidator;

			if (delegatingValidator == null || delegatingValidator.CheckCondition(propertyContext.Instance))
			{
				var collectionPropertyValue = propertyContext.PropertyValue as IEnumerable<TProperty>;

				if (collectionPropertyValue != null)
				{

					var validators = collectionPropertyValue.Select((v, count) => {
						var newContext = context.CloneForChildValidator(context.InstanceToValidate);
						newContext.PropertyChain.Add(propertyName);
						newContext.PropertyChain.AddIndexer(count);

						var newPropertyContext = new PropertyValidatorContext(newContext, this, newContext.PropertyChain.ToString(), v);

						return validator.ValidateAsync(newPropertyContext, cancellation)
							.Then(fs => results.AddRange(fs));
					});


					return
					TaskHelpers.Iterate(
						validators,
						cancellationToken: cancellation
					).Then(() => results.AsEnumerable(), runSynchronously: true);
				}
			}

			return TaskHelpers.FromResult(Enumerable.Empty<ValidationFailure>());
		}

		/// <summary>
		/// Invokes the validator
		/// </summary>
		/// <param name="context"></param>
		/// <param name="validator"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		protected override IEnumerable<Results.ValidationFailure> InvokePropertyValidator(ValidationContext context, Validators.IPropertyValidator validator, string propertyName) {
			var propertyContext = new PropertyValidatorContext(context, this, propertyName);
			var results = new List<ValidationFailure>();
			var delegatingValidator = validator as IDelegatingValidator;
			if (delegatingValidator == null || delegatingValidator.CheckCondition(propertyContext.Instance)) {
				var collectionPropertyValue = propertyContext.PropertyValue as IEnumerable<TProperty>;

				int count = 0;

				if (collectionPropertyValue != null) {
					foreach (var element in collectionPropertyValue) {
						var newContext = context.CloneForChildCollectionValidator(context.InstanceToValidate);
						newContext.PropertyChain.Add(propertyName);
						newContext.PropertyChain.AddIndexer(count++);

						var newPropertyContext = new PropertyValidatorContext(newContext, this, newContext.PropertyChain.ToString(), element);

						results.AddRange(validator.Validate(newPropertyContext));
					}
				}
			}
			return results;
		}
	}
}