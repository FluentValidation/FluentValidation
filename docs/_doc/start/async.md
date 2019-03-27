---
title: Async Validation
---

In some situations, you may wish to define asynchronous rules, for example when working with an external API. By default, FluentValidation allows custom rules defined with `MustAsync` or `CustomAsync` to be run asyhchronously, as well as defining asychronous coniditions with `WhenAsync`. 

A simplistic solution that checks if a user ID is alrady in use using an external web API: 

```csharp
public class CustomerValidator : AbstractValidator<Customer> {
  SomeExternalWebApiClient _client;

  public CustomerValidator(SomeExternalWebApiClient client) {
    _client = client;

    RuleFor(x => x.Id).MustAsync(async (id, cancellation) => {
      bool exists = await _client.IdExists(id);
      return !exists;
    }).WithMessage("ID Must be unique");
  }
}
```

Invoking the validator is essentially the same, but you should now invoke it by calling `ValidateAsync`:

```csharp
var validator = new CustomerValidator();
var result = await validator.ValidateAsync(customer);
```


<div class="callout-block callout-warning"><div class="icon-holder" markdown="1">*&nbsp;*{: .fa .fa-bug}
</div><div class="content" markdown="1">
{: .callout-title}
#### Warning

If your validator contains asynchronous validators or asynchronous conditions, it's important that you *always* 
call `ValidateAsync` on your validator and never `Validate`. If you call `Validate`, then your asynchronous rules *will be run synchronously*, which is not desirable.

You should not use asynchronous rules when [using automatic validation with ASP.NET](/aspnet) as ASP.NET's validation pipeline is not asynchronous. If you use asynchronous rules with ASP.NET's automatic validation, they will always be run synchronously. 

</div></div>
