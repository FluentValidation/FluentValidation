# 9.0 Upgrade Guide

### Introduction

FluentValidation 9.0 is a major release that included several breaking changes. Please review this document before upgrading from FluentValidation 8.x to 9.

### Supported Platforms

Support for the following platforms has been dropped:
- netstandard1.1
- netstandard1.6
- net45

FluentValidation still supports netstandard2 and net461, meaning that it'll run on .NET Core 2.0 or higher (3.1 recommended), or .NET Framework 4.6.1 or higher.

FluentValidation.AspNetCore requires .NET Core 2.1 or 3.1 (3.1 recommended).

Integration with MVC5/WebApi 2 is no longer supported - both the FluentValidation.Mvc5 and FluentValidation.WebApi packages were deprecated with the release of FluentValidation 8, but they will now no longer receive further updates. They will continue to run on .NET Framework 4.6.1 or higher, but we recommend migrating to .NET Core as soon as possible.

### Default Email Validation Mode Changed

FluentValidation supports 2 methods for validating email addresses.

The first is compatible with .NET Core's `EmailAddressAttribute` and performs a simple check that an email address contains an `@` character. The second uses a regular expression that is mostly compatible with .NET 4.x's `EmailAddressAttribute`, which also used a regular expression.

In FluentValidation 8 and older, the regex-based email validation was the default. As of 9.0, the ASP.NET Core-compatible email validator is now the default. This change was made to be consistent with ASP.NET Core's default behaviour.

If you still want to validate email addresses using the old regular expression, you can specify `RuleFor(customer => customer.Email).EmailAddress(EmailValidationMode.Net4xRegex);`. This will give a deprecation warning.

[See the documentation on the email validator](built-in-validators.html#email-validator) for more details on why regular expressions shouldn't be used for validating email addresses.

### TestHelper updates

The TestHelper has been updated with several syntax improvements. It is now possible to chain additional assertions on to `ShouldHaveValidationErrorFor` and `ShouldNotHaveValidationErrorFor`, eg:

```csharp
var validator = new InlineValidator<Person>();
validator.RuleFor(x => x.Surname).NotNull().WithMessage("required");
validator.RuleFor(x => x.Address.Line1).NotEqual("foo");

// New advanced test syntax
var result = validator.TestValidate(new Person { Address = new Address()) };
result.ShouldHaveValidationErrorFor(x => x.Surname).WithMessage("required");
result.ShouldNotHaveValidationErrorFor(x => x.Address.Line1);
```

[See the documentation for full details on the Test Helper](testing)

### Equal/NotEqual string comparisons

FluentValidation 4.x-8.x contained a bug where using `NotEqual`/`Equal` on string properties would perform a culture-specific check, which would lead to unintented results. 9.0 reverts the bad change which introduced this several years ago. An ordinal string comparison will now be performed instead.

[See the documentation for further details.](built-in-validators.html#equal-validator)

### Removal of non-generic Validate overload

The `IValidator.Validate(object model)` overload has been removed to improve type safety. If you were using this method before, you can use the overload that accepts an `IValidationContext` instead:

```csharp
var context = new ValidationContext<object>(model);
var result = validator.Validate(context);
```

### Removal of non-generic ValidationContext.

The non-generic `ValidationContext` has been removed. Anywhere that previously used this class will either accept a `ValidationContext<T>` or a non-generic `IValidationContext` interface instead. If you previously made use of this class in custom code, you will need to update it to use one of these as appropriate.

### Transform updates

The `Transform` method can now be used to transform a property value to a different type prior to validation occurring. [See the documentation for further details.](transform)

### Severity with callback

Prior to 9.0, changing a rule's severity required hard-coding the severity:

```csharp
RuleFor(x => x.Surname).NotNull().WithSeverity(Severity.Warning);
```

Alternatively, this can now be generated from a callback, allowing the severity to be dynamically determined:

```csharp
RuleFor(x => x.Surname).NotNull().WithSeverity(x => Severity.Warning);
```

### Changes to the ScalePrecisionValidator

The algorithm used by the `ScalePrecision` validator has been updated to match SQL Server and other RDBMS systems. The algorithm now correctly checks how many digits are to the left of the decimal point, which it didn't do before. 

### ChildValidatorAdaptor and IncludeRule now have generic parameters

The `ChildvalidatorAdaptor` and `IncludeRule` classes now have generic type parameters. This will not affect users of the public API, but may affect anyone using the internal API. 

### Removed inferring property names from [Display] attribute

Older versions of FluentValidation allowed inferring a property's name from the presence of the `[Display]` or `[DisplayName]` attributes on the property. This behaviour has been removed as it causes conflicts with ASP.NET Core's approach to localization using these attributes.

If you want to preserve this old behaviour, you can use a custom display name resolver which can be set during your application's startup routine:

```csharp
FluentValidation.ValidatorOptions.DisplayNameResolver = (type, memberInfo, expression) => {
	return memberInfo.GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>()?.GetName();
};
```

### ComparisonProperty formatting

The `{ComparisonProperty}` error message placeholder (used in various validators that compare two properties, such as `LessThanOrEqual`) is now formatted consistently with the `{PropertyName}` placeholder, so PascalCased property names will be split.

### Renamed ShouldValidateAsync

Renamed the `PropertyValidator.ShouldValidateAsync` method to `ShouldValidateAsynchronously` to indicate that this is not an async method, which is usually denoted by the Async suffix.

### Removal of WithLocalizedMessage

This is only relevant if you use RESX-based localization with strongly-typed wrapper classes generated by Visual Studio. Older versions of FluentValidation allowed the use of specifying a resource name and resource type in a call to `WithLocalizedMessage`:

```csharp
RuleFor(x => x.Surname).NotNull().WithLocalizedMessage(typeof(MyLocalizedMessages), "SurnameRequired");
```

This syntax has been superceded by the callback syntax. To access the localized messages with a strongly-typed wrapper, you should now explicitly access the wrapper property inside a callback:

```csharp
RuleFor(x => x.Surname).NotNull().WithMessage(x => MyLocalizedMessages.SurnameRequired);
```

Note that support for localization with `IStringLocalzier` is unchanged.

[Full documentation on localization.](localization)

### SetCollectionValidator removed

`SetCollectionValidator` has been removed. This was [deprecated in 8.0](upgrading-to-8).

### Removal of Other Deprecated Features

Several other methods/properties that were deprecated in FluentValidation 8 have been removed in 9.0.

- `ReplacePlaceholderWithValue` and `GetPlaceholder` from `MesageFormatter`
- `ResourceName` and `ResourceType` have been removed from `IStringSource`.
- `ResourceName` has been removed from `ValidationFailure`.
- `Instance` was removed from `PropertyValidatorContext` - use `InstanceToValidate` instead.
- `DelegatingValidator` has been removed
- `FluentValidation.Internal.Comparer` has been removed
- `FluentValidation.Internal.TrackingCollection` is now internal

