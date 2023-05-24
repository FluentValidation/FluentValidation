# Other Advanced Features

These features are not normally used in day-to-day use, but provide some additional extensibility points that may be useful in some circumstances.

## PreValidate

If you need to run specific code every time a validator is invoked, you can do this by overriding the `PreValidate` method. This method takes a `ValidationContext` as well as a `ValidationResult`, which you can use to customise the validation process.

The method should return `true` if validation should continue, or `false` to immediately abort. Any modifications that you made to the `ValidationResult` will be returned to the user.

Note that this method is called before FluentValidation performs its standard null-check against the model being validated, so you can use this to generate an error if the whole model is null, rather than relying on FluentValidation's standard behaviour in this case (which is to throw an exception):

```csharp
public class MyValidator : AbstractValidator<Person> 
{
  public MyValidator() 
  {
    RuleFor(x => x.Name).NotNull();
  }

  protected override bool PreValidate(ValidationContext<Person> context, ValidationResult result) 
  {
    if (context.InstanceToValidate == null) 
    {
      result.Errors.Add(new ValidationFailure("", "Please ensure a model was supplied."));
      return false;
    }
    return true;
  }
}
```

## Root Context Data

For advanced users, it's possible to pass arbitrary data into the validation pipeline that can be accessed from within custom property validators. This is particularly useful if you need to make a conditional decision based on arbitrary data not available within the object being validated, as validators are stateless.

The `RootContextData` property is a `Dictionary<string, object>` available on the `ValidationContext`.:

```csharp
var person = new Person();
var context = new ValidationContext<Person>(person);
context.RootContextData["MyCustomData"] = "Test";
var validator = new PersonValidator();
validator.Validate(context);
```

The RootContextData can then be accessed inside any custom property validators, as well as calls to `Custom`:

```csharp
RuleFor(x => x.Surname).Custom((x, context) => 
{
  if(context.RootContextData.ContainsKey("MyCustomData")) 
  {
    context.AddFailure("My error message");
  }
});
```

## Customizing the Validation Exception

If you use the `ValidateAndThrow` method to [throw an exception when validation fails](start.html#throwing-exceptions) FluentValidation will internally throw a `ValidationException`. You can customzie this behaviour so a different exception is thrown by overriding the `RaiseValidationException` in your validator. 

This simplistic example wraps the default `ValidationException` in an `ArgumentException` instead:

```csharp
protected override void RaiseValidationException(ValidationContext<T> context, ValidationResult result)
{
    var ex = new ValidationException(result.Errors);
    throw new ArgumentException(ex.Message, ex);
}
```

This approach is useful if you always want to throw a specific custom exception type every time `ValidateAndThrow` is invoked.

As an alternative you could create your own extension method that calls `Validate` and then throws your own custom exception if there are validation errors. 


```csharp
public static class FluentValidationExtensions
{
    public static void ValidateAndThrowArgumentException<T>(this IValidator<T> validator, T instance)
    {
        var res = validator.Validate(instance);

        if (!res.IsValid)
        {
            var ex = new ValidationException(res.Errors);
            throw new ArgumentException(ex.Message, ex);
        }
    }
}
```

This approach is more useful if you only want to throw the custom exception when your specific method is invoked, rather than any time `ValidateAndThrow` is invoked.