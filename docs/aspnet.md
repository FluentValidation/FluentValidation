# ASP.NET Core

### Getting Started

FluentValidation can be integrated with ASP.NET Core. Once enabled, MVC will use FluentValidation to validate objects that are passed in to controller actions by the model binding infrastructure.

To enable MVC integration, you'll need to add a reference to the `FluentValidation.AspNetCore` assembly by installing the appropriate NuGet package:

```shell
Install-Package FluentValidation.AspNetCore
```

Once installed, you'll need to configure FluentValidation in your app's Startup class by calling the `AddFluentValidation` extension method inside the `ConfigureServices` method (which requires a `using FluentValidation.AspNetCore`). This method must be called directly after calling `AddMvc`.


```csharp
public void ConfigureServices(IServiceCollection services) {
    services.AddMvc(setup => {
      //...mvc setup...
    }).AddFluentValidation();
}
```

In order for ASP.NET to discover your validators, they must be registered with the services collection. You can either do this by calling the `AddTransient` method for each of your validators...


```csharp
public void ConfigureServices(IServiceCollection services) {
    services.AddMvc(setup => {
      //...mvc setup...
    }).AddFluentValidation();

    services.AddTransient<IValidator<Person>, PersonValidator>();
    //etc
}
```

...or by using the `AddFromAssemblyContaining` method to automatically register all validators within a particular assembly. This will automatically find any public, non-abstract types that inherit from `AbstractValidator` and register them with the container (open generics are not supported).

```csharp
services.AddMvc()
  .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<PersonValidator>());
```

```eval_rst
.. note::
  Validators that are registered automatically using `RegisterValidationsFromAssemblyContaining` are registered as `Transient` with the container. You can choose to register them as a different lifetime by instead using the extension method `AddValidatorsFromAssemblyContaining`, or by explicitly registering individual validators with the container instead of auto-registering.
```

This example assumes that the PersonValidator is defined to validate a class called `Person`:

```csharp
public class Person {
	public int Id { get; set; }
	public string Name { get; set; }
	public string Email { get; set; }
	public int Age { get; set; }
}

public class PersonValidator : AbstractValidator<Person> {
	public PersonValidator() {
		RuleFor(x => x.Id).NotNull();
		RuleFor(x => x.Name).Length(0, 10);
		RuleFor(x => x.Email).EmailAddress();
		RuleFor(x => x.Age).InclusiveBetween(18, 60);
	}
}
```


We can use the Person class within our controller and associated view:

```csharp
public class PeopleController : Controller {
	public ActionResult Create() {
		return View();
	}

	[HttpPost]
	public IActionResult Create(Person person) {

		if(! ModelState.IsValid) { // re-render the view when validation failed.
			return View("Create", person);
		}

		Save(person); //Save the person to the database, or some other logic

		TempData["notice"] = "Person successfully created";
		return RedirectToAction("Index");

	}
}
```

...and here's the corresponding view (using Razor):

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
  <input type="submit" value="submtit" />
