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

namespace FluentValidation.Validators;

using System;
using System.Globalization;
using System.Numerics;
using System.Reflection;


public class EnumValidator<T, TProperty> : PropertyValidator<T, TProperty>, IEnumValidator
	where TProperty : struct, Enum {
	private static readonly Func<TProperty, bool> InvalidTypePredicate = static _ => false;
	private readonly Func<TProperty, bool> _validator = CreateValidator();

	public Type EnumType => typeof(TProperty);

	public override string Name => "EnumValidator";

	public override bool IsValid(ValidationContext<T> context, TProperty value) {
		return _validator(value);
	}

	private static bool EvaluateFlagEnumValues<TValue>(TValue propertyValue, TValue[] values)
	where TValue : IBinaryNumber<TValue> {
		var mask = default(TValue);
		foreach (var value in values) {
			if ((value & propertyValue) != value)
				continue;

			mask |= value;
			if (mask == propertyValue)
				return true;
		}

		return false;
	}

	internal static Func<TProperty, bool> CreateValidator() {
		var enumType = typeof(TProperty);

		if (!enumType.IsEnum)
			return InvalidTypePredicate;

		if (enumType.GetCustomAttribute<FlagsAttribute>() is null)
			return value => Enum.IsDefined(enumType, value);

		var values = Enum.GetValuesAsUnderlyingType(enumType);

		if (values.Length == 0)
			return InvalidTypePredicate;

		var value0 = values.GetValue(0);
		if (value0 is null)
			return InvalidTypePredicate;

		var valueTypeCode = Type.GetTypeCode(value0.GetType());

		return valueTypeCode switch {
			TypeCode.Char => propertyValue => EvaluateFlagEnumValues(((IConvertible)propertyValue).ToChar(CultureInfo.InvariantCulture), (char[])values),
			TypeCode.SByte => propertyValue => EvaluateFlagEnumValues(((IConvertible)propertyValue).ToSByte(CultureInfo.InvariantCulture), (sbyte[])values),
			TypeCode.Byte => propertyValue => EvaluateFlagEnumValues(((IConvertible)propertyValue).ToByte(CultureInfo.InvariantCulture), (byte[])values),
			TypeCode.Int16 => propertyValue => EvaluateFlagEnumValues(((IConvertible)propertyValue).ToInt16(CultureInfo.InvariantCulture), (short[])values),
			TypeCode.UInt16 => propertyValue => EvaluateFlagEnumValues(((IConvertible)propertyValue).ToUInt16(CultureInfo.InvariantCulture), (ushort[])values),
			TypeCode.Int32 => propertyValue => EvaluateFlagEnumValues(((IConvertible)propertyValue).ToInt32(CultureInfo.InvariantCulture), (int[])values),
			TypeCode.UInt32 => propertyValue => EvaluateFlagEnumValues(((IConvertible)propertyValue).ToUInt32(CultureInfo.InvariantCulture), (uint[])values),
			TypeCode.Int64 => propertyValue => EvaluateFlagEnumValues(((IConvertible)propertyValue).ToInt64(CultureInfo.InvariantCulture), (long[])values),
			TypeCode.UInt64 => propertyValue => EvaluateFlagEnumValues(((IConvertible)propertyValue).ToUInt64(CultureInfo.InvariantCulture), (ulong[])values),
			_ => InvalidTypePredicate
		};
	}

	protected override string GetDefaultMessageTemplate(string errorCode) {
		return Localized(errorCode, Name);
	}
}

public class NullableEnumValidator<T, TProperty> : PropertyValidator<T, TProperty?>, IEnumValidator
	where TProperty : struct, Enum {
	private readonly Func<TProperty, bool> _validator = EnumValidator<T, TProperty>.CreateValidator();

	public Type EnumType => typeof(TProperty);

	public override string Name => "NullableEnumValidator";

	public override bool IsValid(ValidationContext<T> context, TProperty? value) {
		if (!value.HasValue) {
			return true;
		}

		return _validator(value.Value);
	}
}

public interface IEnumValidator : IPropertyValidator {

	/// <summary>
	/// Enum type being validated.
	/// </summary>
	Type EnumType { get; }
}
