# Including Rules

You can include rules from other validators provided they validate the same type. This allows you to split rules across multiple classes and compose them together (in a similar way to how other languages support traits). For example, imagine you have 2 validators that validate different aspects of a `Person`:

```csharp
public class PersonAgeValidator : AbstractValidator<Person>  {
  public PersonAgeValidator() {
    RuleFor(x => x.DateOfBirth).Must(BeOver18);
  }

  protected bool BeOver18(DateTime date) {
    //...
  }
}

public class PersonNameValidator : AbstractValidator<Person> {
  public PersonNameValidator() {
    RuleFor(x => x.Surname).NotNull().Length(0, 255);
    RuleFor(x => x.Forename).NotNull().Length(0, 255);
  }
}
```

Because both of these validators are targetting the same model type (`Person`), you can combine them using `Include`:

```csharp
public class PersonValidator : AbstractValidator<Person> {
  public PersonValidator() {
    Include(new PersonAgeValidator());
    Include(new PersonNameValidator());
  }
}
```

```eval_rst
.. note::
    You can only include validators that target the same type as the root validator.
```
