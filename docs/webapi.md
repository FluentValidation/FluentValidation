# ASP.NET WebApi 2

```eval_rst
.. warning::
   Integration with ASP.NET WebApi 2 is deprecated. For an optimal experience, we suggest using FluentValidtation with ASP.NET Core.
```

## Getting Started

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

*Note for advanced users* When validators are executed using this automatic integration, the [RootContextData](/advanced.html#root-context-data) contain an entry called `InvokedByWebApi` with a value set to true, which can be used within custom validators to tell whether a validator was invoked automatically by WebApi, or manually. 

## Manual Validation

Sometimes you may want to manually validate an object in a WebApi project. In this case, the validation results can be copied to MVC's modelstate dictionary:

```csharp
public IHttpActionResult DoSomething() {
  var customer = new Customer();
  var validator = new CustomerValidator();
  var results = validator.Validate(customer);

  results.AddToModelState(ModelState, null);
  return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
}
```

The `AddToModelState` method is implemented as an extension method, and requires a using statement for the `FluentValidation.WebApi` namespace. Note that the second parameter is an optional model name, which will cause property names in the ModelState to be prefixed (eg a call to AddToModelState(ModelState, "Foo") will generate property names of "Foo.Id" and "Foo.Name" etc rather than just "Id" or "Name")

## Validator Customization


The downside of using this automatic integration is that you don’t have access to the validator directly which means that you don’t have as much control over the validation processes compared to running the validator manually.

You can use the CustomizeValidatorAttribute to configure how the validator will be run. For example, if you want the validator to only run for a particular ruleset then you can specify that ruleset name by attributing the parameter that is going to be validated:

```csharp
public IHttpActionResult Save([CustomizeValidator(RuleSet="MyRuleset")] Customer cust) {
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
public IHttpActionResult Save([CustomizeValidator(Properties="Surname,Forename")] Customer cust) {
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
public IHttpActionResult Save([CustomizeValidator(Skip=true)] Customer cust) {
  // ...
}
```

## Validator Interceptors


You can further customize this process by using an interceptor. An interceptor has to implement the IValidatorInterceptor interface from the FluentValidation.WebApi namespace:

```csharp
public interface IValidatorInterceptor {
    ValidationContext BeforeMvcValidation(HttpActionContext controllerContext, ValidationContext validationContext);
 
    ValidationResult AfterMvcValidation(HttpActionContext controllerContext, ValidationContext validationContext, ValidationResult result);
}
```

This interface has two methods – BeforeMvcValidation and AfterMvcValidation. If you implement this interface in your validator classes then these methods will be called as appropriate during the MVC validation pipeline.

BeforeMvcValidation is invoked after the appropriate validator has been selected but before it is invoked. One of the arguments passed to this method is a ValidationContext that will eventually be passed to the validator. The context has several properties including a reference to the object being validated. If we want to change which rules are going to be invoked (for example, by using a custom ValidatorSelector) then we can create a new ValidationContext, set its Selector property, and return that from the BeforeMvcValidation method.

Likewise, AfterMvcValidation occurs after validation has occurs. This time, we also have a reference to the result of the validation. Here we can do some additional processing on the error messages before they’re added to modelstate.

As well as implementing this interface directly in a validator class, we can also implement it externally, and specify the interceptor by using a CustomizeValidatorAttribute on an action method parameter:

```csharp
public IHttpActionResult Save([CustomizeValidator(Interceptor=typeof(MyCustomerInterceptor))] Customer cust) {
 //...
}
```

In this case, the interceptor has to be a class that implements IValidatorInterceptor and has a public, parameterless constructor. The advantage of this approach is that your validators don’t have to be in an assembly that directly references System.Web.Mvc.

Note that this is considered to be an advanced scenario. Most of the time you probably won’t need to use an interceptor, but the option is there if you want it.

## Using an IoC Container

When using FluentValidation's WebApi integration you may wish to use an Inversion of Control container to instantiate your validators rather than relying on the attribute based approach. This can be achieved by writing a custom Validator Factory. 

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
FluentValidationModelValidatorProvider.Configure(config, cfg => {
    cfg.ValidatorFactory = new MyValidatorFactory();
});
```
