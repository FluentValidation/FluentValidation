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

// Attribution: This class was contributed to FluentValidation using code posted on StackOverflow by Jon Skeet
// The original code can be found at https://stackoverflow.com/a/764102

/// <summary>
/// Allows a decimal to be validated for scale and precision.
/// Scale would be the number of digits to the right of the decimal point.
/// Precision would be the number of digits. This number includes both the left and the right sides of the decimal point.
///
/// It implies certain range of values that will be accepted by the validator.
/// It permits up to Precision - Scale digits to the left of the decimal point and up to Scale digits to the right.
///
/// It can be configured to use the effective scale and precision
/// (i.e. ignore trailing zeros) if required.
///
/// 123.4500 has an scale of 4 and a precision of 7, but an effective scale
/// and precision of 2 and 5 respectively.
/// </summary>
public class PrecisionScaleValidator<T> : PropertyValidator<T, decimal> {

	public PrecisionScaleValidator(int precision, int scale, bool ignoreTrailingZeros) {
		Scale = scale;
		Precision = precision;
		IgnoreTrailingZeros = ignoreTrailingZeros;

		if (Scale < 0)
			throw new ArgumentOutOfRangeException(nameof(scale), $"Scale must be a positive integer. [value:{Scale}].");

		if (Precision < 0)
			throw new ArgumentOutOfRangeException(nameof(precision), $"Precision must be a positive integer. [value:{Precision}].");

		if (Precision < Scale)
			throw new ArgumentOutOfRangeException(nameof(scale), $"Scale must be less than precision. [scale:{Scale}, precision:{Precision}].");
	}

	public override string Name => "ScalePrecisionValidator";

	public int Scale { get; }

	public int Precision { get; }

	public bool IgnoreTrailingZeros { get; }

	public override bool IsValid(ValidationContext<T> context, decimal decimalValue) {
		var info = Info.Get(decimalValue, IgnoreTrailingZeros);
		var expectedIntegerDigits = Precision - Scale;
		if (info.Scale > Scale || info.IntegerDigits > expectedIntegerDigits) {
			// Precision and scale alone may not be enough to describe why a value is invalid.
			// For example, given an expected precision of 3 and scale of 2, the value "123" is invalid, even though precision
			// is 3 and scale is 0. So as a workaround we can provide actual precision and scale as if value
			// was "right-padded" with zeros to the amount of expected decimals, so that it would look like
			// complement zeros were added in the decimal part for calculation of precision. In the above
			// example actual precision and scale would be printed as 5 and 2 as if value was 123.00.
			var printedActualScale = Math.Max(info.Scale, Scale);
			var printedActualPrecision = Math.Max(info.IntegerDigits, 1) + printedActualScale;

			context.MessageFormatter
				.AppendArgument("ExpectedPrecision", Precision)
				.AppendArgument("ExpectedScale", Scale)
				.AppendArgument("Digits", printedActualPrecision)
				.AppendArgument("ActualScale", printedActualScale);

			return false;
		}
		return true;
	}

	protected override string GetDefaultMessageTemplate(string errorCode) {
		return Localized(errorCode, Name);
	}

	private record struct Info (int Scale, int Precision) { 
		public int IntegerDigits => Precision - Scale;

		public static Info Get(decimal value, bool ignoreTrailingZeros) {
			var scale = value.Scale;
			var precision = 0;
			var tmp = GetMantissa(value);

			// Trim trailing zero's.
			if (ignoreTrailingZeros) {
				while (scale > 0 && tmp % 10 == 0) {
					tmp /= 10;
					scale--;
				}
			}

			// determine the precision.
			while (tmp >= 1) {
				precision++;
				tmp /= 10;
			}

			return new() {
				Scale = scale,
				Precision = precision,
			};
		}

		/// <summary>Returns the positive value with the scale reset to zero.</summary>
		private static decimal GetMantissa(decimal Decimal) {
			var bits = decimal.GetBits(Decimal);
			return new decimal(bits[0], bits[1], bits[2], false, 0);
		}
	}
}
