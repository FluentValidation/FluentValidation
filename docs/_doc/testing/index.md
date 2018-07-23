---
title: Testing
---
#Core support

FluentValidation comes with two extension methods `ShouldHaveValidationErrorFor` and `ShouldNotHaveValidationErrorFor` that can make it easier to write unit tests for validators. For example, imagine the following validator is defined:

```csharp
public class PersonValidator : AbstractValidator<Person> {
   public PersonValidator() {
      RuleFor(person => person.Name).NotNull();
   }
}
```

You could ensure that this validator works correctly by writing the following tests (using NUnit):

```csharp
using NUnit.Framework;
using FluentValidation;
using FluentValidation.TestHelper;

[TestFixture]
public class PersonValidatorTester {
    private PersonValidator validator;

    [SetUp]
    public void Setup() {
       validator = new PersonValidator();
    }

    [Test]
    public void Should_have_error_when_Name_is_null() {
         validator.ShouldHaveValidationErrorFor(person => person.Name, null as string); 
    }
    
    [Test]
    public void Should_not_have_error_when_name_is_specified() {
        validator.ShouldNotHaveValidationErrorFor(person => person.Name, "Jeremy");
    }
}
```

If the assertion fails, then a `ValidationTestException` will be thrown. 

Note that if you have a complex validator that relies on multiple properties being set, you can pass in a pre-populated instance rather than just the property value:

```csharp
   [Test]
    public void Should_not_have_error_when_name_is_specified() {
        var person = new Person { Name = "Jeremy" };
        validator.ShouldNotHaveValidationErrorFor(x => x.Name, person);
    }
```

You can also assert that a complex property has a particular child validator applied to it by using `ShouldHaveChildValidator`

```csharp
validator.ShouldHaveChildValidator(x => x.Address, typeof(AddressValidator));
```
#External support
Another option is to use an external library like a [FluentValidation.Validators.UnitTestExtension](https://github.com/MichalJankowskii/FluentValidation.Validators.UnitTestExtension). With it, you will be able to prepare test in a bit different way:

```csharp
[Test]
public void Should_have_configured_validation_rules_correctly_for_name() {
  var personValidator = new PersonValidator();

  personValidator.ShouldHaveRules(x => x.Name,
    BaseVerifiersSetComposer.Build()
      .AddPropertyValidatorVerifier<NotNullValidator>()
      .Create());
}
```
More information can be found on [the documentation page](https://github.com/MichalJankowskii/FluentValidation.Validators.UnitTestExtension/wiki).