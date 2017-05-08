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

namespace FluentValidation.WebApi
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Http.Metadata;
	using System.Web.Http.Validation;

	using FluentValidation.Internal;
	using FluentValidation.Results;

	public class FluentValidationModelValidator : ModelValidator {
		readonly IValidator validator;

		public FluentValidationModelValidator(IEnumerable<ModelValidatorProvider> validatorProviders, IValidator validator)
			: base(validatorProviders) {
			this.validator = validator;
		}

		public override IEnumerable<ModelValidationResult> Validate(ModelMetadata metadata, object container) {
			if (metadata.Model != null) {
				var selector = ValidatorOptions.ValidatorSelectors.DefaultValidatorSelectorFactory();
				var context = new ValidationContext(metadata.Model, new PropertyChain(), selector);

				var result = validator.Validate(context);

				if (!result.IsValid) {
					return ConvertValidationResultToModelValidationResults(result);
				}
			}
			return Enumerable.Empty<ModelValidationResult>();
		}

		protected virtual IEnumerable<ModelValidationResult> ConvertValidationResultToModelValidationResults(ValidationResult result) {
			return result.Errors.Select(x => new ModelValidationResult
			{
				MemberName = x.PropertyName,
				Message = x.ErrorMessage
			});
		}
	}
}