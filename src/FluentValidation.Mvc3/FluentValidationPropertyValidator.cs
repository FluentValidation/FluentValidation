namespace FluentValidation.Mvc {
	using System;
	using System.Collections.Generic;
	using System.Reflection;
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
		 Basically, this is all just to bypass the bad design in ASP.NET MVC. Boo, hiss. 
		*/
		protected bool ShouldValidate { get; set; }

		public FluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, string propertyDescription, IPropertyValidator validator) : base(metadata, controllerContext) {
			this.propertyDescription = propertyDescription;
			this.validator = validator;
		}

		public override IEnumerable<ModelValidationResult> Validate(object container) {
			if (ShouldValidate) {
				var fakeRule = new PropertyRule(null, x => Metadata.Model, null, null, Metadata.ModelType, null) {
					PropertyName = Metadata.PropertyName
				};

				var fakeParentContext = new ValidationContext(container);
				var context = new PropertyValidatorContext(fakeParentContext, fakeRule, Metadata.PropertyName);
				var result = validator.Validate(context);

				foreach (var failure in result) {
					yield return new ModelValidationResult { Message = failure.ErrorMessage };
				}
			}
		}

		protected bool TypeAllowsNullValue(Type type) {
			return (!type.IsValueType || Nullable.GetUnderlyingType(type) != null);
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			var supportsClientValidation = validator as IClientValidatable;
			
			if(supportsClientValidation != null) {
				return supportsClientValidation.GetClientValidationRules(Metadata, ControllerContext);
			}

			return Enumerable.Empty<ModelClientValidationRule>();
		}
	}

	internal class RequiredFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		public RequiredFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, string propertyDescription, IPropertyValidator validator) : base(metadata, controllerContext, propertyDescription, validator) {
			bool isNonNullableValueType = !TypeAllowsNullValue(metadata.ModelType);
			bool nullWasSpecified = metadata.Model == null;

			ShouldValidate = isNonNullableValueType && nullWasSpecified;
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			var formatter = new MessageFormatter().AppendPropertyName(propertyDescription);
			var message = formatter.BuildMessage(validator.ErrorMessageSource.GetString());
			yield return new ModelClientValidationRequiredRule(message);
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

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			var formatter = new MessageFormatter()
				.AppendPropertyName(propertyDescription)
				.AppendArgument("MinLength", LengthValidator.Min)
				.AppendArgument("MaxLength", LengthValidator.Max);

			string message = LengthValidator.ErrorMessageSource.GetString();

			if(LengthValidator.ErrorMessageSource.ResourceType == typeof(Messages)) {
				// If we're using the default resources then the mesage for length errors will have two parts, eg:
				// '{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.
				// We can't include the "TotalLength" part of the message because this information isn't available at the time the message is constructed.
				// Instead, we'll just strip this off by finding the index of the period that separates the two parts of the message.

				message = message.Substring(0, message.IndexOf(".") + 1);
			}

			message = formatter.BuildMessage(message);

			yield return new ModelClientValidationStringLengthRule(message, LengthValidator.Min, LengthValidator.Max) ;
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

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			var formatter = new MessageFormatter().AppendPropertyName(propertyDescription);
			string message = formatter.BuildMessage(RegexValidator.ErrorMessageSource.GetString());
			yield return new ModelClientValidationRegexRule(message, RegexValidator.Expression);
		}
	}

	internal class RangeFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		InclusiveBetweenValidator RangeValidator {
			get { return (InclusiveBetweenValidator)validator; }
		}
		
		public RangeFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, string propertyDescription, IPropertyValidator validator) : base(metadata, controllerContext, propertyDescription, validator) {
			ShouldValidate=false;
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			var formatter = new MessageFormatter()
				.AppendPropertyName(propertyDescription)
				.AppendArgument("From", RangeValidator.From)
				.AppendArgument("To", RangeValidator.To);

			string message = RangeValidator.ErrorMessageSource.GetString();

			if (RangeValidator.ErrorMessageSource.ResourceType == typeof(Messages)) {
				// If we're using the default resources then the mesage for length errors will have two parts, eg:
				// '{PropertyName}' must be between {From} and {To}. You entered {Value}.
				// We can't include the "Value" part of the message because this information isn't available at the time the message is constructed.
				// Instead, we'll just strip this off by finding the index of the period that separates the two parts of the message.

				message = message.Substring(0, message.IndexOf(".") + 1);
			}

			message = formatter.BuildMessage(message);

			yield return new ModelClientValidationRangeRule(message, RangeValidator.From, RangeValidator.To);
		}
	}

	internal class EqualToFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		EqualValidator EqualValidator {
			get { return (EqualValidator)validator; }
		}
		
		public EqualToFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, string propertyDescription, IPropertyValidator validator) : base(metadata, controllerContext, propertyDescription, validator) {
			ShouldValidate = false;
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			var propertyToCompare = EqualValidator.MemberToCompare as PropertyInfo;
			if(propertyToCompare != null) {
				// If propertyToCompare is not null then we're comparing to another property.
				// If propertyToCompare is null then we're either comparing against a literal value, a field or a method call.
				// We only care about property comparisons in this case.

				var formatter = new MessageFormatter()
					.AppendPropertyName(propertyDescription)
					.AppendArgument("PropertyValue", propertyToCompare.Name);


				string message = formatter.BuildMessage(EqualValidator.ErrorMessageSource.GetString());
				yield return new ModelClientValidationEqualToRule(message, CompareAttribute.FormatPropertyForClientValidation(propertyToCompare.Name)) ;
			}
		}
	}

	internal class EmailFluentValidationPropertyValidator : FluentValidationPropertyValidator {
		private IEmailValidator EmailValidator {
			get { return (IEmailValidator)validator; }
		}

		public EmailFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext, string propertyDescription, IPropertyValidator validator) : base(metadata, controllerContext, propertyDescription, validator) {
			ShouldValidate=false;
		}

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			var formatter = new MessageFormatter().AppendPropertyName(propertyDescription);
			string message = formatter.BuildMessage(EmailValidator.ErrorMessageSource.GetString());
			
			yield return new ModelClientValidationRule {
				ValidationType = "email",
				ErrorMessage = message
			};
		}
	}
}