</form>
```

Now when you post the form, MVC's model-binding infrastructure will validate the `Person` object with the `PersonValidator`, and add the validation results to ModelState.

*Note for advanced users* When validators are executed using this automatic integration, the [RootContextData](/advanced.html#root-context-data) contains an entry called `InvokedByMvc` with a value set to true, which can be used within custom validators to tell whether a validator was invoked automatically (by MVC), or manually.

### Compatibility with ASP.NET's built-in Validation

By default, after FluentValidation is executed, any other validator providers will also have a chance to execute. This means you can mix FluentValidation with DataAnnotations attributes (or any other ASP.NET ModelValidatorProvider implementation).

If you want to disable this behaviour so that FluentValidation is the only validation library that executes, you can set the `RunDefaultMvcValidationAfterFluentValidationExecutes` to false in your application startup routine:

```csharp
services.AddMvc().AddFluentValidation(fv => {
 fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
});
```

*Note* If you do set `RunDefaultMvcValidationAfterFluentValidationExecutes` to false, then support for `IValidatableObject` will also be disabled.

### Implicit vs Explicit Child Property Validation

When validating complex object graphs, by default, you must explicitly specify any child validators for complex properties by using `SetValidator` ([see the section on validating complex properties](/start.html#complex-properties))

When running an ASP.NET MVC application, you can also optionally enable implicit validation for child properties. When this is enabled, instead of having to specify child validators using `SetValidator`, MVC's validation infrastructure will recursively attempt to automatically find validators for each property. This can be done by setting `ImplicitlyValidateChildProperties` to true:

```csharp
services.AddMvc().AddFluentValidation(fv => {
 fv.ImplicitlyValidateChildProperties = true;
});
```

Note that if you enable this behaviour you should not use `SetValidator` for child properties, or the validator will be executed twice.

### Clientside Validation

FluentValidation is a server-side framework, and does not provide any client-side validation directly. However, it can provide metadata which, when applied to the generated HTML elements, can be used by a client-side framework such as jQuery Validate, in the same way that ASP.NET's default validation attributes work.

Note that not all rules defined in FluentValidation will work with ASP.NET's client-side validation. For example, any rules defined using a condition (with When/Unless), custom validators, or calls to `Must` will not run on the client side. Nor will any rules in a `RuleSet` (although this can be changed - see the section below on "RuleSet for client-side messages"). The following validators are supported on the client:

* NotNull/NotEmpty
* Matches (regex)
* InclusiveBetween (range)
* CreditCard
* Email
* EqualTo (cross-property equality comparison)
* MaxLength
* MinLength
* Length

### Manual validation

Sometimes you may want to manually validate an object in a MVC project. In this case, the validation results can be copied to MVC's modelstate dictionary:

```csharp
public ActionResult DoSomething() {
  var customer = new Customer();
  var validator = new CustomerValidator();
  var results = validator.Validate(customer);

  results.AddToModelState(ModelState, null);
  return View();
}
```

The AddToModelState method is implemented as an extension method, and requires a using statement for the FluentValidation namespace. Note that the second parameter is an optional model name, which will cause property names in the ModelState to be prefixed (eg a call to AddToModelState(ModelState, "Foo") will generate property names of "Foo.Id" and "Foo.Name" etc rather than just "Id" or "Name")

### Validator customization

The downside to using this automatic integration is that you don’t have access to the validator directly which means that you don’t have as much control over the validation processes compared to running the validator manually.

You can use the CustomizeValidatorAttribute to configure how the validator will be run. For example, if you want the validator to only run for a particular ruleset then you can specify that ruleset name by attributing the parameter that is going to be validated:

```csharp
public ActionResult Save([CustomizeValidator(RuleSet="MyRuleset")] Customer cust) {
  // ...
}
```

This is the equivalent of specifying the ruleset if you were to pass a ruleset name to a validator:

```csharp
var validator = new CustomerValidator();
var customer = new Customer();
var result = validator.Validate(customer, ruleSet: "MyRuleset");
```

The attribute can also be used to invoke validation for individual properties:

```csharp
public ActionResult Save([CustomizeValidator(Properties="Surname,Forename")] Customer cust) {
  // ...
}
```
…which would be the equivalent of specifying properties in the call to validator.Validate:

```csharp
var validator = new CustomerValidator();
var customer = new Customer();
var result = validator.Validate(customer, properties: new[] { "Surname", "Forename" });
```

You can also use the CustomizeValidatorAttribute to skip validation for a particular type. This is useful for if you need to validate a type manually (for example, if you want to perform async validation then you'll need to instantiate the validator manually and call ValidateAsync as MVC's validation pipeline is not asynchronous).

```csharp
public ActionResult Save([CustomizeValidator(Skip=true)] Customer cust) {
  // ...
}
```


### Validator Interceptors

You can further customize this process by using an interceptor. An interceptor has to implement the IValidatorInterceptor interface from the FluentValidation.Mvc namespace:

```csharp
public interface IValidatorInterceptor	{
  ValidationContext BeforeMvcValidation(ControllerContext controllerContext, ValidationContext validationContext);
  ValidationResult AfterMvcValidation(ControllerContext controllerContext, ValidationContext validationContext, ValidationResult result);
}

