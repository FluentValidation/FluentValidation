# ASP.NET Core

FluentValidation can be used within ASP.NET Core web applications to validate incoming models. There are two main approaches for doing this: Manual validation and Automatic validation.

With manual validation, you inject the validator into your controller (or api endpoint), invoke the validator and act upon the result. This is the most straightforward and reliable approach. 

With automatic validation, FluentValidation plugs the validation pipeline that's part of ASP.NET Core MVC and allows models to be validated before a controller action is invoked (during model-binding). This approach to validation is more seamless but has several downsides:

- **Auto validation is not asynchronous**: If your validator contains asynchronous rules then your validator will not be able to run. You will receive an exception at runtime if you attempt to use an asynchronous validator with auto-validation.
- **Auto validation is MVC-only**: Auto-validation only works with MVC Controllers and Razor Pages. It does not work with the more modern parts of ASP.NET such as Minimal APIs or Blazor.

We do not generally recommend using auto validation for new projects, but it is still available for legacy implementations.

Auto validaton is available for Core 3.1, .NET 5, .NET 6 and newer.

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
    _repository = personRepository;
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

Because our validator is registered with the Serivce Provider, it will be injected into our controller via the constructor. We can then make use of the validator inside the `Create` action by invoking it with `ValidateAsync`. 

If validation fails, we need to pass the error messages back down to the view so they can be displayed to the end user. We can do this by defining an extension method for FluentValidation's `ValidationResult` type that copies the error messages into ASP.NET's `ModelState` dictionary:

```csharp
public static class Extensions 
{
  public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState) 
  {
    if (!result.IsValid) 
    {
      foreach (var error in result.Errors) 
      {
        modelState.AddModelError(error.PropertyName, error.ErrorMessage);
      }
    }
  }
}
```

This method is invoked inside the controller action in the example above. 


