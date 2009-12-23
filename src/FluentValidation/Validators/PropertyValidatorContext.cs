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

namespace FluentValidation.Validators {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Attributes;
	using Internal;

	public class PropertyValidatorContext {
		private readonly MessageFormatter messageFormatter;
		private readonly PropertySelector propertyValueFunc;
		private readonly IEnumerable<Func<object, object>> customFormatArgs;
		private bool propertyValueSet;
		private object propertyValue;

		public string PropertyDescription { get; protected set; }
		public string CustomError { get; protected set; }
		public object Instance { get; private set; }

		public MessageFormatter MessageFormatter {
			get { return messageFormatter; }
		}

		//Lazily load the property value
		//to allow the delegating validator to cancel validation before value is obtained
		public object PropertyValue {
			get {
				if (!propertyValueSet) {
					propertyValue = propertyValueFunc(Instance);
					propertyValueSet = true;
				}

				return propertyValue;
			}
		}

		public PropertyValidatorContext(string propertyDescription, object instance, PropertySelector propertyValueFunc)
			: this(propertyDescription, instance, propertyValueFunc, null) {
		}

		public PropertyValidatorContext(string propertyDescription, object instance, PropertySelector propertyValueFunc, string customError) {
			propertyValueFunc.Guard("propertyValueFunc cannot be null");
			PropertyDescription = propertyDescription;
			Instance = instance;
			CustomError = customError;
			messageFormatter = new MessageFormatter();
			this.customFormatArgs = customFormatArgs;
			this.propertyValueFunc = propertyValueFunc;
		}

		public string GetFormattedErrorMessage(Type type, MessageFormatter formatter) {
			string error = CustomError ?? ValidationMessageAttribute.GetMessage(type);

			if (customFormatArgs != null) {
				formatter.AppendAdditionalArguments(
					customFormatArgs.Select(func => func(Instance)).ToArray()
				);
			}

			return formatter.BuildMessage(error);
		}
	}
}