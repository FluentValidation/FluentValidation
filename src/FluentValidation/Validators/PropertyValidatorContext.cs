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
	using Internal;

	public class PropertyValidatorContext : IValidationContext {
		private MessageFormatter _messageFormatter;
		private readonly Lazy<object> _propertyValueContainer;

		public ValidationContext ParentContext { get; private set; }
		public PropertyRule Rule { get; private set; }
		public string PropertyName { get; private set; }

		public string DisplayName => Rule.GetDisplayName(ParentContext);

		public object InstanceToValidate => ParentContext.InstanceToValidate;
		public MessageFormatter MessageFormatter => _messageFormatter ?? (_messageFormatter = ValidatorOptions.MessageFormatterFactory());

		//Lazily load the property value
		//to allow the delegating validator to cancel validation before value is obtained
		public object PropertyValue => _propertyValueContainer.Value;
		
		// Explicit implementation so we don't have to expose the base interface.
		IValidationContext IValidationContext.ParentContext => ParentContext;
		
		public PropertyValidatorContext(ValidationContext parentContext, PropertyRule rule, string propertyName) {
			ParentContext = parentContext;
			Rule = rule;
			PropertyName = propertyName;
			_propertyValueContainer = new Lazy<object>( () => {
				var value = rule.PropertyFunc(parentContext.InstanceToValidate);
				if (rule.Transformer != null) value = rule.Transformer(value);
				return value;
			});
		}

		public PropertyValidatorContext(ValidationContext parentContext, PropertyRule rule, string propertyName, object propertyValue) {
			ParentContext = parentContext;
			Rule = rule;
			PropertyName = propertyName;
			_propertyValueContainer = new Lazy<object>(() => propertyValue);
		}
	}
}