For completeness, here is the corresponding View. This view will pick up the error messages from `ModelState` and dispaly them next to the corresponding property. (If you were writing an API controller, then you'd probably return eithe ra `ValidationProblemDetails` or `BadRequest` instead of a view result)

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

The `FluentValidation.AspNetCore` package provides auto-validation for ASP.NET Core MVC projects by plugging into ASP.NET's validation pipeline. 

```eval_rst
.. warning::
  We no longer recommend using auto-validation for new projects for the reasons mentioned at the start of this page. 
```

Install the `FluentValidation.AspNetCore` package in your web application from the command line:

```shell
dotnet add package FluentValidation.AspNetCore
```

Once installed, you'll need to modify the `ConfigureServices` in your `Startup` to include a call to `AddFluentValidationAutoValidation()`:

```csharp
public void ConfigureServices(IServiceCollection services) 
{
    services.AddMvc();

    // ... other configuration ...

    services.AddFluentValidationAutoValidation();

    services.AddScoped<IValidator<Person>, PersonValidator>();
}
```

This method must be called after `AddMvc` (or `AddControllers`/`AddControllersWithViews`). Make sure you add `using FluentValidation.AspNetCore` to your startup file so the appropriate extension methods are available. 

```eval_rst
.. note::
  The `AddFluentValidationAutoValidation` method is only available in version 11.1 and newer. In older versions, call `services.AddFluentValidation()` instead, which is the equivalent of calling `services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters()`
```

You'll also still need to register your validators in the same as our manual validation example above (either by calling `AddScoped` for each validator, or by using one of the registration methods provided by `FluentValidation.DependencyInjectionExtensions`).

We can use the `Person` class within our controller and associated view:

```csharp
public class PeopleController : Controller 
{
  public ActionResult Create() 
  {
    return View();
  }

  [HttpPost]
  public IActionResult Create(Person person) 
  {
    if(! ModelState.IsValid) 
    { 
      // re-render the view when validation failed.
      return View("Create", person);
    }

    Save(person); //Save the person to the database, or some other logic

    TempData["notice"] = "Person successfully created";
    return RedirectToAction("Index");
  }
}
```

The view is the same as before:

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

Now when you post the form, MVC's model-binding infrastructure will automatically instantiate the `PersonValidator`, invoke it and add the validation results to `ModelState`.

Unlike the manual validation example, we don't have a reference to the validator directly. Instead, ASP.NET will handle invoking the validator and adding the error messages to `ModelState` before the controller action is invoked. Inside the action, you only need to check `ModelState.IsValid`

```eval_rst
.. warning::
  Remember you can't use asynchronous rules when using auto-validation as ASP.NET's validation pipeline is not asynchronous.
```

### Compatibility with ASP.NET's built-in Validation

After FluentValidation is executed, any other validator providers will also have a chance to execute. This means you can mix FluentValidation auto-validation with [DataAnnotations attributes](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations) (or any other ASP.NET `ModelValidatorProvider` implementation).

If you want to disable this behaviour so that FluentValidation is the only validation library that executes, you can set `DisableDataAnnotationsValidation` to `true` in your application startup routine:

```csharp
services.AddFluentValidationAutoValidation(config => 
{
 config.DisableDataAnnotationsValidation = true;
});
```

*Note* If you do set `DisableDataAnnotationsValidation` then support for `IValidatableObject` will also be disabled.

### Implicit vs Explicit Child Property Validation

```eval_rst
.. warning::
  Implicit validation of child properties is deprecated and will be removed from a future release. The documentation remains here for reference but we no longer recommend taking this approach. `See this issue for details <https://github.com/FluentValidation/FluentValidation/issues/1960>`_.
```

When validating complex object graphs you must explicitly specify any child validators for complex properties by using `SetValidator` ([see the section on validating complex properties](start.html#complex-properties))

When running an ASP.NET MVC application, you can also optionally enable implicit validation for child properties. When this is enabled, instead of having to specify child validators using `SetValidator`, MVC's validation infrastructure will recursively attempt to automatically find validators for each property. This can be done by setting `ImplicitlyValidateChildProperties` to true:

```csharp
services.AddFluentValidationAutoValidation(config => 
{
 config.ImplicitlyValidateChildProperties = true;
});
```

Note that if you enable this behaviour you should not use `SetValidator` for child properties, or the validator will be executed twice.

```eval_rst
.. note::
  The `AddFluentValidationAutoValidation` method is only available in version 11.1 and newer. In older versions, call `services.AddFluentValidation()` instead, which is the equivalent of calling `services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters()`
```

### Implicit Validation of Collection-Type Models

```eval_rst
.. warning::
  Implicit validation of collection-type models is deprecated and will be removed from a future release. The documentation remains here for reference but we no longer recommend taking this approach. `See this issue for details <https://github.com/FluentValidation/FluentValidation/issues/1960>`_.
```

By default, you must create a specific collection validator or enable implicit child property validation to validate a model that is of a collection type. For example, no validation of the following model will occur with the default settings unless you define a validator that inherits from `AbstractValidator<List<Person>>`.

```csharp
public ActionResult DoSomething(List<Person> people) => Ok();
```

With implicit child property validation enabled (see above), you don't have to explicitly create a collection validator class as each person element in the collection will be validated automatically. However, any child properties on the `Person` object will be automatically validated too meaning you can no longer use `SetValidator`. If you don't want this behaviour, you can also optionally enable implicit validation for root collection elements only. For example, if you want each `Person` element in the collection to be validated automatically, but not its child properties you can set `ImplicitlyValidateRootCollectionElements` to true:

```csharp
services.AddFluentValidationAutoValidation(config => 
{
 config.ImplicitlyValidateRootCollectionElements = true;
});
```

Note that this setting is ignored when `ImplicitlyValidateChildProperties` is `true`.

```eval_rst
.. note::
  The `AddFluentValidationAutoValidation` method is only available in version 11.1 and newer. In older versions, call `services.AddFluentValidation()` instead, which is the equivalent of calling `services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters()`
```

### Validator customization

One downside to using auto-validation is that you don't have access to the validator instance meaning you don't have as much control over the validation processes compared to running the validator manually.

You can use the `CustomizeValidatorAttribute` to configure how the validator will be run. For example, if you want the validator to only run for a particular ruleset then you can specify that ruleset name by attributing the parameter that is going to be validated:

```csharp
public ActionResult Save([CustomizeValidator(RuleSet="MyRuleset")] Person person) 
{
  // ...
}
```

This is the equivalent of specifying the ruleset if you were to pass a ruleset name to a validator:

```csharp
var validator = new PersonValidator();
var person = new Person();
var result = validator.Validate(person, options => options.IncludeRuleSet("MyRuleset"));
```

The attribute can also be used to invoke validation for individual properties:

```csharp
public ActionResult Save([CustomizeValidator(Properties="Surname,Forename")] Person person) 
{
  // ...
}
```
…which would be the equivalent of specifying properties in the call to validator.Validate:

```csharp
var validator = new PersonValidator();
var person = new Person();
var result = validator.Validate(person, options => options.IncludeProperties("Surname", "Forename"));
```

You can also use the `CustomizeValidatorAttribute` to skip validation for a particular type. This is useful for if you need to validate a type manually (for example, if you want to perform async validation then you'll need to instantiate the validator manually and call `ValidateAsync` as MVC's validation pipeline is not asynchronous).

```csharp
public ActionResult Save([CustomizeValidator(Skip=true)] Person person) 
{
  // ...
}
```

### Validator Interceptors

You can further customize this process by using an interceptor. An interceptor has to implement the `IValidatorInterceptor` interface from the `FluentValidation.AspNetCore` namespace:

```csharp
public interface IValidatorInterceptor	
{
  IValidationContext BeforeAspNetValidation(ActionContext actionContext, IValidationContext validationContext);
  ValidationResult AfterAspNetValidation(ActionContext actionContext, IValidationContext validationContext, ValidationResult result);
}

```

This interface has two methods – `BeforeAspNetValidation` and `AfterAspNetValidation`. If you implement this interface in your validator classes then these methods will be called as appropriate during the MVC validation pipeline.

`BeforeMvcValidation` is invoked after the appropriate validator has been selected but before it is invoked. One of the arguments passed to this method is a `ValidationContext` that will eventually be passed to the validator. The context has several properties including a reference to the object being validated. If we want to change which rules are going to be invoked (for example, by using a custom `ValidatorSelector`) then we can create a new `ValidationContext`, set its `Selector` property, and return that from the `BeforeAspNetValidation` method.

Likewise, `AfterAspNetValidation` occurs after validation has occurs. This time, we also have a reference to the result of the validation. Here we can do some additional processing on the error messages before they're added to `ModelState`.

As well as implementing this interface directly in a validator class, we can also implement it externally, and specify the interceptor by using a `CustomizeValidatorAttribute` on an action method parameter:

```csharp
public ActionResult Save([CustomizeValidator(Interceptor=typeof(MyCustomerInterceptor))] Customer cust) 
{
 //...
}
```

In this case, the interceptor has to be a class that implements `IValidatorInterceptor` and has a public, parameterless constructor.

Alternatively, you can register a default `IValidatorInterceptor` with the ASP.NET Service Provider. If you do this, then the interceptor will be used for all validators:

```csharp
public void ConfigureServices(IServiceCollection services) 
{
    services.AddMvc();

    services.AddFluentValidationAutoValidation();
    services.AddValidatorsFromAssemblyContaining<PersonValidator>());

    // Register a default interceptor, where MyDefaultInterceptor is a class that
    // implements IValidatorInterceptor.
    services.AddTransient<IValidatorInterceptor, MyDefaultInterceptor>();
}
```

Note that this is considered to be an advanced scenario. Most of the time you probably won't need to use an interceptor, but the option is there if you want it.

```eval_rst
.. note::
  The `AddFluentValidationAutoValidation` method is only available in version 11.1 and newer. In older versions, call `services.AddFluentValidation()` instead, which is the equivalent of calling `services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters()`
