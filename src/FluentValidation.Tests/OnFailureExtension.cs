namespace FluentValidation.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using Results;

// These methods replicate the old OnFailure/OnAnyFailure methods
// that were in FluentValidation versions older than 11.0.
// These serve as an example of how to re-implement this functionality for users
// that still want this by using the AfterExecuted callbacks.
public static class OnFailureExtension {

	public static IRuleBuilderOptions<T, TProperty> OnAnyFailure<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Action<T> onFailure) {
		return rule.Configure(cfg => {
			cfg.AfterRuleExecuted = (context, failures) => {
				if (failures.Any()) {
					onFailure(context.InstanceToValidate);
				}
			};
		});
	}

	public static IRuleBuilderOptions<T, TProperty> OnAnyFailure<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Action<T, IEnumerable<ValidationFailure>> onFailure) {
		return rule.Configure(cfg => {
			cfg.AfterRuleExecuted = (context, failures) => {
				if (failures.Any()) {
					onFailure(context.InstanceToValidate, failures);
				}
			};
		});	}

	public static IRuleBuilderOptions<T, TProperty> OnFailure<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Action<T> onFailure) {
		return rule.Configure(cfg => {
			cfg.Current.SetAfterExecuted((context, value, failure) => {
				if (failure != null) {
					onFailure(context.InstanceToValidate);
				}
			});
		});
	}

	public static IRuleBuilderOptions<T, TProperty> OnFailure<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Action<T, ValidationContext<T>, TProperty> onFailure) {
		return rule.Configure(cfg => {
			cfg.Current.SetAfterExecuted((context, value, failure) => {
				if (failure != null) {
					onFailure(context.InstanceToValidate, context, value);
				}
			});
		});
	}

	public static IRuleBuilderOptions<T, TProperty> OnFailure<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Action<T, ValidationContext<T>, TProperty, string> onFailure) {
		return rule.Configure(cfg => {
			cfg.Current.SetAfterExecuted((context, value, failure) => {
				if (failure != null) {
					onFailure(context.InstanceToValidate, context, value, failure.ErrorMessage);
				}
			});
		});
	}
}
