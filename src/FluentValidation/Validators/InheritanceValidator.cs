using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace FluentValidation.Validators {

  public class InheritanceValidator<T> : AbstractValidator<T>, IDerivedValidatorBuilder<T> where T : class {

    private readonly Dictionary<Type, IValidator> _derivedValidators;

    internal InheritanceValidator() {
      _derivedValidators = new Dictionary<Type, IValidator>();
    }

    /// <summary>
    /// Define a validator for a derived class
    /// </summary>
    /// <typeparam name="TDerived"></typeparam>
    /// <param name="builder"></param>
    public void Include<TDerived>(Action<InlineValidator<TDerived>> builder) where TDerived : class, T {
      var validator = new InlineValidator<TDerived>();
      builder(validator);
      Include(validator);
    }

    /// <summary>
    /// Define a validator for a derived class
    /// </summary>
    /// <typeparam name="TDerived"></typeparam>
    /// <param name="validator"></param>
    public void Include<TDerived>(IValidator<TDerived> validator) where TDerived : class, T {
      _derivedValidators[typeof(TDerived)] = validator;
    }

    public override Task<ValidationResult> ValidateAsync(ValidationContext<T> context, CancellationToken cancellation = new CancellationToken()) {
      var validator = Get(context.InstanceToValidate.GetType());

      if (validator == null)
        return base.ValidateAsync(context, cancellation);

      return validator.ValidateAsync(context, cancellation);
    }

    public override ValidationResult Validate(ValidationContext<T> context) {

      var validator = Get(context.InstanceToValidate.GetType());

      if (validator == null)
        return base.Validate(context);

      return Get(context.InstanceToValidate.GetType()).Validate(context);
    }

    protected override bool PreValidate(ValidationContext<T> context, ValidationResult result) {

      var validator = Get(context.InstanceToValidate.GetType());

      if (validator == null)
        return base.PreValidate(context, result);

      return base.PreValidate(context, result);
    }

    private IValidator Get(Type type) {
      return !_derivedValidators.TryGetValue(type, out var validator) ? null : validator;
    }

  } 
}
