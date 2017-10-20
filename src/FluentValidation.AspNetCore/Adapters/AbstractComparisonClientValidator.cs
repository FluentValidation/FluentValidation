/*#region License
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
    using System;
    using System.Collections.Generic;
    using Internal;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
    using Resources;
    using Validators;

    internal abstract class AbstractComparisonClientValidator<TValidator> : ClientValidatorBase 
        where TValidator: AbstractComparisonValidator {

        protected TValidator AbstractComparisonValidator
        {
            get { return (TValidator)Validator; }
        }

        protected abstract Object MinValue { get; }
        protected abstract Object MaxValue { get; }

        protected AbstractComparisonClientValidator(PropertyRule rule, TValidator validator) : base(rule, validator) {
        }

		protected string GetErrorMessage(ClientModelValidationContext context) {
			var formatter = ValidatorOptions.MessageFormatterFactory()
			  .AppendPropertyName(Rule.GetDisplayName())
			  .AppendArgument("ComparisonValue", AbstractComparisonValidator.ValueToCompare);

			string message;
			try {
				message = AbstractComparisonValidator.ErrorMessageSource.GetString(null);
			}
			catch (FluentValidationMessageFormatException) {
				message = GetDefaultMessage();
			}
			message = formatter.BuildMessage(message);

			return message;
		}

	    protected abstract string GetDefaultMessage();
    }
}*/