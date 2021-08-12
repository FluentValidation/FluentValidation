#region License
// Copyright (c) .NET Foundation and contributors.
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
// The latest version of this file can be found at https://github.com/FluentValidation/FluentValidation
#endregion

namespace FluentValidation.Validators {
	internal class RequiredValidator<T,TProperty> : PropertyValidator<T,TProperty>, IRequiredValidator, INotNullValidator {

		public RequiredValidator(bool allowEmptyStrings) {
			AllowEmptyStrings = allowEmptyStrings;
		}

		/// <summary>
		/// Whether empty strings should be considered valid. Defaults to false.
		/// </summary>
		public bool AllowEmptyStrings { get; }

		public override string Name => "RequiredValidator";

		public override bool IsValid(ValidationContext<T> context, TProperty value) {
			if (value == null) {
				return false;
			}

			return AllowEmptyStrings || value is not string stringValue || !string.IsNullOrWhiteSpace(stringValue);
		}

		protected override string GetDefaultMessageTemplate(string errorCode) {
			return Localized(errorCode, Name);
		}
	}

	public interface IRequiredValidator : IPropertyValidator {
	}
}
