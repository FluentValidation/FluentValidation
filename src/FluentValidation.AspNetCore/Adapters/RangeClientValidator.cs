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
namespace FluentValidation.AspNetCore {
	using Internal;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using Resources;
	using System;
	using System.Globalization;
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
				MergeAttribute(context.Attributes, "data-val-range-max", Convert.ToString(RangeValidator.To, CultureInfo.InvariantCulture));
				MergeAttribute(context.Attributes, "data-val-range-min", Convert.ToString(RangeValidator.From, CultureInfo.InvariantCulture));
			}
		}

		private string GetErrorMessage(ClientModelValidationContext context) {
			var cfg = context.ActionContext.HttpContext.RequestServices.GetValidatorConfiguration();

			var formatter = cfg.MessageFormatterFactory()
				.AppendPropertyName(Rule.GetDisplayName(null))
				.AppendArgument("From", RangeValidator.From)
				.AppendArgument("To", RangeValidator.To);

			string message;

			try {
				message = RangeValidator.Options.ErrorMessageFactory.Invoke(null);
			}
			catch (FluentValidationMessageFormatException) {
				message = cfg.LanguageManager.GetString("InclusiveBetween_Simple");
			}
			catch (NullReferenceException) {
				message = cfg.LanguageManager.GetString("InclusiveBetween_Simple");
			}

			if (message.Contains("{Value}")) {
				message = cfg.LanguageManager.GetString("InclusiveBetween_Simple");
			}
			message = formatter.BuildMessage(message);

			return message;
		}
	}
}
