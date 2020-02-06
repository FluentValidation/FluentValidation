---
title: Manual Validation
---
Sometimes you may want to manually validate an object in a WebApi project. In this case, the validation results can be copied to MVC's modelstate dictionary:

```csharp
public IHttpActionResult DoSomething() {
  var customer = new Customer();
  var validator = new CustomerValidator();
  var results = validator.Validate(customer);

  results.AddToModelState(ModelState, null);
  return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
}
```

The `AddToModelState` method is implemented as an extension method, and requires a using statement for the `FluentValidation.WebApi` namespace. Note that the second parameter is an optional model name, which will cause property names in the ModelState to be prefixed (eg a call to AddToModelState(ModelState, "Foo") will generate property names of "Foo.Id" and "Foo.Name" etc rather than just "Id" or "Name")
