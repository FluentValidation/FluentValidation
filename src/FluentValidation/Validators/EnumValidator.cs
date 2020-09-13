﻿#region License

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
	using System.Reflection;
	using FluentValidation.Internal;
	using Resources;

	public class EnumValidator : PropertyValidator {
		private readonly Type _enumType;

		public EnumValidator(Type enumType) {
			this._enumType = enumType;
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			if (context.PropertyValue == null) return true;

			var underlyingEnumType = Nullable.GetUnderlyingType(_enumType) ?? _enumType;

			if (!underlyingEnumType.IsEnum) return false;

			if (underlyingEnumType.GetCustomAttribute<FlagsAttribute>() != null) {
				return IsFlagsEnumDefined(underlyingEnumType, context.PropertyValue);
			}

			return Enum.IsDefined(underlyingEnumType, context.PropertyValue);
		}

		private static bool IsFlagsEnumDefined(Type enumType, object value) {
			var typeName = Enum.GetUnderlyingType(enumType).Name;

			switch (typeName) {
				case "Byte": {
					var typedValue = (byte) value;
					return EvaluateFlagEnumValues(typedValue, enumType);
				}

				case "Int16": {
					var typedValue = (short) value;

					return EvaluateFlagEnumValues(typedValue, enumType);
				}

				case "Int32": {
					var typedValue = (int) value;

					return EvaluateFlagEnumValues(typedValue, enumType);
				}

				case "Int64": {
					var typedValue = (long) value;

					return EvaluateFlagEnumValues(typedValue, enumType);
				}

				case "SByte": {
					var typedValue = (sbyte) value;

					return EvaluateFlagEnumValues(Convert.ToInt64(typedValue), enumType);
				}

				case "UInt16": {
					var typedValue = (ushort) value;
					return EvaluateFlagEnumValues(typedValue, enumType);
				}

				case "UInt32": {
					var typedValue = (uint) value;
					return EvaluateFlagEnumValues(typedValue, enumType);
				}

				case "UInt64": {
					var typedValue = (ulong) value;
					return EvaluateFlagEnumValues((long) typedValue, enumType);
				}

				default:
					var message = $"Unexpected typeName of '{typeName}' during flags enum evaluation.";
					throw new ArgumentOutOfRangeException(nameof(enumType), message);
			}
		}

		private static bool EvaluateFlagEnumValues(long value, Type enumType) {
			long mask = 0;
			foreach (var enumValue in Enum.GetValues(enumType)) {
				var enumValueAsInt64 = Convert.ToInt64(enumValue);
				if ((enumValueAsInt64 & value) == enumValueAsInt64) {
					mask |= enumValueAsInt64;
					if (mask == value)
						return true;
				}
			}

			return false;
		}

		protected override string GetDefaultMessageTemplate() {
			return Localized(nameof(EnumValidator));
		}
	}
}
