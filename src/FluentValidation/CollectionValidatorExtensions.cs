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
	using System.Threading;
	using System.Threading.Tasks;
	using Internal;
	using Results;
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
		[Obsolete("SetCollectionValidator is deprecated. Please use RuleForEach(..).SetValidator(..) instead.")]
		public static ICollectionValidatorRuleBuilder<T, TCollectionElement> SetCollectionValidator<T, TCollectionElement>(this IRuleBuilder<T, IEnumerable<TCollectionElement>> ruleBuilder, IValidator<TCollectionElement> validator) {
			IValidator<T> parentValidator = null;
			
			// Delegate to the RuleForEach implementation.
			var innerValidator = new WrapperHack<TCollectionElement>(validator.GetType());
			var innerRuleBuilder = (RuleBuilder<IEnumerable<TCollectionElement>, TCollectionElement>)innerValidator.RuleForEach(x => x);
			innerRuleBuilder.SetValidator(validator);
			
			//Copy across any rulesets 
			((IRuleBuilderOptions<T, IEnumerable<TCollectionElement>>)ruleBuilder).Configure(cfg => {
				innerRuleBuilder.Rule.RuleSets = cfg.RuleSets;
			});
			
			ruleBuilder.SetValidator(new ChildValidatorAdaptor(innerValidator, validator.GetType()) {
				PassThroughParentContext = true
			});

			if (ruleBuilder is IExposesParentValidator<T> exposesParentValidator) {
				parentValidator = exposesParentValidator.ParentValidator;
			}

			return new CollectionValidatorRuleBuilder<T, TCollectionElement>(ruleBuilder, innerRuleBuilder, parentValidator);
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
		[Obsolete("SetCollectionValidator is deprecated. Please use RuleForEach(..).SetValidator(..) instead.")]
		public static ICollectionValidatorRuleBuilder<T, TCollectionElement> SetCollectionValidator<T, TCollectionElement, TValidator>(this IRuleBuilder<T, IEnumerable<TCollectionElement>> ruleBuilder, Func<T, TValidator> validator)
			where TValidator : IValidator<TCollectionElement> {
			IValidator<T> parentValidator = null;

			// Delegate to the RuleForEach implementation.
			var innerValidator = new WrapperHack<TCollectionElement>(typeof(TValidator));
			var innerRuleBuilder = ((RuleBuilder<IEnumerable<TCollectionElement>, TCollectionElement>)innerValidator.RuleForEach(x => x));
			innerRuleBuilder.SetValidator(context => {

				while (context.ParentContext != null) {
					context = context.ParentContext;
				}
				
				var model = (T) context.InstanceToValidate;
				return validator(model);
			});

			//Copy across any rulesets 
			((IRuleBuilderOptions<T, IEnumerable<TCollectionElement>>)ruleBuilder).Configure(cfg => {
				innerRuleBuilder.Rule.RuleSets = cfg.RuleSets;
			});
			
			ruleBuilder.SetValidator(new ChildValidatorAdaptor(innerValidator, typeof(TValidator)) {
				PassThroughParentContext = true
			});
			
			if (ruleBuilder is IExposesParentValidator<T> exposesParentValidator) {
				parentValidator = exposesParentValidator.ParentValidator;
			}

			return new CollectionValidatorRuleBuilder<T, TCollectionElement>(ruleBuilder, innerRuleBuilder, parentValidator);
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
			IRuleBuilder<T, IEnumerable<TCollectionElement>> _ruleBuilder;
			IRuleBuilderInitialCollection<IEnumerable<TCollectionElement>, TCollectionElement> _innerRuleBuilder;

			public CollectionValidatorRuleBuilder(IRuleBuilder<T, IEnumerable<TCollectionElement>> ruleBuilder, IRuleBuilderInitialCollection<IEnumerable<TCollectionElement>, TCollectionElement> innerRuleBuilder, IValidator<T> parent) {
				_ruleBuilder = ruleBuilder;
				_innerRuleBuilder = innerRuleBuilder;
				ParentValidator = parent;
			}

			public IRuleBuilderOptions<T, IEnumerable<TCollectionElement>> SetValidator(IPropertyValidator validator) {
				return _ruleBuilder.SetValidator(validator);
			}

			public IRuleBuilderOptions<T, IEnumerable<TCollectionElement>> SetValidator(IValidator<IEnumerable<TCollectionElement>> validator, params string[] ruleSets) {
				return _ruleBuilder.SetValidator(validator, ruleSets);
			}

			public IRuleBuilderOptions<T, IEnumerable<TCollectionElement>> SetValidator<TValidator>(Func<T, TValidator> validatorProvider, params string[] ruleSets)
				where TValidator : IValidator<IEnumerable<TCollectionElement>> {
				return _ruleBuilder.SetValidator(validatorProvider, ruleSets);
			}

			public IRuleBuilderOptions<T, IEnumerable<TCollectionElement>> Configure(Action<PropertyRule> configurator) {
				return ((IRuleBuilderOptions<T, IEnumerable<TCollectionElement>>)_ruleBuilder).Configure(configurator);
			}

			public ICollectionValidatorRuleBuilder<T, TCollectionElement> Where(Func<TCollectionElement, bool> predicate) {
				_innerRuleBuilder.Where(predicate);
				return this;
			}

			public IValidator<T> ParentValidator { get; }
		}

		private class WrapperHack<T> : AbstractValidator<IEnumerable<T>>, IChildValidatorAdaptor {
			public WrapperHack(Type innerValidatorType) {
				ValidatorType = innerValidatorType;
			}
			public Type ValidatorType { get; }
		}
	}
}