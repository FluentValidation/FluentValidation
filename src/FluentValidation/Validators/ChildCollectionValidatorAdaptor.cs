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

namespace FluentValidation.Validators {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using Internal;
	using Results;

	internal class ChildCollectionValidatorAdaptor<TCollection, TCollectionElement> : NoopPropertyValidator where TCollection : class, IEnumerable<TCollectionElement> {
		readonly IValidator<TCollectionElement> childValidator;

		public ChildCollectionValidatorAdaptor(IValidator<TCollectionElement> childValidator) {
			this.childValidator = childValidator;
		}

		public override IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context) {
			if (context.Member == null) {
				throw new InvalidOperationException(string.Format("Nested validators can only be used with Member Expressions."));
			}

			var collection = context.PropertyValue as TCollection;

			if (collection == null) {
				yield break;
			}

			int count = 0;

			foreach (var element in collection) {
				var childPropertyChain = new PropertyChain(context.PropertyChain);
				childPropertyChain.Add(context.Member);
				childPropertyChain.AddIndexer(count++);

				//The ValidatorSelector should not be propogated downwards. 
				//If this collection property has been selected for validation, then all properties on those items should be validated.
				var newContext = new ValidationContext<TCollectionElement>(element, childPropertyChain, new DefaultValidatorSelector());

				var results = childValidator.Validate(newContext).Errors;

				foreach (var result in results) {
					yield return result;
				}
			}
		}

		public override void SetErrorMessage(string message) {
			throw new NotSupportedException("Custom error messages are not supported with child validators.");
		}

		public override void SetErrorMessage(Type errorMessageResourceType, string resourceName) {
			throw new NotSupportedException("Custom error messages are not supported with child validators.");
		}

		public override void SetErrorMessage(Expression<Func<string>> resourceSelector) {
			throw new NotSupportedException("Custom error messages are not supported with child validators.");
		}
	}
}