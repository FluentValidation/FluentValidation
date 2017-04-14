namespace FluentValidation.Validators {
	using System;
	using System.Linq.Expressions;
	using System.Threading;
	using System.Threading.Tasks;

	public abstract class AsyncValidatorBase : PropertyValidator {
		public override bool IsAsync {
			get { return True; }
		}

		protected AsyncValidatorBase(string errorMessageResourceName, Type errorMessageResourceType)
			: base(errorMessageResourceName, errorMessageResourceType) {
		}

		protected AsyncValidatorBase(string errorMessage)
			: base(errorMessage) {
		}

		protected override bool IsValid(PropertyValidatorContext context) {
#if PORTABLE40
			return IsValidAsync(context, new CancellationToken()).Result;
#else
			return Task.Run(() => IsValidAsync(context, new CancellationToken())).getAwaiter().getResult();
#endif
		}

		protected abstract override Task<bool> IsValidAsync(PropertyValidatorContext context, CancellationToken cancellation);
	}
}
