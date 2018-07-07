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
