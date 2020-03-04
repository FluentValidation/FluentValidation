# Asynchronous Validation

In some situations, you may wish to define asynchronous rules, for example when working with an external API. By default, FluentValidation allows custom rules defined with `MustAsync` or `CustomAsync` to be run asynchronously, as well as defining asynchronous conditions with `WhenAsync`. 

A simplistic solution that checks if a user ID is already in use using an external web API: 

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

```eval_rst
.. warning::
  If your validator contains asynchronous validators or asynchronous conditions, it's important that you *always* call `ValidateAsync` on your validator and never `Validate`. If you call `Validate`, then your asynchronous rules *will be run synchronously*, which is not desirable.

  You should not use asynchronous rules when `using automatic validation with ASP.NET <aspnet.html>`_ as ASP.NET's validation pipeline is not asynchronous. If you use asynchronous rules with ASP.NET's automatic validation, they will always be run synchronously. 
```
