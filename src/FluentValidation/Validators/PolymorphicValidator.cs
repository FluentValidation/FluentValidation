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
	using System.Threading;
	using System.Threading.Tasks;
	using Internal;
	using Results;

	/// <summary>
	/// Performs runtime checking of the value being validated, and passes validation off to a subclass validator.
	/// </summary>
	/// <typeparam name="T">Root model type</typeparam>
	/// <typeparam name="TProperty">Base type of property being validated.</typeparam>
	public class PolymorphicValidator<T, TProperty> : ChildValidatorAdaptor<T, TProperty> {
		readonly Dictionary<Type, ValidatorWrapper> _derivedValidators = new Dictionary<Type, ValidatorWrapper>();

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
			_derivedValidators[typeof(TDerived)] = new ValidatorWrapper(derivedValidator, ruleSets);
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
			_derivedValidators[typeof(TDerived)] = new ValidatorWrapper(context => validatorFactory((T)context.ParentContext.InstanceToValidate), ruleSets);
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
			_derivedValidators[typeof(TDerived)] = new ValidatorWrapper(context => validatorFactory((T)context.ParentContext.InstanceToValidate, (TDerived)context.PropertyValue), ruleSets);
			return this;
		}

		public override IValidator<TProperty> GetValidator(PropertyValidatorContext context) {
			// bail out if the current item is null
			if (context.PropertyValue == null) return null;

			if (_derivedValidators.TryGetValue(context.PropertyValue.GetType(), out var derivedValidator)) {
				return derivedValidator;
			}

			return null;
		}

		protected override IValidationContext CreateNewValidationContextForChildValidator(PropertyValidatorContext context, IValidator<TProperty> validator) {
			// Can't use the base overload as the RuleSets are per inheritance validator.

			var selector = validator is ValidatorWrapper wrapper && wrapper.RuleSets?.Length > 0 ? new RulesetValidatorSelector(wrapper.RuleSets) : null;
			var parentContext = ValidationContext<T>.GetFromNonGenericContext(context.ParentContext);
			var newContext = parentContext.CloneForChildValidator((TProperty)context.PropertyValue, true, selector);

			if(!parentContext.IsChildCollectionContext)
				newContext.PropertyChain.Add(context.Rule.PropertyName);

			return newContext;
		}


		// This validator is a pass-through to handle the type conversion.
		private class ValidatorWrapper : IValidator<TProperty> {

			private readonly IValidator _innerValidator;
			private readonly Func<ICommonContext, IValidator> _validatorFactory;
			public string[] RuleSets { get; }

			public ValidatorWrapper(IValidator innerValidator, string[] ruleSets) {
				_innerValidator = innerValidator;
				RuleSets = ruleSets;
			}

			public ValidatorWrapper(Func<ICommonContext, IValidator> validatorFactory, string[] ruleSets) {
				_validatorFactory = validatorFactory;
				RuleSets = ruleSets;
			}

			public ValidationResult Validate(IValidationContext context) {
				return _validatorFactory != null
					? _validatorFactory(context).Validate(context)
					: _innerValidator.Validate(context);
			}

			public Task<ValidationResult> ValidateAsync(IValidationContext context, CancellationToken cancellation = new CancellationToken()) {
				return _validatorFactory != null
					? _validatorFactory(context).ValidateAsync(context, cancellation)
					: _innerValidator.ValidateAsync(context, cancellation);
			}

			public IValidatorDescriptor CreateDescriptor() {
				return _innerValidator.CreateDescriptor();
			}

			public bool CanValidateInstancesOfType(Type type) {
				return _innerValidator.CanValidateInstancesOfType(type);
			}

			public ValidationResult Validate(TProperty instance) {
				// This overload should never be run.
				throw new NotSupportedException();
			}

			public Task<ValidationResult> ValidateAsync(TProperty instance, CancellationToken cancellation = new CancellationToken()) {
				// This overload should never be run.
				throw new NotSupportedException();
			}

			public CascadeMode CascadeMode {
				get => throw new NotSupportedException();
				set => throw new NotSupportedException();
			}
		}
	}
}
