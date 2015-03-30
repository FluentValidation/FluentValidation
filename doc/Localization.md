## Localization

Out of the box, FluentValidation provides translations for the default validation messages in several languages. 

You can also use the `WithLocalizedMessage` method to specify a localized error message for a single validation rule or you can set the global `ResourceProviderType` if you want to replace FluentValidation's default errors with your own localized messages.

## Using WithLocalizedMessages

To localize an error message for a single validation rule you can call `WithLocalizedMessage` passing in a lambda expression that specifies the Resource Type and the property name that should be used when accessing the resource:

```
RuleFor(x => x.Surname).NotNull().WithLocalizedMessage(() => MyLocalizedMessage.SurnameRequiredError);
```
Note that the expression must be for a property that is both `public` and `static`. 

## Using a custom Resource Provider Type

If you want to replace some (or all) of FluentValidation's default messages then you can specify your own resources by setting the ResourceProviderType property on the ValidatorOptions class in your application's startup routine:

```
ValidatorOptions.ResourceProviderType = typeof(MyResources);
```

This class should then declare static properties with names that match FluentValidation's error message resource names:

```
public class MyResources {
   public static string notempty_error {
      get { 
          return "{PropertyName} is required!";
      }
   }
}
```
This would replace the default error message for all "NotEmpty" validation rules with a custom templated error message. 

Note that these messages must be `public` and `static`.

Below is a list of all the error message resource names that you can customize. 
- email_error
- equal_error
- exact_length_error
- exclusivebetween_error
- greaterthan_error
- greaterthanorequal_error
- inclusivebetween_error
- length_error
- lessthan_error
- lessthanorequal_error
- notempty_error
- notequal_error
- notnull_error
- predicate_error
- regex_error
