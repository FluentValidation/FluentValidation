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
		/// Filter that should include/exclude items in the collection.
		/// </summary>
		public Func<TProperty, bool> Filter { get; set; }

		/// <summary>
		/// Constructs the indexer in the property name associated with the error message.
		/// By default this is "[" + index + "]"
		/// </summary>
		public Func<object, IEnumerable<TProperty>, TProperty, int, string> IndexBuilder { get; set; }

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
		protected override async Task<IEnumerable<ValidationFailure>> InvokePropertyValidatorAsync(ValidationContext context, IPropertyValidator validator, string propertyName, CancellationToken cancellation) {
			if (string.IsNullOrEmpty(propertyName)) {
				propertyName = InferPropertyName(Expression);
			}

			var propertyContext = new PropertyValidatorContext(context, this, propertyName);

			if (validator.Options.Condition != null && !validator.Options.Condition(propertyContext)) return Enumerable.Empty<ValidationFailure>();
			if (validator.Options.AsyncCondition != null && !await validator.Options.AsyncCondition(propertyContext, cancellation)) return Enumerable.Empty<ValidationFailure>();

			var collectionPropertyValue = propertyContext.PropertyValue as IEnumerable<TProperty>;

			if (collectionPropertyValue != null) {
				if (string.IsNullOrEmpty(propertyName)) {
					throw new InvalidOperationException("Could not automatically determine the property name ");
				}

				var validatorTasks = collectionPropertyValue.Select(async (v, index) => {
					if (Filter != null && !Filter(v)) {
						return Enumerable.Empty<ValidationFailure>();
					}

					string indexer = index.ToString();
					bool useDefaultIndexFormat = true;

					if (IndexBuilder != null) {
						indexer = IndexBuilder(context.InstanceToValidate, collectionPropertyValue, v, index);
						useDefaultIndexFormat = false;
					}

					var newContext = context.CloneForChildCollectionValidator(context.InstanceToValidate, preserveParentContext: true);
					newContext.PropertyChain.Add(propertyName);
					newContext.PropertyChain.AddIndexer(indexer, useDefaultIndexFormat);

					var newPropertyContext = new PropertyValidatorContext(newContext, this, newContext.PropertyChain.ToString(), v);
					newPropertyContext.MessageFormatter.AppendArgument("CollectionIndex", index);

					return await validator.ValidateAsync(newPropertyContext, cancellation);
				});

				var results = new List<ValidationFailure>();

				foreach (var task in validatorTasks) {
					var failures = await task;
					results.AddRange(failures);
				}

				return results;
			}

			return Enumerable.Empty<ValidationFailure>();
		}

		private string InferPropertyName(LambdaExpression expression) {
			var paramExp = expression.Body as ParameterExpression;

			if (paramExp == null) {
				throw new InvalidOperationException("Could not infer property name for expression: " + expression + ". Please explicitly specify a property name by calling OverridePropertyName as part of the rule chain. Eg: RuleForEach(x => x).NotNull().OverridePropertyName(\"MyProperty\")");
			}

			return paramExp.Name;
		}

		/// <summary>
		/// Invokes the validator
		/// </summary>
		/// <param name="context"></param>
		/// <param name="validator"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		protected override IEnumerable<Results.ValidationFailure> InvokePropertyValidator(ValidationContext context, Validators.IPropertyValidator validator, string propertyName) {
			if (string.IsNullOrEmpty(propertyName)) {
				propertyName = InferPropertyName(Expression);
			}

			var propertyContext = new PropertyValidatorContext(context, this, propertyName);

			if (validator.Options.Condition != null && !validator.Options.Condition(propertyContext)) return Enumerable.Empty<ValidationFailure>();

			var results = new List<ValidationFailure>();
			var collectionPropertyValue = propertyContext.PropertyValue as IEnumerable<TProperty>;

			int count = 0;

			if (collectionPropertyValue != null) {
				if (string.IsNullOrEmpty(propertyName)) {
					throw new InvalidOperationException("Could not automatically determine the property name ");
				}

				foreach (var element in collectionPropertyValue) {
					int index = count++;

					if (Filter != null && !Filter(element)) {
						continue;
					}

					string indexer = index.ToString();
					bool useDefaultIndexFormat = true;

					if (IndexBuilder != null) {
						indexer = IndexBuilder(context.InstanceToValidate, collectionPropertyValue, element, index);
						useDefaultIndexFormat = false;
					}

					var newContext = context.CloneForChildCollectionValidator(context.InstanceToValidate, preserveParentContext: true);
					newContext.PropertyChain.Add(propertyName);
					newContext.PropertyChain.AddIndexer(indexer, useDefaultIndexFormat);

					var newPropertyContext = new PropertyValidatorContext(newContext, this, newContext.PropertyChain.ToString(), element);
					newPropertyContext.MessageFormatter.AppendArgument("CollectionIndex", index);
					results.AddRange(validator.Validate(newPropertyContext));
				}
			}

			return results;
		}

	}
}
