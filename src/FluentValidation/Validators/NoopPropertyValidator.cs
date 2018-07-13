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

	public abstract class NoopPropertyValidator : IPropertyValidator {
		public abstract IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context);

#pragma warning disable 1998
		public virtual async Task<IEnumerable<ValidationFailure>> ValidateAsync(PropertyValidatorContext context, CancellationToken cancellation) {
			return Validate(context);
		}
#pragma warning restore 1998

		public virtual bool ShouldValidateAsync(ValidationContext context) {
			return false;
		}
		
		public PropertyValidatorOptions Options { get; } = PropertyValidatorOptions.Empty;
	}
}