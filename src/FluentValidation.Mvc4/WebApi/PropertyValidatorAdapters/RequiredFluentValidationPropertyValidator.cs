namespace FluentValidation.Mvc.WebApi.PropertyValidatorAdapters
{
	using System.Collections.Generic;
	using System.Web.Http.Metadata;
	using System.Web.Http.Validation;

	using FluentValidation.Internal;
	using FluentValidation.Validators;

	internal class RequiredFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		public RequiredFluentValidationPropertyValidator(ModelMetadata metadata, IEnumerable<ModelValidatorProvider> validatorProviders, PropertyRule rule, IPropertyValidator validator) : base(metadata, validatorProviders, rule, validator) {
		}

		public override bool IsRequired {
			get { return true; }
		} 
	}
}