<img src="https://raw.githubusercontent.com/JeremySkinner/FluentValidation/master/docs/assets/images/logo/fluent-validation-logo.png" alt="FluentValidation" width="250px" />

[Full Documentation](https://fluentvalidation.net)

A small validation library for .NET that uses a fluent interface
and lambda expressions for building validation rules.

### Get Started
FluentValidation can be installed using the Nuget package manager or the `dotnet` CLI.

```
Install-Package FluentValidation
```

For ASP.NET Core integration:
```
Install-Package FluentValidation.AspNetCore
```

For legacy ASP.NET MVC/WebApi integration:

```
Install-Package FluentValidation.MVC5
Install-Package FluentValidation.WebApi
```

---
[![Build Status](https://dev.azure.com/jeremy0621/FluentValidation/_apis/build/status/JeremySkinner.FluentValidation?branchName=master)](https://dev.azure.com/jeremy0621/FluentValidation/_build/latest?definitionId=1&branchName=master)

|         |       |       |
| ------- | ----- | ----- |
| `FluentValidation` | [![NuGet](https://img.shields.io/nuget/v/FluentValidation.svg)](https://nuget.org/packages/FluentValidation) | [![Nuget](https://img.shields.io/nuget/dt/FluentValidation.svg)](https://nuget.org/packages/FluentValidation) |
| `FluentValidation.AspNetCore` | [![NuGet](https://img.shields.io/nuget/v/FluentValidation.AspNetCore.svg)](https://nuget.org/packages/FluentValidation.AspNetCore) | [![Nuget](https://img.shields.io/nuget/dt/FluentValidation.AspNetCore.svg)](https://nuget.org/packages/FluentValidation.AspNetCore)
| `FluentValidation.Mvc5` | [![NuGet](https://img.shields.io/nuget/v/FluentValidation.Mvc5.svg)](https://nuget.org/packages/FluentValidation.Mvc5) | [![Nuget](https://img.shields.io/nuget/dt/FluentValidation.Mvc5.svg)](https://nuget.org/packages/FluentValidation.Mvc5)
| `FluentValidation.WebApi` | [![NuGet](https://img.shields.io/nuget/v/FluentValidation.WebApi.svg)](https://nuget.org/packages/FluentValidation.WebApi) | [![Nuget](https://img.shields.io/nuget/dt/FluentValidation.WebApi.svg)](https://nuget.org/packages/FluentValidation.WebApi)


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
