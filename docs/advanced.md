# Other Advanced Features

These features are not normally used in day-to-day use, but provide some additional extensibility points that may be useful in some circumstances.

## Callbacks

You can make use of the `OnAnyFailure` and `OnFailure` (as of 8.0) callbacks to run a method if validation fails.

`OnAnyFailure` will be invoked if there are any failures within a rule chain:

```csharp
RuleFor(x => x.Surname).NotNull().Must(surname => surname != null && surname.Length <= 200)
  .OnAnyFailure(x => {
    Console.WriteLine("At least one validator in this rule failed");
  })
```

`OnFailure` will be invoked for each validator that fails:

```csharp
RuleFor(x => x.Surname).NotNull().OnFailure(x => Console.WriteLine("Nonull failed"))
  .Must(surname => surname != null && surname.Length <= 200)
  .OnFailure(x => Console.WriteLine("Must failed"));
```

## PreValidate

If you need to run specific code every time a validator is invoked, you can do this by overriding the `PreValidate` method. This method takes a `ValidationContext` as well as a `ValidationResult`, which you can use to customise the validation process.

The method should return `true` if validation should continue, or `false` to immediately abort. Any modifications that you made to the `ValidationResult` will be returned to the user.

Note that this method is called before FluentValidation performs its standard null-check against the model being validated, so you can use this to generate an error if the whole model is null, rather than relying on FluentValidation's standard behaviour in this case (which is to throw an exception):

```csharp
public class MyValidator : AbstractValidator<Person> {
  public MyValidator() {
    RuleFor(x => x.Name).NotNull();
  }

  protected override bool PreValidate(ValidationContext<Person> context, ValidationResult result) {
    if (context.InstanceToValidate == null) {
      result.Errors.Add(new ValidationFailure("", "Please ensure a model was supplied."));
      return false;
    }
    return true;
  }
}
```

## Root Context Data

For advanced users, it's possible to pass arbitrary data into the validation pipeline that can be accessed from within custom property validators. This is particularly useful if you need to make a conditional decision based on arbitrary data not available within the object being validated, as validators are stateless.

The `RootContextData` property is a `Dictionary<string, object>` available on the `ValidationContext`.:

```csharp
var instanceToValidate = new Person();
var context = new ValidationContext<Person>(person);
context.RootContextData["MyCustomData"] = "Test";
var validator = new PersonValidator();
validator.Validate(context);
```

The RootContextData can then be accessed inside any custom property validators, as well as calls to `Custom`:

```csharp
RuleFor(x => x.Surname).Custom((x, context) => {
  if(context.RootContextData.ContainsKey("MyCustomData")) {
    context.AddFailure("My error message");
  }
});
```
