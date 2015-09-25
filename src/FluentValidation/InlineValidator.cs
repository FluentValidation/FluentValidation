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
	/// Validator implementation that allows rules to be defined without inheriting from AbstractValidator.
	/// </summary>
	/// <example>
	/// <code>
	/// public class Customer {
	///   public int Id { get; set; }
	///   public string Name { get; set; }
	/// 
	///   public static readonly InlineValidator&lt;Customer&gt; Validator = new InlineValidator&lt;Customer&gt; {
	///     v =&gt; v.RuleFor(x =&gt; x.Name).NotNull(),
	///     v =&gt; v.RuleFor(x =&gt; x.Id).NotEqual(0),
	///   }
	/// }
	/// </code>
	/// </example>
	/// <typeparam name="T"></typeparam>
	public class InlineValidator<T> : AbstractValidator<T> {
		/// <summary>
		/// Delegate that specifies configuring an InlineValidator.
		/// </summary>
		public delegate IRuleBuilderOptions<T, TProperty> InlineRuleCreator<TProperty>(InlineValidator<T> validator);

		/// <summary>
		/// Allows configuration of the validator.
		/// </summary>
		public void Add<TProperty>(InlineRuleCreator<TProperty> ruleCreator) {
			ruleCreator(this);
		}
	}
}