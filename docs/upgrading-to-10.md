# 10.0 Upgrade Guide

### Introduction

FluentValidation 10.0 is a major release that included several breaking changes. Please review this document carefully before upgrading from FluentValidation 9.x to 10.

The main goals for this release were to improve performance and type safety. To achieve this we have introduced generics throughout FluentValidation's internal model. If you have written custom property validators, or made use of the internal API then you will need to update your code. Users of the public-facing API and fluent interface will be largely unaffected.

### PropertyValidatorContext Deprecated

The `PropertyValidatorContext` class has been deprecated, and various places that previously used this now receive a `ValidationContext<T>` instead. Anywhere that previously called `context.ParentContext` to access the `ValidationContext<T>` can now just use `context` instead. For example:


```csharp
// Before:
RuleFor(x => x.Foo).Must((instance, value, context) => {
  return context.ParentContext.RootContextData.ContainsKey("Something");
});

// After:
RuleFor(x => x.Foo).Must((instance, value, context) => {
  return context.RootContextData.ContainsKey("Something");
});
```

### Custom Property Validators

Custom property validators are now generic, and inherit from either `PropertyValidator<T,TProperty>` or `AsyncPropertyValidator<T,TProperty>`. Property validators that inherit from the old non-generic `PropertyValidator` class will continue to work for now, but you will receive a deprecation warning. We recommend migrating to the new generic classes for better performance and support going forward. The non-generic version will be removed in FluentValidation 11. If you currently inherit from `AsyncValidatorBase` then you'll need to migrate as part of upgrading to 10.0

The following changes should be made in order to migrate:
- The class should inherit from `PropertyValidator<T,TProperty>` (or `AsyncPropertyValidator<T,TProperty>`)
- The method signature for `IsValid` should be updated
- The method signature for `GetDefaultMessageTemplate` should be updated
- The `Name` property should be overridden.

The following example shows a custom property validator before and after migration.

```csharp
// Before:
public class NotNullValidator : PropertyValidator
{
  protected override bool IsValid(PropertyValidatorContext context)
  {
    return context.PropertyValue != null;
  }

  protected override string GetDefaultMessageTemplate()
    => "A value for {PropertyName} is required";
}

// After:
public class NotNullValidator<T,TProperty> : PropertyValidator<T, TProperty>
{
  public override string Name => "NotNullValidator";

  public override bool IsValid(ValidationContext<T> context, TProperty value)
  {
    return value != null;
  }

  protected override string GetDefaultMessageTemplate(string errorCode)
    => "A value for {PropertyName} is required";
}
```

### ValidationResult.Errors type change 

The `Errors` property on the `ValidationResult` class has been changed from `IList<ValidationFailure>` to `List<ValidationFailure>`. 

### Changes to property validator metadata

In previous versions of FluentValidation, a property validator's configuration and the property validator itself were part of the same class (`PropertyValidator`). In FluentValidation 10, these are now separate. The validator itself that performs the work is either an `IPropertyValidator<T,TProperty>` or an `IAsyncPropertyValidator<T,TProperty>` and their configuration is exposed via a `RuleComponent`. Note there is still a non-generic `IPropertyValidator` interface available implemented by both `IPropertyValidator<T,TProperty>` and `IAsyncPropertyValidator<T,TProperty>` but it has fewer properties available.

Various methods and properties that previously returned an `IPropertyValidator` now return a tuple of `(IPropertyValidator Validator, IRuleComponent Options)` where previously they returned an `IPropertyValidator`:

- `IValidatorDescriptor.GetMembersWithValidators`
- `IValidatorDescriptor.GetValidatorsForMember`

When accessing property validators via a rule instance, you must now go via a collection of components:

```csharp
// Before:
IValidationRule rule = ...;
foreach (IPropertyValidator propertyValidator in rule.Validators) {
  // ...
}

// After:
IValidationRule rule = ...;
foreach (IRuleComponent component in rule.Componetnts) {
  IPropertyValiator propertyValidator = component.Validator;
}
```

When accessing the current property validator instance on a rule, you must now go via the `Current` property to get the component first.

```csharp
// before:
PropertyRule rule = ...;
IPropertyValidator currentValidator = rule.CurrentValidator;

// after:
IValidationRule<T,TProperty> rule = ...;
RuleComponent<T, TProperty> component = rule.Current;
IPropertyValidator currentValidator = component.CurrentValidator;
```

### Transform syntax changes

