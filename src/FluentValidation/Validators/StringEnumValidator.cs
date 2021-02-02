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
	using System;
	using System.Linq;
	using System.Reflection;
	using FluentValidation.Internal;
	using Resources;

	public class StringEnumValidator<T> : PropertyValidator<T, string> {
		private readonly Type _enumType;
		private readonly bool _caseSensitive;

		public override string Name => "StringEnumValidator";

		public StringEnumValidator(Type enumType, bool caseSensitive) {
			if (enumType == null) throw new ArgumentNullException(nameof(enumType));

			CheckTypeIsEnum(enumType);

			_enumType = enumType;
			_caseSensitive = caseSensitive;
		}

		public override bool IsValid(ValidationContext<T> context, string value) {
			if (value == null) return true;
			var comparison = _caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
			return Enum.GetNames(_enumType).Any(n => n.Equals(value, comparison));
		}

		private void CheckTypeIsEnum(Type enumType) {
			if (!enumType.IsEnum) {
				string message = $"The type '{enumType.Name}' is not an enum and can't be used with IsEnumName.";
				throw new ArgumentOutOfRangeException(nameof(enumType), message);
			}
		}

		protected override string GetDefaultMessageTemplate(string errorCode) {
			// Intentionally the same message as EnumValidator.
			return Localized(errorCode, "EnumValidator");
		}
	}
}
