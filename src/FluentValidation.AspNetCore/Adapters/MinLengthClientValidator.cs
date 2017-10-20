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
    using Internal;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
    using Resources;
    using Validators;

    internal class MinLengthClientValidator :ClientValidatorBase {

		public override void AddValidation(ClientModelValidationContext context) {
		    var lengthVal = (MinimumLengthValidator)Validator;

		    MergeAttribute(context.Attributes, "data-val", "true");
		    MergeAttribute(context.Attributes, "data-val-minlength", GetErrorMessage(lengthVal, context));
		    MergeAttribute(context.Attributes, "data-val-minlength-min", lengthVal.Min.ToString());
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
		    } catch (FluentValidationMessageFormatException) {
				message = ValidatorOptions.LanguageManager.GetStringForValidator<MinimumLengthValidator>();
			    messageNeedsSplitting = true;
		    }

		    if (messageNeedsSplitting) {
			    // If we're using the default resources then the mesage for length errors will have two parts, eg:
			    // '{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.
			    // We can't include the "TotalLength" part of the message because this information isn't available at the time the message is constructed.
			    // Instead, we'll just strip this off by finding the index of the period that separates the two parts of the message.

			    message = message.Substring(0, message.IndexOf(".") + 1);
		    }

		    message = formatter.BuildMessage(message);
		    return message;
	    }

	    public MinLengthClientValidator(PropertyRule rule, IPropertyValidator validator) : base(rule, validator) {
	    }
    }
}