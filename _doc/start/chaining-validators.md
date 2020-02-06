---
title: Chaining validators
---
You can chain multiple validators together for the same property:

```csharp
using FluentValidation;

public class CustomerValidator : AbstractValidator<Customer> {
  public CustomerValidator() 
    RuleFor(customer => customer.Surname).NotNull().NotEqual("foo");
  }
}
```

This would ensure that the surname is not null and is not equal to the string 'foo'. 
