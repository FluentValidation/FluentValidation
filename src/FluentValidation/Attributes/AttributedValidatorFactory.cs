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
	using Internal;

	/// <summary>
	/// Implementation of IValidatorFactory that looks for ValidatorAttribute instances on the specified type in order to provide the validator instance.
	/// </summary>
	public class AttributedValidatorFactory : IValidatorFactory {
		readonly InstanceCache cache = new InstanceCache();

		/// <summary>
		/// Gets a validator for the appropriate type.
		/// </summary>
		public IValidator<T> GetValidator<T>() {
			return (IValidator<T>)GetValidator(typeof(T));
		}

		/// <summary>
		/// Gets a validator for the appropriate type.
		/// </summary>
		public virtual IValidator GetValidator(Type type) {
			if (type == null)
				return null;

			var attribute = (ValidatorAttribute)Attribute.GetCustomAttribute(type, typeof(ValidatorAttribute));

			if (attribute == null || attribute.ValidatorType == null)
				return null;

			return cache.GetOrCreateInstance(attribute.ValidatorType) as IValidator;
		}
	}
}