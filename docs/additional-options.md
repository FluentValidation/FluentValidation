# Additional Options

Additional information can be associated with validation rules in addition to whether or not an object is valid. FluentValidation enables you to specify a failure's severity, an optional error code and also allows you to associate any arbitrary data with the failure.

## Configuring the Severity of Validation Failures

Given the following example that validates a `Person` object:

```csharp
public class PersonValidator : AbstractValidator<Person> {
  public PersonValidator() {
        RuleFor(person => person.Surname).NotNull();
        RuleFor(person => person.Forename).NotNull();
  }
}
```

By default, if these rules fail they will have a severity of "Error". This can be changed by calling the `WithSeverity` method. For example, if we wanted a missing surname to be identified as a warning instead of an error then we could modify the above line to:

```csharp
RuleFor(x => x.Surname).NotNull().WithSeverity(Severity.Warning);
```

In version 9.0 and above a callback can be used instead, which also gives you access to the item being validated:

```csharp
RuleFor(person => person.Surname).NotNull().WithSeverity(person => Severity.Warning);
```

In this case, the `ValidationResult` would still have an `IsValid` result of `false`. However, in the list of `Errors`, the `ValidationFailure` pertaining to this field will have its `Severity` property set to `Warning`:

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

## ErrorCode and Error Messages

The `ErrorCode` can be useful in providing error messages for lookup. At a high level:

* `ErrorCode` is used as the lookup key for an error message. For example, a `.NotNull()` validator has a default error code of `NotNullValidator`, which is what is used to look up the error messages.
* If you provide an `ErrorCode`, you could also provide a localized message with the name of that error code to create a custom message.
* If you provide an `ErrorCode` but no custom message, the message will fall back to the default message for that validator. You're not required to add a custom message.
* Using `ErrorCode` might be useful to override error messages. For example, if you use a custom `Must()` validator, but you'd like to reuse the text of your `NotNull()` validator's error message, you could utilize `.WithErrorCode("NotNullValidator")` to achieve this result.

## Supplying Additional Validation Information with CustomState

There may be an occasion where you'd like to return contextual information about the state of your validation rule when it was run. The `WithCustomState` method allows you to associate any custom data with the validation results.

We could assign a custom state by modifying a line to read:

```csharp
RuleFor(person => person.Forename).NotNull().WithState(x => 1234);
```

This state is then available within the `CustomState` property of the `ValidationFailure`.

```csharp
var validator = new PersonValidator();
var result = validator.Validate(new Person());
foreach (var failure in result.Errors) {
  Console.WriteLine($"Property: {failure.PropertyName} State: {failure.CustomState}");
}
```

The output would be:

```
Property: Surname State:
Property: Forename State: 1234
```

By default the `CustomState` property will be `null` if `WithState` hasn't been called.
