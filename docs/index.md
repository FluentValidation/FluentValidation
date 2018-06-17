FluentValidation is a validation library for .NET that uses a fluent interface and lambda expressions to build validation rules.

## Example
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

<table class="artifacts">
  <tr>
    <td>&nbsp;</td>
    <td>
      [![Build status](https://ci.appveyor.com/api/projects/status/b9bkth37cdtsifac?svg=true)](https://ci.appveyor.com/project/JeremySkinner/fluentvalidation)
    </td>
    <td>
      [![Tests](https://img.shields.io/appveyor/tests/JeremySkinner/FluentValidation.svg)](https://ci.appveyor.com/project/JeremySkinner/fluentvalidation)
    </td>
  </tr>
  
  <tr>
    <td>FluentValidation</td>
    <td>
      [![NuGet](https://img.shields.io/nuget/v/FluentValidation.svg)](https://nuget.org/packages/FluentValidation)
    </td>
    <td>
      [![Nuget](https://img.shields.io/nuget/dt/FluentValidation.svg)](https://nuget.org/packages/FluentValidation)
    </td>
  </tr>

  <tr>
    <td>FluentValidation.AspNetCore</td>
    <td>
      [![NuGet](https://img.shields.io/nuget/v/FluentValidation.AspNetCore.svg)](https://nuget.org/packages/FluentValidation.AspNetCore)
    </td>
    <td>
      [![Nuget](https://img.shields.io/nuget/dt/FluentValidation.AspNetCore.svg)](https://nuget.org/packages/FluentValidation.AspNetCore)
    </td>
  </tr>

  <tr>
    <td>FluentValidation.Mvc5</td>
    <td>
      [![NuGet](https://img.shields.io/nuget/v/FluentValidation.Mvc5.svg)](https://nuget.org/packages/FluentValidation.Mvc5)
    </td>
    <td>
      [![Nuget](https://img.shields.io/nuget/dt/FluentValidation.Mvc5.svg)](https://nuget.org/packages/FluentValidation.Mvc5)
    </td>
  </tr>

  <tr>
    <td>FluentValidation.WebApi</td>
    <td>
      [![NuGet](https://img.shields.io/nuget/v/FluentValidation.WebApi.svg)](https://nuget.org/packages/FluentValidation.WebApi)
    </td>
    <td>
      [![Nuget](https://img.shields.io/nuget/dt/FluentValidation.WebApi.svg)](https://nuget.org/packages/FluentValidation.WebApi)
    </td>
  </tr>
</table>
