namespace FluentValidation.Validators {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Threading.Tasks;
	using Results;

	public class ChildCollectionValidatorAdaptor : NoopPropertyValidator {
		readonly Func<object, IValidator> childValidatorProvider;
		readonly Type childValidatorType;

		public Type ChildValidatorType {
			get { return childValidatorType; }
		}

		public Func<object, bool> Predicate { get; set; }

		public ChildCollectionValidatorAdaptor(IValidator childValidator) {
			this.childValidatorProvider = (_) => childValidator;
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
				Enumerable.Empty<ValidationFailure>()
			);
		}

		public override Task<IEnumerable<ValidationFailure>> ValidateAsync(PropertyValidatorContext context) {
			throw new NotImplementedException();
			//return ValidateInternal(
			//	context,
			//	items => {
			//		var result = new List<ValidationFailure>();

			//		var tasks = items.Select(tuple => {
			//			var ctx = tuple.Item1;
			//			var validator = tuple.Item2;
			//			return validator.ValidateAsync(ctx).Then(fs => result.AddRange(fs), runSynchronously: true);
			//		});

			//		TaskHelpers.Iterate(
			//			tasks,

			//		)
			//	},
			//	TaskHelpers.FromResult(Enumerable.Empty<ValidationFailure>())
			//);
		}

		private TResult ValidateInternal<TResult>(
			PropertyValidatorContext context, 
			Func<IEnumerable<Tuple<ValidationContext, IValidator>>, TResult> validatorApplicator, 
			TResult emptyResult
		)
		{
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
				.Select((item, index) => new {item, index})
				.Where(a => a.item != null && predicate(a.item))
				.Select(a => {
					var newContext = context.ParentContext.CloneForChildValidator(a.item);
					newContext.PropertyChain.Add(context.Rule.PropertyName);
					newContext.PropertyChain.AddIndexer(a.index);

					var validator = childValidatorProvider(context.Instance);

					return Tuple.Create(newContext, validator);
				});

			return validatorApplicator(itemsToValidate);
				

			//foreach (var element in collection) {
			//	if (element == null || !(predicate(element))) {
			//		// If an element in the validator is null then we want to skip it to prevent NullReferenceExceptions in the child validator.
			//		// We still need to update the counter to ensure the indexes are correct.
			//		count++;
			//		continue;
			//	}

			//	var newContext = context.ParentContext.CloneForChildValidator(element);
			//	newContext.PropertyChain.Add(context.Rule.PropertyName);
			//	newContext.PropertyChain.AddIndexer(count++);

			//	var validator = childValidatorProvider(context.Instance);

			//	var results = validator.Validate(newContext).Errors;

			//	foreach (var result in results) {
			//		yield return result;
			//	}
			//}
		}
	}
}