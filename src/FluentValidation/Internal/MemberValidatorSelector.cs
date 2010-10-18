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

namespace FluentValidation.Internal {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;

	public class MemberValidatorSelector : IValidatorSelector {
		readonly IEnumerable<MemberInfo> members;

		public MemberValidatorSelector(IEnumerable<MemberInfo> members) {
			this.members = members;
		}

		public bool CanExecute<T>(PropertyRule<T> rule, string propertyPath) {
			return members.Any(x => x == rule.Member);
		}

		public static MemberValidatorSelector FromExpressions<T>(IEnumerable<Expression<Func<T, object>>> propertyExpressions) {
			var members = propertyExpressions.Select(x => MemberFromExpression(x)).ToList();
			return new MemberValidatorSelector(members);
		}

		private static MemberInfo MemberFromExpression<T>(Expression<Func<T, object>> expression) {
			var member = expression.GetMember();

			if (member == null) {
				throw new ArgumentException(string.Format("Expression '{0}' does not specify a valid property or field.", expression));
			}

			return member;
		}
	}
}