#region License
// Copyright 2008-2009 Jeremy Skinner (http://www.jeremyskinner.co.uk)
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
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using Results;

	public class NestedCollectionValidator<TCollection, TElement> : IValidator<TCollection>, IValidationRule<TCollection> where TCollection : IEnumerable<TElement> {
		readonly IValidator<TElement> innerValidator;

		public NestedCollectionValidator(IValidator<TElement> innerValidator) {
			this.innerValidator = innerValidator;
		}

		public IEnumerator<IValidationRule<TCollection>> GetEnumerator() {
			return RuleBuilders.GetEnumerator();
		}

		IEnumerable<IValidationRule<TCollection>> RuleBuilders {
			get { yield return this; }
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public IEnumerable<ValidationFailure> Validate(ValidationContext<TCollection> context) {
			var collection = context.InstanceToValidate;

			int count = 0;

			foreach (var element in collection) {
				var propertyChain = new PropertyChain(context.PropertyChain);
				propertyChain.AddIndexer(count++);

				//The ValidatorSelector should not be propogated downwards. 
				//If this collection property has been selected for validation, then all properties on those items should be validated.
				var newContext = new ValidationContext<TElement>(element, propertyChain, new DefaultValidatorSelector());

				var results = innerValidator.SelectMany(x => x.Validate(newContext));

				foreach (var result in results) {
					yield return result;
				}
			}
		}

		#region unimplemented IValidator members
		ValidationResult IValidator.Validate(object instance) {
			throw new NotSupportedException();
		}

		ValidationResult IValidator.Validate(object instance, IValidatorSelector selector) {
			throw new NotSupportedException();
		}

		IValidatorDescriptor IValidator.CreateDescriptor() {
			throw new NotSupportedException();
		}

		ValidationResult IValidator<TCollection>.Validate(TCollection instance) {
			throw new NotSupportedException();
		}

		ValidationResult IValidator<TCollection>.Validate(TCollection instance, IValidatorSelector selector) {
			throw new NotSupportedException();
		}
		#endregion
	}
}