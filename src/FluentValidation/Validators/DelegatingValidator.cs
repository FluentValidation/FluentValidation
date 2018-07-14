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
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using FluentValidation.Internal;
	using Resources;
	using Results;

	public class DelegatingValidator : IPropertyValidator, IDelegatingValidator {
		private readonly Func<PropertyValidatorContext, bool> _condition;
		private readonly Func<PropertyValidatorContext, CancellationToken, Task<bool>> _asyncCondition;
		public IPropertyValidator InnerValidator { get; private set; }

		public bool ShouldValidateAsync(ValidationContext context) {
			return InnerValidator.ShouldValidateAsync(context) || _asyncCondition != null;
		}

		public DelegatingValidator(Func<PropertyValidatorContext, bool> condition, IPropertyValidator innerValidator) {
			_condition = condition;
			_asyncCondition = null;
			InnerValidator = innerValidator;
		}

		public DelegatingValidator(Func<PropertyValidatorContext, CancellationToken, Task<bool>> asyncCondition, IPropertyValidator innerValidator) {
			_condition = _ => true;
			_asyncCondition = asyncCondition;
			InnerValidator = innerValidator;
		}
		
		public IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context) {
			if (_condition(context)) {
				return InnerValidator.Validate(context);
			}

			return Enumerable.Empty<ValidationFailure>();
		}

		public async Task<IEnumerable<ValidationFailure>> ValidateAsync(PropertyValidatorContext context, CancellationToken cancellation) {
			if (!_condition(context))
				return Enumerable.Empty<ValidationFailure>();

			if (_asyncCondition == null)
				return await InnerValidator.ValidateAsync(context, cancellation);

			bool shouldValidate = await _asyncCondition(context, cancellation);

			if (shouldValidate) {
				return await InnerValidator.ValidateAsync(context, cancellation);
			}

			return Enumerable.Empty<ValidationFailure>();
		}

		public bool SupportsStandaloneValidation => false;

		IPropertyValidator IDelegatingValidator.InnerValidator => InnerValidator;

		public bool CheckCondition(PropertyValidatorContext context) {
			return _condition(context);
		}

		public PropertyValidatorOptions Options => InnerValidator.Options;
	}

	public interface IDelegatingValidator : IPropertyValidator {
		IPropertyValidator InnerValidator { get; }
		bool CheckCondition(PropertyValidatorContext context);
	}
}