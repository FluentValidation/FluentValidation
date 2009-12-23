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
		protected IEnumerable<IValidationRule<T>> Rules { get; private set; }

		public ValidatorDescriptor(IEnumerable<IValidationRule<T>> ruleBuilders) {
			this.Rules = ruleBuilders;
		}

		public string GetName(string property) {
			var nameUsed = Rules
				.OfType<IPropertyRule<T>>()
				.Where(x => x.Member.Name == property)
				.Select(x => x.PropertyDescription).FirstOrDefault();

			return nameUsed;
		}

		public ILookup<string, IPropertyValidator> GetMembersWithValidators() {
			return Rules.OfType<ISimplePropertyRule<T>>()
					.ToLookup(x => x.Member.Name, x => x.Validator);
		}

		public IEnumerable<IPropertyValidator> GetValidatorsForMember(string name) {
			return GetMembersWithValidators()[name];
		}

		public string GetName(Expression<Func<T, object>> propertyExpression) {
			var member = propertyExpression.GetMember();

			if (member == null) {
				throw new ArgumentException(string.Format("Cannot retrieve name as expression '{0}' as it does not specify a property.", propertyExpression));
			}

			return GetName(member.Name);
		}
	}
}