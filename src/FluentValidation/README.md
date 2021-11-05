FluentValidation is validation library for .NET that uses a fluent interface
and lambda expressions for building strongly-typed validation rules.

### Supporting the project

If you use FluentValidation in a commercial project,
please sponsor the project financially.
FluentValidation is developed and supported by [@JeremySkinner](https://github.com/JeremySkinner)
for free in his spare time and financial sponsorship helps keep the project going.
You can sponsor the project via either [GitHub sponsors](https://github.com/sponsors/JeremySkinner) or [OpenCollective](https://opencollective.com/FluentValidation).

### Example

With FluentValidation, you can define a class that inherits from `AbstractValidator`
which contains the rules for a particular class. The example below shows how you could define rules
for a `Customer` class, and then how to execute the validator.

```c#
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

// Execute the validator.
ValidationResult results = validator.Validate(customer);

// Inspect any validation failures.
bool success = results.IsValid;
List<ValidationFailure> failures = results.Errors;
```

### Full Documentation

Full documentation can be found at
[https://docs.fluentvalidation.net](https://docs.fluentvalidation.net)

### Release Notes and Change Log

Release notes [can be found on GitHub](https://github.com/FluentValidation/FluentValidation/releases).
