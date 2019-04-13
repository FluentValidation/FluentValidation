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
	using System.Threading;
	using System.Threading.Tasks;
	using FluentValidation.Internal;
	using FluentValidation.Resources;

	/// <summary>
	/// Asynchronous custom validator
	/// </summary>
	public class AsyncPredicateValidator : PropertyValidator {
		private readonly Func<object, object, PropertyValidatorContext, CancellationToken, Task<bool>> _predicate;

		/// <summary>
		/// Creates a new AsyncPredicateValidator
		/// </summary>
		/// <param name="predicate"></param>
		public AsyncPredicateValidator(Func<object, object, PropertyValidatorContext, CancellationToken, Task<bool>> predicate) : base(new LanguageStringSource(nameof(AsyncPredicateValidator))) {
			predicate.Guard("A predicate must be specified.", nameof(predicate));
			this._predicate = predicate;
		}

		protected override Task<bool> IsValidAsync(PropertyValidatorContext context, CancellationToken cancellation) {
			return _predicate(context.InstanceToValidate, context.PropertyValue, context, cancellation);
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			//TODO: For FV 9, throw an exception by default if async validator is being executed synchronously.
			return Task.Run(() => IsValidAsync(context, new CancellationToken())).GetAwaiter().GetResult();
		}

		public override bool ShouldValidateAsync(ValidationContext context) {
			return context.IsAsync();
		}
	}
}