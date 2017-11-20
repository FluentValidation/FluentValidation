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

namespace FluentValidation.Validators {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Attributes;
	using Internal;

	public class PropertyValidatorContext {
		private MessageFormatter messageFormatter;
		private readonly Lazy<object> propertyValueContainer;

		public ValidationContext ParentContext { get; private set; }
		public PropertyRule Rule { get; private set; }
		public string PropertyName { get; private set; }

		[Obsolete("Use DisplaName instead")]
		public string PropertyDescription => DisplayName;

		public string DisplayName => Rule.GetDisplayName(Instance);

		public object Instance => ParentContext.InstanceToValidate;

		public MessageFormatter MessageFormatter => messageFormatter ?? (messageFormatter = ValidatorOptions.MessageFormatterFactory());

		//Lazily load the property value
		//to allow the delegating validator to cancel validation before value is obtained
		public object PropertyValue => propertyValueContainer.Value;

		public PropertyValidatorContext(ValidationContext parentContext, PropertyRule rule, string propertyName) {
			ParentContext = parentContext;
			Rule = rule;
			PropertyName = propertyName;
			propertyValueContainer = new Lazy<object>( () => rule.PropertyFunc(parentContext.InstanceToValidate));
		}

		public PropertyValidatorContext(ValidationContext parentContext, PropertyRule rule, string propertyName, object propertyValue)
		{
			ParentContext = parentContext;
			Rule = rule;
			PropertyName = propertyName;
			propertyValueContainer = new Lazy<object>(() => propertyValue);
		}
	}
}