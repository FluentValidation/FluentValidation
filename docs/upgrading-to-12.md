# 12.0 Upgrade Guide

### Introduction

FluentValidation 12.0 is a major release that included several breaking changes. Please review this document carefully before upgrading from FluentValidation 11.x to 12.

The main goal of this release was removal of deprecated code and removal of support for obsolete platforms. There are no new features in this release.

### Changes in supported platforms

- .NET 5 is no longer supported (Microsoft's support for .NET 5 ended in November 2022)
- .NET Core 3.1 is no longer supported (Microsoft's support for .NET Core 3.1 ended in December 2022)
- .NET Standard is no longer support
- .NET 6 is the minimum supported version 

If you still need .NET Standard 2.0 compatibility then you will need to continue to use FluentValidation 11.x and only upgrade to FluentValidation 12 once you've moved to a more modern version of .NET.  

### Removal of CascadeMode.StopOnFirstFailure

The `StopOnFirstFailure` cascade option was deprecated in FluentValidation 11.0 and has now been removed, along with the `AbstractValidator.CascadeMode` and `ValidatorOptions.Global.CascadeMode` properties which were also deprecated in 11.0. 

If were previously setting `ValidatorOptions.Global.CascadeMode` to `Continue` or `Stop`, you can simply replace this with the following:

```csharp
ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.<YourCurrentValue>;
ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.<YourCurrentValue>;
```

If you were previously setting it to `StopOnFirstFailure`, replace it with the following:

```csharp
ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
```

Similarly, if you were previously setting `AbstractValidator.CascadeMode` to `Continue` or `Stop`, replace this with the following:

```csharp
ClassLevelCascadeMode = CascadeMode.<YourCurrentValue>;
RuleLevelCascadeMode = CascadeMode.<YourCurrentValue>;
```

If you were previously setting it to `StopOnFirstFailure`, replace it with the following:

```csharp
ClassLevelCascadeMode = CascadeMode.Continue;
RuleLevelCascadeMode = CascadeMode.Stop;
```

If you were calling `.Cascade(CascadeMode.StopOnFirstFailure)` in a rule chain, replace `StopOnFirstFailure` with `Stop`.

### Removal of InjectValidator and related methods

The `InjectValidator` method was deprecated in 11.x and removed in 12.0.

This method allowed you to implicitly inject a child validator from the ASP.NET Service Provider:

```csharp
public class PersonValidator : AbstractValidator<Person> 
{
  public PersonValidator() 
  {
    RuleFor(x => x.Address).InjectValidator();
  }
}
```

Assuming that the address property is of type `Address`, the above code would attempt to resolve an `IValidator<Address>` and use this to validator the `Address` property. This method can only be used when working with ASP.NET MVC's auto-validation feature and cannot be used in other contexts. 

Instead of using `InjectValidator`, you should instead use a more traditional constructor injection approach, which is not just limited to ASP.NET MVC:

```csharp
public class PersonValidator : AbstractValidator<Person> 
{
  public PersonValidator(IValidator<Address> addressValidator) 
  {
    RuleFor(x => x.Address).SetValidator(addressValidator);
  }
}
```

### Removal of AbstractValidator.EnsureInstanceNotNull

In previous versions of FluentValidation it was possible to override the `AbstractValidator.EnsureInstanceNotNull` method to disable FluentValidation's root-model null check. The ability to do this was deprecated in 11.5.x and has now been removed. For further details please see [https://github.com/FluentValidation/FluentValidation/issues/2069](https://github.com/FluentValidation/FluentValidation/issues/2069)


### Other breaking API changes 

- The `ITestValidationContinuation` interface now exposes a `MatchedFailures` property (as well as the existing `UnmatchedFailures`)
- The `ShouldHaveAnyValidationError` method has been renamed to `ShouldHaveValidationErrors`
- `ShouldNotHaveAnyValidationErrors` and `ShouldHaveValidationErrors` are now instance methods on `TestValidationResult`, instead of extension methods. 
