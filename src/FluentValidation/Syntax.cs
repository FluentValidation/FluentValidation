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
	using Internal;
	using Syntax;
	using Validators;

	/// <summary>
	/// Rule builder that starts the chain
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TProperty"></typeparam>
	public interface IRuleBuilderInitial<T, TProperty> : IFluentInterface, IRuleBuilder<T, TProperty>, IConfigurable<PropertyRule<T>, IRuleBuilderInitial<T, TProperty>> {
		[Obsolete("Use Cascade(CascadeMode.StopOnFirstFailure) or Cascade(CascadeMode.Continue) instead")]
		CascadeStep<T, TProperty> Cascade();
	}

	/// <summary>
	/// Rule builder 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TProperty"></typeparam>
	public interface IRuleBuilder<T, TProperty> : IFluentInterface {
		/// <summary>
		/// Associates a validator with this the property for this rule builder.
		/// </summary>
		/// <param name="validator">The validator to set</param>
		/// <returns></returns>
		IRuleBuilderOptions<T, TProperty> SetValidator(IPropertyValidator validator);


		/// <summary>
		/// Associates an instance of IValidator with the current property rule.
		/// </summary>
		/// <param name="validator">The validator to use</param>
		IRuleBuilderOptions<T, TProperty> SetValidator(IValidator validator);
	}


	/// <summary>
	/// Rule builder
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TProperty"></typeparam>
	public interface IRuleBuilderOptions<T, TProperty> : IRuleBuilder<T, TProperty>, IConfigurable<PropertyRule<T>, IRuleBuilderOptions<T, TProperty>> {

	}
}