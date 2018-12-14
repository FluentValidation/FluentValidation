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
	using System.Linq;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.ModelBinding;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
#if NETSTANDARD2_0
	// For aspnetcore 2.x (targetting NS2.0), the ValidationVisitor class lives in the .Internal namespace
	using Microsoft.AspNetCore.Mvc.Internal;
#endif
	
	internal class FluentValidationObjectModelValidator : ObjectModelValidator {
		private readonly bool _runMvcValidation;
		private readonly FluentValidationModelValidatorProvider _fvProvider;

		public FluentValidationObjectModelValidator(
			IModelMetadataProvider modelMetadataProvider,
			IList<IModelValidatorProvider> validatorProviders, bool runMvcValidation)
		: base(modelMetadataProvider, validatorProviders) {
			_runMvcValidation = runMvcValidation;
			_fvProvider = validatorProviders.SingleOrDefault(x => x is FluentValidationModelValidatorProvider) as FluentValidationModelValidatorProvider;
		}
		
		public override ValidationVisitor GetValidationVisitor(ActionContext actionContext, IModelValidatorProvider validatorProvider, ValidatorCache validatorCache, IModelMetadataProvider metadataProvider, ValidationStateDictionary validationState) {
			// Setting as to whether we should run only FV or FV + the other validator providers
			var validatorProviderToUse = _runMvcValidation ? validatorProvider : _fvProvider;

			var visitor = new FluentValidationVisitor(
				actionContext,
				validatorProviderToUse,
				validatorCache,
				metadataProvider,
				validationState);

			return visitor;
		}
	}
}