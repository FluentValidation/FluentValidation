---
title: Throwing Exceptions
---

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
