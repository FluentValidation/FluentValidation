# ASP.NET WebApi 2

```eval_rst
.. warning::
   Integration with ASP.NET WebApi 2 is no longer supported as of FluentValidation 9. Please migrate to ASP.NET Core.
```

FluentValidation 8.x provided integration with ASP.NET Web Api 2. This is no longer maintained or supported, and is not compatible with FluentValidation 9 or newer.

For instructions on using these unsupported legacy components with FluentValidation 8, [please review this page](https://github.com/FluentValidation/FluentValidation-LegacyWeb/wiki/WebApi-2-Integration)

# In a WEB API in .NET 5, the FluentValidation configuration must be placed like this in the ConfigureServices method:
```
services
	.AddControllers().AddFluentValidation(configuration =>
      {
                //Telling Fluent Validation to register all validation classes in the assembly
                configuration.RegisterValidatorsFromAssemblyContaining<Startup>();
      });

```
It is also possible to disable validations with fluent validations by adding `configuration.DisableDataAnnotationsValidation = true;` which would look like this:

```
services
	.AddControllers().AddFluentValidation(configuration =>
  	{
			//Telling Fluent Validation to register all validation classes in the assembly
			configuration.RegisterValidatorsFromAssemblyContaining<Startup>();

			//Telling Fluent Validation to no longer perform net core validation
			configuration.DisableDataAnnotationsValidation = true
		});
```

### Source from which I got this knowledge: https://lfppfaria.github.io/c%23/2020/06/14/fluent-validation.html
