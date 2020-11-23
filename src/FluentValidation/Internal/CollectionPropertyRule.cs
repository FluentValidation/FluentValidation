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
	/// <typeparam name="TElement"></typeparam>
	/// <typeparam name="T"></typeparam>
	public class CollectionPropertyRule<T, TElement> : PropertyRule<T> {
		/// <summary>
		/// Initializes new instance of the CollectionPropertyRule class
		/// </summary>
		/// <param name="member"></param>
		/// <param name="propertyFunc"></param>
		/// <param name="expression"></param>
		/// <param name="cascadeModeThunk"></param>
		/// <param name="typeToValidate"></param>
		public CollectionPropertyRule(MemberInfo member, Func<T, object> propertyFunc, LambdaExpression expression, Func<CascadeMode> cascadeModeThunk, Type typeToValidate)
			: base(member, propertyFunc, expression, cascadeModeThunk, typeToValidate) {
		}

		/// <summary>
		/// Filter that should include/exclude items in the collection.
		/// </summary>
		public Func<TElement, bool> Filter { get; set; }

		/// <summary>
		/// Constructs the indexer in the property name associated with the error message.
		/// By default this is "[" + index + "]"
		/// </summary>
		public Func<object, IEnumerable<TElement>, TElement, int, string> IndexBuilder { get; set; }

		/// <summary>
		/// Creates a new property rule from a lambda expression.
		/// </summary>
		public static CollectionPropertyRule<T, TElement> Create(Expression<Func<T, IEnumerable<TElement>>> expression, Func<CascadeMode> cascadeModeThunk, bool bypassCache = false) {
			var member = expression.GetMember();
			var compiled = AccessorCache<T>.GetCachedAccessor(member, expression, bypassCache, "FV_RuleForEach");
			return new CollectionPropertyRule<T, TElement>(member, x => compiled(x), expression, cascadeModeThunk, typeof(TElement));
		}

		internal override IEnumerable<ValidationFailure> Validate(ValidationContext<T> context) {
			string displayName = GetDisplayName(context);

			if (PropertyName == null && displayName == null) {
				//No name has been specified. Assume this is a model-level rule, so we should use empty string instead.
				displayName = string.Empty;
			}

			// Construct the full name of the property, taking into account overriden property names and the chain (if we're in a nested validator)
			string propertyName = context.PropertyChain.BuildPropertyName(PropertyName ?? displayName);

			if (string.IsNullOrEmpty(propertyName)) {
				propertyName = InferPropertyName(Expression);
			}

			// Ensure that this rule is allowed to run.
			// The validatselector has the opportunity to veto this before any of the validators execute.
			if (!context.Selector.CanExecute(this, propertyName, context)) {
				return Enumerable.Empty<ValidationFailure>();
			}

			if (Condition != null) {
				if (!Condition(context)) {
					return Enumerable.Empty<ValidationFailure>();
				}
			}

			if (AsyncCondition != null) {
				if (! AsyncCondition(context, default).GetAwaiter().GetResult()) {
					return Enumerable.Empty<ValidationFailure>();
				}
			}

			var filteredValidators = GetValidatorsToExecute(context);

			if (filteredValidators.Count == 0) {
				// If there are no property validators to execute after running the conditions, bail out.
				return Enumerable.Empty<ValidationFailure>();
			}

			var cascade = CascadeMode;
			var failures = new List<ValidationFailure>();
			var collection = PropertyFunc(context.InstanceToValidate) as IEnumerable<TElement>;

			int count = 0;

			if (collection != null) {
				if (string.IsNullOrEmpty(propertyName)) {
					throw new InvalidOperationException("Could not automatically determine the property name ");
				}

				var actualContext = ValidationContext<T>.GetFromNonGenericContext(context);

				foreach (var element in collection) {
					int index = count++;

					if (Filter != null && !Filter(element)) {
						continue;
					}

					string indexer = index.ToString();
					bool useDefaultIndexFormat = true;

					if (IndexBuilder != null) {
						indexer = IndexBuilder(context.InstanceToValidate, collection, element, index);
						useDefaultIndexFormat = false;
					}

					ValidationContext<T> newContext = actualContext.CloneForChildCollectionValidator(actualContext.InstanceToValidate, preserveParentContext: true);
					newContext.PropertyChain.Add(propertyName);
					newContext.PropertyChain.AddIndexer(indexer, useDefaultIndexFormat);

					var valueToValidate = Transformer != null ? Transformer(element) : element;
					var propertyNameToValidate = newContext.PropertyChain.ToString();

					foreach (var validator in filteredValidators) {
						if (validator.ShouldValidateAsynchronously(context)) {
							failures.AddRange(InvokePropertyValidatorAsync(newContext, validator, propertyNameToValidate, valueToValidate, index, default).GetAwaiter().GetResult());
						}
						else {
							failures.AddRange(InvokePropertyValidator(newContext, validator, propertyNameToValidate, valueToValidate, index));
						}

						// If there has been at least one failure, and our CascadeMode has been set to StopOnFirst
						// then don't continue to the next rule
#pragma warning disable 618
						if (failures.Count > 0 && (cascade == CascadeMode.StopOnFirstFailure || cascade == CascadeMode.Stop)) {
							goto AfterValidate; // 🙃
						}
#pragma warning restore 618
					}
				}
			}

			AfterValidate:

			if (failures.Count > 0) {
				// Callback if there has been at least one property validator failed.
				OnFailure?.Invoke(context.InstanceToValidate, failures);
			}
			else {
				foreach (var dependentRule in DependentRules) {
					failures.AddRange(dependentRule.Validate(context));
				}
			}

			return failures;
		}

		internal override async Task<IEnumerable<ValidationFailure>> ValidateAsync(ValidationContext<T> context, CancellationToken cancellation) {
			if (!context.IsAsync()) {
				context.RootContextData["__FV_IsAsyncExecution"] = true;
			}

			string displayName = GetDisplayName(context);

			if (PropertyName == null && displayName == null) {
				//No name has been specified. Assume this is a model-level rule, so we should use empty string instead.
				displayName = string.Empty;
			}

			// Construct the full name of the property, taking into account overriden property names and the chain (if we're in a nested validator)
			string propertyName = context.PropertyChain.BuildPropertyName(PropertyName ?? displayName);

			if (string.IsNullOrEmpty(propertyName)) {
				propertyName = InferPropertyName(Expression);
			}

			// Ensure that this rule is allowed to run.
			// The validatselector has the opportunity to veto this before any of the validators execute.
			if (!context.Selector.CanExecute(this, propertyName, context)) {
				return Enumerable.Empty<ValidationFailure>();
			}

			if (Condition != null) {
				if (!Condition(context)) {
					return Enumerable.Empty<ValidationFailure>();
				}
			}

			if (AsyncCondition != null) {
				if (! AsyncCondition(context, default).GetAwaiter().GetResult()) {
					return Enumerable.Empty<ValidationFailure>();
				}
			}

			var filteredValidators = await GetValidatorsToExecuteAsync(context, cancellation);

			if (filteredValidators.Count == 0) {
				// If there are no property validators to execute after running the conditions, bail out.
				return Enumerable.Empty<ValidationFailure>();
			}

			var cascade = CascadeMode;
			var failures = new List<ValidationFailure>();
			var collection = PropertyFunc(context.InstanceToValidate) as IEnumerable<TElement>;

			int count = 0;

			if (collection != null) {
				if (string.IsNullOrEmpty(propertyName)) {
					throw new InvalidOperationException("Could not automatically determine the property name ");
				}

				var actualContext = ValidationContext<T>.GetFromNonGenericContext(context);

				foreach (var element in collection) {
					int index = count++;

					if (Filter != null && !Filter(element)) {
						continue;
					}

					string indexer = index.ToString();
					bool useDefaultIndexFormat = true;

					if (IndexBuilder != null) {
						indexer = IndexBuilder(context.InstanceToValidate, collection, element, index);
						useDefaultIndexFormat = false;
					}

					ValidationContext<T> newContext = actualContext.CloneForChildCollectionValidator(actualContext.InstanceToValidate, preserveParentContext: true);
					newContext.PropertyChain.Add(propertyName);
					newContext.PropertyChain.AddIndexer(indexer, useDefaultIndexFormat);

					var valueToValidate = Transformer != null ? Transformer(element) : element;
					var propertyNameToValidate = newContext.PropertyChain.ToString();

					foreach (var validator in filteredValidators) {
						if (validator.ShouldValidateAsynchronously(context)) {
							failures.AddRange(await InvokePropertyValidatorAsync(newContext, validator, propertyNameToValidate, valueToValidate, index, cancellation));
						}
						else {
							failures.AddRange(InvokePropertyValidator(newContext, validator, propertyNameToValidate, valueToValidate, index));
						}

						// If there has been at least one failure, and our CascadeMode has been set to StopOnFirst
						// then don't continue to the next rule
#pragma warning disable 618
						if (failures.Count > 0 && (cascade == CascadeMode.StopOnFirstFailure || cascade == CascadeMode.Stop)) {
							goto AfterValidate; // 🙃
						}
#pragma warning restore 618
					}
				}
			}

			AfterValidate:

			if (failures.Count > 0) {
				// Callback if there has been at least one property validator failed.
				OnFailure?.Invoke(context.InstanceToValidate, failures);
			}
			else {
				foreach (var dependentRule in DependentRules) {
					cancellation.ThrowIfCancellationRequested();
					failures.AddRange(await dependentRule.ValidateAsync(context, cancellation));
				}
			}

			return failures;
		}

		private List<IPropertyValidator> GetValidatorsToExecute(IValidationContext context) {
			// Loop over each validator and check if its condition allows it to run.
			// This needs to be done prior to the main loop as within a collection rule
			// validators' conditions still act upon the root object, not upon the collection property.
			// This allows the property validators to cancel their execution prior to the collection
			// being retrieved (thereby possibly avoiding NullReferenceExceptions).
			// Must call ToList so we don't modify the original collection mid-loop.
			var validators = Validators.ToList();
			int validatorIndex = 0;
			foreach (var validator in Validators) {
				if (validator.Options.HasCondition) {
					if (!validator.Options.InvokeCondition(context)) {
						validators.RemoveAt(validatorIndex);
					}
				}

				if (validator.Options.HasAsyncCondition) {
					if (!validator.Options.InvokeAsyncCondition(context, default).GetAwaiter().GetResult()) {
						validators.RemoveAt(validatorIndex);
					}
				}

				validatorIndex++;
			}

			return validators;
		}

		private async Task<List<IPropertyValidator>> GetValidatorsToExecuteAsync(IValidationContext context, CancellationToken cancellation) {
			// Loop over each validator and check if its condition allows it to run.
			// This needs to be done prior to the main loop as within a collection rule
			// validators' conditions still act upon the root object, not upon the collection property.
			// This allows the property validators to cancel their execution prior to the collection
			// being retrieved (thereby possibly avoiding NullReferenceExceptions).
			// Must call ToList so we don't modify the original collection mid-loop.
			var validators = Validators.ToList();
			int validatorIndex = 0;
			foreach (var validator in Validators) {
				if (validator.Options.HasCondition) {
					if (!validator.Options.InvokeCondition(context)) {
						validators.RemoveAt(validatorIndex);
					}
				}

				if (validator.Options.HasAsyncCondition) {
					if (!await validator.Options.InvokeAsyncCondition(context, cancellation)) {
						validators.RemoveAt(validatorIndex);
					}
				}

				validatorIndex++;
			}

			return validators;
		}

		private async Task<IEnumerable<ValidationFailure>> InvokePropertyValidatorAsync(IValidationContext context, IPropertyValidator validator, string propertyName, object value, int index, CancellationToken cancellation) {
			var newPropertyContext = new PropertyValidatorContext(context, this, propertyName, value);
			newPropertyContext.MessageFormatter.AppendArgument("CollectionIndex", index);
			return await validator.ValidateAsync(newPropertyContext, cancellation);
		}

		private IEnumerable<Results.ValidationFailure> InvokePropertyValidator(IValidationContext context, IPropertyValidator validator, string propertyName, object value, int index) {
			var newPropertyContext = new PropertyValidatorContext(context, this, propertyName, value);
			newPropertyContext.MessageFormatter.AppendArgument("CollectionIndex", index);
			return validator.Validate(newPropertyContext);
		}

		private static string InferPropertyName(LambdaExpression expression) {
			var paramExp = expression.Body as ParameterExpression;

			if (paramExp == null) {
				throw new InvalidOperationException("Could not infer property name for expression: " + expression + ". Please explicitly specify a property name by calling OverridePropertyName as part of the rule chain. Eg: RuleForEach(x => x).NotNull().OverridePropertyName(\"MyProperty\")");
			}

			return paramExp.Name;
		}
	}
}
