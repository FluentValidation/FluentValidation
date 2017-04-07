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
namespace FluentValidation.AspNetCore {
	using System;
	using System.Collections.Generic;
	using Internal;
	using Validators;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

	public abstract class ClientValidatorBase : IClientModelValidator {
		public IPropertyValidator Validator { get; private set; }
		public PropertyRule Rule { get; private set; }

		public ClientValidatorBase(PropertyRule rule, IPropertyValidator validator) {
			this.Validator = validator;
			this.Rule = rule;
		}

		public abstract void AddValidation(ClientModelValidationContext context);

		protected static bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
		{
			if (attributes.ContainsKey(key))
			{
				return false;
			}

			attributes.Add(key, value);
			return true;
		}
	}
}