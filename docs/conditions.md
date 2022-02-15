# Conditions

The `When` and `Unless` methods can be used to specify conditions that control when the rule should execute. For example, this rule on the `CustomerDiscount` property will only execute when `IsPreferredCustomer` is `true`:

```csharp
RuleFor(customer => customer.CustomerDiscount).GreaterThan(0).When(customer => customer.IsPreferredCustomer);
```

The `Unless` method is simply the opposite of `When`.

If you need to specify the same condition for multiple rules then you can call the top-level `When` method instead of chaining the `When` call at the end of the rule:

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
You can set the cascade mode to customise how FluentValidation executes rules and validators when a particular rule in the validator class, or validator in the rule fails.

## Rule-Level Cascade Modes
Imagine you have two validators defined as part of a single rule definition, a `NotNull` validator and a `NotEqual` validator:

```csharp
public class PersonValidator : AbstractValidator<Person> {
  public PersonValidator() {
    RuleFor(x => x.Surname).NotNull().NotEqual("foo");
  }
}
```

This will first check whether the Surname property is not null and then will check if it's not equal to the string "foo". If the first validator (`NotNull`) fails, then by default, the call to `NotEqual` will still be invoked. This can be changed for this specific rule only by specifying a cascade mode of `Stop` (omitting the class and constructor definition from now on; assume that they are still present as above):

```csharp
RuleFor(x => x.Surname).Cascade(CascadeMode.Stop).NotNull().NotEqual("foo");
```

Now, if the `NotNull` validator fails then the `NotEqual` validator will not be executed. This is particularly useful if you have a complex chain where each validator depends on the previous validator to succeed.

The two cascade modes are:
- `Continue` (the default) - always invokes all rules in a validator class, or all validators in a rule, depending on where it is used (see below).
- `Stop` - stops executing a validator class as soon as a rule fails, or stops executing a rule as soon as a validator fails, depending on where it is used (see below).

```eval_rst
.. warning::
The Stop option is only available in FluentValidation 9.1 and newer. In older versions, you can use StopOnFirstFailure instead (see "Stop vs StopOnFirstFailure").
```
If you have a validator class with multiple rules, and would like this `Stop` behaviour to be set for all of your rules, you could do e.g.:
```csharp
RuleFor(x => x.Forename).Cascade(CascadeMode.Stop).NotNull().NotEqual("foo");
RuleFor(x => x.MiddleNames).Cascade(CascadeMode.Stop).NotNull().NotEqual("foo");
RuleFor(x => x.Surname).Cascade(CascadeMode.Stop).NotNull().NotEqual("foo");
```
To avoid repeating `Cascade(CascadeMode.Stop)`, you can set a default value for the rule-level cascade mode by setting the `AbstractValidator.RuleLevelCascadeMode` property, resulting in
```csharp
RuleLevelCascadeMode = CascadeMode.Stop;

RuleFor(x => x.Forename).NotNull().NotEqual("foo");
RuleFor(x => x.MiddleNames).NotNull().NotEqual("foo");
RuleFor(x => x.Surname).NotNull().NotEqual("foo");
```
With default global settings, this code will stop executing any rule whose `NotNull` call fails, and not call `NotEqual`, but it will then continue to the next rule, and always execute all three, regardless of failures. See "Validator Class-Level Cascade Modes" for how to control this behavior. This particular behaviour is useful if you want to create a list of all validation failures, as opposed to only returning the first one.

See "Global Default Cascade Modes" for setting the default value of this property.

## Validator Class-Level Cascade Modes
As well as being set at the rule level, the cascade mode can also be set at validator class-level, using the property `AbstractValidator.ClassLevelCascadeMode`. This controls the cascade behaviour _in between_ rules within that validator, but does not affect the rule-level cascade behaviour described above.

For example, the code above will execute all three rules, even if any of them fail. To stop execution of the validator class completely if any rule fails, you can set `AbstractValidator.ClassLevelCascadeMode` to `Stop`. This will result in complete "fail fast" behavior, and return only return a maximum of one error.

See "Global Default Cascade Modes" for setting the default value of this property.

## Global Default Cascade Modes
To set the default cascade modes at rule-level and/or validator class-level globally, set `ValidatorOptions.Global.DefaultRuleLevelCascadeMode` and/or `ValidatorOptions.Global.DefaultClassLevelCascadeMode` during your application's startup routine. Both of these default to `Continue`.
```eval_rst
.. warning::
The RuleLevelCascadeMode, ClassLevelCascadeMode, and their global defaults are only available in FluentValidation 11 and newer. See below.
```
## Introduction of RuleLevelCascadeMode and ClassLevelCascadeMode (and deprecation of CascadeMode)
The `AbstractValidator.RuleLevelCascadeMode`, `AbstractValidator.ClassLevelCascadeMode`, and their global defaults were introduced in FluentValidation 11

