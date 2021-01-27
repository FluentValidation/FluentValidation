# RuleSets

RuleSets allow you to group validation rules together which can be executed together as a group whilst ignoring other rules:

For example, let's imagine we have 3 properties on a Person object (Id, Surname and Forename) and have a validation rule for each. We could group the Surname and Forename rules together in a “Names” RuleSet:

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

Here the two rules on Surname and Forename are grouped together in a “Names” RuleSet. We can invoke only these rules by passing additional options to the Validate method:

```csharp
var validator = new PersonValidator();
var person = new Person();
var result = validator.Validate(person, options => options.IncludeRuleSets("Names"));
```

```eval_rst
.. note::
  Many of the methods in FluentValidation are extension methods such as "Validate" above and require the FluentValidation namespace to be imported via an using statement, e.g. "using FluentValidation;". 
```

This allows you to break down a complex validator definition into smaller segments that can be executed in isolation. If you call `Validate` without passing a ruleset then only rules not in a RuleSet will be executed.

You can execute multiple rulesets by passing multiple ruleset names to `IncludeRuleSets`:

```csharp
var result = validator.Validate(person, options => {
  options.IncludeRuleSets("Names", "MyRuleSet", "SomeOtherRuleSet");
});
```

You can also include all the rules not part of a ruleset by using calling `IncludeRulesNotInRuleSet`, or by using the special name "default" (case insensitive):

```csharp
validator.Validate(person, options => {
  // Option 1: IncludeRulesNotInRuleSet is the equivalent of using the special ruleset name "default"
  options.IncludeRuleSets("Names").IncludeRulesNotInRuleSet();
  // Option 2: This does the same thing.
  option.IncludeRuleSets("Names", "default");
});
```

This would execute rules in the MyRuleSet set, and those rules not in any ruleset. Note that you shouldn't create your own ruleset called "default", as FluentValidation will treat these rules as not being in a ruleset.

You can force all rules to be executed regardless of whether or not they're in a ruleset by calling `IncludeAllRuleSets` (this is the equivalent of using `IncludeRuleSets("*")` )

```csharp
validator.Validate(person, options => {
  options.IncludeAllRuleSets();
});
```

```eval_rst
.. note::
  The syntax above using the "options" callback is only available in FluentValidation 9.1 and newer. If you are using FluentValidation 9.0 and older, please read the section below.
```

## RuleSets in FluentValidation 9.0 (or older)

```eval_rst
.. warning::
  The syntax in this section is deprecated and will be removed in FluentValidation 10.
```

Invoking RuleSets in FluentValidation 9.0 and older requires the use of a slightly different syntax, by passing the ruleset names to a named `ruleSet` parameter:

```csharp
var validator = new PersonValidator();
var person = new Person();
var result = validator.Validate(person, ruleSet: "Names");
```

This is the equivalent of the first example above which executes a single ruleset.

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
