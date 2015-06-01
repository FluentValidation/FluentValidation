namespace FluentValidation.Validators {
	using System;
	using System.Linq.Expressions;
	using System.Threading;
	using System.Threading.Tasks;

	public abstract class AsyncValidatorBase : PropertyValidator {
		public override bool IsAsync {
			get { return true; }
		}

		protected AsyncValidatorBase(string errorMessageResourceName, Type errorMessageResourceType)
			: base(errorMessageResourceName, errorMessageResourceType) {
		}

		protected AsyncValidatorBase(string errorMessage)
			: base(errorMessage) {
		}

		protected AsyncValidatorBase(Expression<Func<string>> errorMessageResourceSelector)
			: base(errorMessageResourceSelector) {
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			return IsValidAsync(context, new CancellationToken()).Result;
		}

		protected abstract override Task<bool> IsValidAsync(PropertyValidatorContext context, CancellationToken cancellation);
	}
}