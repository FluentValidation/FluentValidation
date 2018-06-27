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
