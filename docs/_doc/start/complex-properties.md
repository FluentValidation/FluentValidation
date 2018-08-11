---
title: Complex Properties
---

Validators can be re-used for complex properties. For example, imagine you have two classes, Customer and Address:

```csharp
public class Customer {
  public string Name { get; set; }
  public Address Address { get; set; }
}

public class Address {
  public string Line1 { get; set; }
  public string Line2 { get; set; }
  public string Town { get; set; }
  public string County { get; set; }
  public string Postcode { get; set; }
}
```

... and you define an AddressValidator:

```csharp
public class AddressValidator : AbstractValidator<Address> {
  public AddressValidator() {
    RuleFor(address => address.Postcode).NotNull();
    //etc
  }
}
```

... you can then re-use the AddressValidator in the CustomerValidator definition:

```csharp
public class CustomerValidator : AbstractValidator<Customer> {
  public CustomerValidator() {
    RuleFor(customer => customer.Name).NotNull();
    RuleFor(customer => customer.Address).SetValidator(new AddressValidator());
  }
} 
```

... so when you call `Validate` on the CustomerValidator it will run through the validators defined in both the CustomerValidator and the AddressValidator and combine the results into a single ValidationResult. 

If the child property is null, then the child validator will not be executed.

Instead of using a child validator, you can define child rules inline, eg:

```csharp
RuleFor(customer => customer.Address.PostCode).NotNull()
``` 

In this case, a null check will *not* be performed automatically on `Address`, so you should explicitly add a condition

```csharp
RuleFor(customer => customer.Address.PostCode).NotNull().When(customer => customer.Address != null)
```