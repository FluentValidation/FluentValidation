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
// The latest version of this file can be found at https://github.com/jeremyskinner/FluentValidation
#endregion

namespace FluentValidation.Validators
{
    using System;
    using System.Linq.Expressions;
    using Resources;

    /// <summary>
    /// Allows a decimal to be validated for scale and precision.  
    /// Scale would be the number of digits to the right of the decimal point.  
    /// Precision would be the number of digits.  
    /// 
    /// It can be configured to use the effective scale and precision 
    /// (i.e. ignore trailing zeros) if required.
    /// 
    /// 123.4500 has an scale of 4 and a precision of 7, but an effective scale
    /// and precision of 2 and 5 respectively.
    /// </summary>
    public class ScalePrecisionValidator : PropertyValidator
    {
        public ScalePrecisionValidator(int scale, int precision) : base(new LanguageStringSource(nameof(ScalePrecisionValidator)))
        {
            Init(scale, precision);
        }

        public int Scale { get; set; }

        public int Precision { get; set; }

        public bool? IgnoreTrailingZeros { get; set; }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var decimalValue = context.PropertyValue as decimal?;

            if (decimalValue.HasValue)
            {
                var scale = GetScale(decimalValue.Value);
                var precision = GetPrecision(decimalValue.Value);
                if (scale > Scale || precision > Precision)
                {
                    context.MessageFormatter
                        .AppendArgument("expectedPrecision", Precision)
                        .AppendArgument("expectedScale", Scale)
                        .AppendArgument("digits", precision - scale)
                        .AppendArgument("actualScale", scale);

                    return false;
                }
            }

            return true;
        }

        private void Init(int scale, int precision)
        {
            Scale = scale;
            Precision = precision;

            if (Scale < 0)
                throw new ArgumentOutOfRangeException(
                    "scale", string.Format("Scale must be a positive integer. [value:{0}].", Scale));
            if (Precision < 0)
                throw new ArgumentOutOfRangeException(
                    "precision", string.Format("Precision must be a positive integer. [value:{0}].", Precision));
            if (Precision < Scale)
                throw new ArgumentOutOfRangeException(
                    "scale",
                    string.Format("Scale must be less than precision. [scale:{0}, precision:{1}].", Scale, Precision));
        }

        private static UInt32[] GetBits(decimal Decimal)
        {
            // We want the integer parts as uint
            // C# doesn't permit int[] to uint[] conversion, but .NET does. This is somewhat evil...
            return (uint[]) (object) decimal.GetBits(Decimal);
        }

        private static decimal GetMantissa(decimal Decimal)
        {
            var bits = GetBits(Decimal);
            return (bits[2]*4294967296m*4294967296m) + (bits[1]*4294967296m) + bits[0];
        }

        private static uint GetUnsignedScale(decimal Decimal)
        {
            var bits = GetBits(Decimal);
            uint scale = (bits[3] >> 16) & 31;
            return scale;
        }

        private int GetScale(decimal Decimal)
        {
            uint scale = GetUnsignedScale(Decimal);
            if (IgnoreTrailingZeros.HasValue && IgnoreTrailingZeros.Value)
            {
                return (int) (scale - NumTrailingZeros(Decimal));
            }
            return (int) scale;
        }

        private static uint NumTrailingZeros(decimal Decimal)
        {
            uint trailingZeros = 0;
            uint scale = GetUnsignedScale(Decimal);
            for (decimal tmp = GetMantissa(Decimal); tmp%10m == 0 && trailingZeros < scale; tmp /= 10)
            {
                trailingZeros++;
            }
            return trailingZeros;
        }

        private int GetPrecision(decimal Decimal)
        {
            // Precision: number of times we can divide by 10 before we get to 0        
            uint precision = 0;
            if (Decimal != 0m)
            {
                for (decimal tmp = GetMantissa(Decimal); tmp >= 1; tmp /= 10)
                {
                    precision++;
                }
                if (IgnoreTrailingZeros.HasValue && IgnoreTrailingZeros.Value)
                {
                    return (int) (precision - NumTrailingZeros(Decimal));
                }
            }
            else
            {
                // Handle zero differently. It's odd.
                precision = (uint) GetScale(Decimal) + 1;
            }
            return (int) precision;
        }
    }
}
