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
	using System;
	using System.Collections.Generic;
	using System.Web.Http;
	using System.Web.Http.Metadata;
	using System.Web.Http.Validation;

	using FluentValidation.Attributes;
	using FluentValidation.Internal;
	using FluentValidation.Validators;

	public delegate ModelValidator FluentValidationModelValidationFactory(ModelMetadata metadata, IEnumerable<ModelValidatorProvider> validatorProviders, PropertyRule rule, IPropertyValidator validator);


	public class FluentValidationModelValidatorProvider : ModelValidatorProvider {
		public IValidatorFactory ValidatorFactory { get; set; }


		/// <summary>
		/// Enabling this maintains compatibility with FluentValidation 6.4, where discovery of validators was limited to top level models. 
		/// </summary>
		public bool DisableDiscoveryOfPropertyValidators { get; set; } = false;

		public FluentValidationModelValidatorProvider(IValidatorFactory validatorFactory = null) {
			ValidatorFactory = validatorFactory ?? new AttributedValidatorFactory();
		}

		/// <summary>
		/// Initializes the FluentValidationModelValidatorProvider using the default options and adds it in to the ModelValidatorProviders collection.
		/// </summary>
		public static void Configure(HttpConfiguration configuration, Action<FluentValidationModelValidatorProvider> configurationExpression = null) {
			configurationExpression = configurationExpression ?? delegate { };

			var provider = new FluentValidationModelValidatorProvider();
			configurationExpression(provider);
		    configuration.Services.Replace(typeof(IBodyModelValidator), new FluentValidationBodyModelValidator());
			configuration.Services.Add(typeof(ModelValidatorProvider), provider);
		}

		public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, IEnumerable<ModelValidatorProvider> validatorProviders)
		{
			if (DisableDiscoveryOfPropertyValidators && IsValidatingProperty(metadata)) {
				yield break;
			}

			IValidator validator = ValidatorFactory.GetValidator(metadata.ModelType);
			
			if (validator == null) {
				yield break;
			}

			yield return new FluentValidationModelValidator(validatorProviders, validator);
		}

		protected virtual bool IsValidatingProperty(ModelMetadata metadata) {
			return metadata.ContainerType != null && !string.IsNullOrEmpty(metadata.PropertyName);
		}
	}
}