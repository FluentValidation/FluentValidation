# Custom Validators

There are several ways to create a custom, reusable validator. The recommended way is to make use of the [Predicate Validator](built-in-validators.html#predicate-validator) to write a custom validation function, but you can also use the `Custom` method to take full control of the validation process.

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

Here we create an extension method on `IRuleBuilder<T,TProperty>`, and we use a generic type constraint to ensure this method only appears in intellisense for List types. Inside the method, we call the Must method in the same way as before but this time we call it on the passed-in `RuleBuilder` instance. We also pass in the number of items for comparison as a parameter. Our rule definition can now be rewritten to use this method:

```csharp
RuleFor(x => x.Pets).ListMustContainFewerThan(10);
```

## Custom message placeholders

We can extend the above example to include a more useful error message. At the moment, our custom validator always returns the message "The list contains too many items" if validation fails. Instead, let's change the message so it returns "'Pets' must contain fewer than 10 items." This can be done by using custom message placeholders. FluentValidation supports several message placeholders by default including `{PropertyName}` and `{PropertyValue}` ([see this list for more](built-in-validators)), but we can also add our own.

We need to modify our extension method slightly to use a different overload of the `Must` method, one that accepts a `ValidationContext<T>` instance. This context provides additional information and methods we can use when performing validation:

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

The resulting message will now be `'Pets' must contain fewer than 10 items.` We could even extend this further to include the number of elements that the list contains like this:

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

The advantage of this approach is that it allows you to return multiple errors for the same rule (by calling the `context.AddFailure` method multiple times). In the above example, the property name in the generated error will be inferred as "Pets", although this could be overridden by calling a different overload of `AddFailure`:

```csharp
context.AddFailure("SomeOtherProperty", "The list must contain 10 items or fewer");
// Or you can instantiate the ValidationFailure directly:
context.AddFailure(new ValidationFailure("SomeOtherProperty", "The list must contain 10 items or fewer");
```

As before, this could be wrapped in an extension method to simplify the consuming code.

```csharp
public static IRuleBuilderOptionsConditions<T, IList<TElement>> ListMustContainFewerThan<T, TElement>(this IRuleBuilder<T, IList<TElement>> ruleBuilder, int num) {

  return ruleBuilder.Custom((list, context) => {
     if(list.Count > 10) {
       context.AddFailure("The list must contain 10 items or fewer");
     }
   });
}
```

## Reusable Property Validators

In some cases where your custom logic is very complex, you may wish to move the custom logic into a separate class. This can be done by writing a class that inherits from the abstract `PropertyValidator<T,TProperty>` class (this is how all of FluentValidation's built-in rules are defined).

```eval_rst
.. note::
  This is an advanced technique that is usually unnecessary - the `Must` and `Custom` methods explained above are usually more appropriate.
```

We can recreate the above example using a custom `PropertyValidator` implementation like this:

```csharp
using System.Collections.Generic;
using FluentValidation.Validators;

public class ListCountValidator<T, TCollectionElement> : PropertyValidator<T, IList<TCollectionElement>> {
	private int _max;

	public ListCountValidator(int max) {
		_max = max;
	}

	public override bool IsValid(ValidationContext<T> context, IList<TCollectionElement> list) {
		if(list != null && list.Count >= _max) {
			context.MessageFormatter.AppendArgument("MaxElements", _max);
			return false;
		}

		return true;
	}

  public override string Name => "ListCountValidator";

	protected override string GetDefaultMessageTemplate(string errorCode)
		=> "{PropertyName} must contain fewer than {MaxElements} items.";
}
```
When you inherit from `PropertyValidator` you must override the `IsValid` method. This method receives two vaues - the `ValidationContext<T>` representing the current validation run, and the value of the property. The method should return a boolean indicating whether validation was successful. The generic type parameters on the base class represent the root instance being validated, and the type of the property that our custom validator can act upon. In this case we're constraining the custom validator to types that implement `IList<TCollectionElement>` although this can be left open if desired.

Note that the error message to use is specified by overriding `GetDefaultMessageTemplate`.

To use the new custom validator you can call `SetValidator` when defining a validation rule.

```csharp
public class PersonValidator : AbstractValidator<Person> {
    public PersonValidator() {
       RuleFor(person => person.Pets).SetValidator(new ListCountValidator<Person, Pet>(10));
    }
}
```

As with the first example, you can wrap this in an extension method to make the syntax nicer:
```csharp
public static class MyValidatorExtensions {
   public static IRuleBuilderOptions<T, IList<TElement>> ListMustContainFewerThan<T, TElement>(this IRuleBuilder<T, IList<TElement>> ruleBuilder, int num) {
      return ruleBuilder.SetValidator(new ListCountValidator<T, TElement>(num));
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

As another simpler example, this is how FluentValidation's own `NotNull` validator is implemented:

```csharp
public class NotNullValidator<T,TProperty> : PropertyValidator<T,TProperty> {

  public override string Name => "NotNullValidator";

  public override bool IsValid(ValidationContext<T> context, TProperty value) {
    return value != null;
  }

  protected override string GetDefaultMessageTemplate(string errorCode)
    => "'{PropertyName}' must not be empty.";
}

```

```eval_rst
.. note::
  Prior to FluentValidation 10.0, the PropertyValidator class did not have generic type parameters.
```
