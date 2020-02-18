# Custom Validators

There are several ways to create a custom, reusable validator. The recommended way is to make use of the [Predicate Validator](/built-in-validators.html#predicate-validator) to write a custom validation function, but you can also write a custom implementation of the PropertyValidator class.

For these examples, we'll imagine a scenario where you want to create a reusable validator that will ensure a List object contains fewer than 10 items.

## Predicate Validator
The simplest way to implement a custom validator is by using the `Must` method, which internally uses the `PredicateValidator`.

Imagine we have the following class:
```csharp
public class Person {
  public IList<Pet> Pets {get;set;} = new List<Pet>();
}
```

To ensure our list property contains fewer than 10 items, we could do this:

```csharp
public class PersonValidator : AbstractValidator<Person> {
  public PersonValidator() {
    RuleFor(x => x.Pets).Must(list => list.Count < 10)
      .WithMessage("The list must contain fewer than 10 items");
  }
}
```

To make this logic reusable, we can wrap it an extension method that acts upon any `List<T>` type.

```csharp
public static class MyCustomValidators {
  public static IRuleBuilderOptions<T, IList<TElement>> ListMustContainFewerThan<T, TElement>(this IRuleBuilder<T, IList<TElement>> ruleBuilder, int num) {
	return ruleBuilder.Must(list => list.Count < num).WithMessage("The list contains too many items");
  }
}
```

Here we create an extension method on `IRuleBuilder<T,TProperty>`, and we use a generic type constraint to ensure this method only appears in intellisense for List types. Inside the method, we call the Must method in the same way as before but this time we call it on the passed-in RuleBuilder instance. We also pass in the number of items for comparison as a parameter. Our rule definition can now be rewritten to use this method:

```csharp
RuleFor(x => x.Pets).ListMustContainFewerThan(10);
```

## Custom message placeholders

We can extend the above example to include a more useful error message. At the moment, our custom validator always returns the message "The list contains too many items" if validation fails. Instead, let's change the message so it returns "'Pets' must contain fewer than 10 items." This can be done by using custom message placeholders. FluentValidation supports several message placeholders by default including `{PropertyName}` and `{PropertyValue}` ([see this list for more](built-in-validators)), but we can also add our own.

We need to modify our extension method slightly to use a different overload of the `Must` method, one that accepts a `PropertyValidatorContext` instance. This context provides additional information and methods we can use when performing validation:

```csharp
public static IRuleBuilderOptions<T, IList<TElement>> ListMustContainFewerThan<T, TElement>(this IRuleBuilder<T, IList<TElement>> ruleBuilder, int num) {

  return ruleBuilder.Must((rootObject, list, context) => {
    context.MessageFormatter.AppendArgument("MaxElements", num);
    return list.Count < num;
  })
  .WithMessage("{PropertyName} must contain fewer than {MaxElements} items.");
}
```

Note that the overload of Must that we're using now accepts 3 parameters: the root (parent) object, the property value itself, and the context. We use the context to add a custom message replacement value of `MaxElements` and set its value to the number passed to the method. We can now use this placeholder as `{MaxElements}` within the call to `WithMessage`.

The resulting message will now be  `'Pets' must contain fewer than 10 items.` We could even extend this further to include the number of elements that the list contains like this:

```csharp
public static IRuleBuilderOptions<T, IList<TElement>> ListMustContainFewerThan<T, TElement>(this IRuleBuilder<T, IList<TElement>> ruleBuilder, int num) {

  return ruleBuilder.Must((rootObject, list, context) => {
    context.MessageFormatter
      .AppendArgument("MaxElements", num)
      .AppendArgument("TotalElements", list.Count);

    return list.Count < num;
  })
  .WithMessage("{PropertyName} must contain fewer than {MaxElements} items. The list contains {TotalElements} element");
}
```

## Writing a Custom Validator

If you need more control of the validation process than is available with `Must`, you can write a custom rule using the `Custom` method. This method allows you to manually create the `ValidationFailure` instance associated with the validation error. Usually, the framework does this for you, so it is more verbose than using `Must`.


```csharp
public class PersonValidator : AbstractValidator<Person> {
  public PersonValidator() {
   RuleFor(x => x.Pets).Custom((list, context) => {
     if(list.Count > 10) {
       context.AddFailure("The list must contain 10 items or fewer");
     }
   });
  }
}
```

The advantage of this approach is that it allows you to return multiple errors for the same rule (by calling the `context.AddFailure` method multiple times). In the above example, the property name in the generated error will be inferred as "Pets", although this could be overridden by calling a different overload of AddFailure:

```csharp
context.AddFailure("SomeOtherProperty", "The list must contain 10 items or fewer");
// Or you can instantiate the ValidationFailure directly:
context.AddFailure(new ValidationFailure("SomeOtherProperty", "The list must contain 10 items or fewer");
```

As before, this could be wrapped in an extension method to simplify the consuming code.

```csharp
public static IRuleBuilderInitial<T, IList<TElement>> ListMustContainFewerThan<T, TElement>(this IRuleBuilder<T, IList<TElement>> ruleBuilder, int num) {

  return ruleBuilder.Custom((list, context) => {
     if(list.Count > 10) {
       context.AddFailure("The list must contain 10 items or fewer");
     }
   });
}
```

## Reusable Property Validators

In some cases where your custom logic is very complex, you may wish to move the custom logic into a separate class. This can be done by writing a class that inherits from the abstract `PropertyValidator` (this is how all of FluentValidation's built-in rules are defined).

```eval_rst
.. note::
  This is an advanced technique that is usually unnecessary - the `Must` and `Custom` methods explained above are usually more appropriate.
```

We can recreate the above example using a custom PropertyValidator implementation like this:

```csharp
using System.Collections.Generic;
using FluentValidation.Validators;

public class ListCountValidator<T> : PropertyValidator {
        private int _max;

	public ListCountValidator(int max)
		: base("{PropertyName} must contain fewer than {MaxElements} items.") {
		_max = max;
	}

	protected override bool IsValid(PropertyValidatorContext context) {
		var list = context.PropertyValue as IList<T>;

		if(list != null && list.Count >= _max) {
			context.MessageFormatter.AppendArgument("MaxElements", _max);
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

Note that the error message to use is specified in the constructor. The simplest way to define your error message is to use the string (as in this example) but you can also used localized error messages by specifying either a resource type and resource name. For more details, please see [Localization](localization)

To use the new custom validator you can call `SetValidator` when defining a validation rule.

```csharp
public class PersonValidator : AbstractValidator<Person> {
    public PersonValidator() {
       RuleFor(person => person.Pets).SetValidator(new ListCountValidator<Pet>(10));
    }
}
```

As with the first example, you can wrap this in an extension method to make the syntax nicer:
```csharp
public static class MyValidatorExtensions {
   public static IRuleBuilderOptions<T, IList<TElement>> ListMustContainFewerThan<T, TElement>(this IRuleBuilder<T, IList<TElement>> ruleBuilder, int num) {
      return ruleBuilder.SetValidator(new ListCountValidator<TElement>(num));
   }
}
```

...which can then be chained like any other validator:

```csharp
public class PersonValidator : AbstractValidator<Person> {
    public PersonValidator() {
       RuleFor(person => person.Pets).ListMustContainFewerThan(10);
    }
}
```
