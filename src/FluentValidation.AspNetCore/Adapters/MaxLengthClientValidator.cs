namespace FluentValidation.AspNetCore {
    using Internal;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
    using Resources;
    using Validators;

    internal class MaxLengthClientValidator : AbstractComparisonClientValidator<LessThanOrEqualValidator> {
		protected override object MinValue
		{
			get { return null; }
		}

		protected override object MaxValue
		{
			get { return AbstractComparisonValidator.ValueToCompare; }
		}

		public MaxLengthClientValidator(PropertyRule rule, IPropertyValidator validator)
            : base(rule, validator) {
		}

		public override void AddValidation(ClientModelValidationContext context) {
			MergeAttribute(context.Attributes, "data-val", "true");
			MergeAttribute(context.Attributes, "data-val-minlength", GetErrorMessage(context));
			MergeAttribute(context.Attributes, "data-val-minlength-max", MaxValue.ToString());
		}

	    protected override string GetDefaultMessage() {
			return Messages.lessthanorequal_error;
		}
    }
}