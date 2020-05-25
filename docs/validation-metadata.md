# Validation Metadata

When executing validations, there is often more nuance than whether the object is valid or not. Is a failure an error and an operation cannot continue? Or is the validation a warning that we might want to pass along to a user and have them select to continue an operation? What about returning an error code that your users expect?

FluentValidation enables you to configure this sort of metadata out of the box.

## Configuring the Severity of Validation Errors

Given the example below of validating a person:

```csharp
 public class PersonValidator : AbstractValidator<Person> {
  public PersonValidator() {
        RuleFor(x => x.Surname).NotNull();
        RuleFor(x => x.Forename).NotNull();
  }
}
```

What if it was our desire that a missing surname was a warning, because we truly only cared about a forename?

We could modify the above line to:

```csharp
RuleFor(x => x.Surname).NotNull().WithSeverity(Severity.Warning);
```

Or, in version 9.0 and above:

```csharp
RuleFor(x => x.Surname).NotNull().WithSeverity(x=> x.Warning);
```

In this case, the `ValidationResult` would still have an `IsValid` result of `false`. However, in the list of `Errors`, the `ValidationFailure` pertaining to this field will have `Severity` property set to `Warning`.

By default, the severity level of every validation is `Error`. Available options are `Error`, `Warning`, or `Info`.

## Returning Custom Codes using ErrorCode

Given the below `PersonValidator`:

```csharp
 public class PersonValidator : AbstractValidator<Person> {
  public PersonValidator() {
        RuleFor(x => x.Surname).NotNull();
        RuleFor(x => x.Forename).NotNull();
  }
}
```

What if we wanted to add a custom error code when the Surname validation fails?

We could modify the Surname rule to:

```csharp
RuleFor(x => x.Surname).NotNull().WithErrorCode("ERR1234");
```

In this case, the `ValidationResult` would have, in the list of `Errors`, a `ValidationFailure` pertaining to this field with the `ErrorCode` property set to the string that you've specified in your validator.

## Supplying Additional Validation Information with CustomState

There may be an occasion where you'd like to return contextual information about the state of your validation when it was run. The `WithCustomState` method allows you to set a custom state object that you can return along with your validation results.

Given the below `PersonValidator`:

```csharp
 public class PersonValidator : AbstractValidator<Person> {
  public PersonValidator() {
        RuleFor(x => x.Surname).NotNull();
        RuleFor(x => x.Forename).NotNull();
  }
}
```

We could assign a custom state by modifying a line to read:

```csharp
RuleFor(x => x.Forename).NotNull().WithState(x => 1234);
```

Then, within the `Errors` property of a validation result, the `ValidationFailure` that corresponds to the property will have a `CustomState` property that contains the contents as set within the validator.
