---
title: Validator Interceptors
---

You can further customize this process by using an interceptor. An interceptor has to implement the IValidatorInterceptor interface from the FluentValidation.WebApi namespace:

```csharp
public interface IValidatorInterceptor {
    ValidationContext BeforeMvcValidation(HttpActionContext controllerContext, ValidationContext validationContext);
 
    ValidationResult AfterMvcValidation(HttpActionContext controllerContext, ValidationContext validationContext, ValidationResult result);
}
```

This interface has two methods – BeforeMvcValidation and AfterMvcValidation. If you implement this interface in your validator classes then these methods will be called as appropriate during the MVC validation pipeline.

BeforeMvcValidation is invoked after the appropriate validator has been selected but before it is invoked. One of the arguments passed to this method is a ValidationContext that will eventually be passed to the validator. The context has several properties including a reference to the object being validated. If we want to change which rules are going to be invoked (for example, by using a custom ValidatorSelector) then we can create a new ValidationContext, set its Selector property, and return that from the BeforeMvcValidation method.

Likewise, AfterMvcValidation occurs after validation has occurs. This time, we also have a reference to the result of the validation. Here we can do some additional processing on the error messages before they’re added to modelstate.

As well as implementing this interface directly in a validator class, we can also implement it externally, and specify the interceptor by using a CustomizeValidatorAttribute on an action method parameter:

```csharp
public IHttpActionResult Save([CustomizeValidator(Interceptor=typeof(MyCustomerInterceptor))] Customer cust) {
 //...
}
```

In this case, the interceptor has to be a class that implements IValidatorInterceptor and has a public, parameterless constructor. The advantage of this approach is that your validators don’t have to be in an assembly that directly references System.Web.Mvc.

Note that this is considered to be an advanced scenario. Most of the time you probably won’t need to use an interceptor, but the option is there if you want it.
