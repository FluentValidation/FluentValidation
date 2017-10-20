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

	internal class StringLengthClientValidator : ClientValidatorBase {
		public StringLengthClientValidator(PropertyRule rule, IPropertyValidator validator)
			: base(rule, validator) {
		}

		public override void AddValidation(ClientModelValidationContext context) {
			var lengthVal = (LengthValidator)Validator;

			MergeAttribute(context.Attributes, "data-val", "true");
			MergeAttribute(context.Attributes, "data-val-length", GetErrorMessage(lengthVal, context));
			MergeAttribute(context.Attributes, "data-val-length-max", lengthVal.Max.ToString());
			MergeAttribute(context.Attributes, "data-val-length-min", lengthVal.Min.ToString());
		}

		private string GetErrorMessage(LengthValidator lengthVal, ClientModelValidationContext context) {

			var formatter = ValidatorOptions.MessageFormatterFactory()
				.AppendPropertyName(Rule.GetDisplayName())
				.AppendArgument("MinLength", lengthVal.Min)
				.AppendArgument("MaxLength", lengthVal.Max);

			bool messageNeedsSplitting = lengthVal.ErrorMessageSource.ResourceType == typeof(LanguageManager);

			string message;
			try {
				message = lengthVal.ErrorMessageSource.GetString(null);
			}
			catch (FluentValidationMessageFormatException) {
				if (lengthVal is ExactLengthValidator) {
					message = ValidatorOptions.LanguageManager.GetStringForValidator<ExactLengthValidator>();
				}
				else {
					message = ValidatorOptions.LanguageManager.GetStringForValidator<LengthValidator>();
				}

				messageNeedsSplitting = true;
			}

			if (messageNeedsSplitting)
			{
				// If we're using the default resources then the mesage for length errors will have two parts, eg:
				// '{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.
				// We can't include the "TotalLength" part of the message because this information isn't available at the time the message is constructed.
				// Instead, we'll just strip this off by finding the index of the period that separates the two parts of the message.

				message = message.Substring(0, message.IndexOf(".") + 1);
			}

			message = formatter.BuildMessage(message);
			return message;
		}
	}
}