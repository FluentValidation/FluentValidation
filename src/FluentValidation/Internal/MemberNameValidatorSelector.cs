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

	public class MemberNameValidatorSelector : IValidatorSelector {
		readonly IEnumerable<string> memberNames;

		public MemberNameValidatorSelector(IEnumerable<string> memberNames) {
			this.memberNames = memberNames;
		}

		public bool CanExecute (PropertyRule rule, string propertyPath) {
			return memberNames.Any(x => x == propertyPath);
		}

		public static MemberNameValidatorSelector FromExpressions<T>(params Expression<Func<T, object>>[] propertyExpressions) {
			var members = propertyExpressions.Select(MemberFromExpression).ToList();
			return new MemberNameValidatorSelector(members);
		}

		private static string MemberFromExpression<T>(Expression<Func<T, object>> expression) {
			var member = expression.GetMember();

			if (member == null) {
				throw new ArgumentException(string.Format("Expression '{0}' does not specify a valid property or field.", expression));
			}

			return member.Name;
		}
	}
}