In older versions, there was only one property controlling cascade modes: `AbstractValidator.CascadeMode`. Changing this value would set the cascade mode at both validator class-level and rule-level. Therefore, for example, if you wanted to have the above-described functionality where you create a list of validation errors, by stopping on failure at rule-level to avoid crashes, but continuing at validator class-level, you would need to set `AbstractValidator.CascadeMode` to `Continue`, and then repeat `Cascade(CascadeMode.Stop)` on every rule chain (or use the deprecated `StopOnFirstFailure` with a warning; see "Stop vs StopOnFirstFailure").

The new properties enable finer control of the cascade mode at the different levels, with less repetition.

```eval_rst
.. warning::
In FluentValidation11, there are _no_ breaking changes to cascade modes. The `AbstractValidator.CascadeMode` property (and its global default property) are still present, and they function exactly the same as they did before, but they do so by setting / returning the values of `RuleLevelCascadeMode` and `ClassLevelCascadeMode`, as opposed to being used by the code directly.

However, `AbstractValidator.CascadeMode` and its global default are deprecated, and will be removed in a future version. To convert to the new properties, see <TODO link to 11 update docs>.
```

## Stop vs StopOnFirstFailure

In FluentValidation 9.0 and older, the `CascadeMode.StopOnFirstFailure` option was used to provide control over the default cascade mode at rule-level, but its use was not intuitive. There was no  `CascadeMode.Stop` option. 

With `StopOnFirstFailure`,  the following would provide the example behavior described previously (stop any rule if it fails, but then continue executing at validator class-level, so that all rules are executed):

```csharp
CascadeMode = CascadeMode.StopOnFirstFailure;

RuleFor(x => x.Forename).NotNull().NotEqual("foo");
RuleFor(x => x.MiddleNames).NotNull().NotEqual("foo");
RuleFor(x => x.Surname).NotNull().NotEqual("foo");
```
If they all fail, you will get three validation errors. That is the equivalent of doing

```csharp
RuleFor(x => x.Forename).Cascade(CascadeMode.StopOnFirstFailure).NotNull().NotEqual("foo");
RuleFor(x => x.MiddleNames).Cascade(CascadeMode.StopOnFirstFailure).NotNull().NotEqual("foo");
RuleFor(x => x.Surname).Cascade(CascadeMode.StopOnFirstFailure).NotNull().NotEqual("foo");
```
This behaviour caused a lot of confusion over the years, so the `Stop` option was introduced in FluentValidation 9.1. Using `Stop` instead of `StopOnFirstFailure`, _any_ failure at all would stop execution, so only the first failure result would be returned.

The `Stop` option was introduced rather than changing the behaviour of `StopOnFirstFailure` as this would've been a very subtle breaking change, so we thought it was best to maintain the existing behaviour while adding a new option. `StopOnFirstFailure` was marked as Obsolete in FluentValidation 9.1, and generated compiler warnings.

See also the FluentValidation 11 changes described above, that further change how the above code would be written. `StopOnFirstFailure` is still available as of version 11 and has no breaking changes to how it functions. However, in a future version, it _will_ be removed, now that it's possible to replicate its behavior using the new properties `AbstractValidator.RuleLevelCascadeMode` and `AbstractValidator.ClassLevelCascadeMode`.

# Dependent Rules

By default, all rules in FluentValidation are separate and cannot influence one another. This is intentional and necessary for asynchronous validation to work. However, there may be some cases where you want to ensure that some rules are only executed after another has completed. You can use `DependentRules` to do this.

To use dependent rules, call the `DependentRules` method at the end of the rule that you want others to depend on. This method accepts a lambda expression inside which you can define other rules that will be executed only if the first rule passes:

```csharp
RuleFor(x => x.Surname).NotNull().DependentRules(() => {
  RuleFor(x => x.Forename).NotNull();
});
```

Here the rule against Forename will only be run if the Surname rule passes.

_Author's note_: Personally I do not particularly like using dependent rules as I feel it's fairly hard to read, especially with a complex set of rules. In many cases, it can be simpler to use `When` conditions combined with `CascadeMode` to prevent rules from running in certain situations. Even though this can sometimes mean more duplication, it is often easier to read.