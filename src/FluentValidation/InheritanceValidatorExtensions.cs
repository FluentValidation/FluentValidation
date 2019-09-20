

namespace FluentValidation {
  using Validators;
  using System;
  using System.Collections.Generic;

  public static class InheritanceValidatorExtensions {

    /// <summary>
    /// Add validator(s) for the derived types for this model
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="ruleBuilder"></param>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, TProperty> AddInheritance<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Action<IDerivedValidatorBuilder<TProperty>> builder) where TProperty : class {
      var inheritanceValidator = new InheritanceValidator<TProperty>();
      builder(inheritanceValidator);
      return ruleBuilder.SetValidator(inheritanceValidator);
    }


  }
}
