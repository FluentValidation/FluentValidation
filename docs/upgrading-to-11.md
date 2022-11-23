
# 11.0 Upgrade Guide

### Introduction

FluentValidation 11.0 is a major release that included several breaking changes. Please review this document carefully before upgrading from FluentValidation 10.x to 11.

There were 3 main goals for this release:
- Removing deprecated code and support for obsolete platforms
- Update sync-over-async workflows to clearly throw an exception
- Remove ambiguity in handling of `CascadeMode` settings

Below is a summary of all the changes in this release:

### Changes in supported platforms

- .NET Core 2.1 is no longer supported as Microsoft has stopped support for this platform.

### Sync-over-async now throws an exception

In FluentValidation 10.x and older, if you attempted to run an asynchronous validator synchronously, the asynchronous rules would silently be run synchronously. This was unintutive and would lead to deadlocks. 

Starting in FluentValidation 11.0, validators that contain asynchronous rules will now throw a `AsyncValidatorInvokedSynchronouslyException` if you attempt to invoke them synchronously. You must invoke these validators asynchronously.

This affects rules that contain any of the following:
- Calls to `MustAsync`
- Calls to `WhenAsync` and `UnlessAsync`
- Calls to `CustomAsync`
- Use of any custom async validators 

### OnFailure and OnAnyFailure removed

The deprecated methods `OnFailure` and `OnAnyFailure` have been removed.

These were callbacks that could be used to define an action that would be called when a particular rule fails. These methods were deprecated in 10.x as they allowed the standard FluentValidation workflow to be bypassed, and additionally they have caused various maintenance issues since they were introduced. 

If you were previously using `OnFailure` or `OnAnyFailure` to perform custom logic after validation, we recommend using a `Custom` validator instead.

### Test Helper changes

The deprecated extension methods `validator.ShouldHaveValidationErrorFor` and `validator.ShouldNotHaveValidationErrorFor` have been removed. The recommended alternative is to use `TestValidate` instead, [which is covered in the documentation here](https://docs.fluentvalidation.net/en/latest/testing.html).

### Cascade Mode Changes

The `CascadeMode` properties on `AbstractValidator` and `ValidatorOptions.Global` have been deprecated and replaced with the properties `RuleLevelCascadeMode` and `ClassLevelCascadeMode` which provide finer-grained control for setting the cascade mode.

If you are currently setting `ValidatorOptions.Global.CascadeMode` to `Continue` or `Stop`, you can simply replace this with

```csharp
ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.<YourCurrentValue>;
ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.<YourCurrentValue>;
```

If you are currently setting it to `StopOnFirstFailure`, replace it with

```csharp
ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Continue; // Not actually needed as this is the default. Just here for completeness.
ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
```

Similarly, if you are currently setting `AbstractValidator.CascadeMode` to `Continue` or `Stop`, replace this with

```csharp
ClassLevelCascadeMode = CascadeMode.<YourCurrentValue>;
RuleLevelCascadeMode = CascadeMode.<YourCurrentValue>;
```

If you are currently setting it to `StopOnFirstFailure`, replace it with

```csharp
ClassLevelCascadeMode = CascadeMode.Continue;
RuleLevelCascadeMode = CascadeMode.Stop;
```

If you are calling `.Cascade(CascadeMode.StopOnFirstFailure)` in a rule chain, replace `StopOnFirstFailure` with `Stop` (this has always had the same behavior at rule-level since `Stop` was introduced anyway).

All of the changes described above are exactly what the code does now anyway - e.g. if you set `AbstractValidator.CascadeMode` to `Stop`, it sets `AbstractValidator.DefaultRuleLevelCascadeMode` and `AbstractValidator.DefaultClassLevelCascadeMode` to `Stop`, and doesn't use `AbstractValidator.CascadeMode` in any logic internally.

You may also be able to remove some now-unneeded calls to `.Cascade` at rule-level. For example, if you have the cascade mode at validator class-level set to `Continue`, and are repeating `.Cascade(CascadeMode.Stop[/StopOnFirstFailure])` for each rule, you can now replace this with

```csharp
ClassLevelCascadeMode = CascadeMode.Continue;
RuleLevelCascadeMode = CascadeMode.Stop;
```

...or their global default equivalents. 

 See [this page in the documentation](https://docs.fluentvalidation.net/en/latest/conditions.html#setting-the-cascade-mode) for details of how cascade modes work.

As `StopOnFirstFailure` is deprecated and scheduled for removal, it cannot be assigned to either of the two new `AbstractValidator` properties or their global equivalents (it still can be assigned to the also-deprecated `AbstractValidator.CascadeMode`). Attempting to set the new properties to `StopOnFirstFailure` will simply result in `Stop` being used instead.

### MessageBuilder changes

If you use the `MessageBuilder` functionality to provide custom logic for error message creation then please note that as of 11.0 you can only have a single `MessageBuilder` associated with a rule chain. This property is also now set-only. In previous versions you may have had code like this:

```csharp
return ruleBuilder.Configure(rule => {
  var originalMessageBuilder = rule.MessageBuilder;
  rule.MessageBuilder = context => {
    
    // ... some custom logic in here.
    
    return originalMessageBuilder?.Invoke(context) ?? context.GetDefaultMessage();
  };
});
```

Now as this property is set-only you'll need to update it to remove references to `originalMessageBuilder`:

```csharp
return ruleBuilder.Configure(rule => {
  rule.MessageBuilder = context => {
    // ... some custom logic in here.
    return context.GetDefaultMessage();
  };
});
```

This means you can no longer chain MessageBuilders together, and whichever one is set last will be the only one associated with the rule, so please confirm that you aren't relying on the previous behaviour before making this change. 


### ASP.NET Core Integration changes

The deprecated property `RunDefaultMvcValidationAfterFluentValidationExecutes` within the ASP.NET Configuration has been removed. 

If you were making use of this property, you should use `DisableDataAnnotationsValidation` instead. Note that this property is the inverse of the previous behaviour:

```csharp
// Before:
services.AddFluentValidation(fv => {
  fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
});

// After:
services.AddFluentValidation(fv => {
  fv.DisableDataAnnotationsValidation = true;
});

```

### Removal of backwards compatibility property validator layer

The non-generic `PropertyValidator` class (and associated classes/helpers) have been removed. These classes were deprecated in 10.0. If you are still using this class, you should migrate to the generic `PropertyValidator<T,TProperty>` instead. 

### Internal API Changes

Several of the methods in the Internal API have been removed. These changes don't affect use of the public fluent interface, but may impact library developers or advanced users.

- `IValidationRule<T,TProperty>.CurrentValidator` has been removed (use the `Current` property instead)
-`IValidationRule<T,TProperty>.Current` now returns an `IRuleComponent<T,TProperty>` interface instead of `RuleComponent<T,TProperty>` (necessary to support variance) 
-`IValidationRule<T,TProperty>.MessageBuilder`'s argument is now an `IMessageBuilderContext<T,TProperty>` interface instead of `MessageBuilderContext<T,TProperty>` class (necessary to support variance)
- `IValidationRule<T,TProperty>.MessageBuilder` is now set-only, and has no getter exposed (needed to support variance), meaning you can only have one message builder per rule chain. 
- `IRuleComponent<T,TProperty>.CustomStateProvider` is now set-only to support variance
- `IRuleComponent<T,TProperty>.SeverityProvider` is now set-only to support variance
- `GetErrorMessage` is no longer exposed on `IRuleComponent<T,TProperty>`
- Remove deprecated `Options` property from `RuleComponent`
- The `MemberAccessor` class has been removed as it's no longer used
