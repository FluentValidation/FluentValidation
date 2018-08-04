---
layout: home
title: Home
tagline: |
  A popular .NET library for building strongly-typed validation rules.
navigation:
  - /start
  - /built-in-validators
  - /custom-validators
  - /localization
  - /testing
  - /aspnet
  - /news
  - /community
  - /legal
---

![NuGet](https://img.shields.io/nuget/v/FluentValidation.svg) ![Nuget](https://img.shields.io/nuget/dt/FluentValidation.svg) ![Build status](https://ci.appveyor.com/api/projects/status/b9bkth37cdtsifac?svg=true) ![Tests](https://img.shields.io/appveyor/tests/JeremySkinner/FluentValidation.svg)

<div class="cta-container">

[*&nbsp;*{: .fa .fa-cloud-download} Download Now][NUGET]{: .btn .btn-primary .btn-cta}
[*&nbsp;*{: .fa .fa-github} View On Github][GITHUB]{: .btn .btn-green .btn-primary .btn-cta}

</div>

[NUGET]: https://nuget.org/packages/FluentValidation
[GITHUB]: https://github.com/JeremySkinner/FluentValidation

```csharp
public class CustomerValidator : AbstractValidator<Customer> {
  public CustomerValidator() {
    RuleFor(x => x.Surname).NotEmpty();
    RuleFor(x => x.Forename).NotEmpty().WithMessage("Please specify a first name");
    RuleFor(x => x.Discount).NotEqual(0).When(x => x.HasDiscount);
    RuleFor(x => x.Address).Length(20, 250);
    RuleFor(x => x.Postcode).Must(BeAValidPostcode).WithMessage("Please specify a valid postcode");
  }

  private bool BeAValidPostcode(string postcode) {
    // custom postcode validating logic goes here
  }
}
```