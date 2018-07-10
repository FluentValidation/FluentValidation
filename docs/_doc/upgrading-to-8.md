---
title: Upgrading to FluentValidation 8
---

FluentValidation 8.0 is a major release that included several breaking changes. Please review this document before upgrading from FluentValidation 7.x to 8.

### AbstractValidator is deprecated

The biggest change in FluentValidation 8 is that `AbstractValidator` is deprecated and has been marked as Obsolete. Instead of inheriting from `AbstractValidator` and defining rules in the constructor, you should now inherit from `ValidatorBase` and define your rules in the `Rules` method. 

FluentValidation 7:
```csharp
public class PersonValidator : AbstractValidator<Person> {
  public PersonValidator() {
    RuleFor(x => x.Surname).NotNull().WithMesage("Please enter your surname");
  }
}
```

Equivalent in FluentValidation 8:

```csharp
public class PersonValidator : ValidatorBase<Person> {
  protected override void Rules() {
    RuleFor(x => x.Surname).NotNull().WithMesage("Please enter your surname");
  }
}
```

Using this new approach, the first time a validator of a specific type is constructed its `Rules` method will be called to construct the rules. These rules will then be cached. Subsequent instantiations will use the cached rules rather than calling the `Rules` method again.

#### Why was this done?

By moving rule construction out of the validator's constructor and into a separate method, we can now provide complete caching of Rules. 

In FluentValidation 7, we introduced a cache for the expression trees, and in 8 this is extended further to cache entire rule definitions. This brings a performance benefit, especially in ASP.NET applications where validators are instantiated many times.

<div class="callout-block callout-warning"><div class="icon-holder">*&nbsp;*{: .fa .fa-bug}
</div><div class="content">
{: .callout-title}
#### AbstractValidator is not going away (yet)

Any existing validators you have that inherit from `AbstractValidator` will continue to function as before, although you will receive a compilation warning recommending that you move to use `ValidatorBase` instead.

</div></div>

### Asynchronous Validation updates

There have been many underlying changes to the asynchronous validation workflow in FluentValidation 8. 

<div class="callout-block callout-info"><div class="icon-holder">*&nbsp;*{: .fa .fa-info-circle}
</div><div class="content">
{: .callout-title}

If you make heavy use of asynchronous validation in FluentValidation, we would really appreciate you trying out the preview builds of FluentValidation 8 and report any issues that you encounter.

</div></div>

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

### Why was this done?

`SetCollectionValidator` was added to FluentValidation in its initial versions to provide a way to use a child validator against each element in a collection. `RuleForEach` was added later and provides a more comprehensive way of validating collections (as you can define in-line rules with RuleForEach too). It doesn't make sense to provide 2 ways to do the same thing.

### Many old and deprecated methods have been removed

FluentValidation 8 removes many old/deprecated methods that have been marked as obsolete for a long time.

- Removed the pre-7 way of performing custom validation (`Custom` and `CustomAsync`). Use `RuleFor(x => x).Custom()` instead. [See the section on Custom Validators](/custom-validators)
- The old localization mechanism that was deprecated with the release of FluentValidation 7. This included several overloads of `WithLocalizedName` and `WithLocalizedMessage`. [See the section on localization for more details](/localization).
- The `RemoveRule`, `ReplaceRule` and `ClearRules` methods that have been marked obsolete for many years (FluentValidation does not offer a replacement for these as runtime modification of validation rules is not recommended or supported in any way)
- Removed various async method overloads that didn't accept a `CancellationToken` (use the overloads that do accept them instead.)

### Other changes
`IStringSource.GetString` now receives a context, instead of a model. If you have custom `IStringSource` implementations, you will need to update them. 

