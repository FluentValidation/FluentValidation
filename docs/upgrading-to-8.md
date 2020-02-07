# 8.0 Upgrade Guide

### Introduction

FluentValidation 8.0 is a major release that included several breaking changes. Please review this document before upgrading from FluentValidation 7.x to 8.

### Asynchronous Validation updates

There have been several major underlying changes to the asynchronous validation workflow in FluentValidation 8. These should not have any impact to any existing asynchronous code other than that some methods now take a `CancellationToken` when they didn't before.

These changes were made to remove the internal dependency on the old Microsoft `TaskHelper` classes and use `async/await` instead.

### SetCollectionValidator is deprecated

Instead of using `SetCollectionValidator` you should use FluentValidation's `RuleForEach` support instead:

FluentValidation 7:
```csharp
RuleFor(x => x.AddressLines).SetCollectionValidator(new AddressLineValidator());
```

FluentValidation 8:
```csharp
RuleForEach(x => x.AddressLines).SetValidator(new AddressLineValidator());
```

#### Why was this done?

`SetCollectionValidator` was added to FluentValidation in its initial versions to provide a way to use a child validator against each element in a collection. `RuleForEach` was added later and provides a more comprehensive way of validating collections (as you can define in-line rules with RuleForEach too). It doesn't make sense to provide 2 ways to do the same thing.

### Several properties have been removed from PropertyValidator

`CustomStateProvider`, `Severity`, `ErrorMessageSource` and `ErrorCodeSource` are no longer directly exposed on `PropertyValidator`, you should now access them via the `Options` property on `PropertyValidator` instead.

#### Why was this done?

It allows extra options/configuration to be added to property validators without introducing breaking changes to the interface going forward.

### ValidatorAttribute and AttributedValidatorFactory have been moved to a separate package

Use of the `ValidatorAttribute` to wire up validators is no longer recommended and have been moved to a separate `FluentValidation.ValidatorAttribute` package.

- In ASP.NET Core projects, you should use the service provider to wire models to their validators (this has been the default behaviour for ASP.NET Core projects since FluentValidation 7)
- For desktop or mobile applications, we recommend using an IoC container to wire up validators, although you can still use the attribute approach by explicitly installing the `FluentValidation.ValidatorAttribute` package.
- In legacy ASP.NET projects (MVC 5 and WebApi 2), the ValidatorAttribute is still the default approach, and the `FluentValidation.ValidatorAttribute` package will be automatically installed for compatibility. However, we recommend using an IoC container instead if you can.

### Validating properties by path

You can now validate specific properties using a full path, eg:

```csharp
validator.Validate(customer, "Address.Line1", "Address.Line2");
```

### Validating a specific ruleset with SetValidator

Previously, if you defined a child validator with `SetValidator`, then whichever ruleset you invoked on the parent validator will cascade to the child validator.
Now you can explicitly define which ruleset will run on the child:

```csharp
RuleFor(x => x.Address).SetValidator(new AddressValidator(), "myRuleset");
```

### Many old and deprecated methods have been removed

FluentValidation 8 removes many old/deprecated methods that have been marked as obsolete for a long time.

- Removed the pre-7 way of performing custom validation (`Custom` and `CustomAsync`). Use `RuleFor(x => x).Custom()` instead. [See the section on Custom Validators](/custom-validators)
- The old localization mechanism that was deprecated with the release of FluentValidation 7. This included several overloads of `WithLocalizedName` and `WithLocalizedMessage`. [See the section on localization for more details](/localization).
- The `RemoveRule`, `ReplaceRule` and `ClearRules` methods that have been marked obsolete for many years (FluentValidation does not offer a replacement for these as runtime modification of validation rules is not recommended or supported in any way)
- Removed various async method overloads that didn't accept a `CancellationToken` (use the overloads that do accept them instead.)

### Other changes
`IStringSource.GetString` now receives a context, instead of a model. If you have custom `IStringSource` implementations, you will need to update them.

