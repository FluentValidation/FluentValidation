﻿#region License
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
	using System;
	using System.Collections.Generic;
	using Internal;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using Resources;
	using Validators;

	internal class RegexClientValidator : ClientValidatorBase {

		public RegexClientValidator(IValidationRule rule, IRuleComponent component)
			: base(rule, component) {
		}

		public override void AddValidation(ClientModelValidationContext context) {
			var cfg = context.ActionContext.HttpContext.RequestServices.GetValidatorConfiguration();
			var regexVal = (IRegularExpressionValidator)Validator;
			var formatter = cfg.MessageFormatterFactory().AppendPropertyName(Rule.GetDisplayName(null));
			string messageTemplate;
			try {
				messageTemplate = Component.GetUnformattedErrorMessage();
			}
			catch (NullReferenceException) {
				messageTemplate = cfg.LanguageManager.GetString("RegularExpressionValidator");
			}

			string message = formatter.BuildMessage(messageTemplate);

			MergeAttribute(context.Attributes, "data-val", "true");
			MergeAttribute(context.Attributes, "data-val-regex", message);
			MergeAttribute(context.Attributes, "data-val-regex-pattern", regexVal.Expression);
		}
	}
}
