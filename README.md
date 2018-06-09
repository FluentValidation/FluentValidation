![FluentValidation](fv.png)

 [Full Documentation](https://github.com/JeremySkinner/FluentValidation/wiki) 

A small validation library for .NET that uses a fluent interface 
and lambda expressions for building validation rules.

[![Build status](https://ci.appveyor.com/api/projects/status/b9bkth37cdtsifac?svg=true)](https://ci.appveyor.com/project/JeremySkinner/fluentvalidation) 

[![Nuget](https://img.shields.io/nuget/dt/FluentValidation.svg?label=FluentValidation%20Downloads)](https://nuget.org/packages/FluentValidation) 
[![Nuget](https://img.shields.io/nuget/dt/FluentValidation.AspNetCore.svg?label=FluentValidation.AspNetCore%20Downloads)](https://nuget.org/packages/FluentValidation.AspNetCore) <br/>
[![Nuget](https://img.shields.io/nuget/dt/FluentValidation.Mvc5.svg?label=FluentValidation.Mvc5%20Downloads)](https://nuget.org/packages/FluentValidation.Mvc5) 
[![Nuget](https://img.shields.io/nuget/dt/FluentValidation.WebApi.svg?label=FluentValidation.WebApi%20Downloads)](https://nuget.org/packages/FluentValidation.WebApi)

### NuGet Packages

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

### Example
```csharp
using FluentValidation;

public class CustomerValidator: AbstractValidator<Customer> {
  public CustomerValidator() {
    RuleFor(customer => customer.Surname).NotEmpty();
    RuleFor(customer => customer.Forename).NotEmpty().WithMessage("Please specify a first name");
    RuleFor(customer => customer.Discount).NotEqual(0).When(customer => customer.HasDiscount);
    RuleFor(customer => customer.Address).Length(20, 250);
    RuleFor(customer => customer.Postcode).Must(BeAValidPostcode).WithMessage("Please specify a valid postcode");
  }

  private bool BeAValidPostcode(string postcode) {
    // custom postcode validating logic goes here
  }
}

Customer customer = new Customer();
CustomerValidator validator = new CustomerValidator();
ValidationResult results = validator.Validate(customer);

bool validationSucceeded = results.IsValid;
IList<ValidationFailure> failures = results.Errors;
```

### Further Documentation

[Documentation can be found here.](https://github.com/JeremySkinner/FluentValidation/wiki) 
