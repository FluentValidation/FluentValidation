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
	using Attributes;
	using Resources;

	public class NotEmptyValidator : PropertyValidator, INotEmptyValidator {
		readonly object defaultValueForType;

		public NotEmptyValidator(object defaultValueForType) : base(() => Messages.notempty_error) {
			this.defaultValueForType = defaultValueForType;
			SupportsStandaloneValidation = true;
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			if (context.PropertyValue == null || IsInvalidString(context.PropertyValue) ||
				Equals(context.PropertyValue, defaultValueForType)) {
				return false;
			}

			return true;
		}

		private bool IsInvalidString(object value) {
			if(value is string) {
				return IsNullOrWhiteSpace(value as string);
			}
			return false;
		}

		private bool IsNullOrWhiteSpace(string value) {
			if (value != null) {
				for (int i = 0; i < value.Length; i++) {
					if (!char.IsWhiteSpace(value[i])) {
						return false;
					}
				}
			}
			return true;

		}
	}

	public interface INotEmptyValidator : IPropertyValidator {
	}
}