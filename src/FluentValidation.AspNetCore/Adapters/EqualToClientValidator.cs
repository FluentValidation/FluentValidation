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
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Internal;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using Resources;
	using Validators;

	internal class EqualToClientValidator : ClientValidatorBase {
		EqualValidator EqualValidator => (EqualValidator)Validator;

		public EqualToClientValidator(IValidationRule rule, IPropertyValidator validator) : base(rule, validator) {
		}

		public override void AddValidation(ClientModelValidationContext context) {

			var propertyToCompare = EqualValidator.MemberToCompare as PropertyInfo;

			if (propertyToCompare != null) {
				var cfg = context.ActionContext.HttpContext.RequestServices.GetValidatorConfiguration();
				// If propertyToCompare is not null then we're comparing to another property.
				// If propertyToCompare is null then we're either comparing against a literal value, a field or a method call.
				// We only care about property comparisons in this case.

				var comparisonDisplayName =
					cfg.DisplayNameResolver(Rule.TypeToValidate, propertyToCompare, null)
					?? propertyToCompare.Name.SplitPascalCase();

				var formatter = cfg.MessageFormatterFactory()
					.AppendPropertyName(Rule.GetDisplayName(null))
					.AppendArgument("ComparisonValue", comparisonDisplayName);

				string messageTemplate;
				try {
					messageTemplate = EqualValidator.Options.GetErrorMessage(null);
				}
				catch (NullReferenceException) {
					messageTemplate = cfg.LanguageManager.GetStringForValidator<EqualValidator>();
				}
				string message = formatter.BuildMessage(messageTemplate);
				MergeAttribute(context.Attributes, "data-val", "true");
				MergeAttribute(context.Attributes, "data-val-equalto", message);
				MergeAttribute(context.Attributes, "data-val-equalto-other", "*." + propertyToCompare.Name);
			}

		}

	}
}
