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
	using Results;

	/// <summary>
	/// Performs runtime checking of the value being validated, and passes validation off to a subclass validator.
	/// </summary>
	/// <typeparam name="T">Root model type</typeparam>
	/// <typeparam name="TProperty">Base type of property being validated.</typeparam>
	public class PolymorphicValidator<T, TProperty> : ChildValidatorAdaptor<T, TProperty> {
		readonly Dictionary<Type, IValidator> _derivedValidators = new Dictionary<Type, IValidator>();

		// Need the base constructor call, even though we're just passing null.
		public PolymorphicValidator() : base((IValidator<TProperty>) null, typeof(IValidator<TProperty>)) {
		}

		/// <summary>
		/// Adds a validator to handle a specific subclass.
		/// </summary>
		/// <param name="derivedValidator">The derived validator</param>
		/// <typeparam name="TDerived"></typeparam>
		/// <returns></returns>
		public PolymorphicValidator<T, TProperty> Add<TDerived>(IValidator<TDerived> derivedValidator) where TDerived : TProperty {
			if (derivedValidator == null) throw new ArgumentNullException(nameof(derivedValidator));
			_derivedValidators[typeof(TDerived)] = derivedValidator;
			return this;
		}

		public override IValidator<TProperty> GetValidator(PropertyValidatorContext context) {
			// bail out if the current item is null
			if (context.PropertyValue == null) return null;

			if (_derivedValidators.TryGetValue(context.PropertyValue.GetType(), out var derivedValidator)) {
				return new ValidatorWrapper(derivedValidator);
			}

			return null;
		}

		// This validator is a pass-through to handle the type conversion.
		private class ValidatorWrapper : IValidator<TProperty> {

			private readonly IValidator _innerValidator;

			public ValidatorWrapper(IValidator innerValidator) {
				_innerValidator = innerValidator;
			}

			public ValidationResult Validate(IValidationContext context) {
				return _innerValidator.Validate(context);
			}

			public Task<ValidationResult> ValidateAsync(IValidationContext context, CancellationToken cancellation = new CancellationToken()) {
				return _innerValidator.ValidateAsync(context, cancellation);
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
