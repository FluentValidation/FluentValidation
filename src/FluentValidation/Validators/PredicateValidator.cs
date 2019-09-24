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
	using Internal;
	using Resources;

	public class PredicateValidator : PropertyValidator, IPredicateValidator {
        public delegate bool Predicate(object instanceToValidate, object propertyValue, PropertyValidatorContext propertyValidatorContext);

		private readonly Predicate _predicate;

		public PredicateValidator(Predicate predicate) : base(new LanguageStringSource(nameof(PredicateValidator))) {
			predicate.Guard("A predicate must be specified.", nameof(predicate));
			this._predicate = predicate;
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			if (!_predicate(context.InstanceToValidate, context.PropertyValue, context)) {
				return false;
			}

			return true;
		}
	}

	public interface IPredicateValidator : IPropertyValidator { }
}