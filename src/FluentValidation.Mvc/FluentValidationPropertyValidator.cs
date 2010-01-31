namespace FluentValidation.Mvc {
	using System.Collections.Generic;
	using System.Web.Mvc;
	using Validators;
	using System.Linq;

	public class FluentValidationPropertyValidator : ModelValidator {
		protected readonly IPropertyValidator validator;

		public FluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, IPropertyValidator validator) : base(metadata, controllerContext) {
			this.validator = validator;
		}

		public override IEnumerable<ModelValidationResult> Validate(object container) {
			var context = new PropertyValidatorContext(Metadata.PropertyName, container, Metadata.Model, Metadata.PropertyName);
			var result = validator.Validate(context);

			if(! result.IsValid) {
				yield return new ModelValidationResult {
                    Message = result.Error
				};
			}
		}

		public static ModelValidator Create(ModelMetadata meta, ControllerContext context, IPropertyValidator validator) {
			return new FluentValidationPropertyValidator(meta, context, validator);
		}
	}

	internal class RequiredFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		public RequiredFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, IPropertyValidator validator) : base(metadata, controllerContext, validator) {
		}

		public new static ModelValidator Create(ModelMetadata meta, ControllerContext context, IPropertyValidator validator) {
			return new RequiredFluentValidationPropertyValidator(meta, context, validator);
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			return new[] { new ModelClientValidationRequiredRule(validator.ErrorMessageTemplate) };
		}

		public override bool IsRequired {
			get { return true; }
		}
	}

	internal class StringLengthFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		private ILengthValidator LengthValidator {
			get { return (ILengthValidator)validator; }
		}

		public StringLengthFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, IPropertyValidator validator) : base(metadata, controllerContext, validator) {

		}

		public new static ModelValidator Create(ModelMetadata meta, ControllerContext context, IPropertyValidator validator) {
			return new StringLengthFluentValidationPropertyValidator(meta, context, validator);
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			return new[] { new ModelClientValidationStringLengthRule(LengthValidator.ErrorMessageTemplate, LengthValidator.Min, LengthValidator.Max) };
		}
	}

	internal class RegularExpressionFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		IRegularExpressionValidator RegexValidator {
			get { return (IRegularExpressionValidator)validator;}
		}

		public RegularExpressionFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, IPropertyValidator validator) : base(metadata, controllerContext, validator) {
		}

		public new static ModelValidator Create(ModelMetadata meta, ControllerContext context, IPropertyValidator validator) {
			return new RegularExpressionFluentValidationPropertyValidator(meta, context, validator);
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			return new[] { new ModelClientValidationRegexRule(RegexValidator.ErrorMessageTemplate, RegexValidator.Expression) };

		}
	}

	//TODO: Range.
}