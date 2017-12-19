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
// The latest version of this file can be found at https://github.com/jeremyskinner/FluentValidation
#endregion

namespace FluentValidation.Validators {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading;
	using System.Threading.Tasks;
	using FluentValidation.Results;

	public class ChildCollectionValidatorAdaptor : NoopPropertyValidator {
		static readonly IEnumerable<ValidationFailure> EmptyResult = Enumerable.Empty<ValidationFailure>();
		static readonly Task<IEnumerable<ValidationFailure>> AsyncEmptyResult = TaskHelpers.FromResult(Enumerable.Empty<ValidationFailure>());

		readonly Func<object, IValidator> childValidatorProvider;
		readonly Type childValidatorType;

		public override bool IsAsync {
			get { return true; }
		}

		public Type ChildValidatorType {
			get { return childValidatorType; }
		}

		public Func<object, bool> Predicate { get; set; }

		public ChildCollectionValidatorAdaptor(IValidator childValidator) {
			this.childValidatorProvider = _ => childValidator;
			this.childValidatorType = childValidator.GetType();
		}

		public ChildCollectionValidatorAdaptor(Func<object, IValidator> childValidatorProvider, Type childValidatorType) {
			this.childValidatorProvider = childValidatorProvider;
			this.childValidatorType = childValidatorType;
		}

		public override IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context) {
			return ValidateInternal(
				context,
				items => items.Select(tuple => {
					var ctx = tuple.Item1;
					var validator = tuple.Item2;
					return validator.Validate(ctx).Errors;
				}).SelectMany(errors => errors),
				EmptyResult
			);
		}

		public override Task<IEnumerable<ValidationFailure>> ValidateAsync(PropertyValidatorContext context, CancellationToken cancellation) {
			return ValidateInternal(
				context,
				items => {
					var failures = new List<ValidationFailure>();
					var tasks = items.Select(tuple => {
						var ctx = tuple.Item1;
						var validator = tuple.Item2;
						return validator.ValidateAsync(ctx, cancellation).Then(res => failures.AddRange(res.Errors), runSynchronously: true, cancellationToken: cancellation);
					});
					return TaskHelpers.Iterate(tasks, cancellation).Then(() => failures.AsEnumerable(), runSynchronously: true, cancellationToken: cancellation);
				},
				AsyncEmptyResult
			);
		}

		TResult ValidateInternal<TResult>(
			PropertyValidatorContext context,
			Func<IEnumerable<Tuple<ValidationContext, IValidator>>, TResult> validatorApplicator,
			TResult emptyResult
		) {
			var collection = context.PropertyValue as IEnumerable;

			if (collection == null) {
				return emptyResult;
			}

			var predicate = Predicate ?? (x => true);

			string propertyName = context.Rule.PropertyName;

			if (string.IsNullOrEmpty(propertyName)) {
				propertyName = InferPropertyName(context.Rule.Expression);
			}

			var itemsToValidate = collection
				.Cast<object>()
				.Select((item, index) => new { item, index })
				.Where(a => a.item != null && predicate(a.item))
				.Select(a => {
					var newContext = context.ParentContext.CloneForChildValidator(a.item);
					newContext.PropertyChain.Add(propertyName);
					newContext.PropertyChain.AddIndexer(a.item is IIndexedCollectionItem ? ((IIndexedCollectionItem)a.item).Index : a.index.ToString());

					var validator = childValidatorProvider(context.Instance);

					return Tuple.Create(newContext, validator);
				});

			return validatorApplicator(itemsToValidate);
		}

		private string InferPropertyName(LambdaExpression expression) {
			var paramExp = expression.Body as ParameterExpression;

			if (paramExp == null) {
				throw new InvalidOperationException("Could not infer property name for expression: " + expression + ". Please explicitly specify a property name by calling OverridePropertyName as part of the rule chain. Eg: RuleFor(x => x).SetCollectionValidator(new MyFooValidator()).OverridePropertyName(\"MyProperty\")");
			}

			return paramExp.Name;
		}
	}

    public interface IIndexedCollectionItem
    {
        string Index { get; }
    }
}