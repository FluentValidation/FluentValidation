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

namespace FluentValidation.Syntax {
	using System;
	using Internal;
	using FluentValidation;

	/// <summary>
	/// Specifies the cascade mode for the rule. 
	/// </summary>
	public class CascadeStep<T, TProperty> {
		readonly IRuleBuilderInitial<T, TProperty> rule;

		public CascadeStep(IRuleBuilderInitial<T, TProperty> ruleBuilder) {
			this.rule = ruleBuilder;
		}

		/// <summary>
		///  When a validator fails, the next validator will be run
		/// </summary>
		/// <returns></returns>
		[Obsolete("Use Cascade(CascadeMode.Continue) instead")]
		public IRuleBuilderInitial<T, TProperty> Continue() {
			return rule.Cascade(CascadeMode.Continue);
		}

		/// <summary>
		/// The rule will stop executing as soon as one validator fails.
		/// </summary>
		/// <returns></returns>
		[Obsolete("Use Cascade(CascadeMode.StopOnFirstFailure) instead")]
		public IRuleBuilderInitial<T, TProperty> StopOnFirstFailure() {
			return rule.Cascade(CascadeMode.StopOnFirstFailure);
		}
	}
}