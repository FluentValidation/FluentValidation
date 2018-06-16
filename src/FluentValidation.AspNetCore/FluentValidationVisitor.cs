namespace FluentValidation.AspNetCore {
	using System.Collections.Generic;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Internal;
	using Microsoft.AspNetCore.Mvc.ModelBinding;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

	internal class FluentValidationVisitor : ValidationVisitor {
		public FluentValidationVisitor(ActionContext actionContext, IModelValidatorProvider validatorProvider, ValidatorCache validatorCache, IModelMetadataProvider metadataProvider, ValidationStateDictionary validationState) 
			: base(actionContext, validatorProvider, validatorCache, metadataProvider, validationState) {
			
			ValidateComplexTypesIfChildValidationFails = true;
		}

		public override bool Validate(ModelMetadata metadata, string key, object model, bool alwaysValidateAtTopLevel) {
			Context.HttpContext.Items["_FV_ROOT_METADATA"] = metadata;
			return base.Validate(metadata, key, model, alwaysValidateAtTopLevel);
		}
	}
}