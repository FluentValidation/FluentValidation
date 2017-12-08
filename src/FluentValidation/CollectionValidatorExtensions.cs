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

namespace FluentValidation {
	using System;
	using System.Collections.Generic;
	using Internal;
	using Validators;

	/// <summary>
	/// Extension methods for collection validation rules 
	/// </summary>
	public static class CollectionValidatorExtensions {
		/// <summary>
		/// Associates an instance of IValidator with the current property rule and is used to validate each item within the collection.
		/// </summary>
		/// <param name="ruleBuilder">Rule builder</param>
		/// <param name="validator">The validator to use</param>
		public static ICollectionValidatorRuleBuilder<T, TCollectionElement> SetCollectionValidator<T, TCollectionElement>(this IRuleBuilder<T, IEnumerable<TCollectionElement>> ruleBuilder, IValidator<TCollectionElement> validator) {
			var adaptor = new ChildCollectionValidatorAdaptor(validator);
			ruleBuilder.SetValidator(adaptor);
			IValidator<T> parentValidator = null;

			if (ruleBuilder is IExposesParentValidator<T> exposesParentValidator)
			{
				parentValidator = exposesParentValidator.ParentValidator;
			}

			return new CollectionValidatorRuleBuilder<T, TCollectionElement>(ruleBuilder, adaptor, parentValidator);
		}

		/// <summary>
		/// Uses a provider to instantiate a validator instance to be associated with a collection
		/// </summary>
		/// <param name="ruleBuilder"></param>
		/// <param name="validator"></param>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TCollectionElement"></typeparam>
		/// <typeparam name="TValidator"></typeparam>
		/// <returns></returns>
		public static ICollectionValidatorRuleBuilder<T, TCollectionElement> SetCollectionValidator<T, TCollectionElement, TValidator>(this IRuleBuilder<T, IEnumerable<TCollectionElement>> ruleBuilder, Func<T, TValidator> validator)
			where TValidator : IValidator<TCollectionElement> {
			var adaptor = new ChildCollectionValidatorAdaptor(parent => validator((T) parent), typeof(TValidator));
			ruleBuilder.SetValidator(adaptor);

			IValidator<T> parentValidator= null;

			if (ruleBuilder is IExposesParentValidator<T> exposesParentValidator) {
				parentValidator = exposesParentValidator.ParentValidator;
			}

			return new CollectionValidatorRuleBuilder<T, TCollectionElement>(ruleBuilder, adaptor, parentValidator);
		}

		/// <summary>
		/// Collection rule builder syntax
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TCollectionElement"></typeparam>
		public interface ICollectionValidatorRuleBuilder<T,TCollectionElement> : IRuleBuilderOptions<T, IEnumerable<TCollectionElement>> {
			/// <summary>
			/// Defines a condition to be used to determine if validation should run
			/// </summary>
			/// <param name="predicate"></param>
			/// <returns></returns>
			ICollectionValidatorRuleBuilder<T,TCollectionElement> Where(Func<TCollectionElement, bool> predicate);
		}

		private class CollectionValidatorRuleBuilder<T,TCollectionElement> : ICollectionValidatorRuleBuilder<T,TCollectionElement>, IExposesParentValidator<T> {
			IRuleBuilder<T, IEnumerable<TCollectionElement>> ruleBuilder;
			ChildCollectionValidatorAdaptor adaptor;

			public CollectionValidatorRuleBuilder(IRuleBuilder<T, IEnumerable<TCollectionElement>> ruleBuilder, ChildCollectionValidatorAdaptor adaptor, IValidator<T> parent) {
				this.ruleBuilder = ruleBuilder;
				this.adaptor = adaptor;
				ParentValidator = parent;
			}

			public IRuleBuilderOptions<T, IEnumerable<TCollectionElement>> SetValidator(IPropertyValidator validator) {
				return ruleBuilder.SetValidator(validator);
			}

			public IRuleBuilderOptions<T, IEnumerable<TCollectionElement>> SetValidator(IValidator<IEnumerable<TCollectionElement>> validator) {
				return ruleBuilder.SetValidator(validator);
			}

			public IRuleBuilderOptions<T, IEnumerable<TCollectionElement>> SetValidator<TValidator>(Func<T, TValidator> validatorProvider)
				where TValidator : IValidator<IEnumerable<TCollectionElement>> {
				return ruleBuilder.SetValidator(validatorProvider);
			}

			public IRuleBuilderOptions<T, IEnumerable<TCollectionElement>> Configure(Action<PropertyRule> configurator) {
				return ((IRuleBuilderOptions<T, IEnumerable<TCollectionElement>>)ruleBuilder).Configure(configurator);
			}

			public ICollectionValidatorRuleBuilder<T, TCollectionElement> Where(Func<TCollectionElement, bool> predicate) {
				predicate.Guard("Cannot pass null to Where.");
				adaptor.Predicate = x => predicate((TCollectionElement)x);
				return this;
			}

			public IValidator<T> ParentValidator { get; }
		}
	}
}