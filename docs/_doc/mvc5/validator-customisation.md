---
title: Validator Customisation
---

The downside of using this automatic integration is that you don’t have access to the validator directly which means that you don’t have as much control over the validation processes compared to running the validator manually.

You can use the CustomizeValidatorAttribute to configure how the validator will be run. For example, if you want the validator to only run for a particular ruleset then you can specify that ruleset name by attributing the parameter that is going to be validated:

```csharp
public ActionResult Save([CustomizeValidator(RuleSet="MyRuleset")] Customer cust) {
  // ...
}
```

This is the equivalent of specifying the ruleset if you were to pass a ruleset name to a validator:

```csharp
var validator = new CustomerValidator();
var customer = new Customer();
var result = validator.Validate(customer, ruleSet: "MyRuleset");
```

The attribute can also be used to invoke validation for individual properties:

```csharp
public ActionResult Save([CustomizeValidator(Properties="Surname,Forename")] Customer cust) {
  // ...
}
```
…which would be the equivalent of specifying properties in the call to validator.Validate:

```csharp
var validator = new CustomerValidator();
var customer = new Customer();
var result = validator.Validate(customer, properties: new[] { "Surname", "Forename" });
```

You can also use the CustomizeValidatorAttribute to skip validation for a particular type. This is useful for if you need to validate a type manually (for example, if you want to perform async validation then you'll need to instantiate the validator manually and call ValidateAsync as MVC's validation pipeline is not asynchronous).

```csharp
public ActionResult Save([CustomizeValidator(Skip=true)] Customer cust) {
  // ...
}
```
