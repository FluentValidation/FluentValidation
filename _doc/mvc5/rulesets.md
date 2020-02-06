---
title: Specifying a RuleSet for client-side messages
---

If you’re using rulesets alongside ASP.NET MVC, then you’ll notice that by default FluentValidation will only generate client-side error messages for rules not part of any ruleset. You can instead specify that FluentValidation should generate clientside rules from a particular ruleset by attributing your controller action with a RuleSetForClientSideMessagesAttribute:

```csharp
[RuleSetForClientSideMessages("MyRuleset")]
public ActionResult Index() {
   return View(new PersonViewModel());
}
```

You can also use the `SetRulesetForClientsideMessages` extension method within your controller action (you must have the FluentValidation.Mvc namespace imported):

```csharp
public ActionResult Index() {
   ControllerContext.SetRulesetForClientsideMessages("MyRuleset");
   return View(new PersonViewModel());
}
```
