namespace FluentValidation.AspNetCore {
	using Internal;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using Resources;
	using System;
	using System.Globalization;
	using Validators;

	internal class RangeMaxClientValidator : ClientValidatorBase {
		IComparisonValidator RangeValidator => (IComparisonValidator)Validator;

		public RangeMaxClientValidator(IValidationRule rule, IRuleComponent component) : base(rule, component) {
		}

		public override void AddValidation(ClientModelValidationContext context) {
			var compareValue = RangeValidator.ValueToCompare;

			if (compareValue != null) {
				MergeAttribute(context.Attributes, "data-val", "true");
				MergeAttribute(context.Attributes, "data-val-range", GetErrorMessage(context));
				MergeAttribute(context.Attributes, "data-val-range-max", Convert.ToString(compareValue, CultureInfo.InvariantCulture));
				MergeAttribute(context.Attributes, "data-val-range-min", "0");
			}
		}

		private string GetErrorMessage(ClientModelValidationContext context) {
			var cfg = context.ActionContext.HttpContext.RequestServices.GetValidatorConfiguration();

			var formatter = cfg.MessageFormatterFactory()
				.AppendPropertyName(Rule.GetDisplayName(null))
				.AppendArgument("ComparisonValue", RangeValidator.ValueToCompare);

			string message;

			try {
				message = Component.GetUnformattedErrorMessage();
			}
			catch (NullReferenceException) {
				message = cfg.LanguageManager.GetString("LessThanOrEqualValidator");
			}

			message = formatter.BuildMessage(message);

			return message;
		}
	}
}
