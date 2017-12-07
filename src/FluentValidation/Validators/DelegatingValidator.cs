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
		private readonly Func<object, bool> condition;
		private readonly Func<object, CancellationToken, Task<bool>> asyncCondition;
		public IPropertyValidator InnerValidator { get; private set; }

		public virtual bool IsAsync {
			get { return InnerValidator.IsAsync || asyncCondition != null; }
		}

		public DelegatingValidator(Func<object, bool> condition, IPropertyValidator innerValidator) {
			this.condition = condition;
			this.asyncCondition = null;
			InnerValidator = innerValidator;
		}

		public DelegatingValidator(Func<object, CancellationToken, Task<bool>> asyncCondition, IPropertyValidator innerValidator) {
			this.condition = _ => true;
			this.asyncCondition = asyncCondition;
			InnerValidator = innerValidator;
		}

		public IStringSource ErrorMessageSource {
			get { return InnerValidator.ErrorMessageSource; }
			set { InnerValidator.ErrorMessageSource = value; }
		}

		public IStringSource ErrorCodeSource {
			get { return InnerValidator.ErrorCodeSource; }
			set { InnerValidator.ErrorCodeSource = value; }
		}

		public IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context) {
			if (condition(context.Instance)) {
				return InnerValidator.Validate(context);
			}
			return Enumerable.Empty<ValidationFailure>();
		}

		public async  Task<IEnumerable<ValidationFailure>> ValidateAsync(PropertyValidatorContext context, CancellationToken cancellation) {
			if (!condition(context.Instance))
				return Enumerable.Empty<ValidationFailure>();

			if (asyncCondition == null)
				return await InnerValidator.ValidateAsync(context, cancellation);

			bool shouldValidate = await asyncCondition(context.Instance, cancellation);

			if (shouldValidate) {
				return await InnerValidator.ValidateAsync(context, cancellation);
			}

			return Enumerable.Empty<ValidationFailure>();
		}

		public bool SupportsStandaloneValidation {
			get { return false; }
		}

		public Func<PropertyValidatorContext, object> CustomStateProvider {
			get { return InnerValidator.CustomStateProvider; }
			set { InnerValidator.CustomStateProvider = value; }
		}

		public Severity Severity
		{
		    get { return InnerValidator.Severity; }
		    set { InnerValidator.Severity = value; }
		}

		IPropertyValidator IDelegatingValidator.InnerValidator {
			get { return InnerValidator; }
		}

		public bool CheckCondition(object instance) {
			return condition(instance);
		}
	}

	public interface IDelegatingValidator : IPropertyValidator {
		IPropertyValidator InnerValidator { get; }
		bool CheckCondition(object instance);
	}
}