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

namespace FluentValidation {
	/// <summary>
	/// Specifies how rules should cascade when one fails.
	/// </summary>
	public enum CascadeMode {
		/// <summary>
		/// When a rule fails, execution continues to the next rule.
		/// </summary>
		Continue,
		/// <summary>
		/// When a rule fails, validation is stopped and all other rules in the chain will not be executed.
		/// </summary>
		StopOnFirstFailure
	}

	/// <summary>
	/// Specifies where a When/Unless condition should be applied
	/// </summary>
	public enum ApplyConditionTo {
		/// <summary>
		/// Applies the condition to all validators declared so far in the chain.
		/// </summary>
		AllValidators,
		/// <summary>
		/// Applies the condition to the current validator only.
		/// </summary>
		CurrentValidator
	}

     /// <summary>
     /// Specifies the severity of a rule. 
     /// </summary>
	public enum Severity {
		/// <summary>
		/// Error
		/// </summary>
		Error,
		/// <summary>
		/// Warning
		/// </summary>
		Warning,
		/// <summary>
		/// Info
		/// </summary>
		Info
	}
}