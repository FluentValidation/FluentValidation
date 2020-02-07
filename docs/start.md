# Creating your first validator

To define a set of validation rules for a particular object, you will need to create a class that inherits from `AbstractValidator<T>`, where `T` is the type of class that you wish to validate. 

For example, imagine that you have a Customer class:

```csharp
public class Customer {
  public int Id { get; set; }
  public string Surname { get; set; }
  public string Forename { get; set; }
  public decimal Discount { get; set; }
  public string Address { get; set; }
}
```

You would define a set of validation rules for this class by inheriting from `AbstractValidator<Customer>`:

```csharp
using FluentValidation; 

public class CustomerValidator : AbstractValidator<Customer> {
}
```

The validation rules themselves should be defined in the validator class's constructor.

To specify a validation rule for a particular property, call the `RuleFor` method, passing a lambda expression 
that indicates the property that you wish to validate. For example, to ensure that the `Surname` property is not null, 
the validator class would look like this:

```csharp
using FluentValidation;

public class CustomerValidator : AbstractValidator<Customer> {
  public CustomerValidator() {
    RuleFor(customer => customer.Surname).NotNull();
  }
}
```
To run the validator, instantiate the validator object and call the `Validate` method, passing in the object to validate.

```csharp
Customer customer = new Customer();
CustomerValidator validator = new CustomerValidator();

ValidationResult result = validator.Validate(customer);

```

The `Validate` method returns a ValidationResult object. This contains two properties:

- `IsValid` - a boolean that says whether the validation suceeded.
- `Errors` - a collection of ValidationFailure objects containing details about any validation failures.

The following code would write any validation failures to the console:

```csharp
Customer customer = new Customer();
CustomerValidator validator = new CustomerValidator();

ValidationResult results = validator.Validate(customer);

if(! results.IsValid) {
  foreach(var failure in results.Errors) {
    Console.WriteLine("Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage);
  }
}
```

You can also call `ToString` on the `ValidationResult` to combine all error messages into a single string. By default, the messages will be separated with new lines, but if you want to customize this behaviour you can pass a different separator character to `ToString`.

```csharp
ValidationResult results = validator.Validate(customer);
string allMessages = results.ToString("~");     // In this case, each message will be separated with a `~` 
```

*Note* : if there are no validation errors, `ToString()` will return an empty string.

# Chaining validators

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

# Throwing Exceptions

Instead of returning a `ValidationResult`, you can alternatively tell FluentValidation to throw an exception if validation fails by using the `ValidateAndThrow` method:

```csharp
Customer customer = new Customer();
CustomerValidator validator = new CustomerValidator();

validator.ValidateAndThrow(customer);
```

This throws a `ValidationException` which contains the error messages in the Errors property. 

*Note* `ValidateAndThrow` is an extension method, so you must have the `FluentValidation` namespace imported with a `using` statement at the top of your file in order for this method to be available.

```csharp
using FluentValidation;
```

# Complex Properties


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
RuleFor(customer => customer.Address.Postcode).NotNull()
``` 

In this case, a null check will *not* be performed automatically on `Address`, so you should explicitly add a condition

```csharp
RuleFor(customer => customer.Address.Postcode).NotNull().When(customer => customer.Address != null)
```
