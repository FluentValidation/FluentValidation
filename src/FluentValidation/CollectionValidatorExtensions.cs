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
// The latest version of this file can be found at http://fluentvalidation.codeplex.com
#endregion

namespace FluentValidation {
	using System.Collections.Generic;
	using Validators;

	public static class CollectionValidatorExtensions {
		/// <summary>
		/// Associates an instance of IValidator with the current property rule and is used to validate each item within the collection.
		/// </summary>
		/// <param name="validator">The validator to use</param>
		public static IRuleBuilderOptions<T,IEnumerable<TPropertyElement>> SetCollectionValidator<T,TPropertyElement>(this IRuleBuilder<T,IEnumerable<TPropertyElement>> ruleBuilder, IValidator<TPropertyElement> validator) {
			return ruleBuilder.SetValidator(new ChildCollectionValidatorAdaptor(validator));
		}
	}
}