```

This interface has two methods – BeforeMvcValidation and AfterMvcValidation. If you implement this interface in your validator classes then these methods will be called as appropriate during the MVC validation pipeline.

BeforeMvcValidation is invoked after the appropriate validator has been selected but before it is invoked. One of the arguments passed to this method is a ValidationContext that will eventually be passed to the validator. The context has several properties including a reference to the object being validated. If we want to change which rules are going to be invoked (for example, by using a custom ValidatorSelector) then we can create a new ValidationContext, set its Selector property, and return that from the BeforeMvcValidation method.

Likewise, AfterMvcValidation occurs after validation has occurs. This time, we also have a reference to the result of the validation. Here we can do some additional processing on the error messages before they’re added to modelstate.

As well as implementing this interface directly in a validator class, we can also implement it externally, and specify the interceptor by using a CustomizeValidatorAttribute on an action method parameter:

```csharp
public ActionResult Save([CustomizeValidator(Interceptor=typeof(MyCustomerInterceptor))] Customer cust) {
 //...
}
```

In this case, the interceptor has to be a class that implements IValidatorInterceptor and has a public, parameterless constructor.

Alternatively, you can register a default `IValidatorInterceptor` with the ASP.NET Service Provider. If you do this, then the interceptor will be used for all validators:

```csharp
public void ConfigureServices(IServiceCollection services) {
    services
      .AddMvc()
      .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<PersonValidator>());

    // Register a default interceptor, where MyDefaultInterceptor is a class that
    // implements IValidatorInterceptor.
    services.AddTransient<IValidatorInterceptor, MyDefaultInterceptor>();
}
```

Note that this is considered to be an advanced scenario. Most of the time you probably won’t need to use an interceptor, but the option is there if you want it.

### Specifying a RuleSet for client-side messages

If you’re using rulesets alongside ASP.NET MVC, then you’ll notice that by default FluentValidation will only generate client-side error messages for rules not part of any ruleset. You can instead specify that FluentValidation should generate clientside rules from a particular ruleset by attributing your controller action with a RuleSetForClientSideMessagesAttribute:

```csharp
[RuleSetForClientSideMessages("MyRuleset")]
public ActionResult Index() {
   return View(new PersonViewModel());
}
```

You can also use the `SetRulesetForClientsideMessages` extension method within your controller action (you must have the FluentValidation.Mvc namespace imported):

```csharp
public ActionResult Index() {
   ControllerContext.SetRulesetForClientsideMessages("MyRuleset");
   return View(new PersonViewModel());
}
```

### Injecting Child Validators

As an alternative to directly instantiating child validators, with the ASP.NET Core integration you can choose to inject them instead. This can be done via the validator's constructor:

```csharp
public class PersonValidator : AbstractValidator<Person> {
  public PersonValidator(IValidator<Address> addressValidator) {
    RuleFor(x => x.Address).SetValidator(addressValidator);
  }
}
```

Alternatively, as of version 8.2 you can call `InjectValidator` without having to use constructor injection:

```csharp
public class PersonValidator : AbstractValidator<Person> {
  public PersonValidator() {
    RuleFor(x => x.Address).InjectValidator();
  }
}
```

Note that in this case, FluentValidation will attempt to resolve an instance of `IValidator<T>` from ASP.NET's service collection, where `T` is the same type as the property being validated. If you need to explicitly specify the type, then this can be done with the other overload of `InjectValidator` which accepts a func referencing the service provider:

```csharp
public class PersonValidator : AbstractValidator<Person> {
  public PersonValidator() {
    RuleFor(x => x.Address).InjectValidator((services, context) => services.GetService<MyAddressValidator>());
  }
}
```

Please be aware that `InjectValidator` can *only* be used when using automatic validation. It can't be used if you directly invoke the `Validate` method.

### Use with Page Models

Configuration for use with ASP.NET Web Pages and PageModels is exactly the same as with MVC above, but there are several limitations:

- You can't define a validator for the whole page-model, only for models exposed as properties on the page model.
- The `[CustomizeValidator]` attribute is not supported
- the `[RuleSetForClientSideMessages]` attribute is not supported

These are limitations of ASP.NET Web Pages and are not currently something that FluentValidation can work around.
