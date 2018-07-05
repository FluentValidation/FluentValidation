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

namespace FluentValidation {
	using System;

	/// <summary>
	/// Base class for object validators.
	/// </summary>
	/// <typeparam name="T">The type of the object being validated</typeparam>
	[Obsolete("AbstractValidator is deprecated in FluentValidation 8. Please inherit from ValidatorBase<T> instead, and build your rules in the Rules method. For more information about upgrading to FluentValidation 8 please see https://fluentvalidation.net/upgrading-to-8")]
	public abstract class AbstractValidator<T> : ValidatorBase<T> {
		protected sealed override void Rules() {}

		protected AbstractValidator() : base(cacheEnabled: false) {
		}
	}
}
