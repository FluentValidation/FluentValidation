![FluentValidation](fv.png)

 [Full Documentation](https://github.com/JeremySkinner/FluentValidation/wiki) 

A small validation library for .NET that uses a fluent interface 
and lambda expressions for building validation rules.
Written by Jeremy Skinner (http://www.jeremyskinner.co.uk) and licensed under [Apache 2](http://www.apache.org/licenses/LICENSE-2.0.html).

### NuGet Packages

```
Install-Package FluentValidation
```

For ASP.NET MVC integration:

```
Install-Package FluentValidation.MVC5
```
For ASP.NET Core:

```
Install-Package FluentValidation.AspNetCore
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
