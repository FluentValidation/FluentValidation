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
	using Validators;

	/// <summary>
	/// Defines a rule associated with a property.
	/// </summary>
	internal class PropertyRule<T, TProperty> : RuleBase<T, TProperty, TProperty>, IExecutableValidationRule<T> {

		public PropertyRule(MemberInfo member, Func<T, TProperty> propertyFunc, LambdaExpression expression, Func<CascadeMode> cascadeModeThunk, Type typeToValidate)
			: base(member, propertyFunc, expression, cascadeModeThunk, typeToValidate) {
		}

		/// <summary>
		/// Creates a new property rule from a lambda expression.
		/// </summary>
		public static PropertyRule<T, TProperty> Create(Expression<Func<T, TProperty>> expression) {
			return Create(expression, () => ValidatorOptions.Global.CascadeMode);
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

		void IExecutableValidationRule<T>.Validate(ValidationContext<T> context)
			=> Validate(context);

		Task IExecutableValidationRule<T>.ValidateAsync(ValidationContext<T> context, CancellationToken cancellation)
			=> ValidateAsync(context, cancellation);

		/// <summary>
		/// Performs validation using a validation context and returns a collection of Validation Failures.
		/// </summary>
		/// <param name="context">Validation Context</param>
		/// <returns>A collection of validation failures</returns>
		internal virtual void Validate(ValidationContext<T> context) {
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

			// TODO: For FV 9, throw an exception by default if synchronous validator has async condition.
			if (AsyncCondition != null) {
				if (!AsyncCondition(context, default).GetAwaiter().GetResult()) {
					return;
				}
			}

			var cascade = CascadeMode;
			var accessor = new Lazy<TProperty>(() => PropertyFunc(context.InstanceToValidate), LazyThreadSafetyMode.None);
			var totalFailures = context.Failures.Count;

			// Invoke each validator and collect its results.
			foreach (var validator in _validators) {
				if (validator.ShouldValidateAsynchronously(context)) {
					InvokePropertyValidatorAsync(context, validator, propertyName, accessor, default).GetAwaiter().GetResult();
				}
				else {
					InvokePropertyValidator(context, validator, propertyName, accessor);
				}

				// If there has been at least one failure, and our CascadeMode has been set to StopOnFirst
				// then don't continue to the next rule
#pragma warning disable 618
				if (context.Failures.Count > totalFailures && (cascade == CascadeMode.StopOnFirstFailure || cascade == CascadeMode.Stop)) {
#pragma warning restore 618
					break;
				}
			}

			if (context.Failures.Count > totalFailures) {
				// Callback if there has been at least one property validator failed.
				var failuresThisRound = context.Failures.Skip(totalFailures).ToList();
				OnFailure?.Invoke(context.InstanceToValidate, failuresThisRound);
			}
			else {
				foreach (var dependentRule in DependentRules) {
					dependentRule.Validate(context);
				}
			}
		}

		/// <summary>
		/// Performs asynchronous validation using a validation context and returns a collection of Validation Failures.
		/// </summary>
		/// <param name="context">Validation Context</param>
		/// <param name="cancellation"></param>
		/// <returns>A collection of validation failures</returns>
		internal virtual async Task ValidateAsync(ValidationContext<T> context, CancellationToken cancellation) {
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
				if (! await AsyncCondition(context, cancellation)) {
					return;
				}
			}

			var cascade = CascadeMode;
			var accessor = new Lazy<TProperty>(() => PropertyFunc(context.InstanceToValidate), LazyThreadSafetyMode.None);
			var totalFailures = context.Failures.Count;

			// Invoke each validator and collect its results.
			foreach (var validator in _validators) {
				cancellation.ThrowIfCancellationRequested();

				if (validator.ShouldValidateAsynchronously(context)) {
					await InvokePropertyValidatorAsync(context, validator, propertyName, accessor, cancellation);
				}
				else {
					InvokePropertyValidator(context, validator, propertyName, accessor);
				}

				// If there has been at least one failure, and our CascadeMode has been set to StopOnFirst
				// then don't continue to the next rule
#pragma warning disable 618
				if (context.Failures.Count > totalFailures && (cascade == CascadeMode.StopOnFirstFailure || cascade == CascadeMode.Stop)) {
#pragma warning restore 618
					break;
				}
			}

			if (context.Failures.Count > totalFailures) {
				var failuresThisRound = context.Failures.Skip(totalFailures).ToList();
				OnFailure?.Invoke(context.InstanceToValidate, failuresThisRound);
			}
			else {
				foreach (var dependentRule in DependentRules) {
					cancellation.ThrowIfCancellationRequested();
					await dependentRule.ValidateAsync(context, cancellation);
				}
			}
		}

		private async Task InvokePropertyValidatorAsync(ValidationContext<T> context, PropertyValidator<T,TProperty> validator, string propertyName, Lazy<TProperty> accessor, CancellationToken cancellation) {
			if (!validator.InvokeCondition(context)) return;
			if (!await validator.InvokeAsyncCondition(context, cancellation)) return;
			var propertyContext = new PropertyValidatorContext<T, TProperty>(context, this, propertyName, accessor);
			await validator.ValidateAsync(propertyContext, cancellation);
		}

		private protected void InvokePropertyValidator(ValidationContext<T> context, PropertyValidator<T,TProperty> validator, string propertyName, Lazy<TProperty> accessor) {
			if (!validator.InvokeCondition(context)) return;
			var propertyContext = new PropertyValidatorContext<T, TProperty>(context, this, propertyName, accessor);
			validator.Validate(propertyContext);
		}

		void IExecutableValidationRule<T>.AddDependentRules(IEnumerable<IExecutableValidationRule<T>> rules) {
			DependentRules.AddRange(rules);
		}
	}
}
