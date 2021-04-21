# Setting the Severity Level

Given the following example that validates a `Person` object:

```csharp
public class PersonValidator : AbstractValidator<Person> {
  public PersonValidator() {
    RuleFor(person => person.Surname).NotNull();
    RuleFor(person => person.Forename).NotNull();
  }
}
```

By default, if these rules fail they will have a severity of `Error`. This can be changed by calling the `WithSeverity` method. For example, if we wanted a missing surname to be identified as a warning instead of an error then we could modify the above line to:

```
RuleFor(x => x.Surname).NotNull().WithSeverity(Severity.Warning);
```

In version 9.0 and above a callback can be used instead, which also gives you access to the item being validated:

```
RuleFor(person => person.Surname).NotNull().WithSeverity(person => Severity.Warning);
```

In this case, the `ValidationResult` would still have an `IsValid` result of `false`. However, in the list of `Errors`, the `ValidationFailure` associated with this field will have its `Severity` property set to `Warning`:

```csharp
var validator = new PersonValidator();
var result = validator.Validate(new Person());
foreach (var failure in result.Errors) {
  Console.WriteLine($"Property: {failure.PropertyName} Severity: {failure.Severity}");
}
```

The output would be:

```
Property: Surname Severity: Warning
Property: Forename Severity: Error
```

By default, the severity level of every validation rule is `Error`. Available options are `Error`, `Warning`, or `Info`.

To set the severity level globally, you can set the `Severity` property on the static `ValidatorOptions` class during your application's startup routine:

```csharp
ValidatorOptions.Global.Severity = Severity.Info;
```

This can then be overridden by individual rules.