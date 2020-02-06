---
title: Getting Started
---

<div class="callout-block callout-info"><div class="icon-holder" markdown="1">*&nbsp;*{: .fa .fa-info-circle}
</div><div class="content" markdown="1">
{: .callout-title}
#### Deprecation Notice

Integration with ASP.NET WebApi 2 is considered legacy and will not be supported beyond FluentValidation 8.x. 
For an optimal experience, we suggest using FluentValidtation with [ASP.NET Core](/aspnet).

</div></div>

FluentValidation can be configured to work with WebApi 2 projects. To enable WebApi integration, you'll need to add a reference to the `FluentValidation.WebApi` assembly from the appropriate NuGet package:

```shell
Install-Package FluentValidation.WebApi
```

Once installed, you'll need to configure the `FluentValidationModelValidatorProvider` (which lives in the `FluentValidation.WebApi` namespace) during your application's startup routine. This is usually inside the `Register` method of your `WebApiConfig` class which can be found in the App_Start directory. 

```csharp
public static class WebApiConfig {
    public static void Register(HttpConfiguration config) {
        config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        FluentValidationModelValidatorProvider.Configure(config);
    }
```

If you are self-hosting with OWIN, then this should instead be inside your OWIN startup class's `Configuration` method:

```csharp
public class Startup { 
    public void Configuration(IAppBuilder appBuilder) { 
        var config = new HttpConfiguration(); 

        config.Routes.MapHttpRoute( 
            name: "DefaultApi", 
            routeTemplate: "api/{controller}/{id}", 
            defaults: new { id = RouteParameter.Optional } 
        ); 

        FluentValidationModelValidatorProvider.Configure(config);


        appBuilder.UseWebApi(config); 
    } 
}
```

Internally, FluentValidation's WebApi integration makes use of a *validator factory* to determine which validator should be used to validate a particular type. By default, FluentValidation ships with an AttributedValidatorFactory that allows you to link a validator to the type that it validates by decorating the class to validate with an attribute that identifies its corresponding validator:

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
FluentValidationModelValidatorProvider.Configure(config, provider => {
  provider.ValidatorFactory = new MyCustomValidatorFactory();
});
```

Finally, we can create the controller:

```csharp
public class PeopleController : ApiController {
	[HttpPost]
	public IHttpActionResult Create(Person person) {
 
		if(! ModelState.IsValid) { // re-render the view when validation failed.
			return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
		}
 
		return new HttpResponseMessage(HttpStatusCode.OK);
	}
}
```

Now when you post data to the controller's `Create` method (for example, as JSON) then WebApi will automatically call into FluentValidation to find the corresponding validator. Any validation failures will be stored in the controller's `ModelState` dictionary which can be used to generate an error response which can be returned to the client. 

*Note for advanced users* When validators are executed using this automatic integration, the [RootContextData](/start.html#root-context-data) contain an entry called `InvokedByWebApi` with a value set to true, which can be used within custom validators to tell whether a validator was invoked automatically by WebApi, or manually. 
