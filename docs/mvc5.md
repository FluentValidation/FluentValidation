# ASP.NET MVC 5

```eval_rst
.. warning::
   Integration with ASP.NET MVC 5 is deprecated. For an optimal experience, we suggest using FluentValidtation with ASP.NET Core.
```

## Getting Started

FluentValidation can be configured to work with ASP.NET MVC 5 projects.

To enable MVC integration, you'll need to add a reference to the `FluentValidation.Mvc5` assembly from the appropriate NuGet package:

```shell
Install-Package FluentValidation.Mvc5
```

Once installed, you'll need to configure the `FluentValidationModelValidatorProvider` (which lives in the FluentValidation.Mvc namespace) during the `Application_Start` event of your MVC application, which is in your Global.asax. 

```csharp
protected void Application_Start() {
    AreaRegistration.RegisterAllAreas();

    RegisterGlobalFilters(GlobalFilters.Filters);
    RegisterRoutes(RouteTable.Routes);

    FluentValidationModelValidatorProvider.Configure();
}
```

Internally, FluentValidation's MVC integration makes use of a *validator factory* to know how to determine which validator should be used to validate a particular type. By default, FluentValidation ships with an AttributedValidatorFactory that allows you to link a validator to the type that it validates by decorating the class to validate with an attribute that identifies its corresponding validator:

```csharp
[Validator(typeof(PersonValidator))]
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

Instead of using an attribute, you can also use a custom validator factory with an IoC container. You can tell the `FluentValidationModelValidatorProvider` to use a different validator factory by passing a nested closure into the `Configure` method which allows the provider to be customized:

```csharp
FluentValidationModelValidatorProvider.Configure(provider => {
  provider.ValidatorFactory = new MyCustomValidatorFactory();
});
```

Finally, we can create the controller and associated view:

```csharp
public class PeopleController : Controller {
	public ActionResult Create() {
		return View();
	}
 
	[HttpPost]
	public ActionResult Create(Person person) {
 
		if(! ModelState.IsValid) { // re-render the view when validation failed.
			return View("Create", person);
		}
 
		TempData["notice"] = "Person successfully created";
		return RedirectToAction("Index");
 
	}
}
```

...and here's the corresponding view (using Razor):

```html
@Html.ValidationSummary()
 
@using (Html.BeginForm()) {
	Id: @Html.TextBoxFor(x => x.Id) @Html.ValidationMessageFor(x => x.Id)
	<br />
	Name: @Html.TextBoxFor(x => x.Name) @Html.ValidationMessageFor(x => x.Name) 		
	<br />
	Email: @Html.TextBoxFor(x => x.Email) @Html.ValidationMessageFor(x => x.Email)
	<br />
	Age: @Html.TextBoxFor(x => x.Age) @Html.ValidationMessageFor(x => x.Age)
 
	<br /><br />
 
	<input type="submit" value="submit" />
}
```

Now when you post the form MVC’s `DefaultModelBinder` will validate the Person object using the `FluentValidationModelValidatorProvider`.

*Note for advanced users* When validators are executed using this automatic integration, the [RootContextData](/advanced.html#root-context-data) contain an entry called `InvokedByMvc` with a value set to true, which can be used within custom validators to tell whether a validator was invoked automatically by MVC, or manually. 

## Known Limitations

MVC 5 performs validation in two passes. First it tries to convert the input values from the request into the types declared in your model, and then it performs model-level validation using FluentValidation. If you have non-nullable types in your model (such as `int` or `DateTime`) and there are no values submitted in the request, model-level validations will be skipped, and only the type conversion errors will be returned. 

This is a limitation of MVC 5's validation infrastructure, and there is no way to disable this behaviour. If you want all validation failures to be returned in one go, you must ensure that any value types are marked as nullable in your model (you can still enforce non-nullability with a `NotNull` or `NotEmpty` rule as necessary, but the underlying type must allow nulls). This only applies to MVC5 and WebApi 2. ASP.NET Core does not suffer from this issue as the validation infrastructure has been improved. 

## Clientside Validation

Note that FluentValidation will also work with ASP.NET MVC's client-side validation, but not all rules are supported. For example, any rules defined using a condition (with When/Unless), custom validators, or calls to Must will not run on the client side. The following validators are supported on the client:

* NotNull/NotEmpty
* Matches (regex)
* InclusiveBetween (range)
* CreditCard
* Email
* EqualTo (cross-property equality comparison)
* Length

## Manual Validation

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

## Validator Customization

The downside of using this automatic integration is that you don’t have access to the validator directly which means that you don’t have as much control over the validation processes compared to running the validator manually.

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

## Validator Interceptors

You can further customize this process by using an interceptor. An interceptor has to implement the IValidatorInterceptor interface from the FluentValidation.Mvc namespace:

```csharp
public interface IValidatorInterceptor {
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

In this case, the interceptor has to be a class that implements IValidatorInterceptor and has a public, parameterless constructor. The advantage of this approach is that your validators don’t have to be in an assembly that directly references System.Web.Mvc.

Note that this is considered to be an advanced scenario. Most of the time you probably won’t need to use an interceptor, but the option is there if you want it.

## Rulesets for client-side messages

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

## Using an IoC container

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
