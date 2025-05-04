# RuleSets

RuleSets allow you to group validation rules together which can be executed together as a group whilst ignoring other rules:

For example, let's imagine we have 3 properties on a Person object (Id, Surname and Forename) and have a validation rule for each. We could group the Surname and Forename rules together in a “Names” RuleSet:

```csharp
 public class PersonValidator : AbstractValidator<Person> 
 {
  public PersonValidator() 
  {
     RuleSet("Names", () => 
     {
        RuleFor(x => x.Surname).NotNull();
        RuleFor(x => x.Forename).NotNull();
     });

     RuleFor(x => x.Id).NotEqual(0);
  }
}
```

Here the two rules on Surname and Forename are grouped together in a “Names” RuleSet. We can invoke only these rules by passing additional options to the Validate method:

```csharp
var validator = new PersonValidator();
var person = new Person();
var result = validator.Validate(person, options => options.IncludeRuleSets("Names"));
```

```eval_rst
.. note::
  Many of the methods in FluentValidation are extension methods such as "Validate" above and require the FluentValidation namespace to be imported via a using statement, e.g. "using FluentValidation;".
```

This allows you to break down a complex validator definition into smaller segments that can be executed in isolation. If you call `Validate` without passing a ruleset then only rules not in a RuleSet will be executed.

You can execute multiple rulesets by passing multiple ruleset names to `IncludeRuleSets`:

```csharp
var result = validator.Validate(person, options => 
{
  options.IncludeRuleSets("Names", "MyRuleSet", "SomeOtherRuleSet");
});
```

You can also include all the rules not part of a ruleset by calling `IncludeRulesNotInRuleSet`, or by using the special name "default" (case insensitive):

```csharp
validator.Validate(person, options => 
{
  // Option 1: IncludeRulesNotInRuleSet is the equivalent of using the special ruleset name "default"
  options.IncludeRuleSets("Names").IncludeRulesNotInRuleSet();
  // Option 2: This does the same thing.
  option.IncludeRuleSets("Names", "default");
});
```

This would execute rules in the MyRuleSet set, and those rules not in any ruleset. Note that you shouldn't create your own ruleset called "default", as FluentValidation will treat these rules as not being in a ruleset.

You can force all rules to be executed regardless of whether or not they're in a ruleset by calling `IncludeAllRuleSets` (this is the equivalent of using `IncludeRuleSets("*")` )

```csharp
validator.Validate(person, options => 
{
  options.IncludeAllRuleSets();
});
```