The old `Transform` syntax has been removed. See [https://docs.fluentvalidation.net/en/latest/transform.html](transform)

### DI changes

Validators are now registered as `Scoped` rather than `Transient` when using the ASP.NET integration.

### Changes to Interceptors

`IValidatorInterceptor` and `IActionContextValidatorInterceptor` have been combined.
The methods in `IValidatorInterceptor` now accept an `ActionContext` as their first parameter instead of a `ControllerContext`, and `IActionContextValidatorInterceptor` has been removed.

### Changes to ASP.NET client validator adaptors

The signature for adding an ASP.NET Client Validator factories has changed to receive a rule component instead of a property validator. Additionally, as property validator instances are now generic, the lookup key should be a non-generic interface implemented by the property validator.

```csharp

// Before:
public class MyCustomClientsideAdaptor : ClientValidatorBase
{
  public MyCustomClientsideAdaptor(PropertyRule rule, IPropertyValidator validator)
  : base(rule, validator)
  {

  }

  public override void AddValidation(ClientModelValidationContext context)
  {
    // ...
  }
}

services.AddMvc().AddFluentValidation(fv =>
{
  fv.ConfigureClientsideValidation(clientSide =>
  {
    clientSide.Add(typeof(MyCustomPropertyValidator), (context, rule, validator) => new MyCustomClientsideAdaptor(rule, validator));
  })
})


// after:
public class MyCustomClientsideAdaptor : ClientValidatorBase
{
  public MyCustomClientsideAdaptor(IValidationRule rule, IRuleComponent component)
  : base(rule, component)
  {

  }

  public override void AddValidation(ClientModelValidationContext context)
  {
    // ...
  }
}

services.AddMvc().AddFluentValidation(fv =>
{
  fv.ConfigureClientsideValidation(clientSide =>
  {
    clientSide.Add(typeof(IMyCustomPropertyValidator), (context, rule, component) => new MyCustomClientsideAdaptor(rule, component));
  })
})

```

### The internal API

Parts of FluentValidation's internal API have been marked as `internal` which were previously public. This has been done to allow us to evolve and change the internal model going forward. The following classes are affected:

- `RuleBuilder`
- `PropertyRule`
- `CollectionPropertyRule`
- `IncludeRule`

For the majority of cases, if you accessed these classes directly in your code you should be able to use our metadata interfaces to achieve the same result. These include the following:

- `IValidationRule`
- `IValidationRule<T>`
- `IValidationRule<T,TProperty>`
- `ICollectionRule<T, TElement>`
- `IIncludeRule`

Additionally the following methods have been removed from rule instances:
- `RemoveValidator`
- `ReplaceValidator`

### Removal of deprecated code

Several classes, interfaces and methods that were deprecated in FluentValidation 9 and have now been removed:

Related to the generation of error messages, the following have been removed. Alternative methods that receive callbacks are available instead:

- `IStringSource`
- `LazyStringSource`
- `LanguageStringSource`
- `StaticStringSource`

The following additional unused classes and interfaces have been removed:
- `Language`
- `ICommonContext`

The following methods and properties have been removed:
- `ValidationFailure.FormattedMessageArguments`
- `MessageFormatter.AppendAdditionalArguments`
- `MemberNameValidatorSelector.FromExpressions`
- Various utility and extension methods that were previously used throughout the internal API, such as `CooerceToNonGeneric`

Several extension methods that provided overloads of the `Validate` method that were previously deprecated have been removed. Replacements are available:

```csharp
// Validating only specific properties.
// Before:
validator.Validate(instance, x => x.SomeProperty, x => x.SomeOtherProperty);
validator.Validate(instance, "SomeProperty", "SomeOtherProperty");

// After:
validator.Validate(instance, v => {
  v.IncludeProperties(x => x.SomeProperty, x => x.SomeOtherProperty);
});

validator.Validate(instance, v => {
  v.IncludeProperties("SomeProperty", "SomeOtherProperty");
});

// Validating by ruleset:
// Before (comma-delmited string to separate multiple rulesets):
validator.Validate(instance, ruleSet: "SomeRuleSet,AnotherRuleSet");

// After:
// Separate parameters for each ruleset.
validator.Validate(instance, v => {
  v.IncludeRuleSets("SomeRuleSet", "AnotherRuleSet")
});

```

### Other changes

- `ChildValidatorAdaptor.GetValidator` is non-generic again (as it was in FV 8.x)
- The `RuleSets` property on `IValidationRule` instances can now be null. In previous versions this would be initialized to an empty array.
