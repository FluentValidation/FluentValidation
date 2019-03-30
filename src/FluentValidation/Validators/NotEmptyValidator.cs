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
// The latest version of this file can be found at https://github.com/JeremySkinner/FluentValidation
#endregion

namespace FluentValidation.Validators {
	using System;
	using System.Collections;
	using Resources;
	using System.Linq;

    public class NotEmptyValidator : PropertyValidator, INotEmptyValidator {
	    private readonly object _defaultValueForType;

		public NotEmptyValidator(object defaultValueForType) : base(new LanguageStringSource(nameof(NotEmptyValidator))) {
			_defaultValueForType = defaultValueForType;
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			if (context.PropertyValue == null
			    || IsInvalidString(context.PropertyValue)
			    || IsEmptyCollection(context.PropertyValue)
			    || Equals(context.PropertyValue, _defaultValueForType)) {
				return false;
			}

			return true;
		}

		bool IsEmptyCollection(object propertyValue) {
			switch (propertyValue) {
				case ICollection c when c.Count == 0:
				case Array a when a.Length == 0:
				case IEnumerable e when !e.Cast<object>().Any():
					return true;
				default:
					return false;
			}
		}

		bool IsInvalidString(object value) {
			return value is string s && string.IsNullOrWhiteSpace(s);
		}
    }

	public interface INotEmptyValidator : IPropertyValidator {
	}
}