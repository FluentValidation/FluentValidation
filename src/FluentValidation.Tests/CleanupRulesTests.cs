using FluentValidation.Results;
using System;
using System.Threading.Tasks;
using Xunit;

namespace FluentValidation.Tests {
  public class CleanupRulesTests {

    [Fact]
    public void Cleanup_for_settings_rules_without_ctor_with_instanceToValidate() {
      var validator = new PersonValidatorWithotCtor();

      Assert.True(validator.Validate(new Person()).IsValid);
      Assert.False(validator.Validate(new Person { Surname = "A" }).IsValid);
      Assert.True(validator.Validate(new Person { Surname = "A", Email = "@" }).IsValid);
    }
  }

  public class PersonValidatorWithotCtor : BaseValidator<Person> {
    public override async Task Configure(Person instanceToValidate) {

      if (instanceToValidate.Surname?.StartsWith("A") == true) {
        RuleFor(x => x.Email)
            .NotEmpty();
      }

      await Task.CompletedTask;
    }
  }

  // Very useful when you need to fetch the rules from the constructor and have instanceToValidate
  // Clean Constructor is a good practice for Dependency Management
  // Conditional logic can now be configured with standard statements: if-else-switch-etc
  public abstract class BaseValidator<T> : AbstractValidator<T> {

    private readonly Action<T> _action;

    protected override bool PreValidate(ValidationContext<T> context, ValidationResult result) {

      CleanupRules();

      _action(context.InstanceToValidate);

      return base.PreValidate(context, result);
    }

    public BaseValidator() => _action = new Action<T>(async x => await Configure(x));

    public abstract Task Configure(T instanceToValidate);
  }
}
