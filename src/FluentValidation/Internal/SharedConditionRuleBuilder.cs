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

namespace FluentValidation.Internal {
	using System;
	using System.Collections.Generic;

	public interface ISharedConditionRuleBuilder<T> {
	//	void Unless(Func<T, bool> predicate);
	}

	public class SharedConditionRuleBuilder<T> : ISharedConditionRuleBuilder<T> {
		readonly List<PropertyRule> propertyRules = new List<PropertyRule>();

//		public void Unless(Func<T, bool> predicate) {
//			foreach (var rule in propertyRules) {
//				rule.ApplyCondition(x => !predicate((T)x));
//			}
//		}

		public void Add(PropertyRule rule) {
			propertyRules.Add(rule);
		}

		public void ApplyPredicate(Func<T, bool> predicate) {
			foreach (var rule in propertyRules) {
				rule.ApplyCondition(x => predicate((T)x));
			}
		}
	}
}