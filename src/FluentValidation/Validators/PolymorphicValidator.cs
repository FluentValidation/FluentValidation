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

namespace FluentValidation.Validators {
	using System;
	using System.Collections.Generic;
	using Internal;

	/// <summary>
	/// Performs runtime checking of the value being validated, and passes validation off to a subclass validator.
	/// </summary>
	/// <typeparam name="T">Root model type</typeparam>
	/// <typeparam name="TProperty">Base type of property being validated.</typeparam>
	public class PolymorphicValidator<T, TProperty> : ChildValidatorAdaptor<T, TProperty> {
		readonly Dictionary<Type, DerivedValidatorFactory> _derivedValidators = new();

		// Need the base constructor call, even though we're just passing null.
		public PolymorphicValidator() : base((IValidator<TProperty>) null, typeof(IValidator<TProperty>)) {
		}

		/// <summary>
		/// Adds a validator to handle a specific subclass.
		/// </summary>
		/// <param name="derivedValidator">The derived validator</param>
		/// <param name="ruleSets">Optionally specify rulesets to execute. If set, rules not in these rulesets will not be run</param>
		/// <typeparam name="TDerived"></typeparam>
		/// <returns></returns>
		public PolymorphicValidator<T, TProperty> Add<TDerived>(IValidator<TDerived> derivedValidator, params string[] ruleSets) where TDerived : TProperty {
			if (derivedValidator == null) throw new ArgumentNullException(nameof(derivedValidator));
			_derivedValidators[typeof(TDerived)] = new DerivedValidatorFactory(derivedValidator, ruleSets);
			return this;
		}

		/// <summary>
		/// Adds a validator to handle a specific subclass.
		/// </summary>
		/// <param name="validatorFactory">The derived validator</param>
		/// <typeparam name="TDerived"></typeparam>
		/// <param name="ruleSets">Optionally specify rulesets to execute. If set, rules not in these rulesets will not be run</param>
		/// <returns></returns>
		public PolymorphicValidator<T, TProperty> Add<TDerived>(Func<T, IValidator<TDerived>> validatorFactory, params string[] ruleSets) where TDerived : TProperty {
			if (validatorFactory == null) throw new ArgumentNullException(nameof(validatorFactory));
			_derivedValidators[typeof(TDerived)] = new DerivedValidatorFactory((context, _) => validatorFactory(context.InstanceToValidate), ruleSets);
			return this;
		}

		/// <summary>
		/// Adds a validator to handle a specific subclass.
		/// </summary>
		/// <param name="validatorFactory">The derived validator</param>
		/// <typeparam name="TDerived"></typeparam>
		/// <param name="ruleSets">Optionally specify rulesets to execute. If set, rules not in these rulesets will not be run</param>
		/// <returns></returns>
		public PolymorphicValidator<T, TProperty> Add<TDerived>(Func<T, TDerived, IValidator<TDerived>> validatorFactory, params string[] ruleSets) where TDerived : TProperty {
			if (validatorFactory == null) throw new ArgumentNullException(nameof(validatorFactory));
			_derivedValidators[typeof(TDerived)] = new DerivedValidatorFactory((context, value) => validatorFactory(context.InstanceToValidate, (TDerived)value), ruleSets);
			return this;
		}

		/// <summary>
		/// Adds a validator to handle a specific subclass. This method is not publicly exposed as it
		/// takes a non-generic IValidator instance which could result in a type-unsafe validation operation.
		/// It allows derived validaors more flexibility in handling type conversion. If you make use of this method, you
		/// should ensure that the validator can correctly handle the type being validated.
		/// </summary>
		/// <param name="subclassType"></param>
		/// <param name="validator"></param>
		/// <param name="ruleSets">Optionally specify rulesets to execute. If set, rules not in these rulesets will not be run</param>
		/// <returns></returns>
		protected PolymorphicValidator<T, TProperty> Add(Type subclassType, IValidator validator, params string[] ruleSets) {
			if (subclassType == null) throw new ArgumentNullException(nameof(subclassType));
			if (validator == null) throw new ArgumentNullException(nameof(validator));
			if (!validator.CanValidateInstancesOfType(subclassType)) {
				throw new InvalidOperationException($"Validator {validator.GetType().Name} can't validate instances of type {subclassType.Name}");
			}

			_derivedValidators[subclassType] = new DerivedValidatorFactory(validator, ruleSets);
			return this;
		}

		public override IValidator GetValidator(ValidationContext<T> context, TProperty value) {
			// bail out if the current item is null
			if (value == null) return null;

			if (_derivedValidators.TryGetValue(value.GetType(), out var derivedValidatorFactory)) {
				return derivedValidatorFactory.GetValidator(context, value);
			}

			return null;
		}

		private protected override IValidatorSelector GetSelector(ValidationContext<T> context, TProperty value) {
			if (_derivedValidators.TryGetValue(value.GetType(), out var derivedValidatorFactory) && derivedValidatorFactory.RuleSets is {Length: > 0}) {
				return new RulesetValidatorSelector(derivedValidatorFactory.RuleSets);
			}
			return null;
		}

		private class DerivedValidatorFactory {
			private IValidator _innerValidator;
			private readonly Func<ValidationContext<T>, TProperty, IValidator> _factory;
			public string[] RuleSets { get; }

			public DerivedValidatorFactory(IValidator innerValidator, string[] ruleSets) {
				_innerValidator = innerValidator;
				RuleSets = ruleSets;
			}

			public DerivedValidatorFactory(Func<ValidationContext<T>, TProperty, IValidator> factory, string[] ruleSets) {
				RuleSets = ruleSets;
				_factory = factory;
			}

			public IValidator GetValidator(ValidationContext<T> context, TProperty value) {
				return _factory?.Invoke(context, value) ?? _innerValidator;
			}
		}
	}
}
