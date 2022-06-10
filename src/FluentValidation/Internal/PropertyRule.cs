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
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Threading;
	using System.Threading.Tasks;

	/// <summary>
	/// Defines a rule associated with a property.
	/// </summary>
	internal class PropertyRule<T, TProperty> : RuleBase<T, TProperty, TProperty>, IValidationRuleInternal<T, TProperty> {

		public PropertyRule(MemberInfo member, Func<T, TProperty> propertyFunc, LambdaExpression expression, Func<CascadeMode> cascadeModeThunk, Type typeToValidate)
			: base(member, propertyFunc, expression, cascadeModeThunk, typeToValidate) {
		}

		/// <summary>
		/// Creates a new property rule from a lambda expression.
		/// </summary>
		public static PropertyRule<T, TProperty> Create(Expression<Func<T, TProperty>> expression, Func<CascadeMode> cascadeModeThunk, bool bypassCache = false) {
			var member = expression.GetMember();
			var compiled = AccessorCache<T>.GetCachedAccessor(member, expression, bypassCache);
			return new PropertyRule<T, TProperty>(member, x => compiled(x), expression, cascadeModeThunk, typeof(TProperty));
		}

		/// <summary>
		/// Creates a new property rule from a lambda expression.
		/// </summary>
		internal static PropertyRule<T, TProperty> Create<TOld>(Expression<Func<T, TOld>> expression, Func<TOld, TProperty> transformer, Func<CascadeMode> cascadeModeThunk, bool bypassCache = false) {
			var member = expression.GetMember();
			var compiled = AccessorCache<T>.GetCachedAccessor(member, expression, bypassCache);

			TProperty PropertyFunc(T instance)
				=> transformer(compiled(instance));

			return new PropertyRule<T, TProperty>(member, PropertyFunc, expression, cascadeModeThunk, typeof(TOld));
		}

		/// <summary>
		/// Creates a new property rule from a lambda expression.
		/// </summary>
		internal static PropertyRule<T, TProperty> Create<TOld>(Expression<Func<T, TOld>> expression, Func<T, TOld, TProperty> transformer, Func<CascadeMode> cascadeModeThunk, bool bypassCache = false) {
			var member = expression.GetMember();
			var compiled = AccessorCache<T>.GetCachedAccessor(member, expression, bypassCache);

			TProperty PropertyFunc(T instance)
				=> transformer(instance, compiled(instance));

			return new PropertyRule<T, TProperty>(member, PropertyFunc, expression, cascadeModeThunk, typeof(TOld));
		}

		/// <summary>
		/// Performs validation using a validation context and adds collected validation failures to the Context.
		/// </summary>
		/// <param name="context">Validation Context</param>
		/// <param name="useAsync">
		/// Whether asynchronous components are allowed to execute.
		/// This will be set to True when ValidateAsync is called on the root validator.
		/// This will be set to False when Validate is called on the root validator.
		/// When set to True, asynchronous components and asynchronous conditions will be executed.
		/// When set to False, an exception will be thrown if a component can only be executed asynchronously or if a component has an async condition associated with it.
		/// </param>
		/// <param name="cancellation"></param>
		public virtual async ValueTask ValidateAsync(ValidationContext<T> context, bool useAsync, CancellationToken cancellation) {
			string displayName = GetDisplayName(context);

			if (PropertyName == null && displayName == null) {
				//No name has been specified. Assume this is a model-level rule, so we should use empty string instead.
				displayName = string.Empty;
			}

			// Construct the full name of the property, taking into account overriden property names and the chain (if we're in a nested validator)
			string propertyName = context.PropertyChain.BuildPropertyName(PropertyName ?? displayName);

			// Ensure that this rule is allowed to run.
			// The validatselector has the opportunity to veto this before any of the validators execute.
			if (!context.Selector.CanExecute(this, propertyName, context)) {
				return;
			}

			if (Condition != null) {
				if (!Condition(context)) {
					return;
				}
			}

			if (AsyncCondition != null) {
				if (useAsync) {
					if (!await AsyncCondition(context, cancellation)) {
						return;
					}
				}
				else {
					throw new AsyncValidatorInvokedSynchronouslyException();
				}
			}

			var cascade = CascadeMode;
			var accessor = new Lazy<TProperty>(() => PropertyFunc(context.InstanceToValidate), LazyThreadSafetyMode.None);
			var totalFailures = context.Failures.Count;
			context.InitializeForPropertyValidator(propertyName, GetDisplayName, PropertyName);

			// Invoke each validator and collect its results.
			foreach (var component in Components) {
				cancellation.ThrowIfCancellationRequested();
				context.MessageFormatter.Reset();

				if (!component.InvokeCondition(context)) {
					continue;
				}

				if (component.HasAsyncCondition) {
					if (useAsync) {
						if (!await component.InvokeAsyncCondition(context, cancellation)) {
							continue;
						}
					}
					else {
						throw new AsyncValidatorInvokedSynchronouslyException();
					}
				}

				bool valid = await component.ValidateAsync(context, accessor.Value, useAsync, cancellation);

				if (!valid) {
					PrepareMessageFormatterForValidationError(context, accessor.Value);
					var failure = CreateValidationError(context, accessor.Value, component);
					context.Failures.Add(failure);
				}

				// If there has been at least one failure, and our CascadeMode has been set to Stop
				// then don't continue to the next rule
				if (context.Failures.Count > totalFailures && cascade == CascadeMode.Stop) {
					break;
				}
			}

			if (context.Failures.Count <= totalFailures && DependentRules != null) {
				foreach (var dependentRule in DependentRules) {
					cancellation.ThrowIfCancellationRequested();
					await dependentRule.ValidateAsync(context, useAsync, cancellation);
				}
			}
		}

		void IValidationRuleInternal<T>.AddDependentRules(IEnumerable<IValidationRuleInternal<T>> rules) {
			if (DependentRules == null) DependentRules = new();
			DependentRules.AddRange(rules);
		}
	}
}
