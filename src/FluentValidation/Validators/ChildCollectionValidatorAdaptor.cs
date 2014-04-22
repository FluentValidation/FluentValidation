// --------------------------------------------------------------------------
//  <copyright file="ChildCollectionValidatorAdaptor.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation. All rights reserved.
//  </copyright>
// --------------------------------------------------------------------------

namespace FluentValidation.Validators {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
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

		public override Task<IEnumerable<ValidationFailure>> ValidateAsync(PropertyValidatorContext context) {
			return ValidateInternal(
				context,
				items => {
					var failures = new List<ValidationFailure>();
					var tasks = items.Select(tuple => {
						var ctx = tuple.Item1;
						var validator = tuple.Item2;
						return validator.ValidateAsync(ctx).Then(res => failures.AddRange(res.Errors), runSynchronously: true);
					});
					return TaskHelpers.Iterate(tasks).Then(() => failures.AsEnumerable(), runSynchronously: true);
				},
				AsyncEmptyResult
			);
		}

		TResult ValidateInternal<TResult>(
			PropertyValidatorContext context,
			Func<IEnumerable<Tuple<ValidationContext, IValidator>>, TResult> validatorApplicator,
			TResult emptyResult
		) {
			if (context.Rule.Member == null) {
				throw new InvalidOperationException(string.Format("Nested validators can only be used with Member Expressions."));
			}

			var collection = context.PropertyValue as IEnumerable;

			if (collection == null) {
				return emptyResult;
			}

			var predicate = Predicate ?? (x => true);

			var itemsToValidate = collection
				.Cast<object>()
				.Select((item, index) => new { item, index })
				.Where(a => a.item != null && predicate(a.item))
				.Select(a => {
					var newContext = context.ParentContext.CloneForChildValidator(a.item);
					newContext.PropertyChain.Add(context.Rule.PropertyName);
					newContext.PropertyChain.AddIndexer(a.index);

					var validator = childValidatorProvider(context.Instance);

					return Tuple.Create(newContext, validator);
				});

			return validatorApplicator(itemsToValidate);
		}
	}
}