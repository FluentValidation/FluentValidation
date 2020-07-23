# RuleSets

RuleSets allow you to group validation rules together which can be executed together as a group whilst ignoring other rules:

For example, let’s imagine we have 3 properties on a Person object (Id, Surname and Forename) and have a validation rule for each. We could group the Surname and Forename rules together in a “Names” RuleSet:

```csharp
 public class PersonValidator : AbstractValidator<Person> {
  public PersonValidator() {
     RuleSet("Names", () => {
        RuleFor(x => x.Surname).NotNull();
        RuleFor(x => x.Forename).NotNull();
     });
 
     RuleFor(x => x.Id).NotEqual(0);
  }
}
```

Here the two rules on Surname and Forename are grouped together in a “Names” RuleSet. We can invoke only these rules by passing a ruleSet parameter to the Validate extension method (note that this must be a named parameter as this overload has several options available).

```csharp
var validator = new PersonValidator();
var person = new Person();
var result = validator.Validate(person, ruleSet: "Names");
```

This allows you to break down a complex validator definition into smaller segments that can be executed in isolation. If you call `Validate` without passing a ruleset then only rules not in a RuleSet will be executed. 

You can execute multiple rulesets by using a comma-separated list of strings:

```csharp
validator.Validate(person, ruleSet: "Names,MyRuleSet,SomeOtherRuleSet")
```

You can also include all the rules not part of a ruleset by using the special name "default" (case insensitive):

```csharp
validator.Validate(person, ruleSet: "default,MyRuleSet")
```

This would execute rules in the MyRuleSet set, and those rules not in any ruleset. Note that you shouldn't create your own ruleset called "default", as FluentValidation will treat these rules as not being in a ruleset.

You can force all rules to be executed regardless of whether or not they're in a ruleset by specifying a ruleset of "*":

```csharp
validator.Validate(person, ruleSet: "*")
```

```eval_rst
.. note::
    The `Validate` method that takes a ruleset is an extension method so you must have the `FluentValidation` namespace imported with a `using` statment in order to use it. 

```
