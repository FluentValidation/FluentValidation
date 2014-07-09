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
// The latest version of this file can be found at http://fluentvalidation.codeplex.com
#endregion

namespace FluentValidation.Validators {
	using System.Collections;
	using Resources;
	using System.Linq;

    public class NotEmptyValidator : PropertyValidator, INotEmptyValidator {
		readonly object defaultValueForType;

		public NotEmptyValidator(object defaultValueForType) : base(() => Messages.notempty_error) {
			this.defaultValueForType = defaultValueForType;
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			if (context.PropertyValue == null
			    || IsInvalidString(context.PropertyValue)
			    || IsEmptyCollection(context.PropertyValue)
			    || Equals(context.PropertyValue, defaultValueForType)) {
				return false;
			}

			return true;
		}

		bool IsEmptyCollection(object propertyValue) {
			var collection = propertyValue as IEnumerable;
		    return collection != null && !collection.Cast<object>().Any();
		}

		bool IsInvalidString(object value) {
			if (value is string) {
				return IsNullOrWhiteSpace(value as string);
			}

			return false;
		}

		bool IsNullOrWhiteSpace(string value) {
		    if (value == null) {
		        return true;
		    }

		    return value.All(char.IsWhiteSpace);
		}
	}

	public interface INotEmptyValidator : IPropertyValidator {
	}
}