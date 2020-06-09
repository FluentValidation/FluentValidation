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

## Returning Custom Codes using ErrorCode

A custom error code can also be associated with validation rules by using the `WithErrorCode` method. We could modify the Surname rule with the following:
The `ErrorCode` can be useful in providing error messages for lookup. At a high level:

```csharp
RuleFor(person => person.Surname).NotNull().WithErrorCode("ERR1234");
```

The resulting error code can be obtained from the `ErrorCode` property on the `ValidationFailure`:

```csharp
var validator = new PersonValidator();
var result = validator.Validate(new Person());
foreach (var failure in result.Errors) {
  Console.WriteLine($"Property: {failure.PropertyName} Error Code: {failure.ErrorCode}");
}
```

The output would be:

```
Property: Surname Error Code: ERR1234
Property: Forename Error Code: NotNullValidator
```

## ErrorCode and Error Messages

The `ErrorCode` is also used to determine the default error message for a particular validator. At a high level:

* `ErrorCode` is used as the lookup key for an error message. For example, a `.NotNull()` validator has a default error code of `NotNullValidator`, which used to look up the error messages from the `LanguageManager`. [See the documentation on localization.](localization)
* If you provide an `ErrorCode`, you could also provide a localized message with the name of that error code to create a custom message.
* If you provide an `ErrorCode` but no custom message, the message will fall back to the default message for that validator. You're not required to add a custom message.
* Using `ErrorCode` can also be used to override the default error message. For example, if you use a custom `Must()` validator, but you'd like to reuse the `NotNull()` validator's default error message, you can call `.WithErrorCode("NotNullValidator")` to achieve this result.

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
