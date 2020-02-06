---
layout: home
title: Home
tagline: |
  A popular .NET library for building strongly-typed validation rules.
---

![NuGet](https://img.shields.io/nuget/v/FluentValidation.svg) ![Nuget](https://img.shields.io/nuget/dt/FluentValidation.svg) [![Build Status](https://dev.azure.com/jeremy0621/FluentValidation/_apis/build/status/JeremySkinner.FluentValidation?branchName=master)](https://dev.azure.com/jeremy0621/FluentValidation/_build/latest?definitionId=1&branchName=master)

<div class="cta-container" markdown="1">

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

 <div id="cards-wrapper" class="cards-wrapper row" style="margin-top: 60px">
  <div class="item item-green col-md-4 col-sm-6 col-xs-6">
    <div class="item-inner" style="height: 254px;">
      <div class="icon-holder"> <i class="icon fa fa-paper-plane"></i> </div>
      <h3 class="title">Getting Started</h3>
      <p class="intro">Learn how to install FluentValidation and get started creating validators. </p> <a class="link"
        href="https://docs.fluentvalidation.net/en/latest/start.html"><span></span></a>
    </div>
  </div>
  <div class="item item-orange col-md-4 col-sm-6 col-xs-6">
    <div class="item-inner" style="height: 254px;">
      <div class="icon-holder"> <span aria-hidden="true" class="icon icon_puzzle_alt"></span> </div>
      <h3 class="title">Built-in Validators</h3>
      <p class="intro">A complete list of built-in validators. </p> <a class="link"
        href="https://docs.fluentvalidation.net/en/latest/built-in-validators.html"><span></span></a>
    </div>
  </div>
  <div class="item item-blue col-md-4 col-sm-6 col-xs-6">
    <div class="item-inner" style="height: 254px;">
      <div class="icon-holder"> <span aria-hidden="true" class="icon icon_tools"></span> </div>
      <h3 class="title">Custom Validators</h3>
      <p class="intro">Create custom validators </p> <a class="link" href="https://docs.fluentvalidation.net/en/latest/custom-validators.html"><span></span></a>
    </div>
  </div>
  <div class="item item-pink col-md-4 col-sm-6 col-xs-6">
    <div class="item-inner" style="height: 256px;">
      <div class="icon-holder"> <span aria-hidden="true" class="icon icon_document_alt"></span> </div>
      <h3 class="title">Localization</h3>
      <p class="intro">Translating error messages into other languages </p> <a class="link"
        href="https://docs.fluentvalidation.net/en/latest/localization.html"><span></span></a>
    </div>
  </div>
  <div class="item item-primary col-md-4 col-sm-6 col-xs-6">
    <div class="item-inner" style="height: 256px;">
      <div class="icon-holder"> <i class="icon fa fa-graduation-cap"></i> </div>
      <h3 class="title">Testing</h3>
      <p class="intro">Testing your validation rules </p> <a class="link" href="https://docs.fluentvalidation.net/en/latest/testing.html"><span></span></a>
    </div>
  </div>
  <div class="item item-green col-md-4 col-sm-6 col-xs-6">
    <div class="item-inner" style="height: 256px;">
      <div class="icon-holder"> <span aria-hidden="true" class="icon icon_globe"></span> </div>
      <h3 class="title">ASP.NET Integration</h3>
      <p class="intro">Integration with ASP.NET Core, ASP.NET MVC 5 and ASP.NET WebApi 2 </p> <a class="link"
        href="https://docs.fluentvalidation.net/en/latest/aspnet.html"><span></span></a>
    </div>
  </div>
  <div class="item item-red col-md-4 col-sm-6 col-xs-6"><!--Intentionally empty--></div>
  <div class="item item-red col-md-4 col-sm-6 col-xs-6">
    <div class="item-inner">
      <div class="icon-holder">
        <span aria-hidden="true" class="icon icon_document"></span>
      </div>
      <h3 class="title">Full Documentation</h3>
      <p class="intro">Read the full documentation </p>
      <a class="link" href="https://docs.fluentvalidation.net"><span></span></a>
    </div>
  </div>
</div>
