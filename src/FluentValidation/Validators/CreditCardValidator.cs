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
// The latest version of this file can be found at https://github.com/JeremySkinner/FluentValidation
#endregion

namespace FluentValidation.Validators {
	using System.Linq;
	using Resources;


	/// <summary>
	/// Ensures that the property value is a valid credit card number.
	/// </summary>
	public class CreditCardValidator : PropertyValidator {
		// This logic was taken from the CreditCardAttribute in the ASP.NET MVC3 source.

		public CreditCardValidator() : base(new LanguageStringSource(nameof(CreditCardValidator))) {
			
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			var value = context.PropertyValue as string;

			if (value == null) {
				return true;
			}

			value = value.Replace("-", "").Replace(" ", "");

			int checksum = 0;
			bool evenDigit = false;
			// http://www.beachnet.com/~hstiles/cardtype.html
			foreach (char digit in value.ToCharArray().Reverse()) {
				if (!char.IsDigit(digit)) {
					return false;
				}

				int digitValue = (digit - '0') * (evenDigit ? 2 : 1);
				evenDigit = !evenDigit;

				while (digitValue > 0) {
					checksum += digitValue % 10;
					digitValue /= 10;
				}
			}

			return (checksum % 10) == 0;
		}
	}
}