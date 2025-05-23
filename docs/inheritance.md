# Inheritance Validation

As of FluentValidation 9.2, if your object contains a property which is a base class or interface, you can set up specific [child validators](start.html#complex-properties) for individual subclasses/implementors.

For example, imagine the following example:

```csharp
// We have an interface that represents a 'contact',
// for example in a CRM system. All contacts must have a name and email.
public interface IContact 
{
  string Name { get; set; }
  string Email { get; set; }
}

// A Person is a type of contact, with a name and a DOB.
public class Person : IContact 
{
  public string Name { get; set; }
  public string Email { get; set; }

  public DateTime DateOfBirth { get; set; }
}

// An organisation is another type of contact,
// with a name and the address of their HQ.
public class Organisation : IContact 
{
  public string Name { get; set; }
  public string Email { get; set; }

  public Address Headquarters { get; set; }
}

// Our model class that we'll be validating.
// This might be a request to send a message to a contact.
public class ContactRequest 
{
  public IContact Contact { get; set; }

  public string MessageToSend { get; set; }
}
```

Next we create validators for Person and Organisation:

```csharp
public class PersonValidator : AbstractValidator<Person> 
{
  public PersonValidator() 
  {
    RuleFor(x => x.Name).NotNull();
    RuleFor(x => x.Email).NotNull();
    RuleFor(x => x.DateOfBirth).GreaterThan(DateTime.MinValue);
  }
}

public class OrganisationValidator : AbstractValidator<Organisation> 
{
  public OrganisationValidator() 
  {
    RuleFor(x => x.Name).NotNull();
    RuleFor(x => x.Email).NotNull();
    RuleFor(x => x.HeadQuarters).SetValidator(new AddressValidator());
  }
}
```

Now we create a validator for our `ContactRequest`. We can define specific validators for the `Contact` property, depending on its runtime type. This is done by calling `SetInheritanceValidator`, passing in a function that can be used to define specific child validators:

```csharp
public class ContactRequestValidator : AbstractValidator<ContactRequest>
{
  public ContactRequestValidator()
  {

    RuleFor(x => x.Contact).SetInheritanceValidator(v => 
    {
      v.Add<Organisation>(new OrganisationValidator());
      v.Add<Person>(new PersonValidator());
    });

  }
}
```

There are also overloads of `Add` available that take a callback, which allows for lazy construction of the child validators.

This method also works with [collections](collections), where each element of the collection may be a different subclass. For example, taking the above example if instead of a single `Contact` property, the `ContactRequest` instead had a collection of contacts:

```csharp
public class ContactRequest 
{
  public List<IContact> Contacts { get; } = new();
}
```

...then you could define inheritance validation for each item in the collection:

```csharp
public class ContactRequestValidator : AbstractValidator<ContactRequest>
{
  public ContactRequestValidator()
  {

    RuleForEach(x => x.Contacts).SetInheritanceValidator(v => 
    {
      v.Add<Organisation>(new OrganisationValidator());
      v.Add<Person>(new PersonValidator());
    });
  }
}
```

## Limitations

It's important to note that every subclass that you want to be validated *must be explicitly mapped*. For example, the following would not work:

```csharp
public class ContactBaseValidator : AbstractValidator<IContact> 
{
  public ContactBaseValidatoR() 
  {
    RuleFor(x => x.Name).NotNull();
  }
}

public class ContactRequestValidator : AbstractValidator<ContactRequest>
{
  public ContactRequestValidator()
  {

    RuleFor(x => x.Contact).SetInheritanceValidator(v => 
    {
      // THIS WILL NOT WORK.
      // This will not validate instances of Person or Organisation.
      v.Add<IContact>(new ContactBaseValidator());
    });
  }
}
```

In the above example, this would not correctly validate instances of `Person` or `Organisation` as they have not been explicitly mapped. You must explicitly indicate every subclass that you want to have mapped, as per the first example at the top of the page. 
