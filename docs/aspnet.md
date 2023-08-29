# ASP.NET Core

FluentValidation can be used within ASP.NET Core web applications to validate incoming models. There are several approaches for doing this: 

- Manual validation
- Automatic validation (using the ASP.NET validation pipeline)
- Automatic validation (using a filter)

With manual validation, you inject the validator into your controller (or api endpoint), invoke the validator and act upon the result. This is the most straightforward approach and also the easiest to see what's happening. 

With automatic validation, FluentValidation is invoked automatically by ASP.NET earlier in the pipeline which allows models to be validated before a controller action is invoked. 

## Getting started

The following examples will make use of a `Person` object which is validated using a `PersonValidator`. These classes are defined as follows:

```csharp
public class Person 
{
  public int Id { get; set; }
  public string Name { get; set; }
  public string Email { get; set; }
  public int Age { get; set; }
}

public class PersonValidator : AbstractValidator<Person> 
{
  public PersonValidator() 
  {
    RuleFor(x => x.Id).NotNull();
    RuleFor(x => x.Name).Length(0, 10);
    RuleFor(x => x.Email).EmailAddress();
    RuleFor(x => x.Age).InclusiveBetween(18, 60);
  }
}
```

If you're using MVC, Web Api or Razor Pages you'll need to register your validator with the Service Provider in the `ConfigureServices` method of your application's `Startup` class. (note that if you're using Minimal APIs, [see the section on Minimal APIs below](aspnet.html#minimal-apis)). 

```csharp
public void ConfigureServices(IServiceCollection services) 
{
    // If you're using MVC or WebApi you'll probably have
    // a call to AddMvc() or AddControllers() already.
    services.AddMvc();
    
    // ... other configuration ...
    
    services.AddScoped<IValidator<Person>, PersonValidator>();
}
```

Here we register our `PersonValidator` with the service provider by calling `AddScoped`.

```eval_rst
.. note::
  Note that you must register each validator as `IValidator<T>` where `T` is the type being validated. So if you have a `PersonValidator` that inherits from `AbstractValidator<Person>` then you should register it as `IValidator<Person>`
```

Alternatively you can register all validators in a specific assembly by using our Service Collection extensions. To do this you'll need to install the `FluentValidation.DependencyInjectionExtensions` package and then call the appropriate `AddValidators...` extension method on the services collection. [See this page for more details](di.html#automatic-registration)

```csharp
public void ConfigureServices(IServiceCollection services) 
{
    services.AddMvc();

    // ... other configuration ...

    services.AddValidatorsFromAssemblyContaining<PersonValidator>();
}
```

Here we use the `AddValidatorsFromAssemblyContaining` method from the `FluentValidation.DependencyInjectionExtension` package to automatically register all validators in the same assembly as `PersonValidator` with the service provider.

Now that the validators are registered with the service provider you can start working with either manual validation or automatic validation.

## Manual Validation

With the manual validation approach, you'll inject the validator into your controller (or Razor page) and invoke it against the model.

For example, you might have a controller that looks like this:

```csharp
public class PeopleController : Controller 
{
  private IValidator<Person> _validator;
  private IPersonRepository _repository;

  public PeopleController(IValidator<Person> validator, IPersonRepository repository) 
  {
    // Inject our validator and also a DB context for storing our person object.
    _validator = validator;
    _repository = repository;
  }

  public ActionResult Create() 
  {
    return View();
  }

  [HttpPost]
  public async Task<IActionResult> Create(Person person) 
  {
    ValidationResult result = await _validator.ValidateAsync(person);

    if (!result.IsValid) 
    {
      // Copy the validation results into ModelState.
      // ASP.NET uses the ModelState collection to populate 
      // error messages in the View.
      result.AddToModelState(this.ModelState);

      // re-render the view when validation failed.
      return View("Create", person);
    }

    _repository.Save(person); //Save the person to the database, or some other logic

    TempData["notice"] = "Person successfully created";
    return RedirectToAction("Index");
  }
}
```

Because our validator is registered with the Service Provider, it will be injected into our controller via the constructor. We can then make use of the validator inside the `Create` action by invoking it with `ValidateAsync`. 

If validation fails, we need to pass the error messages back down to the view so they can be displayed to the end user. We can do this by defining an extension method for FluentValidation's `ValidationResult` type that copies the error messages into ASP.NET's `ModelState` dictionary:

```csharp
public static class Extensions 
{
  public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState) 
  {
    foreach (var error in result.Errors) 
    {
      modelState.AddModelError(error.PropertyName, error.ErrorMessage);
    }
  }
}
```

This method is invoked inside the controller action in the example above. 


For completeness, here is the corresponding View. This view will pick up the error messages from `ModelState` and display them next to the corresponding property. (If you were writing an API controller, then you'd probably return either a `ValidationProblemDetails` or `BadRequest` instead of a view result)

```html
@model Person

<div asp-validation-summary="ModelOnly"></div>

<form asp-action="Create">
  Id: <input asp-for="Id" /> <span asp-validation-for="Id"></span>
  <br />
  Name: <input asp-for="Name" /> <span asp-validation-for="Name"></span>
  <br />
  Email: <input asp-for="Email" /> <span asp-validation-for="Email"></span>
  <br />
  Age: <input asp-for="Age" /> <span asp-validation-for="Age"></span>

  <br /><br />
  <input type="submit" value="submit" />
</form>
```

## Automatic Validation

Automatic validation instantiates and invokes a validator before the controller action is executed, meaning the ModelState will already be populated with validation results by the time your controller action is invoked. There are 2 implementations for this approach:

- Using ASP.NET's validation pipeline (no longer recommended)
- Using an Action Filter (supported by a 3rd party package)

### Using the ASP.NET Validation Pipeline

The `FluentValidation.AspNetCore` package provides auto-validation for ASP.NET Core MVC projects by plugging into ASP.NET's validation pipeline. 

With automatic validation using the validation pipeline, FluentValidation plugs into ASP.NET's bult-in validation process that's part of ASP.NET Core MVC and allows models to be validated before a controller action is invoked (during model-binding). This approach to validation is more seamless but has several downsides:

- **The ASP.NET validation pipeline is not asynchronous**: If your validator contains asynchronous rules then your validator will not be able to run. You will receive an exception at runtime if you attempt to use an asynchronous validator with auto-validation.
- **It is MVC-only**: This approach for auto-validation only works with MVC Controllers and Razor Pages. It does not work with the more modern parts of ASP.NET such as Minimal APIs or Blazor.
- **It is harder to debug**: The 'magic' nature of auto-validation makes it hard to debug/troubleshoot if something goes wrong as so much is done behind the scenes. 

```eval_rst
.. warning::
  We no longer recommend using this approach for new projects but it is still available for legacy implementations.
```

Instructions for this appraoch can be found in the `FluentValidation.AspNetCore` package [can be found on its project page here](https://github.com/FluentValidation/FluentValidation.AspNetCore#aspnet-core-integration-for-fluentvalidation).

### Using a Filter

An alternative approach for perorming automatic validation is to use an Action Filter. This approach works asynchronously which mitigates the synchronous limitation of the Validation Pipeline approach (above). Support for this approach isn't provided out of the box, but you can use the 3rd party [SharpGrip.FluentValidation.AutoValidation](https://github.com/SharpGrip/FluentValidation.AutoValidation) package for this purpose. 

## Clientside Validation

FluentValidation is a server-side library and does not provide any client-side validation directly. However, it can provide metadata which can be applied to the generated HTML elements for use with a client-side framework such as jQuery Validate in the same way that ASP.NET's default validation attributes work.

To make use of this metadata you'll need to install the separate `FluentValidation.AspNetCore` package. Instructions for installing and using this package [can be found on its project page here](https://github.com/FluentValidation/FluentValidation.AspNetCore#aspnet-core-integration-for-fluentvalidation). Note that this package is no longer supported, but is still available to use. 

Alternatively, instead of using client-side validation you could instead execute your full server-side rules via AJAX using a library such as [FormHelper](https://github.com/sinanbozkus/FormHelper). This allows you to use the full power of FluentValidation, while still having a responsive user experience.

## Minimal APIs

When using FluentValidation with minimal APIs, you can still register the validators with the service provider, (or you can instantiate them directly if they don't have dependencies) and invoke them inside your API endpoint.

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Register validator with service provider (or use one of the automatic registration methods)
builder.Services.AddScoped<IValidator<Person>, PersonValidator>();

// Also registering a DB access repository for demo purposes
// replace this with whatever you're using in your application.
builder.Services.AddScoped<IPersonRepository, PersonRepository>();

app.MapPost("/person", async (IValidator<Person> validator, IPersonRepository repository, Person person) => 
{
  ValidationResult validationResult = await validator.ValidateAsync(person);

  if (!validationResult.IsValid) 
  {
    return Results.ValidationProblem(validationResult.ToDictionary());
  }

  repository.Save(person);
  return Results.Created($"/{person.Id}", person);
});
```

Note the `ToDictionary` method on the `ValidationResult` is only available from FluentValidation 11.1 and newer. In older versions you will need to implement this as an extension method:

```csharp
public static class FluentValidationExtensions
{
  public static IDictionary<string, string[]> ToDictionary(this ValidationResult validationResult)
    {
      return validationResult.Errors
        .GroupBy(x => x.PropertyName)
        .ToDictionary(
          g => g.Key,
          g => g.Select(x => x.ErrorMessage).ToArray()
        );
    }
}

```

Alternatively, instead of manually invoking the validator you could use a filter to apply validation to an endpoint (or group of endpoints). This isn't supported out of the box, but you can use one of the following the third-party package for this purpose:

- [ForEvolve.FluentValidation.AspNetCore.Http](https://github.com/Carl-Hugo/FluentValidation.AspNetCore.Http)
- [SharpGrip.FluentValidation.AutoValidation](https://github.com/SharpGrip/FluentValidation.AutoValidation)
