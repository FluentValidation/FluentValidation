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
namespace FluentValidation.AspNetCore {
	using System.Collections.Generic;
	using Internal;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using Resources;
	using Validators;

	internal class RangeClientValidator : ClientValidatorBase {
		InclusiveBetweenValidator RangeValidator {
			get { return (InclusiveBetweenValidator)Validator; }
		}
		
		public RangeClientValidator(PropertyRule rule, IPropertyValidator validator) : base(rule, validator) {

		}

		public override void AddValidation(ClientModelValidationContext context) {
			if (RangeValidator.To != null && RangeValidator.From != null) {
				MergeAttribute(context.Attributes, "data-val", "true");
				MergeAttribute(context.Attributes, "data-val-range", GetErrorMessage(context));
				MergeAttribute(context.Attributes, "data-val-range-max", RangeValidator.To.ToString());
				MergeAttribute(context.Attributes, "data-val-range-min", RangeValidator.From.ToString());
			}
		}

		private string GetErrorMessage(ClientModelValidationContext context) {
			var formatter = ValidatorOptions.MessageFormatterFactory()
				.AppendPropertyName(Rule.GetDisplayName())
				.AppendArgument("From", RangeValidator.From)
				.AppendArgument("To", RangeValidator.To);
			var messageNeedsSplitting = RangeValidator.ErrorMessageSource.ResourceType == typeof(LanguageManager);

			string message;

			try {
				message = RangeValidator.ErrorMessageSource.GetString(null);
			}
			catch (FluentValidationMessageFormatException) {
				message =ValidatorOptions.LanguageManager.GetStringForValidator<InclusiveBetweenValidator>();
				messageNeedsSplitting = true;
			}

			if (messageNeedsSplitting)
			{
				// If we're using the default resources then the mesage for length errors will have two parts, eg:
				// '{PropertyName}' must be between {From} and {To}. You entered {Value}.
				// We can't include the "Value" part of the message because this information isn't available at the time the message is constructed.
				// Instead, we'll just strip this off by finding the index of the period that separates the two parts of the message.

				message = message.Substring(0, message.IndexOf(".") + 1);
			}
			message = formatter.BuildMessage(message);

			return message;
		}
	}
}
