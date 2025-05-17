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
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;

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

		if (!enumType.IsEnum) {
			return InvalidTypePredicate;
		}

		if (enumType.GetCustomAttribute<FlagsAttribute>() is null) {
			return value => Enum.IsDefined(enumType, value);
		}

		var values = Enum.GetValuesAsUnderlyingType(enumType);
		if (values.Length == 0) return InvalidTypePredicate;

		var elementType = values.GetType().GetElementType();
		if (elementType is null) return InvalidTypePredicate;

		var valueTypeCode = Type.GetTypeCode(elementType);

		return valueTypeCode switch {
			TypeCode.Char => InvokeEvaluateFlagEnumValues((char[])values),
			TypeCode.SByte => InvokeEvaluateFlagEnumValues((sbyte[])values),
			TypeCode.Byte => InvokeEvaluateFlagEnumValues((byte[])values),
			TypeCode.Int16 => InvokeEvaluateFlagEnumValues((short[])values),
			TypeCode.UInt16 => InvokeEvaluateFlagEnumValues((ushort[])values),
			TypeCode.Int32 => InvokeEvaluateFlagEnumValues((int[])values),
			TypeCode.UInt32 => InvokeEvaluateFlagEnumValues((uint[])values),
			TypeCode.Int64 => InvokeEvaluateFlagEnumValues((long[])values),
			TypeCode.UInt64 => InvokeEvaluateFlagEnumValues((ulong[])values),
			_ => InvalidTypePredicate
		};
	}

	private static Func<TProperty, bool> InvokeEvaluateFlagEnumValues<TValue>(TValue[] values)
		where TValue : struct, IBinaryNumber<TValue> {

		var hasZero = false;
		var allFlags = default(TValue);
		foreach (var v in values) {
			if (v == TValue.Zero) hasZero = true;
			allFlags |= v;
		}

		allFlags = ~allFlags;

		var distinctNonZero = values
			.Distinct()
			.Where(x => x != TValue.Zero)
			.ToArray();

		// Sort to favor values that have a superset of bits set, then fall
		// back to default comparison.
		Array.Sort(distinctNonZero, (a, b) => {
			var combined = a & b;
			if (combined == b) return -1;
			if (combined == a) return 1;

			return -Comparer<TValue>.Default.Compare(a, b);
		});

		return propertyValue => {
			var value = Unsafe.BitCast<TProperty, TValue>(propertyValue);

			// Zero is valid if it's a defined value
			if (value == TValue.Zero) {
				return hasZero;
			}

			// All bits set in value must be set in the defined flags
			if (allFlags != TValue.Zero && (value & allFlags) != TValue.Zero) {
				return false;
			}

			// If value can be constructed by ORing any combination of defined values, it's valid.
			// This is true if (value | allFlags) == allFlags (already checked above)
			// But we also want to allow any combination of the defined values, not just the mask
			// So, we check if all bits in value are covered by the defined values
			// Additionally, we want to allow only valid combinations (not partial bits)
			// So, we try to decompose value into a combination of defined values
			var remaining = value;
			foreach (var v in distinctNonZero) {
				if ((value & v) != v) continue;

				remaining &= ~v;
				if (remaining == TValue.Zero) return true;
			}

			return false;
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
