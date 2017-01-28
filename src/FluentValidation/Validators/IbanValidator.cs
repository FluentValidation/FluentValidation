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

namespace FluentValidation.Validators
{
    using System;
    using System.Text;

    public class IbanValidator : PropertyValidator
    {
        public IbanValidator() : base("'{PropertyName}' is not a valid IBAN account number.") { }
        protected override bool IsValid(PropertyValidatorContext context)
        {
            var value = context.PropertyValue as string;

            return IsValid(value);
        }
        private bool IsValid(string bankAccount)
        {
            // http://www.codeproject.com/Tips/775696/IBAN-Validator
            bankAccount = bankAccount?.ToUpper();

            if (string.IsNullOrWhiteSpace(bankAccount))
                return true;

            if(bankAccount.Length < 5)
                return false;

            else if (System.Text.RegularExpressions.Regex.IsMatch(bankAccount, "^[a-zA-Z0-9]*$"))
            {
                bankAccount = bankAccount.Replace(" ", string.Empty);
                string bank =
                bankAccount.Substring(4, bankAccount.Length - 4) + bankAccount.Substring(0, 4);
                int asciiShift = 55;
                var sb = new StringBuilder();
                foreach (char c in bank)
                {
                    int v;
                    if (Char.IsLetter(c)) v = c - asciiShift;
                    else int.TryParse(c.ToString(), out v);
                    sb.Append(v);
                }
                string checkSumString = sb.ToString();
                int checksum = int.Parse(checkSumString.Substring(0, 1));
                for (int i = 1; i < checkSumString.Length; i++)
                {
                    int v = 0;
                    int.TryParse(checkSumString.Substring(i, 1), out v);
                    checksum *= 10;
                    checksum += v;
                    checksum %= 97;
                }
                return checksum == 1;
            }
            else
            {
                return false;
            }
        }
    }
}