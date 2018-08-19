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
	using System.Web.Http.Controllers;
	using System.Web.Http.Metadata;
	using System.Web.Http.Validation;

	using FluentValidation.Internal;
	using FluentValidation.Results;

	public class FluentValidationModelValidator : ModelValidator {
		private readonly IValidator _validator;

		internal CustomizeValidatorAttribute Customizations { get; private set; }
		internal HttpActionContext ActionContext { get; private set; }
		
		public FluentValidationModelValidator(IEnumerable<ModelValidatorProvider> validatorProviders, IValidator validator)
			: base(validatorProviders) {
			this._validator = validator;
		}

		public override IEnumerable<ModelValidationResult> Validate(ModelMetadata metadata, object container) {
			if (metadata.Model != null) {

				var customizations = Customizations ?? new CustomizeValidatorAttribute();
				
				if (customizations.Skip) {
					return Enumerable.Empty<ModelValidationResult>();
				}

				var selector = customizations.ToValidatorSelector();
				var interceptor = customizations.GetInterceptor() ?? (_validator as IValidatorInterceptor);
				var context = new FluentValidation.ValidationContext(metadata.Model, new FluentValidation.Internal.PropertyChain(), selector);
				context.RootContextData["InvokedByWebApi"] = true;


				if (interceptor != null) {
					// Allow the user to provide a customized context
					// However, if they return null then just use the original context.
					context = interceptor.BeforeMvcValidation(ActionContext, context) ?? context;
				}

				var result = _validator.Validate(context);
				
				if (interceptor != null) {
					// allow the user to provide a custom collection of failures, which could be empty.
					// However, if they return null then use the original collection of failures. 
					result = interceptor.AfterMvcValidation(ActionContext, context, result) ?? result;
				}

				if (!result.IsValid) {
					return ConvertValidationResultToModelValidationResults(result);
				}
			}
			return Enumerable.Empty<ModelValidationResult>();
		}

		protected virtual IEnumerable<ModelValidationResult> ConvertValidationResultToModelValidationResults(ValidationResult result) {
			return result.Errors.Select(x => new ModelValidationResult {
				MemberName = x.PropertyName,
				Message = x.ErrorMessage
			});
		}

		public FluentValidationModelValidator CloneWithCustomizations(CustomizeValidatorAttribute customizations, HttpActionContext context) {
			return new FluentValidationModelValidator(ValidatorProviders, _validator) {
				ActionContext = context,
				Customizations = customizations
			};
		}
	}
}