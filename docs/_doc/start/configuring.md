---
titie: Configuring a Validator
sections:
  - Overriding the Message
  - Overrding the Property Name
  - Conditions
  - Setting the Cascade Mode
  - Dependent Rules
  - Root Context Data
  - Using PreValidate
  - Callbacks
---

### Overriding the Message

You can override the default error message for a validator by calling the WithMessage method on a validator definition:

```
RuleFor(customer => customer.Surname).NotNull().WithMessage("Please ensure that you have entered your Surname");
```

Note that custom error messages can contain placeholders for special values such as '{PropertyName}' - which will be replaced in this example with the name of the property being validated. This means the above error message could be re-written as:

```
RuleFor(customer => customer.Surname).NotNull().WithMessage("Please ensure you have entered your {PropertyName}");
```

...and the value 'Surname' will be inserted.

#### Configuring Error Message Parameters (Placeholders)

As specified in the example above, the message can contain placeholders for special values such as '{PropertyName}' - which will be replaced with a specified value. Each type of built-in validator has its own list of placeholders which are supported by it.

The placeholders are:
Used in all validators:
* '{PropertyName}' - The name of the property being validated
* '{PropertyValue}' - The value of the property being validated
These include the predicate validator ('Must' validator), the email and the regex validators.

Used in comparison validators: (Equal, NotEqual, GreaterThan, GreaterThanOrEqual, etc.)
* {ComparisonValue} = Value that the property should be compared to

Used only in the Length validator:
* {MinLength} = Minimum length
* {MaxLength} = Maximum length
* {TotalLength} = Number of characters entered

