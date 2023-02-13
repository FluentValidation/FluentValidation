# Validating specific properties

If your validator contains rules for several properties you can limit execution to only validate specific properties by using the `IncludeProperties` option:

```csharp
// Validator definition
public class CustomerValidator : AbstractValidator<Customer>
{
  public CustomerValidator()
  {
    RuleFor(x => x.Surname).NotNull();
    RuleFor(x => x.Forename).NotNull();
    RuleForEach(x => x.Orders).SetValidator(new OrderValidator());
  }
}
```

```csharp
var validator = new CustomerValidator();
validator.Validate(customer, options => 
{
  options.IncludeProperties(x => x.Surname);
});
```

In the above example only the rule for the `Surname` property will be executed. 

When working with sub-properties of collections, you can use a wildcard indexer (`[]`) to indicate all items of a collection. For example, if you wanted to validate the `Cost` property of every order, you could use the following:

```csharp
var validator = new CustomerValidator();
validator.Validate(customer, options => 
{
  options.IncludeProperties("Orders[].Cost");
});
```

If you want more arbitrary grouping of rules you can use [Rule Sets](rulesets) instead. 