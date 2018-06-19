## Creating a Validator

Before creating any validators, you will need to add a reference to **FluentValidation.dll**

To define a set of validation rules for a particular object, you will need to create a class that inherits from **AbstractValidator&lt;T&gt;**, where T is the type of class that you wish to validate. 

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
for the property that you wish to validate. For example, to ensure that the `Surname` property is not null, 
the validator class would look like this:

```csharp
using FluentValidation;

public class CustomerValidator : AbstractValidator<Customer> {
  public CustomerValidator() {
    RuleFor(customer => customer.Surname).NotNull();
  }
}
```

## Chaining Validators for the Same Property

You can chain multiple validators together for the same property:

```csharp
using FluentValidation;

public class CustomerValidator : AbstractValidator<Customer> {
  public CustomerValidator() {
    RuleFor(customer => customer.Surname).NotNull().NotEqual("foo");
  }
}
```


This would ensure that the surname is not null and is not equal to the string 'foo'. 

To execute the validator, create an instance of the validator class and pass the object that you wish to validate to the `Validate` method.

```csharp
Customer customer = new Customer();
CustomerValidator validator = new CustomerValidator();

ValidationResult results = validator.Validate(customer);
```

## Validation Results

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

## Throwing Exceptions

Instead of returning a ValidationResult, you can alternatively tell FluentValidation to throw an exception if validation fails by using the ValidateAndthrow method:

```csharp
Customer customer = new Customer();
CustomerValidator validator = new CustomerValidator();

validator.ValidateAndThrow(customer);
```

This throws a `ValidationException` which contains the error messages in the Errors property. 

**Note** `ValidateAndThrow` is an extension method, so you must have the FluentValidation namespace imported for this method to be available. 

## Collections
You can use the `RuleForEach` method to apply the same rule to multiple items in a collection:

```csharp
public class Person {
  public List<string> AddressLines {get;set;} = new List<string>();
}
```

```csharp
public class PersonValidator : AbstractValidator<Person> {
  public PersonValidator() {
    RuleForEach(x => x.AddressLines).NotNull();
  }
}
```
The above rule will run a NotNull check against each item in the AddressLines collection. 

## Complex Properties

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


## Collections

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

## RuleSets

RuleSets allow you to group validation rules together which can be executed together as a group whilst ignoring other rules:

For example, let’s imagine we have 3 properties on a Person object (Id, Surname and Forename) and have a validation rule for each. We could group the Surname and Forename rules together in a “Names” RuleSet:

```csharp
 public class PersonValidator : AbstractValidator<Person> {
  public PersonValidator() {
     RuleSet("Names", () => {
        RuleFor(x => x.Surname).NotNull();
        RuleFor(x => x.Forename).NotNull();
     });
 
     RuleFor(x => x.Id).NotEqual(0);
  }
}
```

Here the two rules on Surname and Forename are grouped together in a “Names” RuleSet. We can invoke only these rules by passing a ruleSet parameter to the Validate extension method (note that this must be a named parameter as this overload has several options available).

```csharp
var validator = new PersonValidator();
var person = new Person();
var result = validator.Validate(person, ruleSet: "Names");
```

This allows you to break down a complex validator definition into smaller segments that can be executed in isolation. If you call `Validate` without passing a ruleset then only rules not in a RuleSet will be executed. 

You can execute multiple rulesets by using a comma-separated list of strings:

```csharp
validator.Validate(person, ruleSet: "Names,MyRuleSet,SomeOtherRuleSet")
```

...and you can also include rules not in any ruleset by specifying a ruleset of "default":

```csharp
validator.Validate(person, ruleSet: "default,MyRuleSet")
```

This would execute rules in the MyRuleSet set, and those rules not in any ruleset.

You can force all rules to be executed regardless of whether or not they're in a ruleset by specifying a ruleset of "*":

```csharp
validator.Validate(person, ruleSet: "*")
```

** Note ** The `Validate` method that takes a ruleset is an extension method so you must have the `FluentValidation` namespace imported with a `using` statment in order to use it. 