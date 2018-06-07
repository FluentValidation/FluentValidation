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
	using System.Threading;
	using System.Threading.Tasks;
	using Resources;
	using Results;

	/// <summary>
	/// A custom property validator.
	/// This interface should not be implemented directly in your code as it is subject to change.
	/// Please inherit from <see cref="PropertyValidator">PropertyValidator</see> instead.
	/// </summary>
	public interface IPropertyValidator {
		[Obsolete("Subclasses should implement IShouldValidateAsync.ShouldValidateAync() and instead of using this property.")]
		bool IsAsync { get; }

		IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context);

		Task<IEnumerable<ValidationFailure>> ValidateAsync(PropertyValidatorContext context, CancellationToken cancellation);

		Func<PropertyValidatorContext, object> CustomStateProvider { get; set; }
		Severity Severity { get; set; }
		IStringSource ErrorMessageSource { get; set; }
		IStringSource ErrorCodeSource { get; set; }
	}
	
	//todo: This should ideally be merged with IPropertyValidator. I've left it as a separate interface for now to avoid a breaking change.
	/// <summary>
	/// Indicates that a validator can support asynchronous operation.
	/// </summary>
	public interface IShouldValidateAsync {
		/// <summary>
		/// Determines whether this validator should be run asynchronously or not.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		bool ShouldValidateAsync(ValidationContext context);
	}
}