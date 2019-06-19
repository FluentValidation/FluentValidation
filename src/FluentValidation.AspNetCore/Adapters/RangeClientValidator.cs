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
			var cfg = context.ActionContext.HttpContext.RequestServices.GetValidatorConfiguration();

			var formatter = cfg.MessageFormatterFactory()
				.AppendPropertyName(Rule.GetDisplayName())
				.AppendArgument("From", RangeValidator.From)
				.AppendArgument("To", RangeValidator.To);

			var needsSimplifiedMessage = RangeValidator.Options.ErrorMessageSource is LanguageStringSource;

			string message;

			try {
				message = RangeValidator.Options.ErrorMessageSource.GetString(null);
			}
			catch (FluentValidationMessageFormatException) {
				message = cfg.LanguageManager.GetString("InclusiveBetween_Simple");
				needsSimplifiedMessage = false;
			}

			if (needsSimplifiedMessage && message.Contains("{Value}")) {
				message = cfg.LanguageManager.GetString("InclusiveBetween_Simple");
			}
			message = formatter.BuildMessage(message);

			return message;
		}
	}
}
