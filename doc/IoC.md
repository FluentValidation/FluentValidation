## Using a Validator Factory with an Inversion of Control Container
When using FluentValidation's ASP.NET MVC integration you may wish to use an Inversion of Control container to instantiate your validators. This can be achieved by writing a custom Validator Factory. 

The IValidatorFactory interface defines the contract for validator factories. 

```csharp
public interface IValidatorFactory {
  IValidator<T> GetValidator<T>();
  IValidator GetValidator(Type type);
}
```

Instead of implementing this interface directly, you can inherit from the *ValidatorFactoryBase* class which does most of the work for you. When you inherit from ValidatorFactoryBase you should override the *CreateInstance* method. In this method you should call in to your IoC container to resolve an instance of the specified type or return *null* if it does not exist (many containers have a "TryResolve" method that will do this automatically).

For example, here is a Validator Factory that uses StructureMap:

```csharp
public class StructureMapValidatorFactory : ValidatorFactoryBase {
	public override IValidator CreateInstance(Type validatorType) {
		return ObjectFactory.TryGetInstance(validatorType) as IValidator;
	}
}
```

You can then use this validator factory anywhere that requires an IValidatorFactory instance (eg the MVC integration). 
For more details see this post: http://www.jeremyskinner.co.uk/2010/02/22/using-fluentvalidation-with-an-ioc-container/
