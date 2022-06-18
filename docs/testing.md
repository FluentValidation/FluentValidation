# Test Extensions

FluentValidation provides some extensions that can aid with testing your validator classes.

We recommend treating validators as 'black boxes' - provide input to them and then assert whether the validation results are correct or incorrect.

## Using TestValidate

You can use the `TestValidate` extension method to invoke a validator for testing purposes, and then perform assertions against the result. This makes it easier to write tests for validators.

For example, imagine the following validator is defined:

```csharp
public class PersonValidator : AbstractValidator<Person> 
{
   public PersonValidator() 
   {
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
public class PersonValidatorTester 
{
    private PersonValidator validator;

    [SetUp]
    public void Setup()
    {
       validator = new PersonValidator();
    }

    [Test]
    public void Should_have_error_when_Name_is_null() 
    {
      var model = new Person { Name = null };
      var result = validator.TestValidate(model);
      result.ShouldHaveValidationErrorFor(person => person.Name);
    }

    [Test]
    public void Should_not_have_error_when_name_is_specified() 
    {
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

If you want to make sure no other validation failures occured, except specified by conditions, use method `Only` after the conditions:

```csharp
var result = validator.TestValidate(person);

// Assert that failures only happened for Name property.
result.ShouldHaveValidationErrorFor(person => person.Name).Only();

// Assert that failures only happened for Name property and all have the specified message
result.ShouldHaveValidationErrorFor(person => person.Name)
  .WithErrorMessage("'Name' must not be empty.")
  .Only();
```

There are also inverse methods available (`WithoutMessage`, `WithoutErrorCode`, `WithoutSeverity`, `WithoutCustomState`).

## Asynchronous TestValidate

There is also an asynchronous `TestValidateAsync` method available which corresponds to the regular `ValidateAsync` method. Usage is similar, except the method returns an awaitable `Task` instead.

# Mocking

Validators are intended to be "black boxes" and we don't generally recommend mocking them. Within a test, the recommended appraoch is to supply a real validator instance with known bad data in order to trigger a validation error. 

Mocking validators tends to require that you make assuptions about how the validators are built internally (both the rules contained within them, as well as FluentValidation's own internals). Mocking this behaviour leads to brittle tests that aren't upgrade-safe.

However if you find yourself in a situation where you absoloutely do need to mock a validator, then we suggest using `InlineValidator<T>` to create a stub implementation as this way you can take advantage of re-using FluentValidation's own internal logic for creating validation failures. We *strongly* recommend not using a mocking library. An example of using `InlineValidator` is shown below:

```csharp
// Original validator that relies on an external service.
// External service is used to check that the customer ID is not already used in the database.
public class CustomerValidator : AbstractValidator<Customer> 
{
  public CustomerValidator(ICustomerRepository customerRepository) 
  {
    RuleFor(x => x.Id)
      .Must(id => customerRepository.CheckIdNotInUse(id));
  }
}

// If you needed to stub this failure in a unit/integration test, 
// you could do the following:
var validator = new InlineValidator<Customer>();
validator.RuleFor(x => x.Id).Must(id => false); 

// This instance could then be passed into anywhere expecting an IValidator<Customer>
```
