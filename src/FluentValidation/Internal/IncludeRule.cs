namespace FluentValidation.Internal {
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using Validators;

	/// <summary>
	/// Marker interface indicating an include rule.
	/// </summary>
	public interface IIncludeRule { }

	/// <summary>
	/// Include rule
	/// </summary>
	internal class IncludeRule<T> : PropertyRule<T, T>, IIncludeRule {
		/// <summary>
		/// Creates a new IncludeRule
		/// </summary>
		public IncludeRule(IValidator<T> validator, Func<CascadeMode> cascadeModeThunk, Type typeToValidate)
			: base(null, x => x, null, cascadeModeThunk, typeToValidate) {

			var adaptor = new ChildValidatorAdaptor<T, T>(validator, validator.GetType());
			// Note: ChildValidatorAdaptor implements both IPropertyValidator and IAsyncPropertyValidator
			// So calling AddAsyncValidator will actually register it as supporting both sync and async.
			AddAsyncValidator(adaptor, adaptor);
		}

		/// <summary>
		/// Creates a new IncludeRule
		/// </summary>
		public IncludeRule(Func<ValidationContext<T>, T, IValidator<T>> func,  Func<CascadeMode> cascadeModeThunk, Type typeToValidate, Type validatorType)
			: base(null, x => x, null, cascadeModeThunk, typeToValidate) {
			var adaptor = new ChildValidatorAdaptor<T,T>(func,  validatorType);
			// Note: ChildValidatorAdaptor implements both IPropertyValidator and IAsyncPropertyValidator
			// So calling AddAsyncValidator will actually register it as supporting both sync and async.
			AddAsyncValidator(adaptor, adaptor);
		}

		/// <summary>
		/// Creates a new include rule from an existing validator
		/// </summary>
		public static IncludeRule<T> Create(IValidator<T> validator, Func<CascadeMode> cascadeModeThunk) {
			return new IncludeRule<T>(validator, cascadeModeThunk, typeof(T));
		}

		/// <summary>
		/// Creates a new include rule from an existing validator
		/// </summary>
		public static IncludeRule<T> Create<TValidator>(Func<T, TValidator> func, Func<CascadeMode> cascadeModeThunk)
			where TValidator : IValidator<T> {
			return new IncludeRule<T>((ctx, _) => func(ctx.InstanceToValidate), cascadeModeThunk, typeof(T), typeof(TValidator));
		}

		public override async ValueTask ValidateAsync(ValidationContext<T> context, bool useAsync, CancellationToken cancellation) {
			context.RootContextData[MemberNameValidatorSelector.DisableCascadeKey] = true;
			await base.ValidateAsync(context, useAsync, cancellation);
			context.RootContextData.Remove(MemberNameValidatorSelector.DisableCascadeKey);
		}
	}
}
