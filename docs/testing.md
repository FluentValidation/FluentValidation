# Test Extensions

FluentValidation provides some extensions that can aid with testing your validator classes.

We recommend treating validators as 'black boxes' - provide input to them and then assert whether the validation results are correct or incorrect.

## Using TestValidate

You can use the `TestValidate` extension method to invoke a validator for testing purposes, and then perform assertions against the result. This makes it easier to write tests for validators.

For example, imagine the following validator is defined:

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
      var model = new Person { Name = null };
      var result = validator.TestValidate(model);
      result.ShouldHaveValidationErrorFor(person => person.Name);
    }

    [Test]
    public void Should_not_have_error_when_name_is_specified() {
      var model = new Person { Name = "Jeremy" };
      var result = validator.TestValidate(model);
      result.ShouldNotHaveValidationErrorFor(person => person.Name);
    }
}
```

If the assertion fails, then a `ValidationTestException` will be thrown.

If you have more complex tests, you can use the same technique to perform multiple assertions on a single validation result. For example:

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

You can also chain additional method calls to the result of `ShouldHaveValidationErrorFor` that test individual components of the validation failure including the error message, severity, error code and custom state:

```csharp
var result = validator.TestValidate(person);

result.ShouldHaveValidationErrorFor(person => person.Name)
  .WithErrorMessage("'Name' must not be empty.")
  .WithSeverity(Severity.Error)
  .WithErrorCode("NotNullValidator");
```

There are also inverse methods avaialble (`WithoutMessage`, `WithoutErrorCode`, `WithoutSeverity`, `WithoutCustomState`)

## Asynchronous TestValidate

There is also an asynchronous `TestValidateAsync` method available which corresponds to the regular `ValidateAsync` method. Usage is similar, except the method returns an awaitable `Task` instead.

## Simple test extensions

FluentValidation also comes some simplified test extension wrappers that can be called on the validator directly for simple uses cases. For example, using the same examples above the first two tests could be written like this:

```csharp
[Test]
public void Should_have_error_when_Name_is_null() {
  // Simplified version:
  validator.ShouldHaveValidationErrorFor(person => person.Name, null as string);

  // This is the equivalent of:
  var model = new Person { Name = null };
  var result = validator.TestValidate(model);
  result.ShouldHaveValidationErrorFor(person => person.Name);
}

[Test]
public void Should_not_have_error_when_name_is_specified() {
  // Simplified version:
  validator.ShouldNotHaveValidationErrorFor(person => person.Name, "Jeremy");

  // This is the equivalent of:
  var model = new Person { Name = "Jeremy" };
  var result = validator.TestValidate(model);
  result.ShouldNotHaveValidationErrorFor(person => person.Name);
}
```

There are also asynchronous versions of these methods available.

Note that the simplified test extensions have several constraints:
- The model object must have a default constructor
- The property being validated must be publicly settable.
- You cannot use the simplified extensions for nested properties, or collections.

If you don't meet these constraints then you should stick with using the `TestValidate` method from the first example.
