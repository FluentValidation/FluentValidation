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
	using System.Globalization;
	using System.Linq;
	using System.Resources;
	using Attributes;
	using Internal;
	using Resources;

	public class PropertyValidatorContext {
		protected PropertySelector propertyValueFunc;
		protected bool propertyValueSet;
		protected object propertyValue;

		public string PropertyDescription { get; protected set; }
		public string CustomError { get; protected set; }
		protected IEnumerable<Func<object, object>> customFormatArgs;

		public object Instance { get; protected set; }


		public string GetFormattedErrorMessage(Type type, MessageFormatter formatter) {
			string error = CustomError ?? ValidationMessageAttribute.GetMessage(type);

			if (customFormatArgs != null) {
				formatter.AppendAdditionalArguments(
					customFormatArgs.Select(func => func(Instance)).ToArray()
				);
			}

			return formatter.BuildMessage(error);
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
			: this(propertyDescription, instance, propertyValueFunc, null, null) {
		}

		public PropertyValidatorContext(string propertyDescription, object instance, PropertySelector propertyValueFunc, string customError, IEnumerable<Func<object, object>> customFormatArgs) {
			propertyValueFunc.Guard("propertyValueFunc cannot be null");
			PropertyDescription = propertyDescription;
			Instance = instance;
			CustomError = customError;
			this.customFormatArgs = customFormatArgs;
			this.propertyValueFunc = propertyValueFunc;
		}

	}

	public class PropertyValidatorContext<T, TProperty> : PropertyValidatorContext {
		public PropertyValidatorContext(string propertyDescription, object instance, PropertySelector propertyValueFunc) : base(propertyDescription, instance, propertyValueFunc) {
		}

		public PropertyValidatorContext(string propertyDescription, object instance, PropertySelector propertyValueFunc, string customError, IEnumerable<Func<object, object>> customFormatArgs) : base(propertyDescription, instance, propertyValueFunc, customError, customFormatArgs) {
		}
	}
}