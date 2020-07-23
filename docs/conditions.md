# Conditions

The `When` and `Unless` methods can be used to specify conditions that control when the rule should execute. For example, this rule on the CustomerDiscount property will only execute when IsPreferredCustomer is true:

```csharp
RuleFor(customer => customer.CustomerDiscount).GreaterThan(0).When(customer => customer.IsPreferredCustomer);
```

The `Unless` method is simply the opposite of `When`.

If you need to specify the same condition for multiple rules then you can call the top-level When method instead of chaining the When call at the end of the rule:

```csharp
When(customer => customer.IsPreferred, () => {
   RuleFor(customer => customer.CustomerDiscount).GreaterThan(0);
   RuleFor(customer => customer.CreditCardNumber).NotNull();
});
```

This time, the condition will be applied to both rules. You can also chain a call to `Otherwise` which will invoke rules that don't match the condition:

```csharp
When(customer => customer.IsPreferred, () => {
   RuleFor(customer => customer.CustomerDiscount).GreaterThan(0);
   RuleFor(customer => customer.CreditCardNumber).NotNull();
}).Otherwise(() => {
  RuleFor(customer => customer.CustomerDiscount).Equal(0);
});
```

By default FluentValidation will apply the condition to all preceding validators in the same call to `RuleFor`. If you only want the condition to apply to the validator that immediately precedes the condition, you must explicitly specify this:

```csharp
RuleFor(customer => customer.CustomerDiscount)
    .GreaterThan(0).When(customer => customer.IsPreferredCustomer, ApplyConditionTo.CurrentValidator)
    .EqualTo(0).When(customer => ! customer.IsPreferredCustomer, ApplyConditionTo.CurrentValidator);
```

If the second parameter is not specified, then it defaults to `ApplyConditionTo.AllValidators`, meaning that the condition will apply to all preceding validators in the same chain. 

If you need this behaviour, be aware that you must specify `ApplyConditionTo.CurrentValidator` as part of *every* condition. In the following example the first call to `When` applies to only the call to `Matches`, but not the call to `NotEmpty`. The second call to `When` applies only to the call to `Empty`. 

```csharp
RuleFor(customer => customer.Photo)
    .NotEmpty()
    .Matches("https://wwww.photos.io/\d+\.png")
    .When(customer => customer.IsPreferredCustomer, ApplyConditionTo.CurrentValidator)
    .Empty()
    .When(customer => ! customer.IsPreferredCustomer, ApplyConditionTo.CurrentValidator);
```


# Setting the Cascade mode

You can set the cascade mode to customise how FluentValidation executes chained validators when a particular validator in the chain fails.

Imagine you have two validators defined as part of a single rule definition, a `NotNull` validator and a `NotEqual` validator:

```csharp
RuleFor(x => x.Surname).NotNull().NotEqual("foo");
```

This will first check whether the Surname property is not null and then will check if it's not equal to the string "foo". If the first validator (NotNull) fails, then the call to `NotEqual` will still be invoked. This can be changed by specifying a cascade mode of `StopOnFirstFailure`:

```csharp
RuleFor(x => x.Surname).Cascade(CascadeMode.StopOnFirstFailure).NotNull().NotEqual("foo");
```

Now, if the NotNull validator fails then the NotEqual validator will not be executed. This is particularly useful if you have a complex chain where each validator depends on the previous validator to succeed.

The two cascade modes are:
- `Continue` (the default) - always invokes all validators in a rule definition
- `StopOnFirstFailure` - stops executing a rule as soon as a validator fails

As well as being set at the rule level, the cascade mode can also be set globally for all validators, or for all the rules in a particular validator class. This is the equivalent of setting the cascade mode on every rule within the validator. Not that this still only applies to validators *within the same rule chain* - separate calls to `RuleFor` are treated separately. If one rule fails, it will not prevent a separate rule from running, only validators within the same rule chain.

To set the cascade mode globally, you can set the CascadeMode property on the static ValidatorOptions class during your application's startup routine:

```csharp
ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
```

This can then be overridden by individual validator classes or by individual rules.

To set the cascade mode for all rules inside a single validator class, set the `CascadeMode` property on `AbstractValidator`:

```csharp
public class PersonValidator : AbstractValidator<Person> {
  public PersonValidator() {
    // First set the cascade mode
    CascadeMode = CascadeMode.StopOnFirstFailure;

    RuleFor(x => x.Surname).NotNull().NotEqual("foo");
    RuleFor(x => x.Forename).NotNull().NotEqual("foo");
  }
}
```

Note that this is the equivalent of doing the following:

```csharp
RuleFor(x => x.Surname).Cascade(CascadeMode.StopOnFirstFailure).NotNull().NotEqual("foo");
RuleFor(x => x.Forename).Cascade(CascadeMode.StopOnFirstFailure).NotNull().NotEqual("foo");
```

```eval_rst
.. note::
    Setting the cascade mode only applies to validators within the same `RuleFor` chain. Changing the cascade mode does not affect separate calls to `RuleFor`. If you want prevent one rule from running if a different rule fails, you should instead use Dependent Rules (below), 

```

# Dependent Rules

By default, all rules in FluentValidation are separate and cannot influence one another. This is intentional and necessary for asynchronous validation to work. However, there may be some cases where you want to ensure that some rules are only executed after another has completed. You can use `DependentRules` to do this.

To use DependentRules, call the `DependentRules` method at the end of the rule that you want others to depend on. This method accepts a lambda expression inside which you can define other rules that will be executed only if the first rule passes:

```csharp
RuleFor(x => x.Surname).NotNull().DependentRules(() => {
  RuleFor(x => x.Forename).NotNull();
});
```

Here the rule against Forename will only be run if the Surname rule passes.

_Jeremy's note_: Personally I do not particularly like using DependentRules as I feel it's fairly hard to read, especially with a complex set of rules. In many cases, it can be simpler to use `When` conditions to prevent rules from running in certain situations. Even though this can sometimes mean more duplication, it is often easier to read.
