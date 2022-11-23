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
  The RuleLevelCascadeMode, ClassLevelCascadeMode, and their global defaults are only available in FluentValidation 11 and newer.
```

## Introduction of RuleLevelCascadeMode and ClassLevelCascadeMode (and removal of CascadeMode)
The `AbstractValidator.RuleLevelCascadeMode`, `AbstractValidator.ClassLevelCascadeMode`, and their global defaults were introduced in FluentValidation 11

In older versions, there was only one property controlling cascade modes: `AbstractValidator.CascadeMode`. Changing this value would set the cascade mode at both validator class-level and rule-level. Therefore, for example, if you wanted to have the above-described functionality where you create a list of validation errors, by stopping on failure at rule-level to avoid crashes, but continuing at validator class-level, you would need to set `AbstractValidator.CascadeMode` to `Continue`, and then repeat `Cascade(CascadeMode.Stop)` on every rule chain.

The new properties enable finer control of the cascade mode at the different levels, with less repetition.

```eval_rst
.. warning::
  The `CascadeMode` property was deprecated in FluentValidation 11 and removed in FluentValidation 12. The `RuleLevelCascadeMode` and `ClassLevelCascadeMode` properties should be used instead.
  
  To convert to the new properties, see `the upgrade guide <upgrading-to-11.html#cascade-mode-changes>`_.
```
