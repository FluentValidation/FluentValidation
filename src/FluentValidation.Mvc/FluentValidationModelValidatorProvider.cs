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
	using System.Collections.Generic;
	using System.Web.Mvc;


	/// <summary>
	/// Implementation of ModelValidatorProvider that uses FluentValidation.
	/// Note: We do not support retrieving validators for individual properties, only for the containing object.
	/// </summary>
	public class FluentValidationModelValidatorProvider : ModelValidatorProvider {
		public delegate ModelValidator ModelValidatorFactory(ModelMetadata metaData, ControllerContext context, IValidatorFactory validatorFactory);

		readonly IValidatorFactory validatorFactory;
		readonly ModelValidatorFactory modelValidatorFactory;

		public FluentValidationModelValidatorProvider(IValidatorFactory validatorFactory)
			: this(validatorFactory, CreateDefaultValidator) {
		}

		public FluentValidationModelValidatorProvider(IValidatorFactory validatorFactory, ModelValidatorFactory modelValidatorFactory) {
			this.validatorFactory = validatorFactory;
			this.modelValidatorFactory = modelValidatorFactory;
		}

		public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context) {

			//We do not support retrieving validators for individual properties - only for the entire object.
			if (metadata.ContainerType != null && !string.IsNullOrEmpty(metadata.PropertyName)) {
				yield break;
			}

			yield return modelValidatorFactory(metadata, context, validatorFactory);
		}

		static ModelValidator CreateDefaultValidator(ModelMetadata metadata, ControllerContext context, IValidatorFactory validatorFactory) {
			return new FluentValidationModelValidator(metadata, context, validatorFactory);
		}
	}
}