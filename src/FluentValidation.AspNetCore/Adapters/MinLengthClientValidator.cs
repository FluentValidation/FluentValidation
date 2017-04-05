namespace FluentValidation.AspNetCore {
    using Internal;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
    using Resources;
    using Validators;

    internal class MinLengthClientValidator : AbstractComparisonClientValidator<GreaterThanOrEqualValidator> {

        protected override object MinValue {
            get { return AbstractComparisonValidator.ValueToCompare;  }
        }

        protected override object MaxValue {
            get { return null; }
        }

        public MinLengthClientValidator(PropertyRule rule, IPropertyValidator validator)
            : base(rule, validator) {
        }

	    public override void AddValidation(ClientModelValidationContext context) {
		    if (MinValue != null) {
			    MergeAttribute(context.Attributes, "data-val", "true");
			    MergeAttribute(context.Attributes, "data-val-minlength", GetErrorMessage(context));
			    MergeAttribute(context.Attributes, "data-val-minlength-min", MinValue.ToString());
		    }
	    }

	    protected override string GetDefaultMessage() {
			return Messages.greaterthanorequal_error;

		}
	}
}