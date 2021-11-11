# Dependency Injection

Validators can be used with any dependency injection library, such as `Microsoft.Extensions.DependencyInjection`. To inject a validator for a specific model, you should register the validator with the service provider as `IValidator<T>`, where `T` is the type of object being validated.

For example, imagine you have the following validator defined in your project:

```csharp
public class UserValidator : AbstractValidator<User>
{
  public UserValidator()
  {
    RuleFor(x => x.Name).NotNull();
  }
}
```

This validator can be registered as `IValidator<User>` in your application's startup routine by calling into the .NET service provider. For example, in a Razor pages application the startup routine would look something like this:

```csharp
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddScoped<IValidator<User>, UserValidator>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      // ...
    }
}
```

You can then inject the validator as you would with any other dependency:

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

## Automatic registration

You can also make use of the `FluentValidation.DependencyInjectionExtensions` package which can be used to automatically find all the validators in a specific assembly using an extension method:

```csharp
using FluentValidation.DependencyInjectionExtensions;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<UserValidator>();
        // ...
    }

    // ...
}
```

This will loop through all public types in the same assembly in which `UserValidator` is defined, find all public non-abstract validators and register them with the service provider. By default, these will be registered as `Scoped`, but you can optionally use `Singleton` or `Transient` instead:

```csharp
services.AddValidatorsFromAssemblyContaining<UserValidator>(ServiceLifetime.Transient);
```

If you aren't familiar with the difference between Singleton, Scoped and Transient [please review the Microsoft dependency injection documentation](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection#service-lifetimes)


```eval_rst
.. warning::
   If you register a validator as Singleton, you should ensure that you don't inject anything that's transient or request-scoped into the validator. We typically don't recommend registering validators as Singleton unless you are experienced with using Dependency Injection and know how to troubleshoot issues related to singleton-scoped objects having on non-singleton dependencies. Registering validators as Transient is the simplest and safest option.
```

When using FluentValidation in an ASP.NET project with auto-validation, the same scanning logic can be performed as part of the call to `AddFluentValidation`. [See the documentation on ASP.NET integration for details](aspnet).

Alternative method overloads that take a type instance and an assembly reference exist too:

```csharp
// Load using a type refernce rather than the generic.
services.AddValidatorsFromAssemblyContaining(typeof(UserValidator));

// Load an assembly reference rather than using a marker type.
services.AddValidatorsFromAssembly(Assembly.Load("SomeAssembly"));
```

### Filtering results

You can provide an optional filter function that can be used to exclude some validators from automatic registration. For example, to register all validators *except* the `CustomerValidator` you could write the following:

```csharp
services.AddValidatorsFromAssemblyContaining<MyValidato>(ServiceLifetime.Scoped, 
    filter => filter.ValidatorType != typeof(CustomerValidator));
```

The `CustomerValidator` will not be added to the service provider (but all other validators will)

## Injecting child validators

The `FluentValidation.DependencyInjectionExtensions` package also provides some helper methods for injecting child validators when working with ASP.NET Core. See [this page](aspnet.html#injecting-child-validators).
