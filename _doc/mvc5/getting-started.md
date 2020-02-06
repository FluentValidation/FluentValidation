---
title: Getting Started
---

<div class="callout-block callout-info"><div class="icon-holder" markdown="1">*&nbsp;*{: .fa .fa-info-circle}
</div><div class="content" markdown="1">
{: .callout-title}
#### Deprecation Notice

Integration with ASP.NET MVC 5 is considered legacy and will not be supported beyond FluentValidation 8.x. 
For an optimal experience, we suggest using FluentValidtation with [ASP.NET Core](/aspnet).

</div></div>

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

*Note for advanced users* When validators are executed using this automatic integration, the [RootContextData](/start.html#root-context-data) contain an entry called `InvokedByMvc` with a value set to true, which can be used within custom validators to tell whether a validator was invoked automatically by MVC, or manually. 
