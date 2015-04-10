![FluentValidation](fv.png)

A small validation library for .NET that uses a fluent interface 
and lambda expressions for building validation rules.
Written by Jeremy Skinner (http://www.jeremyskinner.co.uk) and licensed under [Apache 2](http://www.apache.org/licenses/LICENSE-2.0.html).

[![Support FluentValidation](http://www.pledgie.com/campaigns/8403.png?skin_name=chrome)](http://www.pledgie.com/campaigns/8403)

If you find FluentValidation useful, [please consider making a donation](http://www.pledgie.com/campaigns/8403)

### NuGet Packages

[NuGet packages are available](doc/nuget.md)

```
Install-Package FluentValidation
```

For ASP.NET MVC integration:

```
Install-Package FluentValidation.MVC5
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

[Documentation can be found here.](doc/index.md) 
