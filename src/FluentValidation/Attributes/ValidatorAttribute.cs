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

namespace FluentValidation.Attributes {
	using System;

	/// <summary>
	/// Validator attribute to define the class that will describe the Validation rules
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class ValidatorAttribute : Attribute
	{
		/// <summary>
		/// The type of the validator used to validate the current type.
		/// </summary>
		public Type ValidatorType { get; private set; }

		/// <summary>
		/// Creates an instance of the ValidatorAttribute allowing a validator type to be specified.
		/// </summary>
		public ValidatorAttribute(Type validatorType)
		{
			ValidatorType = validatorType;
		}
	}
}