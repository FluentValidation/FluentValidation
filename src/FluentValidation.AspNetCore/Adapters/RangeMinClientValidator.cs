namespace FluentValidation.AspNetCore {
	using Internal;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using Resources;
	using System;
	using System.Globalization;
	using Validators;

	internal class RangeMinClientValidator : ClientValidatorBase {
		IComparisonValidator RangeValidator => (IComparisonValidator)Validator;

		public RangeMinClientValidator(IValidationRule rule, IPropertyValidator validator) : base(rule, validator) {

		}

		public override void AddValidation(ClientModelValidationContext context) {
			var compareValue = RangeValidator.ValueToCompare;

			if (compareValue != null) {
				MergeAttribute(context.Attributes, "data-val", "true");
				MergeAttribute(context.Attributes, "data-val-range", GetErrorMessage(context));
				MergeAttribute(context.Attributes, "data-val-range-min", Convert.ToString(compareValue, CultureInfo.InvariantCulture));
			}
		}

		private string GetErrorMessage(ClientModelValidationContext context) {
			var cfg = context.ActionContext.HttpContext.RequestServices.GetValidatorConfiguration();

			var formatter = cfg.MessageFormatterFactory()
				.AppendPropertyName(Rule.GetDisplayName(null))
				.AppendArgument("ComparisonValue", RangeValidator.ValueToCompare);

			string message;

			try {
				message = RangeValidator.GetUnformattedErrorMessage();
			}
			catch (NullReferenceException) {
				message = cfg.LanguageManager.GetString("GreaterThanOrEqualValidator");
			}

			message = formatter.BuildMessage(message);

			return message;
		}
	}
}