For a complete list of error message placeholders see the the [Built in Validators page](/built-in-validators#built-in-validators). Each built in validator has its own supported placeholders.

It is also possible to use your own custom arguments in the validation message. These can either be static values or references to other properties on the object being validated. This can be done by using the overload of WithMessage that takes a lambda expression, and then passing the values to string.Format or by using string interpolation.

```csharp
//Using static values in a custom message:
RuleFor(customer => x.Surname).NotNull().WithMessage(customer => string.Format("This message references some constant values: {0} {1}", "hello", 5));
//Result would be "This message references some constant values: hello 5"

//Referencing other property values:
RuleFor(customer => customer.Surname)
  .NotNull()
  .WithMessage(customer => $"This message references some other properties: Forename: {customer.Forename} Discount: {customer.Discount}");
//Result would be: "This message references some other properties: Forename: Jeremy Discount: 100"
```

If you want to override all of FluentValidation's default error messages, check out FluentValidation's support for  [Localization](/localization).

### Overriding the Property Name

The default validation error messages contain the property name being validated. For example, if you were to define a validator like this:
```
RuleFor(customer => customer.Surname).NotNull();
```

...then the default error message would be *'Surname' must not be empty*. Although you can override the entire error message by calling WithMessage, you can also replace just the property name by calling WithName:

```
RuleFor(customer => customer.Surname).NotNull().WithName("Last name");
```

Now the error message would be *'Last name' must not be empty.*

Note that this only replaces the name of the property in the error message. When you inspect the Errors collection on the `ValidationResult`, this error will still be associated with a property called `Surname`.
If you want to completely rename the property, you can use the `OverridePropertyName` method instead.

There is also an overload of `WithName` that accepts a lambda expression in a similar way to WithName in the above example

Property name resolution is also pluggable. By default, the name of the property extracted from the MemberExpression passed to RuleFor. If you want change this logic, you can set the `DisplayNameResolver` property on the `ValidatorOptions` class:

```csharp
ValidatorOptions.DisplayNameResolver = (type, member) => {
  if(member != null) {
     return member.Name + "Foo";
  }
  return null;
};
```

This is not a realistic example as it changes all properties to have the suffix "Foo", but hopefully illustrates the point.

Additionally, FluentValidation will respect the use of the DisplayName and Display attributes for generating the property's name within error messages:

```csharp
public class Person {
  [Display(Name="Last name")]
  public string Surname { get; set; }
}
```

### Conditions

The `When` and `Unless` methods can be used to specify conditions that control when the rule should execute. For example, this rule on the CustomerDiscount property will only execute when IsPreferredCustomer is true:

```
RuleFor(customer => customer.CustomerDiscount).GreaterThan(0).When(customer => customer.IsPreferredCustomer);
```

The `Unless` method is simply the opposite of `When`.

If you need to specify the same condition for multiple rules then you can call the top-level When method instead of chaining the When call at the end of the rule:

```csharp
When(customer => customer.IsPreferred, () => {
   RuleFor(customer => customer.CustomerDiscount).GreaterThan(0);
   RuleFor(customer => customer.CreditCardNumber).NotNull();
});
```

This time, the condition will be applied to both rules.

### Setting the Cascade mode

You can set the cascade mode to customise how FluentValidation executes chained validators when a particular validator in the chain fails.

Imagine you have two validators defined as part of a single rule definition, a `NotNull` validator and a `NotEqual` validator:

```csharp
RuleFor(x => x.Surname).NotNull().NotEqual("foo");
```

This will first check whether the Surname property is not null and then will check if it's not equal to the string "foo". If the first validator (NotNull) fails, then the call to `NotEqual` will still be invoked. This can be changed by specifying a cascade mode of `StopOnFirstFailure`:

```csharp
RuleFor(x => x.Surname).Cascade(CascadeMode.StopOnFirstFailure).NotNull().NotEqual("foo");
```

Now, if the NotNull validator fails then the NotEqual validator will not be executed. This is particularly useful if you have a complex chain where each validator depends on the previous validator to succeed.

The two cascade modes are:
- `Continue` (the default) - always invokes all validators in a rule definition
- `StopOnFirstFailure` - stops executing a rule as soon as a validator fails

As well as being set at the rule level, the cascade mode can also be set globally for all validators, or for all the rules in a particular validator class. This is the equivalent of setting the cascade mode on every rule within the validator. Not that this still only applies to validators *within the same rule chain* - separate calls to `RuleFor` are treated separately. If one rule fails, it will not prevent a separate rule from running, only validators within the same rule chain.

To set the cascade mode globally, you can set the CascadeMode property on the static ValidatorOptions class during your application's startup routine:

```csharp
ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
```

This can then be overridden by individual validator classes or by individual rules.

To set the cascade mode for all rules inside a single validator class, set the `CascadeMode` property on `AbstractValidator`:

```csharp
public class PersonValidator : AbstractValidator<Person> {
  public PersonValidator() {
    // First set the cascade mode
    CascadeMode = CascadeMode.StopOnFirstFailure;

    RuleFor(x => x.Surname).NotNull().NotEqual("foo");
    RuleFor(x => x.Forename).NotNull().NotEqual("foo");
  }
}
```

Note that this is the equivalent of doing the following:

```csharp
RuleFor(x => x.Surname).Cascade(CascadeMode.StopOnFirstFailure).NotNull().NotEqual("foo");
RuleFor(x => x.Forename).Cascade(CascadeMode.StopOnFirstFailure).NotNull().NotEqual("foo");
```

<div class="callout-block callout-info"><div class="icon-holder">*&nbsp;*{: .fa .fa-info-circle}
</div><div class="content">
{: .callout-title}
#### Remember

Setting the cascade mode only applies to validators within the same `RuleFor` chain. Changing the cascade mode does not affect separate calls to `RuleFor`. If you want prevent one rule from running if a different rule fails, you should instead use Dependent Rules (below),

</div></div>

#### Dependent Rules

By default, all rules in FluentValidation are separate and cannot influence one another. This is intentional and necessary for asynchronous validation to work. However, there may be some cases where you want to ensure that some rules are only executed after another has completed. You can use `DependentRules` to do this.

To use DependentRules, call the `DependentRules` method at the end of the rule that you want others to depend on. This method accepts a lambda expression inside which you can define other rules that will be executed only if the first rule passes:

```csharp
RuleFor(x => x.Surname).NotNull().DependentRules(() => {
  RuleFor(x => x.Forename).NotNull();
});
```

Here the rule against Forename will only be run if the Surname rule passes.

_Jeremy's note_: Personally I do not particularly like using DependentRules as I feel it's fairly hard to read, especially with a complex set of rules. In many cases, it can be simpler to use `When` conditions to prevent rules from running in certain situations. Even though this can sometimes mean more duplication, it is often easier to read.

#### Root Context Data

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
  if(context.ParentContext.RootContextData.ContainsKey("MyCustomData")) {
    context.AddFailure("My error message");
  }
});
```

### Using PreValidate

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

### Callbacks

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
  .OnAnyFailure(x => Console.WriteLine("Must failed"));
```
