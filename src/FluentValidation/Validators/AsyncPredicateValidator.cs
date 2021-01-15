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
	using System.Threading;
	using System.Threading.Tasks;
	using FluentValidation.Internal;
	using FluentValidation.Resources;

	/// <summary>
	/// Asynchronous custom validator
	/// </summary>
	public class AsyncPredicateValidator<T,TProperty> : PropertyValidator<T,TProperty> {
		private readonly Func<T, TProperty, PropertyValidatorContext<T,TProperty>, CancellationToken, Task<bool>> _predicate;

		public override string Name => "AsyncPredicateValidator";

		/// <summary>
		/// Creates a new AsyncPredicateValidator
		/// </summary>
		/// <param name="predicate"></param>
		public AsyncPredicateValidator(Func<T, TProperty, PropertyValidatorContext<T,TProperty>, CancellationToken, Task<bool>> predicate) {
			predicate.Guard("A predicate must be specified.", nameof(predicate));
			this._predicate = predicate;
		}

		protected override Task<bool> IsValidAsync(PropertyValidatorContext<T,TProperty> context, CancellationToken cancellation) {
			return _predicate(context.InstanceToValidate, context.PropertyValue, context, cancellation);
		}

		protected override bool IsValid(PropertyValidatorContext<T, TProperty> context) {
			//TODO: For FV 9, throw an exception by default if async validator is being executed synchronously.
			return Task.Run(() => IsValidAsync(context, new CancellationToken())).GetAwaiter().GetResult();
		}

		public override bool ShouldValidateAsynchronously(IValidationContext context) {
			return context.IsAsync() || base.ShouldValidateAsynchronously(context);
		}

		protected override string GetDefaultMessageTemplate() {
			return Localized(Name);
		}
	}
}
