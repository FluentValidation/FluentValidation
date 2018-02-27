namespace FluentValidation.AspNetCore {
	using System.Collections.Generic;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Internal;
	using Microsoft.AspNetCore.Mvc.ModelBinding;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

	internal class FluentValidationVisitor : ValidationVisitor {
		public bool ValidateChildren { get; set; }
		
		public FluentValidationVisitor(ActionContext actionContext, IModelValidatorProvider validatorProvider, ValidatorCache validatorCache, IModelMetadataProvider metadataProvider, ValidationStateDictionary validationState) : base(actionContext, validatorProvider, validatorCache, metadataProvider, validationState) {
			this.ValidateComplexTypesIfChildValidationFails = true;
		}

		protected override bool VisitChildren(IValidationStrategy strategy) {
			// If validting a collection property skip validation if validate children is off. 
			// However we can't actually skip it here as otherwise this will affect DataAnnotaitons validation too.
			// Instead store a list of objects to skip in the context, which the validator will check later. 
			if (!ValidateChildren && Metadata.ValidateChildren && Metadata.IsCollectionType && Metadata.MetadataKind == ModelMetadataKind.Property) {

				var skip = Context.HttpContext.Items.ContainsKey("_FV_SKIP") ? Context.HttpContext.Items["_FV_SKIP"] as HashSet<object> : null;

				if (skip == null) {
					skip = new HashSet<object>();
					Context.HttpContext.Items["_FV_SKIP"] = skip;
				}

				skip.Add(Model); 
			}

			return base.VisitChildren(strategy);
		}
	}
}