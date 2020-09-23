namespace FluentValidation.Internal {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Results;
	using Validators;

	/// <summary>
	/// Marker interface indicating an include rule.
	/// </summary>
	public interface IIncludeRule { }

	/// <summary>
	/// Include rule
	/// </summary>
	public class IncludeRule<T> : PropertyRule, IIncludeRule {
		/// <summary>
		/// Creates a new IncludeRule
		/// </summary>
		/// <param name="validator"></param>
		/// <param name="cascadeModeThunk"></param>
		/// <param name="typeToValidate"></param>
		/// <param name="containerType"></param>
		public IncludeRule(IValidator<T> validator, Func<CascadeMode> cascadeModeThunk, Type typeToValidate, Type containerType) : base(null, x => x, null, cascadeModeThunk, typeToValidate, containerType) {
			AddValidator(new ChildValidatorAdaptor<T,T>(validator, validator.GetType()));
		}

		/// <summary>
		/// Creates a new IncludeRule
		/// </summary>
		/// <param name="func"></param>
		/// <param name="cascadeModeThunk"></param>
		/// <param name="typeToValidate"></param>
		/// <param name="containerType"></param>
		/// <param name="validatorType"></param>
		public IncludeRule(Func<PropertyValidatorContext, IValidator<T>> func,  Func<CascadeMode> cascadeModeThunk, Type typeToValidate, Type containerType, Type validatorType) : base(null, x => x, null, cascadeModeThunk, typeToValidate, containerType) {
			AddValidator(new ChildValidatorAdaptor<T,T>(func,  validatorType));
		}

		/// <summary>
		/// Creates a new include rule from an existing validator
		/// </summary>
		/// <param name="validator"></param>
		/// <param name="cascadeModeThunk"></param>
		/// <returns></returns>
		public static IncludeRule<T> Create(IValidator<T> validator, Func<CascadeMode> cascadeModeThunk) {
			return new IncludeRule<T>(validator, cascadeModeThunk, typeof(T), typeof(T));
		}

		/// <summary>
		/// Creates a new include rule from an existing validator
		/// </summary>
		/// <param name="func"></param>
		/// <param name="cascadeModeThunk"></param>
		/// <typeparam name="TValidator"></typeparam>
		/// <returns></returns>
		public static IncludeRule<T> Create<TValidator>(Func<T, TValidator> func, Func<CascadeMode> cascadeModeThunk)
			where TValidator : IValidator<T> {
			return new IncludeRule<T>(ctx => func((T)ctx.InstanceToValidate), cascadeModeThunk, typeof(T), typeof(T), typeof(TValidator));
		}


		public override IEnumerable<ValidationFailure> Validate(IValidationContext context) {
			context.RootContextData[MemberNameValidatorSelector.DisableCascadeKey] = true;
			var result = base.Validate(context).ToList();
			context.RootContextData.Remove(MemberNameValidatorSelector.DisableCascadeKey);
			return result;
		}

		public override async Task<IEnumerable<ValidationFailure>> ValidateAsync(IValidationContext context, CancellationToken cancellation) {
			context.RootContextData[MemberNameValidatorSelector.DisableCascadeKey] = true;
			var result = await base.ValidateAsync(context, cancellation);
			result = result.ToList();
			context.RootContextData.Remove(MemberNameValidatorSelector.DisableCascadeKey);
			return result;
		}
	}
}
