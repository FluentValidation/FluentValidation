# Test Extensions

FluentValidation provides some extensions that can aid with testing your validator classes. 

We recommend treating validators as 'black boxes' - provide input to them and then assert whether the validation results are correct or incorrect.

## Simple test extensions

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

## Advanced test extensions

The methods `ShouldHaveValidationErrorFor` and `ShouldNotHaveValidationErrorFor` used in the examples above are wrappers around FluentValidation's `TestValidate` functionality. If you have more complex tests, you can use `TestValidate` instead to perform multiple assertions on a single validation result. For example:

```csharp
var person = new Person { Name = "Jeremy" };
var result = validator.TestValidate(person);

// Assert that there should be a failure for the Name property.
result.ShouldHaveValidationErrorFor(x => x.Name);

// Assert that there are no failures for the age property. 
result.ShouldNotHaveValidationErrorFor(x => x.Age);

// You can also use a string name for properties that can't be easily represented with a lambda, eg:
result.ShouldHaveValidationErrorFor("Addresses[0].Line1");
```

You can also chain additional method calls to the result of `ShouldHaveValidationErrorFor` that test individual components of the validation failure including the error message, severity error code and custom state:

```csharp
var result = validator.TestValidate(person);

result.ShouldHaveValidationErrorFor(x => x.Name)
  .WithErrorMessage("'Name' must not be empty.")
  .WithSeverity(Severity.Error)
  .WithErrorCode("NotNullValidator");
```

There are also inverse methods avaialble (`WithoutMessage`, `WithoutErrorCode`, `WithoutSeverity`, `WithoutCustomState`)
