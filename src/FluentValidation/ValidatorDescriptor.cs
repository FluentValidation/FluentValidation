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

namespace FluentValidation {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using Internal;
	using Validators;

	/// <summary>
	/// Used for providing metadata about a validator.
	/// </summary>
	public class ValidatorDescriptor<T> : IValidatorDescriptor {
		protected IEnumerable<IValidationRule> Rules { get; private set; }

		public ValidatorDescriptor(IEnumerable<IValidationRule> ruleBuilders) {
			Rules = ruleBuilders;
		}

		public virtual string GetName(string property) {
			var nameUsed = Rules
				.OfType<PropertyRule>()
				.Where(x => x.PropertyName == property)
				.Select(x => x.GetDisplayName()).FirstOrDefault();

			return nameUsed;
		}

		public virtual ILookup<string, IPropertyValidator> GetMembersWithValidators() {
			var query = from rule in Rules.OfType<PropertyRule>()
						where rule.PropertyName != null
						from validator in rule.Validators
						select new { propertyName = rule.PropertyName, validator };

			return query.ToLookup(x => x.propertyName, x => x.validator);
		}

		public IEnumerable<IPropertyValidator> GetValidatorsForMember(string name) {
			return GetMembersWithValidators()[name];
		}

		public IEnumerable<IValidationRule> GetRulesForMember(string name) {
			var query = from rule in Rules.OfType<PropertyRule>()
						where rule.PropertyName == name
						select (IValidationRule)rule;

			return query.ToList();
		}

		public virtual string GetName(Expression<Func<T, object>> propertyExpression) {
			var member = propertyExpression.GetMember();

			if (member == null) {
				throw new ArgumentException(string.Format("Cannot retrieve name as expression '{0}' as it does not specify a property.", propertyExpression));
			}

			return GetName(member.Name);
		}

		public IEnumerable<IPropertyValidator> GetValidatorsForMember<TValue>(MemberAccessor<T, TValue> accessor)
		{
			return from rule in Rules.OfType<PropertyRule>()
			       where Equals(rule.Member, accessor.Member)
			       from validator in rule.Validators
			       select validator;
		} 
	}
}