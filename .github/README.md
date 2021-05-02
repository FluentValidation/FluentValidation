<img src="https://raw.githubusercontent.com/FluentValidation/FluentValidation/gh-pages/assets/images/logo/fluent-validation-logo.png" alt="FluentValidation" width="250px" />

[Full Documentation](https://fluentvalidation.net)

A small validation library for .NET that uses a fluent interface
and lambda expressions for building validation rules.

### Get Started
FluentValidation can be installed using the Nuget package manager or the `dotnet` CLI.

```
dotnet add package FluentValidation
```

For ASP.NET Core integration:
```
dotnet add package FluentValidation.AspNetCore
```
---
### Supporting the project
FluentValidation is developed and supported by [@JeremySkinner](https://github.com/JeremySkinner) for free in his spare time. If you find FluentValidation useful, please consider financially supporting the project via [GitHub sponsors](https://github.com/sponsors/JeremySkinner) or [OpenCollective](https://opencollective.com/FluentValidation)  which will help keep the project going 🙏.

---
[![Build Status](https://github.com/FluentValidation/FluentValidation/workflows/CI/badge.svg)](https://github.com/FluentValidation/FluentValidation/actions?query=workflow%3ACI)

|         |       |       |
| ------- | ----- | ----- |
| `FluentValidation` | [![NuGet](https://img.shields.io/nuget/v/FluentValidation.svg)](https://nuget.org/packages/FluentValidation) | [![Nuget](https://img.shields.io/nuget/dt/FluentValidation.svg)](https://nuget.org/packages/FluentValidation) |
| `FluentValidation.AspNetCore` | [![NuGet](https://img.shields.io/nuget/v/FluentValidation.AspNetCore.svg)](https://nuget.org/packages/FluentValidation.AspNetCore) | [![Nuget](https://img.shields.io/nuget/dt/FluentValidation.AspNetCore.svg)](https://nuget.org/packages/FluentValidation.AspNetCore)


### Example
```csharp
using FluentValidation;

public class CustomerValidator: AbstractValidator<Customer> {
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

var customer = new Customer();
var validator = new CustomerValidator();
ValidationResult results = validator.Validate(customer);

bool success = results.IsValid;
IList<ValidationFailure> failures = results.Errors;
```

### Documentation

[Documentation can be found on the project site.](https://fluentvalidation.net)

### License, Copyright etc

FluentValidation has adopted the code of conduct defined by the Contributor Covenant to clarify expected behavior in our community.
For more information see the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct). 

FluentValidation is copyright &copy; 2008-2021 .NET Foundation, [Jeremy Skinner](https://jeremyskinner.co.uk) and other contributors and is licensed under the [Apache2 license](https://github.com/JeremySkinner/FluentValidation/blob/master/License.txt). 

### .NET Foundation

This project is supported by the [.NET Foundation](https://dotnetfoundation.org).

### JetBrains 

This project is supported by [JetBrains](https://www.jetbrains.com/?from=FluentValidation), who kindly provide licenses for their software, free of charge, to help with the development of this project. 
