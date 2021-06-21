# Dependency Injection

The following example shows how one can inject a validator for a model `User` using `Microsoft.Extensions.DependencyInjection`.

Register the validator in Startup.cs

```c#
builder.Services.AddSingleton<IValidator<User>, UserValidator>();
```

And inject the validator in the class of your choice

```c#
public class UserService
{
    private readonly IValidator<User> _validator;

    public UserService(IValidator<User> validator)
    {
        _validator = validator;
    }

    public async Task DoSomething(User user)
    {
        var validationResult = await _validator.ValidateAsync(user);
    }
}
```