```

## Clientside Validation

FluentValidation is a server-library and does not provide any client-side validation directly. However, it can provide metadata which can be applied to the generated HTML elements for use with a client-side framework such as jQuery Validate in the same way that ASP.NET's default validation attributes work.

Note that not all rules defined in FluentValidation will work with ASP.NET's client-side validation. For example, any rules defined using a condition (with When/Unless), custom validators, or calls to `Must` will not run on the client side. Nor will any rules in a `RuleSet` (although this can be changed - see below). The following validators are supported on the client:

* NotNull/NotEmpty
* Matches (regex)
* InclusiveBetween (range)
* CreditCard
* Email
* EqualTo (cross-property equality comparison)
* MaxLength
* MinLength
* Length

To enable clientside integration you need to install the `FluentValidation.AspNetCore` package and call the `AddFluentValidationClientsideAdapters` in your application startup:

```csharp
public void ConfigureServices(IServiceCollection services) 
{
    services.AddMvc();

    services.AddFluentValidationClientsideAdapters();

    services.AddScoped<IValidator<Person>, PersonValidator>();
    // etc
}
```
```eval_rst
.. note::
  Note that the `AddFluentValidationClientsideAdapters` method is only available in FluentValidation 11.1 and newer. In older versions, you should use the `AddFluentValidation` method which enables *both* auto-validation and clientside adapters. If you only want clientside adapters and don't want auto validation, you can configure this by calling `services.AddFluentValidation(config => config.AutomaticValidationEnabled = false)`
```

Alternatively, instead of using client-side validation you could instead execute your full server-side rules via AJAX using a library such as [FormHelper](https://github.com/sinanbozkus/FormHelper). This allows you to use the full power of FluentValidation, while still having a responsive user experience.

### Specifying a RuleSet for client-side messages

If you're using rulesets alongside ASP.NET MVC, then you'll notice that by default FluentValidation will only generate client-side error messages for rules not part of any ruleset. You can instead specify that FluentValidation should generate clientside rules from a particular ruleset by attributing your controller action with a `RuleSetForClientSideMessagesAttribute`:

```csharp
[RuleSetForClientSideMessages("MyRuleset")]
public ActionResult Index() 
{
   return View(new Person());
}
```

You can also use the `SetRulesetForClientsideMessages` extension method within your controller action, which has the same affect:

```csharp
public ActionResult Index() 
{
   ControllerContext.SetRulesetForClientsideMessages("MyRuleset");
   return View(new Person());
}
```

You can force all rules to be used to generate client-side error message by specifying a ruleset of "*".


## Razor Pages

Both the manual validation and auto validation approaches can be used with Razor pages. 

Auto validation has an additional limitation with Razor pages in that you can't define a validator for the whole Page Model itself, only for models exposed as properties on the page model.

Additionally when using client side integration, you can't use the `[RuleSetForClientsideMessages]` attribute and should use the `SetRulesetForClientsideMessages` extension method within your page handler:

```csharp
public IActionResult OnGet() 
{
   PageContext.SetRulesetForClientsideMessages("MyRuleset");
   return Page();
}
```

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

app.MapPost("/person", async (IPersonRepository repository, Person person) => 
{
  ValidationResult result = await repository.ValidateAsync(person);

  if (!result.IsValid) 
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