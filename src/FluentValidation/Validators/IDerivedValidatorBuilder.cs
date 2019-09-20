using System;

namespace FluentValidation.Validators {
  public interface IDerivedValidatorBuilder<in T> where T : class {

    void Include<TDerived>(Action<InlineValidator<TDerived>> builder) where TDerived : class, T;

    void Include<TDerived>(IValidator<TDerived> validator) where TDerived : class, T;

  } 
}
