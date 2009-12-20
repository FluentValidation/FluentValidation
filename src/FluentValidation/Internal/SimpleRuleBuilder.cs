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
	using Results;

	/// <summary>
	/// A simple implementation of IRuleBuilder that just holds a single validation rule.
	/// </summary>
	public class SimpleRuleBuilder<T> : IValidationRuleCollection<T> {

		readonly IValidationRule<T> rule;

		public SimpleRuleBuilder(IValidationRule<T> rule) {
			this.rule = rule;
		}

		public IEnumerator<IValidationRule<T>> GetEnumerator() {
			return new List<IValidationRule<T>> {rule}.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public IEnumerable<ValidationFailure> Validate(ValidationContext<T> context) {
			return rule.Validate(context);
		}
	}
}