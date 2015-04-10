## Integration with ASP.NET MVC

FluentValidation can be integrated with ASP.NET MVC 4 and WebApi (with some limitations). Once enabled, MVC will use FluentValidation to validate objects that are passed in to controller actions by the model binding infrastructure. 

To enable MVC integration, you'll need to add a reference to the `FluentValidation.Mvc5` assembly (available through the FluentValidation.Mvc5 NuGet packages). 

Once installed, you'll need to configure the *FluentValidationModelValidatorProvider* (which lives in the FluentValidation.Mvc namespace) during the Application_Start event of your MVC application. 

```csharp
protected void Application_Start() {
    AreaRegistration.RegisterAllAreas();

    RegisterGlobalFilters(GlobalFilters.Filters);
    RegisterRoutes(RouteTable.Routes);

    FluentValidationModelValidatorProvider.Configure();
}
```

Internally, FluentValidation's MVC integration makes use of a *validator factory* to know how to work out which validator should be used to validate a particular type. By default, FluentValidation ships with an AttributedValidatorFactory that allows you to link a validator to the type that it validates by decorating the class to validate with an attribute that identifies its corresponding validator:

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

```
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

Note that FluentValidation will also work with ASP.NET MVC's client-side validation, but not all rules are supported. For example, any rules defined using a condition (with When/Unless), custom validators, or calls to Must will not run on the client side. The following validators are supported on the client:

* NotNull/NotEmpty
* Matches (regex)
* InclusiveBetween (range)
* CreditCard
* Email
* EqualTo (cross-property equality comparison)
* Length

## Validator customization

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
