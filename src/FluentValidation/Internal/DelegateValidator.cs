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
// The latest version of this file can be found at http://www.codeplex.com/FluentValidation
#endregion

namespace FluentValidation.Internal {
	using System;
	using System.Collections.Generic;
	using Results;
	using Validators;

	/// <summary>
	/// Custom IValidationRule for performing custom logic.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DelegateValidator<T> : IValidationRule {
		private readonly Func<T, ValidationContext<T>, IEnumerable<ValidationFailure>> func;
		
		/// <summary>
		/// Rule set to which this rule belongs.
		/// </summary>
		public string RuleSet { get; set; }

		/// <summary>
		/// Creates a new DelegateValidator using the specified function to perform validation.
		/// </summary>
		public DelegateValidator(Func<T, ValidationContext<T>, IEnumerable<ValidationFailure>> func) {
			this.func = func;
		}

		/// <summary>
		/// Creates a new DelegateValidator using the specified function to perform validation.
		/// </summary>
		public DelegateValidator(Func<T, IEnumerable<ValidationFailure>> func) {
			this.func = (x, ctx) => func(x);
		}

		/// <summary>
		/// Performs validation using a validation context and returns a collection of Validation Failures.
		/// </summary>
		/// <param name="context">Validation Context</param>
		/// <returns>A collection of validation failures</returns>
		public IEnumerable<ValidationFailure> Validate(ValidationContext<T> context) {
			return func(context.InstanceToValidate, context);
		}

		/// <summary>
		/// The validators that are grouped under this rule.
		/// </summary>
		public IEnumerable<IPropertyValidator> Validators {
			get { yield break; }
		}

		/// <summary>
		/// Performs validation using a validation context and returns a collection of Validation Failures.
		/// </summary>
		/// <param name="context">Validation Context</param>
		/// <returns>A collection of validation failures</returns>
		public IEnumerable<ValidationFailure> Validate(ValidationContext context) {
			var newContext = new ValidationContext<T>((T)context.InstanceToValidate, context.PropertyChain, context.Selector);
			return Validate(newContext);
		}
	}
}