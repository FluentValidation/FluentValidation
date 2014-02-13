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

namespace FluentValidation.Validators {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Internal;
	using Results;

	public class ChildCollectionValidatorAdaptor : NoopPropertyValidator {
		readonly Func<object, IValidator> childValidatorProvider;
        readonly Type childValidatorType;

	    public Type ChildValidatorType
	    {
	        get { return childValidatorType; }
	    }

	    public Func<object, bool> Predicate { get; set; }

		public ChildCollectionValidatorAdaptor(IValidator childValidator) {
            this.childValidatorProvider = (_) => childValidator;
		    this.childValidatorType = childValidator.GetType();
		}

        public ChildCollectionValidatorAdaptor(Func<object, IValidator> childValidatorProvider, Type childValidatorType)
	    {
            this.childValidatorProvider = childValidatorProvider;
            this.childValidatorType = childValidatorType;
	    }

	    public override IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context) {
			if (context.Rule.Member == null) {
				throw new InvalidOperationException(string.Format("Nested validators can only be used with Member Expressions."));
			}

			var collection = context.PropertyValue as IEnumerable;

			if (collection == null) {
				yield break;
			}

			int count = 0;
			
			var predicate = Predicate ?? (x => true);

			foreach (var element in collection) {

				if(element == null || !(predicate(element))) {
					// If an element in the validator is null then we want to skip it to prevent NullReferenceExceptions in the child validator.
					// We still need to update the counter to ensure the indexes are correct.
					count++;
					continue;
				}

				var newContext = context.ParentContext.CloneForChildValidator(element);
				newContext.PropertyChain.Add(context.Rule.PropertyName);
				newContext.PropertyChain.AddIndexer(count++);

			    var validator = childValidatorProvider(context.Instance);
                var results = validator.Validate(newContext).Errors;

				foreach (var result in results) {
					yield return result;
				}
			}
		}
	}
}