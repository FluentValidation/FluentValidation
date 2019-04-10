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

namespace FluentValidation.Validators {
	using System;
	using System.Text.RegularExpressions;
	using Resources;

	/// <summary>
	/// Defines which mode should be used for email validation.
	/// </summary>
	public enum EmailValidationMode {
		/// <summary>
		/// Uses a regular expression for email validation. This is the same regex used by <see cref="System.ComponentModel.DataAnnotations.EmailAddressAttribute"/> in .NET 4.x.
		/// </summary>
		Net4xRegex,

		/// <summary>
		/// Uses the simplified ASP.NET Core logic for checking an email address, which just checks for the presence of an @ sign.
		/// </summary>
		AspNetCoreCompatible,
	}

	//Email regex matches the one used in the DataAnnotations EmailAddressAttribute for consistency/parity with DataAnnotations. This is not a fully comprehensive solution, but is "good enough" for most cases.
	public class EmailValidator : PropertyValidator, IRegularExpressionValidator, IEmailValidator {
		private readonly Regex _regex;

		const string _expression = @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-||_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+([a-z]+|\d|-|\.{0,1}|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])?([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$";

		public EmailValidator() : base(new LanguageStringSource(nameof(EmailValidator))) {
			_regex = CreateRegEx();
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			if (context.PropertyValue == null) return true;

			if (!_regex.IsMatch((string)context.PropertyValue)) {
				return false;
			}

			return true;
		}

		public string Expression => _expression;

		private static Regex CreateRegEx() {
			return new Regex(_expression, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(2.0));
		}
	}


	public class AspNetCoreCompatibleEmailValidator : PropertyValidator, IEmailValidator {
		public AspNetCoreCompatibleEmailValidator() : base(new LanguageStringSource(nameof(EmailValidator))) {

		}

		protected override bool IsValid(PropertyValidatorContext context) {
			var value = context.PropertyValue;

			if (value == null) {
				return true;
			}

			if (!(value is string valueAsString)) {
				return false;
			}

			// only return true if there is only 1 '@' character
			// and it is neither the first nor the last character
			int index = valueAsString.IndexOf('@');

			return
				index > 0 &&
				index != valueAsString.Length - 1 &&
				index == valueAsString.LastIndexOf('@');
		}

		//TODO: Remove this once IEmailValidator no longer implements IRegularExpressionValidator.
		string IRegularExpressionValidator.Expression => null;
	}

	//TODO: Remove IRegularExpresionValidator from the inheritance chain for FV9.
	public interface IEmailValidator : IRegularExpressionValidator {

	}
}
