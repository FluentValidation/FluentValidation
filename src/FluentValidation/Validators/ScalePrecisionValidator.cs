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
	using Resources;

	// Attribution: This class was contributed to FluentValidation using code posted on StackOverflow by Jon Skeet
	// The original code can be found at https://stackoverflow.com/a/764102

	/// <summary>
	/// Allows a decimal to be validated for scale and precision.
	/// Scale would be the number of digits to the right of the decimal point.
	/// Precision would be the number of digits. This number includes both the left and the right sides of the decimal point.
	///
	/// It can be configured to use the effective scale and precision
	/// (i.e. ignore trailing zeros) if required.
	///
	/// 123.4500 has an scale of 4 and a precision of 7, but an effective scale
	/// and precision of 2 and 5 respectively.
	/// </summary>
	public class ScalePrecisionValidator<T> : PropertyValidator<T, decimal> {
		public ScalePrecisionValidator(int scale, int precision) {
			Init(scale, precision);
		}

		public override string Name => "ScalePrecisionValidator";

		public int Scale { get; set; }

		public int Precision { get; set; }

		public bool IgnoreTrailingZeros { get; set; }

		public override bool IsValid(ValidationContext<T> context, decimal decimalValue) {
			var scale = GetScale(decimalValue);
			var precision = GetPrecision(decimalValue);
			var actualIntegerDigits = precision - scale;
			var expectedIntegerDigits = Precision - Scale;
			if (scale > Scale || actualIntegerDigits > expectedIntegerDigits) {
				context.MessageFormatter
					.AppendArgument("ExpectedPrecision", Precision)
					.AppendArgument("ExpectedScale", Scale)
					.AppendArgument("Digits", actualIntegerDigits)
					.AppendArgument("ActualScale", scale);

				return false;
			}
			return true;
		}

		private void Init(int scale, int precision) {
			Scale = scale;
			Precision = precision;

			if (Scale < 0)
				throw new ArgumentOutOfRangeException(
					nameof(scale), $"Scale must be a positive integer. [value:{Scale}].");
			if (Precision < 0)
				throw new ArgumentOutOfRangeException(
					nameof(precision), $"Precision must be a positive integer. [value:{Precision}].");
			if (Precision < Scale)
				throw new ArgumentOutOfRangeException(
					nameof(scale),
					$"Scale must be less than precision. [scale:{Scale}, precision:{Precision}].");
		}

		private static UInt32[] GetBits(decimal Decimal) {
			// We want the integer parts as uint
			// C# doesn't permit int[] to uint[] conversion, but .NET does. This is somewhat evil...
			return (uint[]) (object) decimal.GetBits(Decimal);
		}

		private static decimal GetMantissa(decimal Decimal) {
			var bits = GetBits(Decimal);
			return (bits[2] * 4294967296m * 4294967296m) + (bits[1] * 4294967296m) + bits[0];
		}

		private static uint GetUnsignedScale(decimal Decimal) {
			var bits = GetBits(Decimal);
			uint scale = (bits[3] >> 16) & 31;
			return scale;
		}

		private int GetScale(decimal Decimal) {
			uint scale = GetUnsignedScale(Decimal);
			if (IgnoreTrailingZeros) {
				return (int) (scale - NumTrailingZeros(Decimal));
			}

			return (int) scale;
		}

		private static uint NumTrailingZeros(decimal Decimal) {
			uint trailingZeros = 0;
			uint scale = GetUnsignedScale(Decimal);
			for (decimal tmp = GetMantissa(Decimal); tmp % 10m == 0 && trailingZeros < scale; tmp /= 10) {
				trailingZeros++;
			}

			return trailingZeros;
		}

		private int GetPrecision(decimal Decimal) {
			// Precision: number of times we can divide by 10 before we get to 0
			uint precision = 0;
			for (decimal tmp = GetMantissa(Decimal); tmp >= 1; tmp /= 10) {
				precision++;
			}

			if (IgnoreTrailingZeros) {
				return (int) (precision - NumTrailingZeros(Decimal));
			}

			return (int) precision;
		}

		protected override string GetDefaultMessageTemplate(string errorCode) {
			return Localized(errorCode, Name);
		}
	}
}
