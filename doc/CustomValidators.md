## Custom Validators

There are two ways of creating custom validators. The first is to create a custom property validator, the second is to make use of the Custom method on AbstractValidator.

## Writing a Custom Property Validator

To write a custom property validator you will need to inherit from the abstract PropertyValidator class. For example, imagine you want to create a validator that ensures that a list has fewer than 10 items. You could create the validator like this:


```csharp
using System.Collections.Generic;
using FluentValidation.Validators;

public class ListMustContainFewerThanTenItemsValidator<T> : PropertyValidator {

	public ListMustContainFewerThanTenItemsValidator() 
		: base("Property {PropertyName} contains more than 10 items!") {
		
	}

	protected override bool IsValid(PropertyValidatorContext context) {
		var list = context.PropertyValue as IList<T>;

		if(list != null && list.Count >= 10) {
			return false;
		}

		return true;
	}
}
```
When you inherit from `PropertyValidator` you must override the `IsValid` method. This method takes a `PropertyValidatorContext` object and should return a boolean indicating whether validation succeeded.

The `PropertyValidatorContext` object passed into the Validate method contains several properties:
- `Instance` - the object being validated
- `PropertyDescription` - the name of the property (or alternatively a custom name specifed by a call to WithName
- `PropertyValue` - the value of the property being validated
- `Member` - the MemberInfo describing the property being validated.

Note that the error message to use is specified in the constructor. The simplest way to define your error message is to use the string (as in this example) but you can also used localized error messages by specifying either a resource type and resource name or a lambda expression. For more details, please see [Localization](Localization.md) 

To use the new custom validator you can call `SetValidator` when defining a validation rule. 

```
public class PersonValidator : AbstractValidator<Person> {
    public PersonValidator() {
       RuleFor(person => person.Pets).SetValidator(new ListMustContainFewerThanTenItemsValidator<Pet>());
    }
}
```

Alternatively, you could define an extension method...

```
public static class MyValidatorExtensions {
   public static IRuleBuilderOptions<T, IList<TElement>> MustContainFewerThanTenItems<T, TElement>(this IRuleBuilder<T, IList<TElement>> ruleBuilder) {
      return ruleBuilder.SetValidator(new ListMustContainFewerThanTenItemsValidator<TElement>());
   }
}
```

...which can then be chained like any other validator:

```
public class PersonValidator : AbstractValidator<Person> {
    public PersonValidator() {
       RuleFor(person => person.Pets).MustContainFewerThanTenItems();
    }
}
```

## Using AbstractValidator.Custom

Another approach to using custom validators is to make use of the Custom method defined on AbstractValidator. Using the Custom method, the above example could be written like this:

```
public class PersonValidator : AbstractValidator<Person> {
   public PersonValidator() {
       Custom(person => { 
           return person.Pets.Count >= 10 
              ? new ValidationFailure("Pets", "More than 9 pets is not allowed.")
              : null; 
       });
   }
}
```

Note that Custom is really only designed for complex validation scenarios. Most of the time, the same result can be achieved with much cleaner code by using the PredicateValidator which also allows you to easily write custom validation logic while still maintaining the in-line method chaining. For example:

```
public class PersonValidator : AbstractValidator<Person> {
   public PersonValidator() {
      RuleFor(person => person.Pets).Must(HaveFewerThanTenPets).WithMessage("More than 9 pets is not allowed");
   }

   private bool HaveFewerThanTenPets(IList<Pet> pets) {
      return pets.Count < 10;
   }
}
```
