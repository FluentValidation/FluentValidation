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
	using System.Threading;
	using System.Threading.Tasks;
	using Validators;

	internal class ConditionBuilder<T> {
		private TrackingCollection<IValidationRule> _rules;

		public ConditionBuilder(TrackingCollection<IValidationRule> rules) {
			_rules = rules;
		}

		/// <summary>
		/// Defines a condition that applies to several rules
		/// </summary>
		/// <param name="predicate">The condition that should apply to multiple rules</param>
		/// <param name="action">Action that encapsulates the rules.</param>
		/// <returns></returns>
		public IConditionBuilder When(Func<T, ValidationContext<T>, bool> predicate, Action action) {
			var propertyRules = new List<IValidationRule>();

			using (_rules.OnItemAdded(propertyRules.Add)) {
				action();
			}

			// Generate unique ID for this shared condition.
			var id = "_FV_Condition_" + Guid.NewGuid();

			bool Condition(ValidationContext context) {
				string cacheId = null;

				if (context.InstanceToValidate != null) {
					cacheId = id + context.InstanceToValidate.GetHashCode();

					if (context.RootContextData.TryGetValue(cacheId, out var value)) {
						if (value is bool result) {
							return result;
						}
					}
				}

				var executionResult = predicate((T)context.InstanceToValidate, (ValidationContext<T>)context);
				if (context.InstanceToValidate != null) {
					context.RootContextData[cacheId] = executionResult;
				}
				return executionResult;
			}

			// Must apply the predicate after the rule has been fully created to ensure any rules-specific conditions have already been applied.
			foreach (var rule in propertyRules) {
				rule.ApplySharedCondition(Condition);
			}

			return new ConditionOtherwiseBuilder(_rules, Condition);
		}

		/// <summary>
		/// Defines an inverse condition that applies to several rules
		/// </summary>
		/// <param name="predicate">The condition that should be applied to multiple rules</param>
		/// <param name="action">Action that encapsulates the rules</param>
		public IConditionBuilder Unless(Func<T, ValidationContext<T>, bool> predicate, Action action) {
			return When((x, context) => !predicate(x, context), action);
		}
	}

	internal class AsyncConditionBuilder<T> {
		private TrackingCollection<IValidationRule> _rules;

		public AsyncConditionBuilder(TrackingCollection<IValidationRule> rules) {
			_rules = rules;
		}

		/// <summary>
		/// Defines an asynchronous condition that applies to several rules
		/// </summary>
		/// <param name="predicate">The asynchronous condition that should apply to multiple rules</param>
		/// <param name="action">Action that encapsulates the rules.</param>
		/// <returns></returns>
		public IConditionBuilder WhenAsync(Func<T, ValidationContext<T>, CancellationToken, Task<bool>> predicate, Action action) {
			var propertyRules = new List<IValidationRule>();

			using (_rules.OnItemAdded(propertyRules.Add)) {
				action();
			}

			// Generate unique ID for this shared condition.
			var id = "_FV_AsyncCondition_" + Guid.NewGuid();

			async Task<bool> Condition(ValidationContext context, CancellationToken ct) {
				string cacheId = null;
				if (context.InstanceToValidate != null) {
					cacheId = id + context.InstanceToValidate.GetHashCode();

					if (context.RootContextData.TryGetValue(cacheId, out var value)) {
						if (value is bool result) {
							return result;
						}
					}
				}

				var executionResult = await predicate((T)context.InstanceToValidate, (ValidationContext<T>)context, ct);
				if (context.InstanceToValidate != null) {
					context.RootContextData[cacheId] = executionResult;
				}
				return executionResult;
			}

			foreach (var rule in propertyRules) {
				rule.ApplySharedAsyncCondition(Condition);
			}

			return new AsyncConditionOtherwiseBuilder(_rules, Condition);
		}

		/// <summary>
		/// Defines an inverse asynchronous condition that applies to several rules
		/// </summary>
		/// <param name="predicate">The asynchronous condition that should be applied to multiple rules</param>
		/// <param name="action">Action that encapsulates the rules</param>
		public IConditionBuilder UnlessAsync(Func<T, ValidationContext<T>, CancellationToken, Task<bool>> predicate, Action action) {
			return WhenAsync(async (x, context, ct) => !await predicate(x, context, ct), action);
		}
	}

	internal class ConditionOtherwiseBuilder : IConditionBuilder {
		private TrackingCollection<IValidationRule> _rules;
		private readonly Func<ValidationContext, bool> _condition;

		public ConditionOtherwiseBuilder(TrackingCollection<IValidationRule> rules, Func<ValidationContext, bool> condition) {
			_rules = rules;
			_condition = condition;
		}

		public virtual void Otherwise(Action action) {
			var propertyRules = new List<IValidationRule>();

			Action<IValidationRule> onRuleAdded = propertyRules.Add;

			using (_rules.OnItemAdded(onRuleAdded)) {
				action();
			}

			foreach (var rule in propertyRules) {
				rule.ApplySharedCondition(ctx => !_condition(ctx));
			}
		}
	}

	internal class AsyncConditionOtherwiseBuilder : IConditionBuilder {
		private TrackingCollection<IValidationRule> _rules;
		private readonly Func<ValidationContext, CancellationToken, Task<bool>> _condition;

		public AsyncConditionOtherwiseBuilder(TrackingCollection<IValidationRule> rules, Func<ValidationContext, CancellationToken, Task<bool>> condition) {
			_rules = rules;
			_condition = condition;
		}

		public virtual void Otherwise(Action action) {
			var propertyRules = new List<IValidationRule>();

			Action<IValidationRule> onRuleAdded = propertyRules.Add;

			using (_rules.OnItemAdded(onRuleAdded)) {
				action();
			}

			foreach (var rule in propertyRules) {
				rule.ApplySharedAsyncCondition(async (ctx, ct) => !await _condition(ctx, ct));
			}
		}
	}
}
