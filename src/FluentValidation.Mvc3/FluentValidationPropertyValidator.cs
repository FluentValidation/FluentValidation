namespace FluentValidation.Mvc {
	using System;
	using System.Collections.Generic;
	using System.Web.Mvc;
	using Internal;
	using Resources;
	using Validators;
	using System.Linq;

	public class FluentValidationPropertyValidator : ModelValidator {
		protected readonly string propertyDescription;
		protected readonly IPropertyValidator validator;

		/*
		 This might seem a bit strange, but we do *not* want to do any work in these validators.
		 They should only be used for metadata purposes.
		 This is so that the validation can be left to the actual FluentValidationModelValidator.
		 The exception to this is the Required validator - these *do* need to run standalone
		 in order to bypass MVC's "A value is required" message which cannot be turned off.
		*/
		protected bool ShouldValidate { get; set; }

		public FluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, string propertyDescription, IPropertyValidator validator) : base(metadata, controllerContext) {
			this.propertyDescription = propertyDescription;
			this.validator = validator;
		}

		public override IEnumerable<ModelValidationResult> Validate(object container) {
			if (ShouldValidate) {
				var context = new PropertyValidatorContext(Metadata.PropertyName, container, Metadata.Model, Metadata.PropertyName);
				var result = validator.Validate(context);

				foreach (var failure in result) {
					yield return new ModelValidationResult { Message = failure.ErrorMessage };
				}
			}
		}

		public static ModelValidator Create(ModelMetadata meta, ControllerContext context, string propertyDescription, IPropertyValidator validator) {
			return new FluentValidationPropertyValidator(meta, context, propertyDescription, validator);
		}

		protected bool TypeAllowsNullValue(Type type) {
			return (!type.IsValueType || Nullable.GetUnderlyingType(type) != null);
		}
	}

	internal class RequiredFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		public RequiredFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, string propertyDescription, IPropertyValidator validator) : base(metadata, controllerContext, propertyDescription, validator) {
			bool isNonNullableValueType = !TypeAllowsNullValue(metadata.ModelType);
			bool nullWasSpecified = metadata.Model == null;

			ShouldValidate = isNonNullableValueType && nullWasSpecified;
		}

		public new static ModelValidator Create(ModelMetadata meta, ControllerContext context, string propertyDescription, IPropertyValidator validator) {
			return new RequiredFluentValidationPropertyValidator(meta, context, propertyDescription, validator);
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			var formatter = new MessageFormatter().AppendPropertyName(propertyDescription);
			var message = formatter.BuildMessage(validator.ErrorMessageSource.GetString());
			return new[] { new ModelClientValidationRequiredRule(message) };
		}

		public override bool IsRequired {
			get { return true; }
		}
	}

	internal class StringLengthFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		private ILengthValidator LengthValidator {
			get { return (ILengthValidator)validator; }
		}

		public StringLengthFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, string propertyDescription, IPropertyValidator validator)
			: base(metadata, controllerContext, propertyDescription, validator) {
			ShouldValidate = false;
		}

		public new static ModelValidator Create(ModelMetadata meta, ControllerContext context, string propertyDescription, IPropertyValidator validator) {
			return new StringLengthFluentValidationPropertyValidator(meta, context, propertyDescription, validator);
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			var formatter = new MessageFormatter().AppendPropertyName(propertyDescription);
			string message = LengthValidator.ErrorMessageSource.GetString();

			if(LengthValidator.ErrorMessageSource.ResourceType == typeof(Messages)) {
				// If we're using the default resources then the mesage for length errors will have two parts, eg:
				// '{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.
				// We can't include the "TotalLength" part of the message because this information isn't available at the time the message is constructed.
				// Instead, we'll just strip this off by finding the index of the period that separates the two parts of the message.

				message = message.Substring(0, message.IndexOf(".") + 1);

				// also append the min/max bits
				formatter.AppendArgument("MinLength", LengthValidator.Min)
					.AppendArgument("MaxLength", LengthValidator.Max);
			}

			message = formatter.BuildMessage(message);

			return new[] { new ModelClientValidationStringLengthRule(message, LengthValidator.Min, LengthValidator.Max) };
		}
	}

	internal class RegularExpressionFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		IRegularExpressionValidator RegexValidator {
			get { return (IRegularExpressionValidator)validator;}
		}

		public RegularExpressionFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, string propertyDescription, IPropertyValidator validator)
			: base(metadata, controllerContext, propertyDescription, validator) {
			ShouldValidate = false;
		}

		public new static ModelValidator Create(ModelMetadata meta, ControllerContext context, string propertyDescription, IPropertyValidator validator) {
			return new RegularExpressionFluentValidationPropertyValidator(meta, context, propertyDescription, validator);
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			var formatter = new MessageFormatter().AppendPropertyName(propertyDescription);
			string message = formatter.BuildMessage(RegexValidator.ErrorMessageSource.GetString());
			return new[] { new ModelClientValidationRegexRule(message, RegexValidator.Expression) };

		}
	}

	//TODO: Range.
}