namespace FluentValidation.AspNetCore {
	using System.Collections.Generic;
	using System.Reflection;
	using Internal;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using Resources;
	using Validators;

	internal class EqualToClientValidator : ClientValidatorBase {
		EqualValidator EqualValidator {
			get { return (EqualValidator)Validator; }
		}
		
		public EqualToClientValidator(PropertyRule rule, IPropertyValidator validator) : base(rule, validator) {
		}

		public override void AddValidation(ClientModelValidationContext context) {

			var propertyToCompare = EqualValidator.MemberToCompare as PropertyInfo;

			if (propertyToCompare != null)
			{
				// If propertyToCompare is not null then we're comparing to another property.
				// If propertyToCompare is null then we're either comparing against a literal value, a field or a method call.
				// We only care about property comparisons in this case.

				var comparisonDisplayName =
					ValidatorOptions.DisplayNameResolver(Rule.TypeToValidate, propertyToCompare, null)
					?? propertyToCompare.Name.SplitPascalCase();

				var formatter = new MessageFormatter()
					.AppendPropertyName(Rule.GetDisplayName())
					.AppendArgument("ComparisonValue", comparisonDisplayName);

				string messageTemplate;
				try {
					messageTemplate = EqualValidator.ErrorMessageSource.GetString(null);
				}
				catch (FluentValidationMessageFormatException) {
					messageTemplate = Messages.equal_error;
				}
				string message = formatter.BuildMessage(messageTemplate);
				MergeAttribute(context.Attributes, "data-val", "true");
				MergeAttribute(context.Attributes, "data-val-equalto", message);
				MergeAttribute(context.Attributes, "data-val-equalto-other", propertyToCompare.Name);
			}
		
		}

	}
}