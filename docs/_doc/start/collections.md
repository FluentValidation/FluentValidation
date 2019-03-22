---
title: Collections
---

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
The above rule will run a NotNull check against each item in the `AddressLines` collection. 

You can optionally include or exclude certain items in the collection from being validated by using the `Where` method. Note this must come directly after the call to `RuleForEach`:

```csharp
RuleForEach(x => x.Orders)
  .Where(x => x.Cost != null)
  .SetValidator(new OrderValidator());
```
As of version 8.2, an alternative to using `RuleForEach` is to call `ForEach` as part of a regular `RuleFor`. With this approach you can combine rules that act upon the entire collection with rules which act upon individual elements within the collection. For example:

```csharp
RuleFor(x => x.Orders)
  .Must(x => x.Orders.Count <= 10).WithMessage("No more than 10 orders are allowed");

RuleForEach(x => x.Orders)
  .Must(order => order.Total > 0).WithMessage("Orders must have a total of more than 0")
```

The above 2 rules could be re-written as:

```csharp
RuleFor(x => x.Orders)
  .Must(x => x.Orders.Count <= 10).WithMessage("No more than 10 orders are allowed")
  .ForEach(orderRule => {
    orderRule.Must(order => order.Total > 0).WithMessage("Orders must have a total of more than 0")
  });
```

I personally think that using 2 rules is clearer and easier to read, but the option of combining them is available with the `ForEach` method.
