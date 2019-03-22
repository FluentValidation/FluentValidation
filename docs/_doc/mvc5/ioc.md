---
title: Using an Inversion of Control Container
---

When using FluentValidation's ASP.NET MVC 5 integration you may wish to use an Inversion of Control container to instantiate your validators rather than relying on the attribute based approach. This can be achieved by writing a custom Validator Factory. 

The IValidatorFactory interface defines the contract for validator factories. 

```csharp
public interface IValidatorFactory {
  IValidator<T> GetValidator<T>();
  IValidator GetValidator(Type type);
}
```

Instead of implementing this interface directly, you can inherit from the `ValidatorFactoryBase` class which does most of the work for you. When you inherit from ValidatorFactoryBase you should override the `CreateInstance` method. In this method you should call in to your IoC container to resolve an instance of the specified type or return `null` if it does not exist (many containers have a "TryResolve" method that will do this automatically).

Once you've implemented this interface, you can set the `ValidatorFactory` of the provider during application startup:

```csharp
FluentValidationModelValidatorProvider.Configure(cfg => {
    cfg.ValidatorFactory = new MyValidatorFactory();
});
```