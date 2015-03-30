## Overriding the Default Error

You can override the default error message for a validator by calling the WithMessage method on a validator definition:

```
RuleFor(customer => customer.Surname).NotNull().WithMessage("Please ensure that you have entered your Surname");
```

Note that custom error messages can contain placeholders for special values such as the name of the property being validated. This means the above error message could be re-written as:

```
RuleFor(customer => customer.Surname).NotNull().WithMessage("Please ensure you have entered your {PropertyName}");
```

...and the value 'Surname' will be inserted. For a complete list of error message placeholders see the Validators page. 

It is also possible to use your own custom arguments in the validation message. These can either be static values or references to other properties on the object being validated:

```csharp
//Using static values in a custom message:
RuleFor(customer => x.Surname).NotNull().WithMessage("This message references some static values: {0} {1}", "hello", 5);
//Result would be "This message references some static values: hello 5"

//Referencing other property values:
RuleFor(customer => customer.Surname)
  .NotNull()
  .WithMesasge("This message references some other properties: Forename: {0} Discount: {1}", 
    customer => customer.Forename, 
    customer => customer.Discount
  );
//Result would be: "This message references some other properties: Forename: Jeremy Discount: 100"
```

If you want to override all of FluentValidation's default error messages, check out FluentValidation's support for [Localization](Localization.md).

## Overriding the Default Property Name

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

Property name resolution is also pluggable. By default, the name of the property extracted from the MemberExpression passed to RuleFor. If you want change this logic, you can set the `PropertyNameResolver` property on the `ValidatorOptions` class:

```csharp
ValidatorOptions.PropertyNameResolver = (type, member) => {
  if(member != null) {
     return member.Name + "Foo";
  }
  return null;
};
```

This is not a realistic example as it changes all properties to have the suffix "Foo", but hopefully illustrates the point. 

Additionally, FluentValidation will respect the use of the DisplayName and Display attributes for generating the property's name within error messages:

```
public class Person {
  [Display(Name="Last name")]
  public string Surname { get; set; }
}
```

## Specifying a condition with When/Unless

The `When` and `Unless` methods can be used to specify conditions that control when the rule should execute. For example, this rule on the CustomerDiscount property will only execute when IsPreferredCustomer is true:

```
RuleFor(customer => customer.CustomerDiscount).GreaterThan(0).When(customer => customer.IsPreferredCustomer);
```

The `Unless` method is simply the opposite of `When`. 

If you need to specify the same condition for multiple rules then you can call the top-level When method instead of chaining the When call at the end of the rule:

```
When(customer => customer.IsPreferred, () => {
   RuleFor(customer => customer.CustomerDiscount).GreaterThan(0);
   RuleFor(customer => customer.CreditCardNumber).NotNull();
});
```

This time, the condition will be applied to both rules.

## Setting the Cascade mode

You can set the cascade mode to customise how FluentValidation executes chained validators when a particular validator in the chain fails.

Imagine you have two validators defined as part of a single rule definition, a `NotNull` validator and a `NotEqual` validator:

```
RuleFor(x => x.Surname).NotNull().NotEqual("foo");
```

This will first check whether the Surname property is not null and then will check if it's not equal to the string "foo". If the first validator (NotNull) fails, then the call to `NotEqual` will still be invoked. This can be changed by specifying a cascade mode of `StopOnFirstFailure`:

```
RuleFor(x => x.Surname).Cascade(CascadeMode.StopOnFirstFailure).NotNull().NotEqual("foo");
```

Now, if the NotNull validator fails then the NotEqual validator will not be executed. This is particularly useful if you have a complex chain where each validator depends on the previous validator to succeed. 

The two cascade modes are:
- `Continue` (the default) - always invokes all validators in a rule definition
- `StopOnFirstFailure` - stops executing a rule as soon as a validator fails

As well as being set at the rule level, the cascade mode can also be set globally for all validators, or for all the rules in a particular validator class.

To set the cascade mode globally, you can set the CascadeMode property on the static ValidatorOptions class during your application's startup routine:

```
ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
```

This can then be overriden by individual validator classes or by individual rules.

To set the cascade mode for all rules inside a single validator class, set the `CascadeMode` property on `AbstractValidator`:

```csharp
public class PersonValidator : AbstractValidator<Person> {
  public PersonValidator() {
    
    // First set the cascade mode
    CascadeMode = CascadeMode.StopOnFirstFailure;
    
    // Rule definitions follow
    RuleFor(...) 
    RuleFor(...)

   }
}
```
