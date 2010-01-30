#region License
// Copyright 2008-2009 Jeremy Skinner (http://www.jeremyskinner.co.uk)
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
// The latest version of this file can be found at http://www.codeplex.com/FluentValidation
#endregion

namespace FluentValidation.Mvc {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;
	using Validators;


	/// <summary>
	/// Implementation of ModelValidatorProvider that uses FluentValidation.
	/// </summary>
	public class FluentValidationModelValidatorProvider : ModelValidatorProvider {
		readonly IValidatorFactory validatorFactory;
		public static bool AddImplicitRequiredValidator { get; set; }

		static FluentValidationModelValidatorProvider() {
			AddImplicitRequiredValidator = true;
		}

		public FluentValidationModelValidatorProvider(IValidatorFactory validatorFactory) {
			this.validatorFactory = validatorFactory;
		}

		public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context) {
			if (IsValidatingProperty(metadata)) {
				return GetValidatorsForProperty(metadata, context, validatorFactory.GetValidator(metadata.ContainerType));
			}

			return GetValidatorsForModel(metadata, context, validatorFactory.GetValidator(metadata.ModelType));
		}

		IEnumerable<ModelValidator> GetValidatorsForProperty(ModelMetadata metadata, ControllerContext context, IValidator validator) {
			var modelValidators = new List<ModelValidator>();

			if (validator != null) {
				var descriptor = validator.CreateDescriptor();

				var validators = descriptor.GetValidatorsForMember(metadata.PropertyName)
					.Where(x => x.SupportsStandaloneValidation);

				foreach(var propertyValidator in validators) {
					modelValidators.Add(new FluentValidationPropertyValidator(metadata, context, propertyValidator));
				}
			}

			if(metadata.IsRequired && AddImplicitRequiredValidator) {
				bool hasRequiredValidators = modelValidators.Any(x => x.IsRequired);

				//Is the model is 'Required' then we assume it must have a NotNullValidator. 
				//This is consistent with the behaviour of the DataAnnotationsModelValidatorProvider
				//which silently adds a RequiredAttribute

				if(! hasRequiredValidators) {
					modelValidators.Add(CreateNotNullValidatorForProperty(metadata, context));
				}
			}

			return modelValidators;
		}

		ModelValidator CreateNotNullValidatorForProperty(ModelMetadata metadata, ControllerContext cc) {
			var validator = new NotNullValidator();
			return new FluentValidationPropertyValidator(metadata, cc, validator);
		}

		IEnumerable<ModelValidator> GetValidatorsForModel(ModelMetadata metadata, ControllerContext context, IValidator validator) {
			if (validator != null) {
				yield return new FluentValidationModelValidator(metadata, context, validator);
			}
		}

		bool IsValidatingProperty(ModelMetadata metadata) {
			return metadata.ContainerType != null && !string.IsNullOrEmpty(metadata.PropertyName);
		}
	}
}