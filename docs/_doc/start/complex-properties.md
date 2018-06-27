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

#### Collections

Validators can also be re-used on properties that contain collections of other objects. For example, imagine a Customer object that has a collection of Orders:

```csharp
public class Customer {
   public IList<Order> Orders { get; set; }
}

public class Order {
  public string ProductName { get; set; }
  public decimal? Cost { get; set; }
}

var customer = new Customer();
customer.Orders = new List<Order> {
  new Order { ProductName = "Foo" },
  new Order { Cost = 5 } 
};
```

... and you've already defined an OrderValidator:

```csharp
public class OrderValidator : AbstractValidator<Order> {
    public OrderValidator() {
        RuleFor(x => x.ProductName).NotNull();
        RuleFor(x => x.Cost).GreaterThan(0);
    }
}
```

....this validator can be used within the CustomerValidator definition:

```csharp
public class CustomerValidator : AbstractValidator<Customer> {
    public CustomerValidator() {
        RuleFor(x => x.Orders).SetCollectionValidator(new OrderValidator());
    }
}

var validator = new CustomerValidator();
var results = validator.Validate(customer);
```

When the validator is executed, the error messages will reflect the placement of the order object within the collection:

```csharp
foreach(var result in results.Errors) {
   Console.WriteLine("Property name: " + result.PropertyName);
   Console.WriteLine("Error: " + result.ErrorMessage);
   Console.WriteLine("");
}
```

```
Property name: Orders[0].Cost
Error: 'Cost' must be greater than '0'.

Property name: Orders[1].ProductName
Error: 'Product Name' must not be empty.
```

You can optionally include or exclude certain items in the collection from being validated by using the Where method:

```csharp
 RuleFor(x => x.Orders).SetCollectionValidator(new OrderValidator())
        .Where(x => x.Cost != null);
```
